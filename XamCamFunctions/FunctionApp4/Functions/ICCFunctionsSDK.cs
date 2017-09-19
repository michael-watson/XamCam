
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
//using System.Security.Cryptography;
//using System.Globalization;
//using FunctionApp4.DataModels.UploadFileToBlobStorage;
//using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.Azure;
//using System.Collections.Generic;
//using Microsoft.WindowsAzure.MediaServices.Client;


//namespace FunctionApp4
//{
//    public static class ICCFunctionsSDK
//    {
//        //CONSTANTS NEEDED FOR AZURE AD
//        static string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
//        static string GrantType = "client_credentials";
//        static string ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
//        static string ClientID = "8d631792-ed10-46aa-bd09-b8ca1641bc6f";
//        static string RequestedResource = "https://rest.media.azure.net";


//        static string AzureMediaServicesRestAPIEndpoint = "https://xamcammediaservice.restv2.westus.media.azure.net/api/";

//        [FunctionName("AADServicePrincipalAuthentication")]
//        public static async Task<HttpResponseMessage> AADServicePrincipalAuthentication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        {

//            ///////////////////////////////////////////////////////
//            //AAD SERVICE PRINCIPAL AUTHENTICATION
//            ///////////////////////////////////////////////////////

//            // Specify your Azure AD tenant domain, for example "microsoft.onmicrosoft.com".
//            var tokenCredentials = new AzureAdTokenCredentials(
//                tenantId,
//                new AzureAdClientSymmetricKey(ClientID, ClientSecret),
//                AzureEnvironments.AzureCloudEnvironment);

//            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

//            // Specify your REST API endpoint, for example "https://accountname.restv2.westcentralus.media.azure.net/API".
//            CloudMediaContext context = new CloudMediaContext(new Uri(AzureMediaServicesRestAPIEndpoint), tokenProvider);

//            var assets = context.Assets;
//            foreach (var a in assets)
//            {
//                Console.WriteLine(a.Name);
//            }

//            ///////////////////////////////////////////////////////
//            //UPLOAD A FILE
//            ///////////////////////////////////////////////////////

//            AssetCreationOptions myAssetCreationOption = AssetCreationOptions.None;

//            //GET ASSET FROM POST REQUEST 
//            var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();
            
//            //CREATE ASSET TO CREATE

//            string assetName = myUploadedFile.FileName;
//            //TODO Confirm with Watson that this FileName is unique
//            //FileName will be both the Asset and AssetFile name
//            AssetCreationOptions myAssetCreationOptions = AssetCreationOptions.None;
            
//            //CREATE AN ASSET 
//            IAsset inputAsset = context.Assets.Create(assetName, myAssetCreationOptions);

//            //CREATE + UPLOAD THE ASSET FILE
//            var assetFile = inputAsset.AssetFiles.Create(myUploadedFile.File);
//            //assetFile.U

            




//            static public IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string singleFilePath)
//            {
//                if (!File.Exists(singleFilePath))
//                {
//                    Console.WriteLine(“File does not exist.“);
//                    return null;
//                }

//                var assetName = Path.GetFileNameWithoutExtension(singleFilePath);
//                IAsset inputAsset = _context.Assets.Create(assetName, assetCreationOptions);

//                var assetFile = inputAsset.AssetFiles.Create(Path.GetFileName(singleFilePath));

//                Console.WriteLine(“Upload { 0}“, assetFile.Name);

//            assetFile.Upload(singleFilePath);
//            Console.WriteLine(“Done uploading { 0}“, assetFile.Name);

//            return inputAsset;
//        }





//    }

//        //[FunctionName("SDKMPVGetAzureADAuthTokenConsolidated")]
//        //    public static async Task<HttpResponseMessage> SDKMPVRunGetAzureADTokenConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        //    {

//        //        var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();

//        //        HttpClient httpClient = new HttpClient();

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));
//        //        myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage httpResponseMessageWithADToken = await httpClient.SendAsync(myRequest);

//        //        //EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
//        //        var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
//        //        var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
//        //        var azureADToken = resultObject.access_token;

//        //        ////////////////////////////////////////////////////////////////////////////////
//        //        // PostCreateAnAsset
//        //        ////////////////////////////////////////////////////////////////////////////////


//        //        if (httpClient.DefaultRequestHeaders != null)
//        //        {
//        //            httpClient.DefaultRequestHeaders.Clear();
//        //        }

//        //        //  Bearer Token
//        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
//        //        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//        //        httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//        //        httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage myPostCreateAssetRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

//        //        //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //        FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody createAnAssetBody = new FunctionApp4.DataModels.CreateAnAsset.CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
//        //        string PostCreateAnAssetjsonBody = JsonConvert.SerializeObject(createAnAssetBody);
//        //        myPostCreateAssetRequest.Content = new StringContent(PostCreateAnAssetjsonBody, Encoding.UTF8, "application/json");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage myPostCreateAssetResponseMessage = await httpClient.SendAsync(myPostCreateAssetRequest);

//        //        //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //        var PostCreateAssetStringResult = await myPostCreateAssetResponseMessage.Content.ReadAsStringAsync();

//        //        //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //        var myPostCreateAssetResponseMessageResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAnAsset.RootObject>(PostCreateAssetStringResult);

//        //        var myPostCreateAssetRequestdObjectResults = myPostCreateAssetResponseMessageResultObject.d;
//        //        var myPostCreateAssetRequestdObjectResultsResultId = myPostCreateAssetRequestdObjectResults.Id;

//        //        ////////////////////////////////////////////////////////////////////////////////
//        //        // PostCreateAnAssetFile
//        //        ////////////////////////////////////////////////////////////////////////////////

//        //        if (httpClient.DefaultRequestHeaders != null)
//        //        {
//        //            httpClient.DefaultRequestHeaders.Clear();
//        //        }

//        //        //  Bearer Token
//        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
//        //        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//        //        httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
//        //        httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage myPostCreateAnAssetFileRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

//        //        //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //        FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody createdAssetFileBody = new FunctionApp4.DataModels.CreateAssetFile.CreateAssetFileBody
//        //        {
//        //            IsEncrypted = "false",
//        //            IsPrimary = "true",
//        //            MimeType = "video/mp4",
//        //            Name = "TestVideo.mp4",
//        //            //ParentAssetId = "nb:cid:UUID:498c1cac-fe58-4099-9c72-32cfde165f01"
//        //            ParentAssetId = myPostCreateAssetRequestdObjectResultsResultId

//        //        };

//        //        string myPostCreateAnAssetFilejsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
//        //        myPostCreateAnAssetFileRequest.Content = new StringContent(myPostCreateAnAssetFilejsonBody, Encoding.UTF8, "application/json");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage myPostCreateAnAssetFileResponseMessage = await httpClient.SendAsync(myPostCreateAnAssetFileRequest);

//        //        //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //        var myPostCreateAnAssetFileResponseMessagestringResult = myPostCreateAnAssetFileResponseMessage.Content.ReadAsStringAsync().Result;

//        //        //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //        var myPostCreateAnAssetFileResponseMessageresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAssetFile.RootObject>(myPostCreateAnAssetFileResponseMessagestringResult);

//        //        var myPostCreateAnAssetFiledObjectResults = myPostCreateAnAssetFileResponseMessageresultObject.d;
//        //        var myPostCreateAnAssetFilecreateAssetFileId = myPostCreateAnAssetFiledObjectResults.Id;

//        //        ////////////////////////////////////////////////////////////////////////////////
//        //        // PostCreateAccessPolicy
//        //        ////////////////////////////////////////////////////////////////////////////////

//        //        if (httpClient.DefaultRequestHeaders != null)
//        //        {
//        //            httpClient.DefaultRequestHeaders.Clear();
//        //        }

//        //        //  Bearer Token
//        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
//        //        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//        //        httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//        //        httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage myPostCreateAccessPolicyRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

//        //        //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //        FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody createdAccessPolicyBody = new FunctionApp4.DataModels.CreateAccessPolicy.CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
//        //        string myPostCreateAccessPolicyjsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
//        //        myPostCreateAccessPolicyRequest.Content = new StringContent(myPostCreateAccessPolicyjsonBody, Encoding.UTF8, "application/json");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage myPostCreateAccessPolicyResponseMessage = await httpClient.SendAsync(myPostCreateAccessPolicyRequest);


//        //        //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //        var myPostCreateAccessPolicystringResult = myPostCreateAccessPolicyResponseMessage.Content.ReadAsStringAsync().Result;

//        //        //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //        var myPostCreateAccessPolicyresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateAccessPolicy.RootObject>(myPostCreateAccessPolicystringResult);

//        //        var myPostCreateAccessPolicydObjectResults = myPostCreateAccessPolicyresultObject.d;
//        //        var myPostCreateAccessPolicyaccessPolicyIdResults = myPostCreateAccessPolicydObjectResults.Id;

//        //        ////////////////////////////////////////////////////////////////////////////////
//        //        // PostCreateLocator
//        //        ////////////////////////////////////////////////////////////////////////////////

//        //        if (httpClient.DefaultRequestHeaders != null)
//        //        {
//        //            httpClient.DefaultRequestHeaders.Clear();
//        //        }


//        //        //  Bearer Token
//        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
//        //        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
//        //        httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
//        //        httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage myPostCreateLocatorRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

//        //        //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //        FunctionApp4.DataModels.CreateLocator.CreateLocatorBody createdLocatorBody = new FunctionApp4.DataModels.CreateLocator.CreateLocatorBody
//        //        {
//        //            AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults,
//        //            AssetId = myPostCreateAssetRequestdObjectResultsResultId,
//        //            StartTime = DateTime.Now,
//        //            Type = 1
//        //        };
//        //        string myPostCreateLocatorjsonBody = JsonConvert.SerializeObject(createdLocatorBody);
//        //        myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage myPostCreateLocatorResponseMessage = await httpClient.SendAsync(myPostCreateLocatorRequest);

//        //        //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //        var myPostCreateLocatorstringResult = myPostCreateLocatorResponseMessage.Content.ReadAsStringAsync().Result;

//        //        //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //        var myPostCreateLocatorresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<FunctionApp4.DataModels.CreateLocator.RootObject>(myPostCreateLocatorstringResult);

//        //        var myPostCreateLocatordObjectResults = myPostCreateLocatorresultObject.d;
//        //        var myPostCreateLocatorlocatorResults = myPostCreateLocatordObjectResults.Id;

//        //        const string assetIDSearchString = "blob.core.windows.net/";
//        //        var indexOfContainerNameStart = myPostCreateLocatordObjectResults.BaseUri.IndexOf(assetIDSearchString) + assetIDSearchString.Length;
//        //        var containerName = myPostCreateLocatordObjectResults.BaseUri.Substring(indexOfContainerNameStart);

//        //        ///////////////////////////////////
//        //        ///// UPLOAD TO BLOB STORAGE
//        //        //////////////////////////////////

//        //        //THIS REQUIRES CONFIGURATION FILE
//        //        //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
//        //        //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constants.BlobURLAndKey);
//        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//        //        // Retrieve a reference to a container.
//        //        CloudBlobContainer container = blobClient.GetContainerReference(containerName);

//        //        // Create the container if it doesn't already exist.
//        //        container.CreateIfNotExists();

//        //        //By default, the new container is private, 
//        //        //meaning that you must specify your storage access key to download blobs 
//        //        //from this container.If you want to make the files within the container available 
//        //        //to everyone, you can set the container to be public using the following code:
//        //        container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

//        //        // Retrieve reference to a blob named "myblob".
//        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(myUploadedFile.FileName);

//        //        //IN CASE YOU NEED TO SET THE MEDIA TYPE
//        //        //https://stackoverflow.com/questions/24621664/uploading-blockblob-and-setting-contenttype

//        //        blockBlob.UploadFromByteArray(myUploadedFile.File, 0, myUploadedFile.File.Length);


//        //        ////////////////////////////////////////////////////////////////////////////////
//        //        // GET AND RETURN THE LIST OF ITMES IN BLOB 
//        //        ////////////////////////////////////////////////////////////////////////////////

//        //        if (httpClient.DefaultRequestHeaders != null)
//        //        {
//        //            httpClient.DefaultRequestHeaders.Clear();
//        //        }

//        //        //CREATE HTTP REQUEST
//        //        HttpRequestMessage getTheListOfItemsRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("http://localhost:7071/api/MVPGetVideosConsolidated"));
//        //        //HttpRequestMessage getTheListOfItemsRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("http://iccfunction.azurewebsites.net/api/GetVideosConsolidated"));

//        //        //ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
//        //        ContainerInformationModel createdGetListBody = new ContainerInformationModel
//        //        {
//        //            //TO WORK WITH A FIXED ASSET/CONTAINER NAME - USE THE FOLLOWING
//        //            //ContainerName = "asset-6c8510d9-7c8b-4dca-b7df-332739ce809a" 
//        //            ContainerName = containerName


//        //        };

//        //        string myGetListOfBlobsjsonString = JsonConvert.SerializeObject(createdGetListBody);
//        //        getTheListOfItemsRequest.Content = new StringContent(myGetListOfBlobsjsonString, Encoding.UTF8, "application/json");

//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        HttpResponseMessage myGetListResponseMessage = await httpClient.SendAsync(getTheListOfItemsRequest);

//        //        //EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
//        //        var myListOfBlobsHttpResult = myGetListResponseMessage.Content.ReadAsStringAsync().Result;

//        //        //DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
//        //        var myListOfBlobsResults =
//        //            Newtonsoft.Json.JsonConvert.DeserializeObject<List<FunctionApp4.DataModels.ReturnedListOfBlogs.RootObject>>
//        //            (myListOfBlobsHttpResult);

//        //        var myListOfBlobs = myListOfBlobsResults;

//        //        return myPostCreateLocatorResponseMessage;

//        //    }

//        //    //UPLOAD TO A SPECIFIC CONTAINER
//        //    //string containerName = "asset-6c8510d9-7c8b-4dca-b7df-332739ce809a";

//        //    [FunctionName("SDKMVPPostItemToSpecifiedBlobContainer")]
//        //    public static async Task<HttpResponseMessage> SDKMVPRunPostItemToSpecifiedBlobContainer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
//        //    {
//        //        var myUploadedFile = await req.Content.ReadAsAsync<UploadedFile>();

//        //        ///////////////////////////////////
//        //        ///// UPLOAD TO BLOB STORAGE
//        //        //////////////////////////////////

//        //        //METHOD 1: CREATION VIA CONFIGURATION FILE
//        //        //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
//        //        //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//        //        //METHOD 2: CREATION VIA BLOB URL AND KEY
//        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constants.BlobURLAndKey);
//        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//        //        string containerName = "asset-6c8510d9-7c8b-4dca-b7df-332739ce809a";

//        //        // Retrieve a reference to a container.
//        //        CloudBlobContainer container = blobClient.GetContainerReference(containerName);

//        //        // Create the container if it doesn't already exist.
//        //        container.CreateIfNotExists();

//        //        //By default, the new container is private, 
//        //        //meaning that you must specify your storage access key to download blobs 
//        //        //from this container.If you want to make the files within the container available 
//        //        //to everyone, you can set the container to be public using the following code:
//        //        container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

//        //        // Retrieve reference to a blob, use the file name of your choice
//        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(myUploadedFile.FileName);

//        //        //Uload the file from a ByteArray
//        //        blockBlob.UploadFromByteArray(myUploadedFile.File, 0, myUploadedFile.File.Length);

//        //        ////////////////////////////////////////////////////////
//        //        //SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
//        //        ////////////////////////////////////////////////////////

//        //        HttpResponseMessage postFileInCreatedBlob = new HttpResponseMessage(HttpStatusCode.OK);
//        //        //httpRM.Content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");

//        //        return postFileInCreatedBlob;
//        //    }


//        //    [FunctionName("SDKMVPGetVideosConsolidated")]
//        //    public static async Task<HttpResponseMessage> SDKMVPRunGetVideosConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
//        //    {
//        //        string nameOfContainerForAccount = "asset-6c8510d9-7c8b-4dca-b7df-332739ce809a";

//        //        // Retrieve storage account from connection string.
//        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constants.BlobURLAndKey);

//        //        // Create the blob client.
//        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//        //        // Retrieve reference to a previously created container.
//        //        CloudBlobContainer container = blobClient.GetContainerReference(nameOfContainerForAccount);

//        //        List<MediaAssetsInBlobContainer> listOfBlobs = new List<MediaAssetsInBlobContainer>();

//        //        // Loop over items within the container and output the length and URI.
//        //        foreach (IListBlobItem item in container.ListBlobs(null, true))
//        //        {
//        //            if (item.GetType() == typeof(CloudBlockBlob))
//        //            {
//        //                CloudBlockBlob blob = (CloudBlockBlob)item;
//        //                var temp = new MediaAssetsInBlobContainer()
//        //                {
//        //                    MediaAssetUri = blob.Uri.ToString(),
//        //                    MediaAssetName = blob.Name.ToString()


//        //                };
//        //                listOfBlobs.Add(temp);
//        //                Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

//        //            }
//        //            else if (item.GetType() == typeof(CloudPageBlob))
//        //            {
//        //                CloudPageBlob pageBlob = (CloudPageBlob)item;
//        //                Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

//        //            }
//        //            else if (item.GetType() == typeof(CloudBlobDirectory))
//        //            {
//        //                CloudBlobDirectory directory = (CloudBlobDirectory)item;
//        //                Console.WriteLine("Directory: {0}", directory.Uri);
//        //            }
//        //        }

//        //        //TAKE THE LIST AND THEN ADD IT TO JSON AND SEND IT BACK
//        //        log.Info("Partial Return List with One Object processed a request.");
//        //        string jsonResult = JsonConvert.SerializeObject(listOfBlobs);
//        //        var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
//        //        httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
//        //        return httpRM;
//        //    }

//        //}
//    }
//}
