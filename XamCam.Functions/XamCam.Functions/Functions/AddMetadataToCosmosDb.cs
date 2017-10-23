using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions.Functions
{ 
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class AddMetadataToCosmosDb
    {
        [FunctionName(nameof(AddMetadataToCosmosDb))]
        public static void Run(
            [QueueTrigger(QueueNames.MediaToAddToCosmosDb)]MediaMetadata mediaMetadataFromQueue,
            [DocumentDB(nameof(MediaMetadata), EnvironmentVariables.CosmosDbCollectionId, ConnectionStringSetting = nameof(EnvironmentVariables.CosmosDBConnectionString), CreateIfNotExists = true, Id = "{documentId}")] out MediaMetadata mediaMetadataForCosmos, string documentId,
            TraceWriter log)
        {
            log.Info($"{QueueNames.MediaToAddToCosmosDb} triggered");

            try
            {
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
