
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Text;
using System;
using Newtonsoft.Json;
using FunctionApp4.DataModels;
using System.Security.Cryptography;
using System.Globalization;
using FunctionApp4.DataModels.UploadFileToBlobStorage;

namespace FunctionApp4
{
    public static class ICCFunctionsConsolidated
    {
        //CONSTANTS NEEDED FOR AZURE AD
        static string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        static string GrantType = "client_credentials";
        static string ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
        static string ClientID = "8d631792-ed10-46aa-bd09-b8ca1641bc6f";
        static string RequestedResource = "https://rest.media.azure.net";


        //static FunctionApp4.DataModels.CreateAccessPolicy.D ReturnedCreateAccessPolicyDObject { get; set; }
        //static string ReturnedCreateAnAssetPolicyId { get; set; }
        //static FunctionApp4.DataModels.CreateLocator.D ReturnedCreateLocatorDObject { get; set; }
        //static string ReturnedCreateLocatorId { get; set; }
        //static string ReturnedBaseUri { get; set; }
        //static string ReturnedContentAccessComponent { get; set; }

        [FunctionName("GetAzureADAuthTokenConsolidated")]
        public static async Task<HttpResponseMessage> RunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();

            HttpClient httpClient = new HttpClient();

            //CREATE HTTP REQUEST
            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));
            myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage httpResponseMessageWithADToken = await httpClient.SendAsync(myRequest);

            //EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
            var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
            var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
            var azureADToken = resultObject.access_token;

            ////////////////////////////////////////////////////////////////////////////////
            // PostCreateAnAsset
            ////////////////////////////////////////////////////////////////////////////////


            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Clear();
            }

            //  Bearer Token
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            //CREATE HTTP REQUEST
            HttpRequestMessage myPostCreateAssetRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
            FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
            string PostCreateAnAssetjsonBody = JsonConvert.SerializeObject(createAnAssetBody);
            myPostCreateAssetRequest.Content = new StringContent(PostCreateAnAssetjsonBody, Encoding.UTF8, "application/json");

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage myPostCreateAssetResponseMessage = await httpClient.SendAsync(myPostCreateAssetRequest);

            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
            var PostCreateAssetStringResult = await myPostCreateAssetResponseMessage.Content.ReadAsStringAsync();

            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
            var myPostCreateAssetResponseMessageResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(PostCreateAssetStringResult);

            var myPostCreateAssetRequestdObjectResults = myPostCreateAssetResponseMessageResultObject.d;
            var myPostCreateAssetRequestdObjectResultsResultId = myPostCreateAssetRequestdObjectResults.Id;

            ////////////////////////////////////////////////////////////////////////////////
            // PostCreateAnAssetFile
            ////////////////////////////////////////////////////////////////////////////////

            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Clear();
            }

            //  Bearer Token
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            //CREATE HTTP REQUEST
            HttpRequestMessage myPostCreateAnAssetFileRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
            FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
            {
                IsEncrypted = "false",
                IsPrimary = "true",
                MimeType = "video/mp4",
                Name = "TestVideo.mp4",
                //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
                ParentAssetId = myPostCreateAssetRequestdObjectResultsResultId

            };

            string myPostCreateAnAssetFilejsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
            myPostCreateAnAssetFileRequest.Content = new StringContent(myPostCreateAnAssetFilejsonBody, Encoding.UTF8, "application/json");

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage myPostCreateAnAssetFileResponseMessage = await httpClient.SendAsync(myPostCreateAnAssetFileRequest);

            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
            var myPostCreateAnAssetFileResponseMessagestringResult = myPostCreateAnAssetFileResponseMessage.Content.ReadAsStringAsync().Result;

            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
            var myPostCreateAnAssetFileResponseMessageresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(myPostCreateAnAssetFileResponseMessagestringResult);

            var myPostCreateAnAssetFiledObjectResults = myPostCreateAnAssetFileResponseMessageresultObject.d;
            var myPostCreateAnAssetFilecreateAssetFileId = myPostCreateAnAssetFiledObjectResults.Id;

            ////////////////////////////////////////////////////////////////////////////////
            // PostCreateAccessPolicy
            ////////////////////////////////////////////////////////////////////////////////

            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Clear();
            }

            //  Bearer Token
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            //CREATE HTTP REQUEST
            HttpRequestMessage myPostCreateAccessPolicyRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
            FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
            string myPostCreateAccessPolicyjsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
            myPostCreateAccessPolicyRequest.Content = new StringContent(myPostCreateAccessPolicyjsonBody, Encoding.UTF8, "application/json");

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage myPostCreateAccessPolicyResponseMessage = await httpClient.SendAsync(myPostCreateAccessPolicyRequest);


            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
            var myPostCreateAccessPolicystringResult = myPostCreateAccessPolicyResponseMessage.Content.ReadAsStringAsync().Result;

            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
            var myPostCreateAccessPolicyresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(myPostCreateAccessPolicystringResult);

            var myPostCreateAccessPolicydObjectResults = myPostCreateAccessPolicyresultObject.d;
            var myPostCreateAccessPolicyaccessPolicyIdResults = myPostCreateAccessPolicydObjectResults.Id;

            ////////////////////////////////////////////////////////////////////////////////
            // PostCreateLocator
            ////////////////////////////////////////////////////////////////////////////////

            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Clear();
            }


            //  Bearer Token
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
            httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            //CREATE HTTP REQUEST
            HttpRequestMessage myPostCreateLocatorRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
            FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
            {
                AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults,
                AssetId = myPostCreateAssetRequestdObjectResultsResultId,
                StartTime = DateTime.Now,
                Type = 1
            };
            string myPostCreateLocatorjsonBody = JsonConvert.SerializeObject(createdLocatorBody);
            myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage myPostCreateLocatorResponseMessage = await httpClient.SendAsync(myPostCreateLocatorRequest);

            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
            var myPostCreateLocatorstringResult = myPostCreateLocatorResponseMessage.Content.ReadAsStringAsync().Result;

            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
            var myPostCreateLocatorresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(myPostCreateLocatorstringResult);

            var myPostCreateLocatordObjectResults = myPostCreateLocatorresultObject.d;
            var myPostCreateLocatorlocatorResults = myPostCreateLocatordObjectResults.Id;

            const string assetIDSearchString = "blob.core.windows.net/";
            var indexOfContainerNameStart = myPostCreateLocatordObjectResults.BaseUri.IndexOf(assetIDSearchString) + assetIDSearchString.Length;
            var containerName = myPostCreateLocatordObjectResults.BaseUri.Substring(indexOfContainerNameStart);

            ///////////////////////////////////
            ///// UPLOAD TO BLOB STORAGE
            //////////////////////////////////

            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Clear();
            }

            var xmsversion = "2017-04-17";
            var accountKey = "N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";
            var accountName = "xamcamstorage";
            var requestMethod = "PUT";
            var date = DateTime.UtcNow.AddSeconds(-300).ToString("R", CultureInfo.InvariantCulture);
            var blobType = "BlockBlob";

            //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
            UploadFileToBlobStorageBody thePutUploadFileToBlobStorage = new UploadFileToBlobStorageBody
            {
                FileViaByteArray = myUploadedFile.File
            };

            string uploadToBlobStoragejsonBody = JsonConvert.SerializeObject(thePutUploadFileToBlobStorage);
            int contentLengthInt = Encoding.UTF8.GetByteCount(uploadToBlobStoragejsonBody);
            string authorizationHeader = CreateAuthorizationHeader(requestMethod, blobType, date, xmsversion, contentLengthInt, "application/json", accountName, accountKey, containerName, myUploadedFile.FileName);

            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", xmsversion);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", date);
            httpClient.DefaultRequestHeaders.Add("x-ms-blob-type", blobType);

            //CREATE HTTP REQUEST
            HttpRequestMessage myUploadToBlobStorageRequest = new HttpRequestMessage(HttpMethod.Put, String.Format("{0}/{1}{2}", myPostCreateLocatordObjectResults.BaseUri, myUploadedFile.FileName, myPostCreateLocatordObjectResults.ContentAccessComponent));

            myUploadToBlobStorageRequest.Content = new StringContent(uploadToBlobStoragejsonBody, Encoding.UTF8, "application/json");
            myUploadToBlobStorageRequest.Content.Headers.Add("Content-Length", contentLengthInt.ToString());

            //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
            HttpResponseMessage UploadToBlobStorageResponseMessage = await httpClient.SendAsync(myUploadToBlobStorageRequest);

            //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
            var stringResult2 = await UploadToBlobStorageResponseMessage.Content.ReadAsStringAsync();

            //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
            //var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.UploadFileToBlobStorage.RootObject>(stringResult);

            //var dObjectResults = resultObject.d;
            //var locatorResults = dObjectResults.Id;

            ////ASSIGN OBJECTS TO THE CLASS LEVEL PROPERTIES 
            //ReturnedUploadFileToBlobStorage = dObjectResults;
            //ReturnedReturnedUploadFileToBlobStorageId = locatorResults;

            //RETURN THE HTTP RESPONSE MESSAGE
            return UploadToBlobStorageResponseMessage;







        }

        static string CreateAuthorizationHeader(string httpVerb, string xMsBlobType, string xMsDate, string xMsVersion, long contentLength, string contentType, string storageAccountName, string accountKey, string containerName, string blobName)
        {

            string headerResource = $"x-ms-blob-type:{xMsBlobType}\nx-ms-date:{xMsDate}\nx-ms-version:{xMsVersion}";
            string urlResource = $"/{storageAccountName}/{containerName}/{blobName}";
            string stringToSign = $"{httpVerb}\n\n\n{contentLength}\n\n{contentType}\n\n\n\n\n\n\n{headerResource}\n{urlResource}";

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(accountKey));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            String AuthorizationHeader = String.Format("{0} {1}:{2}", "SharedKey", storageAccountName, signature);
            return AuthorizationHeader;
        }
    }
}