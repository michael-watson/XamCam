//using System.Net;
//using Newtonsoft.Json;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;

//using System;
//using Microsoft.WindowsAzure.MediaServices.Client;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.IO;
//using System.Globalization;
//using Newtonsoft.Json;
//using Microsoft.Azure;
//using System.Net;
//using System.Security.Cryptography;
//using Microsoft.Azure.WebJobs;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;


//namespace XamCamFunctions
//{
//    public static class XamCamWebHook
//    {
//    //    [FunctionName("XamCamWebHook")]
//    //    public static async Task<object> RunXamCamWebHook([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
//    //    {
//    //        log.Info($"Webhook was triggered!");

//    //        string jsonContent = await req.Content.ReadAsStringAsync();
//    //        dynamic data = JsonConvert.DeserializeObject(jsonContent);

//    //        if (data.first == null || data.last == null)
//    //        {
//    //            return req.CreateResponse(HttpStatusCode.BadRequest, new
//    //            {
//    //                error = "Please pass first/last properties in the input object"
//    //            });
//    //        }

//    //        return req.CreateResponse(HttpStatusCode.OK, new
//    //        {
//    //            greeting = $"Hello {data.first} {data.last}!"
//    //        });
//    //    }
    

//    static readonly string _AADTenantDomain = Constants.tenantId; //Environment.GetEnvironmentVariable("AMSAADTenantDomain");
//    static readonly string _RESTAPIEndpoint =  Constants.MediaServiceRestEndpoint;             //Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");

//    static readonly string _AMSClientId = Constants.ClientID;   //Environment.GetEnvironmentVariable("AMSClientId");
//    static readonly string _AMSClientSecret = Constants.ClientSecret;  //Environment.GetEnvironmentVariable("AMSClientSecret");

//    static CloudMediaContext _context = null;

//    //WEBHOOK
//    internal const string SignatureHeaderKey = "sha256";
//    internal const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";

//    static string _webHookEndpoint = Constants.WebHookEndpoint;//"https://iccfunction.azurewebsites.net/api/XamCamWebHook";   // Environment.GetEnvironmentVariable("WebHookEndpoint");
//    static string _signingKey = Constants.WebHookSigningKey; //"3S2OvxtXJ6CbWcA3fHoCT9M3yVxDrqh9PxpOhSN2VgxMWTBtZuwLcw=="; //Environment.GetEnvironmentVariable("SigningKey");


//        [FunctionName("XamCamWebHook")]
//    public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
//    {
//        log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

//        Task<byte[]> taskForRequestBody = req.Content.ReadAsByteArrayAsync();
//        byte[] requestBody = await taskForRequestBody;

//        string jsonContent = await req.Content.ReadAsStringAsync();
//        log.Info($"Request Body = {jsonContent}");

//        IEnumerable<string> values = null;
//        if (req.Headers.TryGetValues("ms-signature", out values))
//        {
//            byte[] signingKey = Convert.FromBase64String(_signingKey);
//            string signatureFromHeader = values.FirstOrDefault();

//            if (VerifyWebHookRequestSignature(requestBody, signatureFromHeader, signingKey))
//            {
//                string requestMessageContents = Encoding.UTF8.GetString(requestBody);

//                NotificationMessage msg = JsonConvert.DeserializeObject<NotificationMessage>(requestMessageContents);

//                if (VerifyHeaders(req, msg, log))
//                {
//                    string newJobStateStr = (string)msg.Properties.Where(j => j.Key == "NewState").FirstOrDefault().Value;
//                    if (newJobStateStr == "Finished")
//                    {
//                        AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(_AADTenantDomain,
//                                    new AzureAdClientSymmetricKey(_AMSClientId, _AMSClientSecret),
//                                    AzureEnvironments.AzureCloudEnvironment);

//                        AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

//                        _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

//                        if (_context != null)
//                        {
//                            string urlForClientStreaming = PublishAndBuildStreamingURLs(msg.Properties["JobId"]);
//                            log.Info($"URL to the manifest for client streaming using HLS protocol: {urlForClientStreaming}");
//                        }
//                    }

//                    return req.CreateResponse(HttpStatusCode.OK, string.Empty);
//                }
//                else
//                {
//                    log.Info($"VerifyHeaders failed.");
//                    return req.CreateResponse(HttpStatusCode.BadRequest, "VerifyHeaders failed.");
//                }
//            }
//            else
//            {
//                log.Info($"VerifyWebHookRequestSignature failed.");
//                return req.CreateResponse(HttpStatusCode.BadRequest, "VerifyWebHookRequestSignature failed.");
//            }
//        }

//        return req.CreateResponse(HttpStatusCode.BadRequest, "Generic Error.");
//    }

//    private static string PublishAndBuildStreamingURLs(String jobID)
//    {
//        IJob job = _context.Jobs.Where(j => j.Id == jobID).FirstOrDefault();
//        IAsset asset = job.OutputMediaAssets.FirstOrDefault();

//        // Create a 30-day readonly access policy. 
//        // You cannot create a streaming locator using an AccessPolicy that includes write or delete permissions.
//        IAccessPolicy policy = _context.AccessPolicies.Create("Streaming policy",
//        TimeSpan.FromDays(30),
//        AccessPermissions.Read);

//        // Create a locator to the streaming content on an origin. 
//        ILocator originLocator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, asset,
//        policy,
//        DateTime.UtcNow.AddMinutes(-5));

//        // Get a reference to the streaming manifest file from the  
//        // collection of files in the asset. 
//        var manifestFile = asset.AssetFiles.Where(f => f.Name.ToLower().
//                    EndsWith(".ism")).
//                    FirstOrDefault();

//        // Create a full URL to the manifest file. Use this for playback
//        // in streaming media clients. 
//        string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest" + "(format=m3u8-aapl)";
//        return urlForClientStreaming;

//    }

//    private static bool VerifyWebHookRequestSignature(byte[] data, string actualValue, byte[] verificationKey)
//    {
//        using (var hasher = new HMACSHA256(verificationKey))
//        {
//            byte[] sha256 = hasher.ComputeHash(data);
//            string expectedValue = string.Format(CultureInfo.InvariantCulture, SignatureHeaderValueTemplate, ToHex(sha256));

//            return (0 == String.Compare(actualValue, expectedValue, System.StringComparison.Ordinal));
//        }
//    }

//    private static bool VerifyHeaders(HttpRequestMessage req, NotificationMessage msg, TraceWriter log)
//    {
//        bool headersVerified = false;

//        try
//        {
//            IEnumerable<string> values = null;
//            if (req.Headers.TryGetValues("ms-mediaservices-accountid", out values))
//            {
//                string accountIdHeader = values.FirstOrDefault();
//                string accountIdFromMessage = msg.Properties["AccountId"];

//                if (0 == string.Compare(accountIdHeader, accountIdFromMessage, StringComparison.OrdinalIgnoreCase))
//                {
//                    headersVerified = true;
//                }
//                else
//                {
//                    log.Info($"accountIdHeader={accountIdHeader} does not match accountIdFromMessage={accountIdFromMessage}");
//                }
//            }
//            else
//            {
//                log.Info($"Header ms-mediaservices-accountid not found.");
//            }
//        }
//        catch (Exception e)
//        {
//            log.Info($"VerifyHeaders hit exception {e}");
//            headersVerified = false;
//        }

//        return headersVerified;
//    }

//    private static readonly char[] HexLookup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

//    /// <summary>
//    /// Converts a <see cref="T:byte[]"/> to a hex-encoded string.
//    /// </summary>
//    private static string ToHex(byte[] data)
//    {
//        if (data == null)
//        {
//            return string.Empty;
//        }

//        char[] content = new char[data.Length * 2];
//        int output = 0;
//        byte d;

//        for (int input = 0; input < data.Length; input++)
//        {
//            d = data[input];
//            content[output++] = HexLookup[d / 0x10];
//            content[output++] = HexLookup[d % 0x10];
//        }

//        return new string(content);
//    }

//    internal enum NotificationEventType
//    {
//        None = 0,
//        JobStateChange = 1,
//        NotificationEndPointRegistration = 2,
//        NotificationEndPointUnregistration = 3,
//        TaskStateChange = 4,
//        TaskProgress = 5
//    }

//    internal sealed class NotificationMessage
//    {
//        public string MessageVersion { get; set; }
//        public string ETag { get; set; }
//        public NotificationEventType EventType { get; set; }
//        public DateTime TimeStamp { get; set; }
//        public IDictionary<string, string> Properties { get; set; }
//    }

       










//    }
//}
