//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;
//using System.Text;
//using System;
//using Newtonsoft.Json;
//using FunctionApp4.DataModels;

//namespace FunctionApp4
//{
//    public static class ICCFunctionsConsolidated
//    {
//        [FunctionName("HttpTriggerCSharpConsolidated")]
//        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            log.Info("C# HTTP trigger function processed a request.");

//            // parse query parameter
//            string name = req.GetQueryNameValuePairs()
//                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
//                .Value;

//            // Get request body
//            dynamic data = await req.Content.ReadAsAsync<object>();

//            // Set name to query string or body data
//            name = name ?? data?.name;

//            return name == null
//                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
//                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
//        }


//        //CONSTANTS NEEDED FOR AZURE AD
//        static string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
//        static string GrantType = "client_credentials";
//        static string ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
//        static string ClientID = "8d631792-ed10-46aa-bd09-b8ca1641bc6f";
//        static string RequestedResource = "https://rest.media.azure.net";

//        //STATIC HTTPCLIENT
//        static HttpClient ICCHttpClient { get; set; }

//        //STATIC AD BEARER TOKEN
//        static String ADBearerToken { get; set; }

//        //        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        //        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UploadFile/{uploadedFile}")]HttpRequestMessage req, string uploadedFile, TraceWriter log)

//        //class UploadedFile
//        //{
//        //    public byte[] File { get; set; }
//        //}

//        [FunctionName("GetAzureADAuthTokenConsolidated")]
//        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {

//            var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();

//            // var uploadedFileAsByteArray = Convert.FromBase64String(myUploadedFile.File);

//            //var ADToken = resultObject.access_token;

//            HttpClient httpClient = new HttpClient();




//            //ASSIGN ADToken TO THE PROPERTY ADBearerToken
//            //ADBearerToken = ADToken;


//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));
//            myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage httpResponseMessageWithADToken = await httpClient.SendAsync(myRequest);

//            //EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
//            var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
//            var ADToken = resultObject.access_token;

//            //ASSIGN ADToken TO THE PROPERTY ADBearerToken
//            ADBearerToken = ADToken;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            //   return httpResponseMessageWithADToken;
//            //}

//            ////////////////////////////////////////////////////////////////////////////////
//            // PostCreateAnAsset
//            ////////////////////////////////////////////////////////////////////////////////

//            //[FunctionName("PostCreateAnAsset")]
//            //public static async Task<HttpResponseMessage> RunPostCreateAnAsset([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //{
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            //HttpClient httpClient = new HttpClient();


//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADToken);
//            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST


//            HttpRequestMessage myPostCreateAssetRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
//            string PostCreateAnAssetjsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//            myPostCreateAssetRequest.Content = new StringContent(PostCreateAnAssetjsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage myPostCreateAssetResponseMessage = await httpClient.SendAsync(myPostCreateAssetRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var PostCreateAssetStringResult = myPostCreateAssetResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var myPostCreateAssetResponseMessageResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(PostCreateAssetStringResult);

//            var myPostCreateAssetRequestdObjectResults = myPostCreateAssetResponseMessageResultObject.d;
//            var myPostCreateAssetRequestdObjectResultsResultId = myPostCreateAssetRequestdObjectResults.Id;

//            ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //ReturnedCreateAnAssetDObject = myPostCreateAssetRequestdObjectResults;
//            //ReturnedCreateAnAssetId = myPostCreateAssetRequestdObjectResultsResultId;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            //return webServicesAPIResponseMessage;


//            ////////////////////////////////////////////////////////////////////////////////
//            // PostCreateAnAssetFile
//            ////////////////////////////////////////////////////////////////////////////////

//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myPostCreateAnAssetFileRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//            {
//                IsEncrypted = "false",
//                IsPrimary = "true",
//                MimeType = "video/mp4",
//                Name = "TestVideo.mp4",
//                //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//                ParentAssetId = myPostCreateAssetRequestdObjectResultsResultId

//            };

//            string myPostCreateAnAssetFilejsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//            myPostCreateAnAssetFileRequest.Content = new StringContent(myPostCreateAnAssetFilejsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage myPostCreateAnAssetFileResponseMessage = await httpClient.SendAsync(myPostCreateAnAssetFileRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var myPostCreateAnAssetFileResponseMessagestringResult = myPostCreateAnAssetFileResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var myPostCreateAnAssetFileResponseMessageresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(myPostCreateAnAssetFileResponseMessagestringResult);

//            var myPostCreateAnAssetFiledObjectResults = myPostCreateAnAssetFileResponseMessageresultObject.d;
//            var myPostCreateAnAssetFilecreateAssetFileId = myPostCreateAnAssetFiledObjectResults.Id;


//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //           ReturnedCreateAnAssetFileDObject = myPostCreateAnAssetFiledObjectResults;
//            //           ReturnedCreateAnAssetFileId = myPostCreateAnAssetFilecreateAssetFileId;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            //            return webServicesAPIResponseMessage;


//            ////////////////////////////////////////////////////////////////////////////////
//            // PostCreateAccessPolicy
//            ////////////////////////////////////////////////////////////////////////////////

//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myPostCreateAccessPolicyRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//            string myPostCreateAccessPolicyjsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//            myPostCreateAccessPolicyRequest.Content = new StringContent(myPostCreateAccessPolicyjsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage myPostCreateAccessPolicyResponseMessage = await httpClient.SendAsync(myPostCreateAccessPolicyRequest);


//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var myPostCreateAccessPolicystringResult = myPostCreateAccessPolicyResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var myPostCreateAccessPolicyresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(myPostCreateAccessPolicystringResult);

//            var myPostCreateAccessPolicydObjectResults = myPostCreateAccessPolicyresultObject.d;
//            var myPostCreateAccessPolicyaccessPolicyIdResults = myPostCreateAccessPolicydObjectResults.Id;


//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateAccessPolicyDObject = myPostCreateAccessPolicydObjectResults;
//            ReturnedCreateAnAssetPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults;
//            // ReturnedAssetId = 

//            //RETURN THE HTTP RESPONSE MESSAGE
//            //       return webServicesAPIResponseMessage;


//            ////////////////////////////////////////////////////////////////////////////////
//            // PostCreateLocator
//            ////////////////////////////////////////////////////////////////////////////////

//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }


//            //  Bearer Token
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADToken);
//            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myPostCreateLocatorRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//            {
//                // AccessPolicyId = "nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
//                //AssetId = "nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
//                //AccessPolicyId = ReturnedCreateAnAssetPolicyId,
//                AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults,
//                //AssetId = ReturnedCreateAnAssetId,
//                AssetId = myPostCreateAssetRequestdObjectResultsResultId,
//                //                AssetId = myPostCreateAnAssetFilecreateAssetFileId,

//                StartTime = DateTime.Now, //.AddSeconds(30),
//                Type = 1
//            };
//            string myPostCreateLocatorjsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//            myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage myPostCreateLocatorResponseMessage = await httpClient.SendAsync(myPostCreateLocatorRequest);


//            //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
//            //serializerSettings.Converters.Add(new IsoDateTimeConverter());

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var myPostCreateLocatorstringResult = myPostCreateLocatorResponseMessage.Content.ReadAsStringAsync().Result;
//            //var result = webServicesAPIResponseMessage.Content.ReadAsAsync<FunctionApp4.DataModels.CreateLocator.RootObject>(serializerSettings);




//            //GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);


//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var myPostCreateLocatorresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(myPostCreateLocatorstringResult);

//            var myPostCreateLocatordObjectResults = myPostCreateLocatorresultObject.d;
//            var myPostCreateLocatorlocatorResults = myPostCreateLocatordObjectResults.Id;

//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateLocatorDObject = myPostCreateLocatordObjectResults;
//            ReturnedCreateLocatorId = myPostCreateLocatorlocatorResults;

//            ReturnedBaseUri = myPostCreateLocatordObjectResults.BaseUri;
//            ReturnedContentAccessComponent = myPostCreateLocatordObjectResults.ContentAccessComponent;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            // return webServicesAPIResponseMessage;
//            //return myPostCreateLocatorResponseMessage;


//            ///////////////////////////////////
//            ///// UPLOAD TO BLOB STORAGE
//            //////////////////////////////////

//            if (httpClient.DefaultRequestHeaders != null)
//            {
//                httpClient.DefaultRequestHeaders.Clear();
//            }
//            string nsString = "2017-04-17"

//            string XamCamStorageKey = "xamcamstorage:N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";

//            var dateTimeNowStringLongFormat = DateTime.UtcNow;
//            //string convertDateTimeToParameter(DateTime dateTime) =>
//            //string dateTimeNowCorrectFormat = dateTimeNowStringLongFormat.ToString("yyyyMMddThhmmZ");
//            string dateTimeNowCorrectFormatR = dateTimeNowStringLongFormat.ToString("R", System.Globalization.CultureInfo.Invariant);
//            //  Bearer Token
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey", XamCamStorageKey);
//            httpClient.DefaultRequestHeaders.Add("x-ms-version", nsString); // "2017-04-17");
//            //            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-date", "2017-08-15");
//            httpClient.DefaultRequestHeaders.Add("x-ms-date", dateTimeNowCorrectFormatR);

//            //string watsonsFileName = "myFile.mp3";
//            string CanonResource = String.Format("{0}/{1}{2}", ReturnedBaseUri, myUploadedFile.FileName, ReturnedContentAccessComponent);

//            string StringToSign =
//               "Put" + "\n" +
//               //Content - Encoding 
//               +"\n" +
//               //Content - Language 
//               +"\n" +
//               //Content - Length 
//               +"\n" +
//               //Content - MD5 
//               +"\n" +
//               //Content - Type
//               "application/json"
//               + "\n" +
//               //Date 
//               +"\n" +
//               //If - Modified - Since 
//               +"\n" +
//               //If - Match 
//               +"\n" +
//               //If - None - Match 
//               +"\n" +
//               //If - Unmodified - Since 
//               +"\n" +
//               //Range 
//               +"\n" +
//               //CanonicalizedHeaders 
//               "x-ms-date:" + dateTimeNowCorrectFormatR + //Sat, 21 Feb 2015 00:48:38 GMT
//               "x-ms-version:" + nsString
//                +
//                CanonResource;

//            //CanonicalizedResource;

//            //            Signature = Base64(HMAC - SHA256(UTF8(StringToSign)))


//            //CREATE HTTP REQUEST
//            HttpRequestMessage myUploadToBlobStorageRequest = new HttpRequestMessage(HttpMethod.Put, String.Format("{0}/{1}{2}", ReturnedBaseUri, myUploadedFile.FileName, ReturnedContentAccessComponent));  // String.Format("https://xamcamstorage.blob.core.windows.net/asset-e7c7ce9e-c127-43ea-8457-4d81f4852a29"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody thePutUploadFileToBlobStorage =
//                new FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody
//                {
//                    FileViaByteArray = myUploadedFile.File
//                };




//            string UploadToBlobStoragejsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage);
//            //        myRequest.Content = new StringContent(string UploadToBlobStoragejsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage, Encoding.UTF8, "application/json");

//            myUploadToBlobStorageRequest.Content = new StringContent(UploadToBlobStoragejsonBody, Encoding.UTF8, "application/json");

//            // myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");


//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage UploadToBlobStorageResponseMessage = await httpClient.SendAsync(myUploadToBlobStorageRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult2 = UploadToBlobStorageResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            //var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.UploadFileToBlobStorage.RootObject>(stringResult);

//            //var dObjectResults = resultObject.d;
//            //var locatorResults = dObjectResults.Id;

//            ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //ReturnedUploadFileToBlobStorage = dObjectResults;
//            //ReturnedReturnedUploadFileToBlobStorageId = locatorResults;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return UploadToBlobStorageResponseMessage;







//        }



//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;
//using System.Text;
//using System;
//using Newtonsoft.Json;
//using FunctionApp4.DataModels;

//namespace FunctionApp4
//    {
//        public static class ICCFunctionsConsolidated
//        {
//            [FunctionName("HttpTriggerCSharpConsolidated")]
//            public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            {
//                log.Info("C# HTTP trigger function processed a request.");

//                // parse query parameter
//                string name = req.GetQueryNameValuePairs()
//                    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
//                    .Value;

//                // Get request body
//                dynamic data = await req.Content.ReadAsAsync<object>();

//                // Set name to query string or body data
//                name = name ?? data?.name;

//                return name == null
//                    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
//                    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
//            }


//            //CONSTANTS NEEDED FOR AZURE AD
//            static string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
//            static string GrantType = "client_credentials";
//            static string ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
//            static string ClientID = "8d631792-ed10-46aa-bd09-b8ca1641bc6f";
//            static string RequestedResource = "https://rest.media.azure.net";

//            //STATIC HTTPCLIENT
//            static HttpClient ICCHttpClient { get; set; }

//            //STATIC AD BEARER TOKEN
//            static String ADBearerToken { get; set; }

//            //        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UploadFile/{uploadedFile}")]HttpRequestMessage req, string uploadedFile, TraceWriter log)

//            //class UploadedFile
//            //{
//            //    public byte[] File { get; set; }
//            //}

//            [FunctionName("GetAzureADAuthTokenConsolidated")]
//            public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            {

//                var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();

//                // var uploadedFileAsByteArray = Convert.FromBase64String(myUploadedFile.File);

//                //var ADToken = resultObject.access_token;

//                HttpClient httpClient = new HttpClient();




//                //ASSIGN ADToken TO THE PROPERTY ADBearerToken
//                //ADBearerToken = ADToken;


//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }

//                //CREATE HTTP REQUEST
//                HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));
//                myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage httpResponseMessageWithADToken = await httpClient.SendAsync(myRequest);

//                //EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
//                var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
//                var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
//                var ADToken = resultObject.access_token;

//                //ASSIGN ADToken TO THE PROPERTY ADBearerToken
//                ADBearerToken = ADToken;

//                //RETURN THE HTTP RESPONSE MESSAGE
//                //   return httpResponseMessageWithADToken;
//                //}

//                ////////////////////////////////////////////////////////////////////////////////
//                // PostCreateAnAsset
//                ////////////////////////////////////////////////////////////////////////////////

//                //[FunctionName("PostCreateAnAsset")]
//                //public static async Task<HttpResponseMessage> RunPostCreateAnAsset([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//                //{
//                //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//                //HttpClient httpClient = new HttpClient();


//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }

//                //  Bearer Token
//                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADToken);
//                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//                httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//                httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//                //CREATE HTTP REQUEST


//                HttpRequestMessage myPostCreateAssetRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//                //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//                FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
//                string PostCreateAnAssetjsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//                myPostCreateAssetRequest.Content = new StringContent(PostCreateAnAssetjsonBody, Encoding.UTF8, "application/json");

//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage myPostCreateAssetResponseMessage = await httpClient.SendAsync(myPostCreateAssetRequest);

//                //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//                var PostCreateAssetStringResult = myPostCreateAssetResponseMessage.Content.ReadAsStringAsync().Result;

//                //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//                var myPostCreateAssetResponseMessageResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(PostCreateAssetStringResult);

//                var myPostCreateAssetRequestdObjectResults = myPostCreateAssetResponseMessageResultObject.d;
//                var myPostCreateAssetRequestdObjectResultsResultId = myPostCreateAssetRequestdObjectResults.Id;

//                ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//                //ReturnedCreateAnAssetDObject = myPostCreateAssetRequestdObjectResults;
//                //ReturnedCreateAnAssetId = myPostCreateAssetRequestdObjectResultsResultId;

//                //RETURN THE HTTP RESPONSE MESSAGE
//                //return webServicesAPIResponseMessage;


//                ////////////////////////////////////////////////////////////////////////////////
//                // PostCreateAnAssetFile
//                ////////////////////////////////////////////////////////////////////////////////

//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }

//                //  Bearer Token
//                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//                httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//                httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//                //CREATE HTTP REQUEST
//                HttpRequestMessage myPostCreateAnAssetFileRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//                //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//                FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//                {
//                    IsEncrypted = "false",
//                    IsPrimary = "true",
//                    MimeType = "video/mp4",
//                    Name = "TestVideo.mp4",
//                    //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//                    ParentAssetId = myPostCreateAssetRequestdObjectResultsResultId

//                };

//                string myPostCreateAnAssetFilejsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//                myPostCreateAnAssetFileRequest.Content = new StringContent(myPostCreateAnAssetFilejsonBody, Encoding.UTF8, "application/json");

//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage myPostCreateAnAssetFileResponseMessage = await httpClient.SendAsync(myPostCreateAnAssetFileRequest);

//                //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//                var myPostCreateAnAssetFileResponseMessagestringResult = myPostCreateAnAssetFileResponseMessage.Content.ReadAsStringAsync().Result;

//                //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//                var myPostCreateAnAssetFileResponseMessageresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(myPostCreateAnAssetFileResponseMessagestringResult);

//                var myPostCreateAnAssetFiledObjectResults = myPostCreateAnAssetFileResponseMessageresultObject.d;
//                var myPostCreateAnAssetFilecreateAssetFileId = myPostCreateAnAssetFiledObjectResults.Id;


//                //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//                //           ReturnedCreateAnAssetFileDObject = myPostCreateAnAssetFiledObjectResults;
//                //           ReturnedCreateAnAssetFileId = myPostCreateAnAssetFilecreateAssetFileId;

//                //RETURN THE HTTP RESPONSE MESSAGE
//                //            return webServicesAPIResponseMessage;


//                ////////////////////////////////////////////////////////////////////////////////
//                // PostCreateAccessPolicy
//                ////////////////////////////////////////////////////////////////////////////////

//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }

//                //  Bearer Token
//                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//                httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//                httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//                //CREATE HTTP REQUEST
//                HttpRequestMessage myPostCreateAccessPolicyRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//                //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//                FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//                string myPostCreateAccessPolicyjsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//                myPostCreateAccessPolicyRequest.Content = new StringContent(myPostCreateAccessPolicyjsonBody, Encoding.UTF8, "application/json");

//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage myPostCreateAccessPolicyResponseMessage = await httpClient.SendAsync(myPostCreateAccessPolicyRequest);


//                //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//                var myPostCreateAccessPolicystringResult = myPostCreateAccessPolicyResponseMessage.Content.ReadAsStringAsync().Result;

//                //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//                var myPostCreateAccessPolicyresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(myPostCreateAccessPolicystringResult);

//                var myPostCreateAccessPolicydObjectResults = myPostCreateAccessPolicyresultObject.d;
//                var myPostCreateAccessPolicyaccessPolicyIdResults = myPostCreateAccessPolicydObjectResults.Id;


//                //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//                ReturnedCreateAccessPolicyDObject = myPostCreateAccessPolicydObjectResults;
//                ReturnedCreateAnAssetPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults;
//                // ReturnedAssetId = 

//                //RETURN THE HTTP RESPONSE MESSAGE
//                //       return webServicesAPIResponseMessage;


//                ////////////////////////////////////////////////////////////////////////////////
//                // PostCreateLocator
//                ////////////////////////////////////////////////////////////////////////////////

//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }


//                //  Bearer Token
//                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADToken);
//                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//                httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//                httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//                //CREATE HTTP REQUEST
//                HttpRequestMessage myPostCreateLocatorRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//                //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//                FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//                {
//                    // AccessPolicyId = "nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
//                    //AssetId = "nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
//                    //AccessPolicyId = ReturnedCreateAnAssetPolicyId,
//                    AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults,
//                    //AssetId = ReturnedCreateAnAssetId,
//                    AssetId = myPostCreateAssetRequestdObjectResultsResultId,
//                    //                AssetId = myPostCreateAnAssetFilecreateAssetFileId,

//                    StartTime = DateTime.Now, //.AddSeconds(30),
//                    Type = 1
//                };
//                string myPostCreateLocatorjsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//                myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");

//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage myPostCreateLocatorResponseMessage = await httpClient.SendAsync(myPostCreateLocatorRequest);


//                //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
//                //serializerSettings.Converters.Add(new IsoDateTimeConverter());

//                //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//                var myPostCreateLocatorstringResult = myPostCreateLocatorResponseMessage.Content.ReadAsStringAsync().Result;
//                //var result = webServicesAPIResponseMessage.Content.ReadAsAsync<FunctionApp4.DataModels.CreateLocator.RootObject>(serializerSettings);




//                //GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);


//                //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//                var myPostCreateLocatorresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(myPostCreateLocatorstringResult);

//                var myPostCreateLocatordObjectResults = myPostCreateLocatorresultObject.d;
//                var myPostCreateLocatorlocatorResults = myPostCreateLocatordObjectResults.Id;

//                //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//                ReturnedCreateLocatorDObject = myPostCreateLocatordObjectResults;
//                ReturnedCreateLocatorId = myPostCreateLocatorlocatorResults;

//                ReturnedBaseUri = myPostCreateLocatordObjectResults.BaseUri;
//                ReturnedContentAccessComponent = myPostCreateLocatordObjectResults.ContentAccessComponent;

//                //RETURN THE HTTP RESPONSE MESSAGE
//                // return webServicesAPIResponseMessage;
//                //return myPostCreateLocatorResponseMessage;


//                ///////////////////////////////////
//                ///// UPLOAD TO BLOB STORAGE
//                //////////////////////////////////

//                if (httpClient.DefaultRequestHeaders != null)
//                {
//                    httpClient.DefaultRequestHeaders.Clear();
//                }
//                string nsString = "2017-04-17"
    
//            string XamCamStorageKey = "xamcamstorage:N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";

//                string accountKey = "N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";
//                string accountName = "xamcamstorage";

//                var dateTimeNowStringLongFormat = DateTime.UtcNow;
//                //string convertDateTimeToParameter(DateTime dateTime) =>
//                //string dateTimeNowCorrectFormat = dateTimeNowStringLongFormat.ToString("yyyyMMddThhmmZ");
//                string dateTimeNowCorrectFormatR = dateTimeNowStringLongFormat.ToString("R", System.Globalization.CultureInfo.Invariant);
//                //  Bearer Token
//                //            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey", XamCamStorageKey);
//                httpClient.DefaultRequestHeaders.Add("x-ms-version", nsString); // "2017-04-17");
//                                                                                //            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-date", "2017-08-15");
//                httpClient.DefaultRequestHeaders.Add("x-ms-date", dateTimeNowCorrectFormatR);

//                //string watsonsFileName = "myFile.mp3";
//                string CanonResource = String.Format("{0}/{1}{2}", ReturnedBaseUri, myUploadedFile.FileName, ReturnedContentAccessComponent);

//                string StringToSign =
//                   "Put" + "\n" +
//                   //Content - Encoding 
//                   +"\n" +
//                   //Content - Language 
//                   +"\n" +
//                   //Content - Length 
//                   +"\n" +
//                   //Content - MD5 
//                   +"\n" +
//                   //Content - Type
//                   "application/json"
//                   + "\n" +
//                   //Date 
//                   +"\n" +
//                   //If - Modified - Since 
//                   +"\n" +
//                   //If - Match 
//                   +"\n" +
//                   //If - None - Match 
//                   +"\n" +
//                   //If - Unmodified - Since 
//                   +"\n" +
//                   //Range 
//                   +"\n" +
//                   //CanonicalizedHeaders 
//                   "x-ms-date:" + dateTimeNowCorrectFormatR + //Sat, 21 Feb 2015 00:48:38 GMT
//                   "x-ms-version:" + nsString
//                    +
//                    CanonResource;

//                //CanonicalizedResource;

//                string authorizationHeader = GenerateSharedKey(stringToSign, accountKey, accountName);

//                //            Signature = Base64(HMAC - SHA256(UTF8(StringToSign)))

//                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey", $"{accountName}:{authorizationHeader}");


//                //CREATE HTTP REQUEST
//                HttpRequestMessage myUploadToBlobStorageRequest = new HttpRequestMessage(HttpMethod.Put, String.Format("{0}/{1}{2}", ReturnedBaseUri, myUploadedFile.FileName, ReturnedContentAccessComponent));  // String.Format("https://xamcamstorage.blob.core.windows.net/asset-e7c7ce9e-c127-43ea-8457-4d81f4852a29"));

//                //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//                FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody thePutUploadFileToBlobStorage =
//                    new FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody
//                    {
//                        FileViaByteArray = myUploadedFile.File
//                    };




//                string UploadToBlobStoragejsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage);
//                //        myRequest.Content = new StringContent(string UploadToBlobStoragejsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage, Encoding.UTF8, "application/json");

//                myUploadToBlobStorageRequest.Content = new StringContent(UploadToBlobStoragejsonBody, Encoding.UTF8, "application/json");

//                // myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");


//                //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//                HttpResponseMessage UploadToBlobStorageResponseMessage = await httpClient.SendAsync(myUploadToBlobStorageRequest);

//                //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//                var stringResult2 = UploadToBlobStorageResponseMessage.Content.ReadAsStringAsync().Result;

//                //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//                //var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.UploadFileToBlobStorage.RootObject>(stringResult);

//                //var dObjectResults = resultObject.d;
//                //var locatorResults = dObjectResults.Id;

//                ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//                //ReturnedUploadFileToBlobStorage = dObjectResults;
//                //ReturnedReturnedUploadFileToBlobStorageId = locatorResults;

//                //RETURN THE HTTP RESPONSE MESSAGE
//                return UploadToBlobStorageResponseMessage;







//            }

//            private static string GenerateSharedKey(string stringToSign, string key, string account)
//            {
//                string signature;
//                var unicodeKey = Convert.FromBase64String(key);
//                using (var hmacSha256 = new HMACSHA256(unicodeKey))
//                {
//                    var dataToHmac = Encoding.UTF8.GetBytes(stringToSign);
//                    signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
//                }
//                return string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "SharedKey", account, signature);
//            }



//            //static FunctionApp4.DataModels.CreateAnAsset.D ReturnedCreateAnAssetDObject { get; set; }
//            //static string ReturnedCreateAnAssetId { get; set; }

//            //[FunctionName("PostCreateAnAsset")]
//            //public static async Task<HttpResponseMessage> RunPostCreateAnAsset([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //{
//            //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            //    HttpClient httpClient;

//            //    if (ICCHttpClient == null)
//            //    {
//            //        httpClient = new HttpClient();
//            //        ICCHttpClient = httpClient;
//            //    }

//            //    if (ICCHttpClient.DefaultRequestHeaders != null)
//            //    {
//            //        ICCHttpClient.DefaultRequestHeaders.Clear();
//            //    }

//            //    //  Bearer Token
//            //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //    //CREATE HTTP REQUEST
//            //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//            //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            //    FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
//            //    string jsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//            //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(stringResult);

//            //    var dObjectResults = resultObject.d;
//            //    var ResultId = dObjectResults.Id;

//            //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //    ReturnedCreateAnAssetDObject = dObjectResults;
//            //    ReturnedCreateAnAssetId = ResultId;

//            //    //RETURN THE HTTP RESPONSE MESSAGE
//            //    return webServicesAPIResponseMessage;
//            //}

//            //static FunctionApp4.DataModels.CreateAssetFile.D ReturnedCreateAnAssetFileDObject { get; set; }
//            //static string ReturnedCreateAnAssetFileId { get; set; }

//            //[FunctionName("PostCreateAnAssetFile")]
//            //public static async Task<HttpResponseMessage> RunPostCreateAnAssetFile([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //{
//            //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            //    HttpClient httpClient;

//            //    if (ICCHttpClient == null)
//            //    {
//            //        httpClient = new HttpClient();
//            //        ICCHttpClient = httpClient;
//            //    }

//            //    if (ICCHttpClient.DefaultRequestHeaders != null)
//            //    {
//            //        ICCHttpClient.DefaultRequestHeaders.Clear();
//            //    }

//            //    //  Bearer Token
//            //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //    //CREATE HTTP REQUEST
//            //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//            //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            //    FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//            //    {
//            //        IsEncrypted = "false",
//            //        IsPrimary = "true",
//            //        MimeType = "video/mp4",
//            //        Name = "TestVideo.mp4",
//            //        //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//            //        ParentAssetId = ReturnedCreateAnAssetId

//            //    };

//            //    string jsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//            //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(stringResult);

//            //    var dObjectResults = resultObject.d;
//            //    var createAssetFileId = dObjectResults.Id;


//            //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //    ReturnedCreateAnAssetFileDObject = dObjectResults;
//            //    ReturnedCreateAnAssetFileId = createAssetFileId;
//            //    // ReturnedAssetId = 

//            //    //RETURN THE HTTP RESPONSE MESSAGE
//            //    return webServicesAPIResponseMessage;

//            //}


//            static FunctionApp4.DataModels.CreateAccessPolicy.D ReturnedCreateAccessPolicyDObject { get; set; }
//            static string ReturnedCreateAnAssetPolicyId { get; set; }

//            //[FunctionName("PostCreateAccessPolicy")]
//            //public static async Task<HttpResponseMessage> RunPostCreateAccessPolicy([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //{
//            //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            //    HttpClient httpClient;

//            //    if (ICCHttpClient == null)
//            //    {
//            //        httpClient = new HttpClient();
//            //        ICCHttpClient = httpClient;
//            //    }

//            //    if (ICCHttpClient.DefaultRequestHeaders != null)
//            //    {
//            //        ICCHttpClient.DefaultRequestHeaders.Clear();
//            //    }

//            //    //  Bearer Token
//            //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //    //CREATE HTTP REQUEST
//            //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//            //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            //    FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//            //    string jsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//            //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(stringResult);

//            //    var dObjectResults = resultObject.d;
//            //    var accessPolicyIdResults = dObjectResults.Id;


//            //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //    ReturnedCreateAccessPolicyDObject = dObjectResults;
//            //    ReturnedCreateAnAssetPolicyId = accessPolicyIdResults;
//            //    // ReturnedAssetId = 

//            //    //RETURN THE HTTP RESPONSE MESSAGE
//            //    return webServicesAPIResponseMessage;
//            //}

//            static FunctionApp4.DataModels.CreateLocator.D ReturnedCreateLocatorDObject { get; set; }
//            static string ReturnedCreateLocatorId { get; set; }
//            static string ReturnedBaseUri { get; set; }
//            static string ReturnedContentAccessComponent { get; set; }



//            // public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Hello/{id}")]HttpRequestMessage req, string id, TraceWriter log)



//            //[FunctionName("PostCreateLocator")]
//            //public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//            //{
//            //    ////INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            //    //HttpClient httpClient;

//            //    //if (ICCHttpClient == null)
//            //    //{
//            //    //    httpClient = new HttpClient();
//            //    //    ICCHttpClient = httpClient;
//            //    //}

//            //    //if (ICCHttpClient.DefaultRequestHeaders != null)
//            //    //{
//            //    //    ICCHttpClient.DefaultRequestHeaders.Clear();
//            //    //}

//            //    //  Bearer Token
//            //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");


//            //    //CREATE HTTP REQUEST
//            //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//            //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            //    FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//            //    {
//            //        // AccessPolicyId = "nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
//            //        //AssetId = "nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
//            //        AccessPolicyId = ReturnedCreateAnAssetPolicyId,
//            //        AssetId = ReturnedCreateAnAssetId,

//            //        StartTime = DateTime.Now.AddSeconds(30),
//            //        Type = 1
//            //    };
//            //    string jsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//            //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);


//            //    //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
//            //    //serializerSettings.Converters.Add(new IsoDateTimeConverter());

//            //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;
//            //    //var result = webServicesAPIResponseMessage.Content.ReadAsAsync<FunctionApp4.DataModels.CreateLocator.RootObject>(serializerSettings);




//            //    //GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);


//            //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(stringResult);

//            //    var dObjectResults = resultObject.d;
//            //    var locatorResults = dObjectResults.Id;

//            //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            //    ReturnedCreateLocatorDObject = dObjectResults;
//            //    ReturnedCreateLocatorId = locatorResults;

//            //    //RETURN THE HTTP RESPONSE MESSAGE
//            //    return webServicesAPIResponseMessage;
//            //}


















//        }
//    }


//    //static FunctionApp4.DataModels.CreateAnAsset.D ReturnedCreateAnAssetDObject { get; set; }
//    //static string ReturnedCreateAnAssetId { get; set; }

//    //[FunctionName("PostCreateAnAsset")]
//    //public static async Task<HttpResponseMessage> RunPostCreateAnAsset([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//    //{
//    //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//    //    HttpClient httpClient;

//    //    if (ICCHttpClient == null)
//    //    {
//    //        httpClient = new HttpClient();
//    //        ICCHttpClient = httpClient;
//    //    }

//    //    if (ICCHttpClient.DefaultRequestHeaders != null)
//    //    {
//    //        ICCHttpClient.DefaultRequestHeaders.Clear();
//    //    }

//    //    //  Bearer Token
//    //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//    //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//    //    //CREATE HTTP REQUEST
//    //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//    //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//    //    FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
//    //    string jsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//    //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//    //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//    //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//    //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//    //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//    //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//    //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(stringResult);

//    //    var dObjectResults = resultObject.d;
//    //    var ResultId = dObjectResults.Id;

//    //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//    //    ReturnedCreateAnAssetDObject = dObjectResults;
//    //    ReturnedCreateAnAssetId = ResultId;

//    //    //RETURN THE HTTP RESPONSE MESSAGE
//    //    return webServicesAPIResponseMessage;
//    //}

//    //static FunctionApp4.DataModels.CreateAssetFile.D ReturnedCreateAnAssetFileDObject { get; set; }
//    //static string ReturnedCreateAnAssetFileId { get; set; }

//    //[FunctionName("PostCreateAnAssetFile")]
//    //public static async Task<HttpResponseMessage> RunPostCreateAnAssetFile([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//    //{
//    //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//    //    HttpClient httpClient;

//    //    if (ICCHttpClient == null)
//    //    {
//    //        httpClient = new HttpClient();
//    //        ICCHttpClient = httpClient;
//    //    }

//    //    if (ICCHttpClient.DefaultRequestHeaders != null)
//    //    {
//    //        ICCHttpClient.DefaultRequestHeaders.Clear();
//    //    }

//    //    //  Bearer Token
//    //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//    //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//    //    //CREATE HTTP REQUEST
//    //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//    //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//    //    FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//    //    {
//    //        IsEncrypted = "false",
//    //        IsPrimary = "true",
//    //        MimeType = "video/mp4",
//    //        Name = "TestVideo.mp4",
//    //        //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//    //        ParentAssetId = ReturnedCreateAnAssetId

//    //    };

//    //    string jsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//    //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//    //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//    //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//    //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//    //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//    //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//    //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(stringResult);

//    //    var dObjectResults = resultObject.d;
//    //    var createAssetFileId = dObjectResults.Id;


//    //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//    //    ReturnedCreateAnAssetFileDObject = dObjectResults;
//    //    ReturnedCreateAnAssetFileId = createAssetFileId;
//    //    // ReturnedAssetId = 

//    //    //RETURN THE HTTP RESPONSE MESSAGE
//    //    return webServicesAPIResponseMessage;

//    //}


//    static FunctionApp4.DataModels.CreateAccessPolicy.D ReturnedCreateAccessPolicyDObject { get; set; }
//    static string ReturnedCreateAnAssetPolicyId { get; set; }

//    //[FunctionName("PostCreateAccessPolicy")]
//    //public static async Task<HttpResponseMessage> RunPostCreateAccessPolicy([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//    //{
//    //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//    //    HttpClient httpClient;

//    //    if (ICCHttpClient == null)
//    //    {
//    //        httpClient = new HttpClient();
//    //        ICCHttpClient = httpClient;
//    //    }

//    //    if (ICCHttpClient.DefaultRequestHeaders != null)
//    //    {
//    //        ICCHttpClient.DefaultRequestHeaders.Clear();
//    //    }

//    //    //  Bearer Token
//    //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//    //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//    //    //CREATE HTTP REQUEST
//    //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//    //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//    //    FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//    //    string jsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//    //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//    //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//    //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//    //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//    //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//    //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//    //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(stringResult);

//    //    var dObjectResults = resultObject.d;
//    //    var accessPolicyIdResults = dObjectResults.Id;


//    //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//    //    ReturnedCreateAccessPolicyDObject = dObjectResults;
//    //    ReturnedCreateAnAssetPolicyId = accessPolicyIdResults;
//    //    // ReturnedAssetId = 

//    //    //RETURN THE HTTP RESPONSE MESSAGE
//    //    return webServicesAPIResponseMessage;
//    //}

//    static FunctionApp4.DataModels.CreateLocator.D ReturnedCreateLocatorDObject { get; set; }
//    static string ReturnedCreateLocatorId { get; set; }
//    static string ReturnedBaseUri { get; set; }
//    static string ReturnedContentAccessComponent { get; set; }



//    // public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Hello/{id}")]HttpRequestMessage req, string id, TraceWriter log)



//    //[FunctionName("PostCreateLocator")]
//    //public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//    //{
//    //    ////INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//    //    //HttpClient httpClient;

//    //    //if (ICCHttpClient == null)
//    //    //{
//    //    //    httpClient = new HttpClient();
//    //    //    ICCHttpClient = httpClient;
//    //    //}

//    //    //if (ICCHttpClient.DefaultRequestHeaders != null)
//    //    //{
//    //    //    ICCHttpClient.DefaultRequestHeaders.Clear();
//    //    //}

//    //    //  Bearer Token
//    //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//    //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//    //    ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");


//    //    //CREATE HTTP REQUEST
//    //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//    //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//    //    FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//    //    {
//    //        // AccessPolicyId = "nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
//    //        //AssetId = "nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
//    //        AccessPolicyId = ReturnedCreateAnAssetPolicyId,
//    //        AssetId = ReturnedCreateAnAssetId,

//    //        StartTime = DateTime.Now.AddSeconds(30),
//    //        Type = 1
//    //    };
//    //    string jsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//    //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//    //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//    //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);


//    //    //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
//    //    //serializerSettings.Converters.Add(new IsoDateTimeConverter());

//    //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//    //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;
//    //    //var result = webServicesAPIResponseMessage.Content.ReadAsAsync<FunctionApp4.DataModels.CreateLocator.RootObject>(serializerSettings);




//    //    //GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);


//    //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//    //    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(stringResult);

//    //    var dObjectResults = resultObject.d;
//    //    var locatorResults = dObjectResults.Id;

//    //    //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//    //    ReturnedCreateLocatorDObject = dObjectResults;
//    //    ReturnedCreateLocatorId = locatorResults;

//    //    //RETURN THE HTTP RESPONSE MESSAGE
//    //    return webServicesAPIResponseMessage;
//    //}


















//}
//}