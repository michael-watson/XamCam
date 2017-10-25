using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace XamCam.Functions.Functions
{
    public static class GetMediaMetadata
    {
        [FunctionName(nameof(GetMediaMetadata))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [DocumentDB(EnvironmentVariables.CosmosDbDatabaseId, EnvironmentVariables.CosmosDbCollectionId, ConnectionStringSetting = nameof(EnvironmentVariables.CosmosDBConnectionString), SqlQuery = "SELECT * from x where x.Id != null")] IEnumerable<MediaMetadata> mediaMetadataList,
            TraceWriter log)
        {
            log.Info($"{nameof(GetMediaMetadata)} triggered");

            return req.CreateResponse(HttpStatusCode.OK, mediaMetadataList);
        }
    }
}
