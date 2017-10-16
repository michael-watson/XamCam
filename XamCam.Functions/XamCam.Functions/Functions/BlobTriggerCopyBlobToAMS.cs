using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;

namespace XamCam.Functions
{
    public static class BlobTriggerCopyBlobToAMS
    {
        const string SignatureHeaderKey = "sha256";
        const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";

        static readonly string _AADTenantDomain = Constants.TenantId;
        static readonly string _RESTAPIEndpoint = Constants.MediaServiceRestEndpoint;
        static readonly string _mediaservicesClientId = Constants.ClientID;
        static readonly string _mediaservicesClientSecret = Constants.ClientSecret;
        static readonly string _connectionString = Constants.BlobURLAndKey;
        static readonly char[] _hexLookup = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        static string _webHookEndpoint = Constants.WebHookEndpoint;
        static string _signingKey = Constants.WebHookSigningKey;

        static CloudMediaContext _context = null;
        static CloudStorageAccount _destinationStorageAccount = null;


        [FunctionName("CopyBlobToAMS")]
        public async static Task CopyBlobToAMS([BlobTrigger("mediaassetblobcontainer20170928/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            //#error 
            //uncomment error tag to ensure that developer adds appropriate strings in Azure Function or local.settings.json

            //SETUP NOTE 
            //When running locally add full blob storage connection string to local.settings.json with key "AzureWebJobsStorage" 
            //When running on cloud - make sure that in the Azure Portal - under your Function > Application Settings to add an environmental variable with key "AzureWebJobsStorage" with the full blob storage connection string

            // NOTE that the variables {fileName} here come from the path setting in function.json
            // and are passed into the  Run method signature above. We can use this to make decisions on what type of file
            // was dropped into the input container for the function. 

            // No need to do any Retry strategy in this function, By default, the SDK calls a function up to 5 times for a 
            // given blob. If the fifth try fails, the SDK adds a message to a queue named webjobs-blobtrigger-poison.

            log.Info($"C# Blob trigger function processed: {name}.mp4");
            log.Info($"Media Services REST endpoint : {_RESTAPIEndpoint}");

            try
            {
                var tokenCredentials = new AzureAdTokenCredentials(_AADTenantDomain,
                                            new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                            AzureEnvironments.AzureCloudEnvironment);

                var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

                var createdURI = $"https://xamcamstorage.blob.core.windows.net/mediaassetblobcontainer20170928/{name}";

                var uploadedBlob = new CloudBlockBlob(new Uri(createdURI));

                var newAssetFromBlob = await CreateAssetFromBlob(uploadedBlob, name, log);

                // Step 2: Create an Encoding Job
                //PART II

                var keyBytes = Convert.FromBase64String(Constants.WebHookSigningKey);

                var existingEndpoint = _context.NotificationEndPoints.Where(e => e.Name == "FunctionWebHook3").FirstOrDefault();

                INotificationEndPoint endpoint;
                if (existingEndpoint != null)
                {
                    Console.WriteLine("Webhook endpoint already exists");
                    endpoint = existingEndpoint;
                }
                else
                {
                    endpoint = _context.NotificationEndPoints.Create("FunctionWebHook3",
                                                                        NotificationEndPointType.WebHook,
                                                                        Constants.WebHookEndpoint,
                                                                        keyBytes);

                    Console.WriteLine($"Notification Endpoint Created with Key : {keyBytes}");
                }

                // Declare a new encoding job with the Standard encoder
                var encodingJob = _context.Jobs.Create("Azure Function - MES Job");

                // Get a media processor reference, and pass to it the name of the 
                // processor to use for the specific task.
                var processor = GetLatestMediaProcessorByName("Media Encoder Standard");

                // Create a task with the encoding details, using a custom preset
                var encodingTask = encodingJob.Tasks.AddNew("Encode with Adaptive Streaming",
                                                                processor,
                                                                "Adaptive Streaming",
                                                                TaskOptions.None);

                // Specify the input asset to be encoded.
                encodingTask.InputAssets.Add(newAssetFromBlob);

                // Add an output asset to contain the results of the job. 
                // This output is specified as AssetCreationOptions.None, which 
                // means the output asset is not encrypted. 
                encodingTask.OutputAssets.AddNew(name, AssetCreationOptions.None);

                // Add the WebHook notification to this Task and request all notification state changes.
                // Note that you can also add a job level notification
                // which would be more useful for a job with chained tasks.  
                if (endpoint != null)
                {
                    encodingTask.TaskNotificationSubscriptions.AddNew(NotificationJobState.All, endpoint, true);
                    Console.WriteLine("Created Notification Subscription for endpoint: {0}", _webHookEndpoint);
                }
                else
                {
                    Console.WriteLine("No Notification Endpoint is being used");
                }

                encodingJob.Submit();
                log.Info("Job Submitted");

                Console.WriteLine($"Expect WebHook to be triggered for the Job ID: {encodingJob.Id}");
                Console.WriteLine($"Expect WebHook to be triggered for the Task ID: {encodingTask.Id}");

            }
            catch (Exception ex)
            {
                log.Error("ERROR: failed.");
                log.Info($"StackTrace : {ex.StackTrace}");
                throw ex;
            }
        }

        //////////////////////////////////////////////////////////
        /// WEBHOOK CALLED AFTER THE PROCESSING TASK IS COMPLETED
        //////////////////////////////////////////////////////////
        [FunctionName("NewXamCamWebHookThree")]
        public static async Task<HttpResponseMessage> RunNewXamCamWebHookThree([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

            var mediaAsByteArrary = await req.Content.ReadAsByteArrayAsync();

            log.Info($"Request Body = {await req.Content.ReadAsStringAsync()}");

            if (req.Headers.TryGetValues("ms-signature", out IEnumerable<string> values))
            {
                byte[] signingKey = Convert.FromBase64String(_signingKey);
                string signatureFromHeader = values.FirstOrDefault();

                if (IsWebHookRequestSignatureValid(mediaAsByteArrary, signatureFromHeader, signingKey))
                {
                    var requestMessageContents = Encoding.UTF8.GetString(mediaAsByteArrary);

                    var notificationMessage = JsonConvert.DeserializeObject<NotificationMessage>(requestMessageContents);

                    if (AreHeadersValid(req, notificationMessage, log))
                    {
                        var newJobStateStr = notificationMessage.Properties.Where(j => j.Key.Equals("NewState")).FirstOrDefault().Value;
                        if (newJobStateStr == "Finished")
                        {
                            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(_AADTenantDomain,
                                        new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                        AzureEnvironments.AzureCloudEnvironment);

                            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                            _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

                            if (_context != null)
                            {
                                string urlForClientStreaming = PublishAndBuildStreamingURLs(notificationMessage.Properties["JobId"]);
                                log.Info($"URL to the manifest for client streaming using HLS protocol: {urlForClientStreaming}");

                                //DO THE C# STRING MANIPULATION GET BOTH SMOOTH STREAMING AND MPEG DASH
                                var indexOfHLS = urlForClientStreaming.IndexOf("(format=m3u8-aapl)", StringComparison.Ordinal);
                                var smoothStreamingURL = urlForClientStreaming.Substring(0, indexOfHLS);
                                var theMPEGDashURL = ($"{smoothStreamingURL}(format=mpd-time-csf)");

                                //GET THE FILENAME FROM THE MANIFEST URL

                                var numberOfLettersInAMSUrlWithSlash = Constants.AMSUrlWithSlash.Length;
                                var smoothStreamingURLWithoutAMSUrlWithSlash = smoothStreamingURL.Substring(numberOfLettersInAMSUrlWithSlash, smoothStreamingURL.Length - numberOfLettersInAMSUrlWithSlash);
                                var indexOfFirstSlash = smoothStreamingURLWithoutAMSUrlWithSlash.IndexOf("/", StringComparison.Ordinal);
                                var fileNameWithISM = smoothStreamingURLWithoutAMSUrlWithSlash.Substring(indexOfFirstSlash + 1, smoothStreamingURLWithoutAMSUrlWithSlash.Length - indexOfFirstSlash - 1);
                                var indexOfISM = fileNameWithISM.IndexOf(".ism", StringComparison.Ordinal);
                                var fileNameWithoutISM = fileNameWithISM.Substring(0, indexOfISM);
                                var filename = ($"{fileNameWithoutISM}.mp4");

                                //FIND OBJECT BASED ON SAME NAME OF FILE IN COSMOS DB AND ADD THE NEW URLS
                                var relevantDocumentFromBlobUpload = await CosmosDBService.GetMediaFileByFileNameAsync(filename);

                                relevantDocumentFromBlobUpload.hLS = urlForClientStreaming;
                                relevantDocumentFromBlobUpload.smoothStreaming = smoothStreamingURL;
                                relevantDocumentFromBlobUpload.mPEGDash = theMPEGDashURL;
                                relevantDocumentFromBlobUpload.mediaAssetUri = smoothStreamingURL;

                                await CosmosDBService.PutMediaAssetAsync(relevantDocumentFromBlobUpload);
                            }
                        }
                        return req.CreateResponse(HttpStatusCode.OK, string.Empty);
                    }
                    else
                    {
                        log.Info($"VerifyHeaders failed.");
                        return req.CreateResponse(HttpStatusCode.BadRequest, "VerifyHeaders failed.");
                    }
                }
                else
                {
                    log.Info($"VerifyWebHookRequestSignature failed.");
                    return req.CreateResponse(HttpStatusCode.BadRequest, "VerifyWebHookRequestSignature failed.");
                }
            }
            return req.CreateResponse(HttpStatusCode.BadRequest, "Generic Error.");
        }

        /// <summary>
        /// Creates a new asset and copies blobs from the specifed storage account.
        /// </summary>
        /// <param name="blob">The specified blob.</param>
        /// <returns>The new asset.</returns>
        /// 
        static async Task<IAsset> CreateAssetFromBlob(CloudBlockBlob blob, string assetName, TraceWriter log)
        {
            //Get a reference to the storage account that is associated with the Media Services account. 
            _destinationStorageAccount = CloudStorageAccount.Parse(_connectionString);

            // Create a new asset. 
            var asset = _context.Assets.Create(blob.Name, AssetCreationOptions.None);
            log.Info($"Created new asset {asset.Name}");

            IAccessPolicy writePolicy = _context.AccessPolicies.Create("writePolicy",
            TimeSpan.FromHours(4), AccessPermissions.Write);
            ILocator destinationLocator = _context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);
            CloudBlobClient destBlobStorage = _destinationStorageAccount.CreateCloudBlobClient();

            // Get the destination asset container reference
            string destinationContainerName = (new Uri(destinationLocator.Path)).Segments[1];
            CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);

            try
            {
                assetContainer.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                log.Error("ERROR:" + ex.Message);
            }

            log.Info("Created asset.");

            // Get hold of the destination blob
            CloudBlockBlob destinationBlob = assetContainer.GetBlockBlobReference(blob.Name);

            // Copy Blob
            try
            {
                using (var stream = await blob.OpenReadAsync())
                {
                    await destinationBlob.UploadFromStreamAsync(stream);
                }

                log.Info("Copy Complete.");

                var assetFile = asset.AssetFiles.Create(blob.Name);
                assetFile.ContentFileSize = blob.Properties.Length;
                assetFile.IsPrimary = true;
                assetFile.Update();
                asset.Update();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Info(ex.StackTrace);
                log.Info("Copy Failed.");
                throw;
            }

            destinationLocator.Delete();
            writePolicy.Delete();

            return asset;
        }

        static string PublishAndBuildStreamingURLs(String jobID)
        {
            IJob job = _context.Jobs.Where(j => j.Id == jobID).FirstOrDefault();
            IAsset asset = job.OutputMediaAssets.FirstOrDefault();

            // Create a 30-day readonly access policy. 
            // You cannot create a streaming locator using an AccessPolicy that includes write or delete permissions.
            IAccessPolicy policy = _context.AccessPolicies.Create("Streaming policy",
            TimeSpan.FromDays(30),
            AccessPermissions.Read);

            // Create a locator to the streaming content on an origin. 
            ILocator originLocator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, asset,
            policy,
            DateTime.UtcNow.AddMinutes(-5));

            // Get a reference to the streaming manifest file from the  
            // collection of files in the asset. 
            var manifestFile = asset.AssetFiles
                                    .Where(f => f.Name.ToLower().EndsWith(".ism", StringComparison.Ordinal))
                                    .FirstOrDefault();

            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest" + "(format=m3u8-aapl)";
            return urlForClientStreaming;
        }

        static bool IsWebHookRequestSignatureValid(byte[] data, string actualValue, byte[] verificationKey)
        {
            using (var hasher = new HMACSHA256(verificationKey))
            {
                byte[] sha256 = hasher.ComputeHash(data);
                string expectedValue = string.Format(CultureInfo.InvariantCulture, SignatureHeaderValueTemplate, ToHex(sha256));

                return (0 == String.Compare(actualValue, expectedValue, StringComparison.Ordinal));
            }
        }

        static bool AreHeadersValid(HttpRequestMessage req, NotificationMessage msg, TraceWriter log)
        {
            bool headersVerified = false;

            try
            {
                if (req.Headers.TryGetValues("ms-mediaservices-accountid", out IEnumerable<string> values))
                {
                    var accountIdHeader = values.FirstOrDefault();
                    var accountIdFromMessage = msg.Properties["AccountId"];

                    if (0 == string.Compare(accountIdHeader, accountIdFromMessage, StringComparison.OrdinalIgnoreCase))
                        headersVerified = true;
                    else
                        log.Info($"accountIdHeader={accountIdHeader} does not match accountIdFromMessage={accountIdFromMessage}");
                }
                else
                {
                    log.Info($"Header ms-mediaservices-accountid not found.");
                }
            }
            catch (Exception e)
            {
                log.Info($"VerifyHeaders hit exception {e}");
                headersVerified = false;
            }
            return headersVerified;
        }

        /// <summary>
        /// Converts a <see cref="T:byte[]"/> to a hex-encoded string.
        /// </summary>
        static string ToHex(byte[] data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            char[] content = new char[data.Length * 2];
            int output = 0;
            byte d;

            for (int input = 0; input < data.Length; input++)
            {
                d = data[input];
                content[output++] = _hexLookup[d / 0x10];
                content[output++] = _hexLookup[d % 0x10];
            }
            return new string(content);
        }

        static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
            ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        internal enum NotificationEventType
        {
            None = 0,
            JobStateChange = 1,
            NotificationEndPointRegistration = 2,
            NotificationEndPointUnregistration = 3,
            TaskStateChange = 4,
            TaskProgress = 5
        }

        internal sealed class NotificationMessage
        {
            public string MessageVersion { get; set; }
            public string ETag { get; set; }
            public NotificationEventType EventType { get; set; }
            public DateTime TimeStamp { get; set; }
            public IDictionary<string, string> Properties { get; set; }
        }
    }
}