using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cam.Functions
{
    public static class TestFunction
    {
        [FunctionName("TestFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Test/{id}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            if (req.Method == HttpMethod.Post)
            {
                var data = await req.Content.ReadAsByteArrayAsync();
                var container = await getContainerReference();

                // write a blob to the container
                int unique = 0;
                CloudBlockBlob blob = container.GetBlockBlobReference($"{id}.{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}.mp4");
                
                while(await blob.ExistsAsync())
                {
                    blob = container.GetBlockBlobReference($"{id}.{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}--{unique}.mp4");
                    unique++;
                }
                
                await blob.UploadFromByteArrayAsync(data, 0, data.Length);

                return req.CreateResponse(HttpStatusCode.OK, $"{blob.Name} created successfully");
            }
            else if (req.Method == HttpMethod.Get)
            {
                var container = await getContainerReference();

                var blob = container.GetBlockBlobReference(id);
                var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),//Set this date/time according to your requirements
                });
                var urlToBePlayed = string.Format("{0}{1}", blob.Uri, sas);//This is the URI which 

                return req.CreateResponse(HttpStatusCode.OK, urlToBePlayed);
            }

            return req.CreateResponse(HttpStatusCode.BadGateway, $"Only GET and POST request are supported");
        }

        async static Task<CloudBlobContainer> getContainerReference()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                       + "AccountName=homecamstorage"
                       + ";AccountKey=BiAjEqFBTIwf0kWsmEyyg8JR1rXwuluAR1kLk/n4JqAyzDAmmQ5Eris4HJr/0xaeTvZsrJ+A3yXQzdLYmPkPOw=="
                       + ";EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            // Create container. Name must be lower case.
            var container = serviceClient.GetContainerReference("videocontainer");
            await container.CreateIfNotExistsAsync();

            return container;
        }
    }
}