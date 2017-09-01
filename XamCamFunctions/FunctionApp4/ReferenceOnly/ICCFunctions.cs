//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;
//using System.Text;
//using System;
//using FunctionApp4.DataModels;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

//namespace FunctionApp4
//{
//    public static class ICCFunctions
//    {
//        [FunctionName("HttpTriggerCSharp7")]
//        public static async Task<HttpResponseMessage> RunHttpTriggerCSharp7([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
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

//        [FunctionName("GetAzureADAuthTokenV1")]
//        public static async Task<HttpResponseMessage> RunGetAzureADTokenV1([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {

//            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

//            HttpClient httpClient1 = new HttpClient();
//            httpClient1.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token");
//            var ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";

//            myRequest.Content = new StringContent("grant_type=client_credentials" + "&client_id=8d631792-ed10-46aa-bd09-b8ca1641bc6f" + "&client_secret=" + ClientSecret + "&resource=https://rest.media.azure.net", Encoding.UTF8, "application/x-www-form-urlencoded");
            
//            HttpResponseMessage myhrm = await httpClient1.SendAsync(myRequest);

//            var result = myhrm.Content.ReadAsStringAsync().Result;
//            var myResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(result);

//            var ADToken = myResult.access_token;

//            var mystring = "hi";
        
//            return myhrm;

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

//        [FunctionName("GetAzureADAuthToken")]
//        public static async Task<HttpResponseMessage> RunGetAzureADToken([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {

//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null) {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }
       
////            ICCHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));            
//            myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage httpResponseMessageWithADToken = await ICCHttpClient.SendAsync(myRequest);

//            //EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
//            var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
//            var ADToken = resultObject.access_token;

//            //ASSIGN ADToken TO THE PROPERTY ADBearerToken
//            ADBearerToken = ADToken;
            
//            //RETURN THE HTTP RESPONSE MESSAGE
//            return httpResponseMessageWithADToken;
//        }



//        static D ReturnedWebServicesAPIDObject { get; set; }
//        static List<string> ReturnedWebServicesAPIEntitySets { get; set; }


//        [FunctionName("GetWebServicesAPI")]
//        public static async Task<HttpResponseMessage> RunGetWebServicesAPI([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null)
//            {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15" );
//            ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
            
//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, String.Format("https://mediaserviceshack.restv2.westus.media.azure.net/api"));

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.RootObject>(stringResult);
            
//            var dObjectResults = resultObject.d;
//            var entitySetsResults = dObjectResults.EntitySets;

//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedWebServicesAPIDObject = dObjectResults;
//            ReturnedWebServicesAPIEntitySets = entitySetsResults;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return webServicesAPIResponseMessage;
//        }

//        static FunctionApp4.DataModels.CreateAnAsset.D ReturnedCreateAnAssetDObject { get; set; }
//        static string ReturnedCreateAnAssetId { get; set; }
        
//        [FunctionName("PostCreateAnAsset")]
//        public static async Task<HttpResponseMessage> RunPostCreateAnAsset([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null)
//            {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0"};
//            string jsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//            myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(stringResult);

//            var dObjectResults = resultObject.d;
//            var ResultId = dObjectResults.Id;

//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateAnAssetDObject = dObjectResults;
//            ReturnedCreateAnAssetId = ResultId;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return webServicesAPIResponseMessage;
//        }

//        static FunctionApp4.DataModels.CreateAssetFile.D ReturnedCreateAnAssetFileDObject { get; set; }
//        static string ReturnedCreateAnAssetFileId { get; set; }

//        [FunctionName("PostCreateAnAssetFile")]
//        public static async Task<HttpResponseMessage> RunPostCreateAnAssetFile([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null)
//            {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//            { IsEncrypted = "false",
//                IsPrimary = "true",
//                MimeType = "video/mp4",
//                Name ="TestVideo.mp4",
//                //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//                ParentAssetId = ReturnedCreateAnAssetId

//            };

//            string jsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//            myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(stringResult);

//            var dObjectResults = resultObject.d;
//            var createAssetFileId = dObjectResults.Id;


//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateAnAssetFileDObject = dObjectResults;
//            ReturnedCreateAnAssetFileId = createAssetFileId;
//            // ReturnedAssetId = 

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return webServicesAPIResponseMessage;

//        }


//        static FunctionApp4.DataModels.CreateAccessPolicy.D ReturnedCreateAccessPolicyDObject { get; set; }
//        static string ReturnedCreateAnAssetPolicyId { get; set; }

//        [FunctionName("PostCreateAccessPolicy")]
//        public static async Task<HttpResponseMessage> RunPostCreateAccessPolicy([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null)
//            {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//            ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//            string jsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//            myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(stringResult);

//            var dObjectResults = resultObject.d;
//            var accessPolicyIdResults = dObjectResults.Id;


//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateAccessPolicyDObject = dObjectResults;
//            ReturnedCreateAnAssetPolicyId = accessPolicyIdResults;
//            // ReturnedAssetId = 

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return webServicesAPIResponseMessage;
//        }

//        static FunctionApp4.DataModels.CreateLocator.D ReturnedCreateLocatorDObject { get; set; }
//        static string ReturnedCreateLocatorId { get; set; }


//       // public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Hello/{id}")]HttpRequestMessage req, string id, TraceWriter log)



//        [FunctionName("PostCreateLocator")]
//        public static async Task<HttpResponseMessage> RunPostCreateLocator([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {
//            //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//            HttpClient httpClient;

//            if (ICCHttpClient == null)
//            {
//                httpClient = new HttpClient();
//                ICCHttpClient = httpClient;
//            }

//            if (ICCHttpClient.DefaultRequestHeaders != null)
//            {
//                ICCHttpClient.DefaultRequestHeaders.Clear();
//            }

//            //  Bearer Token
//            ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ADBearerToken);
//            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//            ICCHttpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//            ICCHttpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");


//            //CREATE HTTP REQUEST
//            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//            FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//            {
//                // AccessPolicyId = "nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
//                //AssetId = "nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
//                AccessPolicyId = ReturnedCreateAnAssetPolicyId,
//                AssetId = ReturnedCreateAnAssetId,

//                StartTime = DateTime.Now.AddSeconds(30),
//                Type = 1
//            };
//            string jsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//            myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//            HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);


//            //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
//            //serializerSettings.Converters.Add(new IsoDateTimeConverter());

//            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//            var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;
//            //var result = webServicesAPIResponseMessage.Content.ReadAsAsync<FunctionApp4.DataModels.CreateLocator.RootObject>(serializerSettings);




//            //GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);


//            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(stringResult);

//            var dObjectResults = resultObject.d;
//            var locatorResults = dObjectResults.Id;

//            //ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//            ReturnedCreateLocatorDObject = dObjectResults;
//            ReturnedCreateLocatorId = locatorResults;

//            //RETURN THE HTTP RESPONSE MESSAGE
//            return webServicesAPIResponseMessage;
//        }



//        //static FunctionApp4.DataModels.D ReturnedUploadFileToBlobStorage { get; set; }
//        //static string ReturnedReturnedUploadFileToBlobStorageId { get; set; }

//        //[FunctionName("PutUploadFileToBlobStorage")]
//        //public static async Task<HttpResponseMessage> RunPutUploadFileToBlobStorage([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        //{
//        //    //INITALIZE HTTP CLIENT IF NECESSARY, CLEAR OLD REQUEST HEADERS, AND INITALIZE RELEVANT REQUEST HEADERS
//        //    HttpClient httpClient;

//        //    if (ICCHttpClient == null)
//        //    {
//        //        httpClient = new HttpClient();
//        //        ICCHttpClient = httpClient;
//        //    }

//        //    if (ICCHttpClient.DefaultRequestHeaders != null)
//        //    {
//        //        ICCHttpClient.DefaultRequestHeaders.Clear();
//        //    }

//        //    string XamCamStorageKey = "xamcamstorage:N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";

//        //    //  Bearer Token
//        //    ICCHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey", XamCamStorageKey);
//        //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-version", "2017-04-17");
//        //    //            ICCHttpClient.DefaultRequestHeaders.Add("x-ms-date", "2017-08-15");
//        //    ICCHttpClient.DefaultRequestHeaders.Add("x-ms-date", "2017-08-23");


//        //    //CREATE HTTP REQUEST
//        //    HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Put, String.Format("https://xamcamstorage.blob.core.windows.net/asset-e7c7ce9e-c127-43ea-8457-4d81f4852a29"));

//        //    //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //    FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody thePutUploadFileToBlobStorage = new FunctionApp4.DataModels.UploadFileToBlobStorage.UploadFileToBlobStorageBody { BlogBase64String = "\"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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA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" };

//        //    string jsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage);
//        //    myRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//        //    //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //    HttpResponseMessage webServicesAPIResponseMessage = await ICCHttpClient.SendAsync(myRequest);

//        //    //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //    var stringResult = webServicesAPIResponseMessage.Content.ReadAsStringAsync().Result;

//        //    //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //    //var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.UploadFileToBlobStorage.RootObject>(stringResult);

//        //    //var dObjectResults = resultObject.d;
//        //    //var locatorResults = dObjectResults.Id;

//        //    ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
//        //    //ReturnedUploadFileToBlobStorage = dObjectResults;
//        //    //ReturnedReturnedUploadFileToBlobStorageId = locatorResults;

//        //    //RETURN THE HTTP RESPONSE MESSAGE
//        //    return webServicesAPIResponseMessage;

//        //}










//    }
//}