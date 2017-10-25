using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace XamCam.Functions.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class PublishMediaAsset
    {
        [FunctionName(nameof(PublishMediaAsset))]
        public static void Run(
            [QueueTrigger(QueueNames.MediaToPusblish)]MediaMetadata mediaMetadataFromQueue,
            [Queue(QueueNames.MediaToAddToCosmosDb)] out MediaMetadata mediaMetadataToAddToCosmosDb,
            TraceWriter log)
        {
            log.Info($"{nameof(PublishMediaAsset)} triggered");

            try
            {
                mediaMetadataToAddToCosmosDb = mediaMetadataFromQueue;

                var asset = AzureMediaServices.GetAsset(mediaMetadataFromQueue);

                var (manifestUri, hlsUri, mpegDashUri) = AzureMediaServices.BuildStreamingURIs(asset);

                mediaMetadataToAddToCosmosDb.ManifestUri = manifestUri;
                mediaMetadataToAddToCosmosDb.HLSUri = hlsUri;
                mediaMetadataToAddToCosmosDb.MPEGDashUri = mpegDashUri;
            }
            catch(Exception e)
            {
                log.Info($"Error: {e.Message}");
                mediaMetadataToAddToCosmosDb = null;
            }
        }
    }
}
