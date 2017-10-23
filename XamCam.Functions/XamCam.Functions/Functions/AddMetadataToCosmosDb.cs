using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions.Functions
{
    public static class AddMetadataToCosmosDb
    {
        [FunctionName(nameof(AddMetadataToCosmosDb))]
        public static void Run(
            [QueueTrigger(QueueNames.MediaToAddToCosmosDb, Connection = EnvironmentVariables.StorageConnectionString)]MediaMetadata mediaMetadataFromQueue,
            [DocumentDB(nameof(MediaMetadata), "{collectionId}", ConnectionStringSetting = nameof(EnvironmentVariables.CosmosDBConnectionString), CreateIfNotExists = true, Id = "{documentId}")] out MediaMetadata mediaMetadataForCosmos, string collectionId, string documentId,
            TraceWriter log)
        {
            log.Info($"{QueueNames.MediaToAddToCosmosDb} triggered");

            try
            {
                collectionId = EnvironmentVariables.CosmosDbCollectionId;
                documentId = mediaMetadataFromQueue?.Id;
                mediaMetadataForCosmos = mediaMetadataFromQueue;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                mediaMetadataForCosmos = null;
            }
        }
    }
}
