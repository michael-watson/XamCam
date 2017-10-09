using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using XamCamFunctions.DataModels;

namespace XamCamFunctions
{
    public static class XamCamFunctionsICC
    {
        [FunctionName("GetMediaAssets")]
        public static async Task<HttpResponseMessage> RunGetMediaAssets([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // ALTERNATIVE ALLOWING PASSING PARAMETER IN URL
            // public static async Task<HttpResponseMessage> MVPRunGetVideosConsolidated([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "MVPGetVideosConsolidated/{email}")]HttpRequestMessage req, string email, TraceWriter log)
            // Remove string email = "user@xamarin.com";

            string email = "user@xamarin.com";
            List<MediaAssetsWithMetaData> listOfVideos = new List<MediaAssetsWithMetaData>();

            //LIST OF MEDIA FILES FROM COSMOS DB
            listOfVideos = await XamCamFunctions.CosmosDB.CosmosDBService.GetAllMediaAssetsByEmailAsync(email);

            //ADD LIST TO JSON AND SEND RESPONSE MESSAGE BACK
            string jsonResult = JsonConvert.SerializeObject(listOfVideos);
            var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
            return httpRM;
        }

        [FunctionName("PostMediaAssetToSpecifiedBlobContainer")]
        public static async Task<HttpResponseMessage> MVPRunPostItemToSpecifiedBlobContainer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PostMediaAssetToSpecifiedBlobContainer/{deviceId}/{videoTitle}")]HttpRequestMessage req, string deviceId, string videoTitle, TraceWriter log)
        {
            var fileAsBytes = await req.Content.ReadAsByteArrayAsync();
            var myUploadedFile = new UploadedFile
            {
                Title = videoTitle,
                FileName = $"{deviceId}_{DateTime.UtcNow.Ticks}.mp4",
                File = fileAsBytes,
                UploadedAt = DateTime.UtcNow
            };

            ///////////////////////////////////
            ///// UPLOAD TO BLOB STORAGE
            //////////////////////////////////

            //THIS REQUIRES CONFIGURATION FILE
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constants.BlobURLAndKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            string containerName = "mediaassetblobcontainer20170928";

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //By default, the new container is private, 
            //meaning that you must specify your storage access key to download blobs 
            //from this container.If you want to make the files within the container available 
            //to everyone, you can set the container to be public using the following code:

            var perm = new BlobContainerPermissions();
            perm.PublicAccess = BlobContainerPublicAccessType.Blob;
            container.SetPermissions(perm);

            //container.SetPermissions(new BlobContainerPermissions
            //{
            //    PublicAccess = BlobContainerPublicAccessType.Blob
            //});

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(myUploadedFile.FileName);

            //IN CASE YOU NEED TO SET THE MEDIA TYPE
            //https://stackoverflow.com/questions/24621664/uploading-blockblob-and-setting-contenttype

            try
            {
                ////////////////////////////////////////////////////////
                //UPLOAD FILE FROM BYTE ARRAY
                ////////////////////////////////////////////////////////

                blockBlob.UploadFromByteArray(myUploadedFile.File, 0, myUploadedFile.File.Length);

                //////////////////////////////////////////////////////
                /////////  SAVE TO COSMOS DB
                //////////////////////////////////////////////////////
                
                MediaAssetsWithMetaData uploadMediaAssetsWithMetaData = new MediaAssetsWithMetaData()
                {
                    id = Guid.NewGuid().ToString(),
                    email = "user@xamarin.com",
                    //mediaAssetUri = newLocator,
                    title = myUploadedFile.Title,
                    fileName = myUploadedFile.FileName,
                    uploadedAt = myUploadedFile.UploadedAt
                };

                await CosmosDB.CosmosDBService.PostMediaAssetAsync(uploadMediaAssetsWithMetaData);

                ////////////////////////////////////////////////////////
                //SEND HTTP RESPONSE MESSAGE
                ////////////////////////////////////////////////////////

                //HttpResponseMessage postFileInCreatedBlob2 = new HttpResponseMessage(HttpStatusCode.OK, "success");
                //httpRM.Content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");

                return req.CreateResponse<string>(HttpStatusCode.OK, "success");
            }

            catch (Exception ex)
            {
                log.Error($"ERROR copying blobs to target output: {ex.Message}");

                ////////////////////////////////////////////////////////
                //SEND HTTP RESPONSE MESSAGE
                ////////////////////////////////////////////////////////

                HttpResponseMessage errorInCreatingBlob = new HttpResponseMessage(HttpStatusCode.BadRequest);
                var errorMessage = "Function did not upload to blob container.";
                errorInCreatingBlob.Content = new StringContent(errorMessage, System.Text.Encoding.UTF8, "application/json");
                return errorInCreatingBlob;
            }
        }
    }
}
