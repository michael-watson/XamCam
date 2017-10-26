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
            [QueueTrigger(QueueNames.MediaToPublish)]MediaMetadata mediaMetadataFromQueue,
            [Queue(QueueNames.MediaToAddToCosmosDb)] out MediaMetadata mediaMetadataToAddToCosmosDb,
            TraceWriter log)
        {
            log.Info($"{nameof(PublishMediaAsset)} triggered");

            IAsset asset = null;

            try
            {
                mediaMetadataToAddToCosmosDb = mediaMetadataFromQueue;

                asset = AzureMediaServices.GetAsset(mediaMetadataFromQueue);

                var (manifestUri, hlsUri, mpegDashUri) = AzureMediaServices.BuildStreamingURLs(asset);

                mediaMetadataToAddToCosmosDb.BlobStorageMediaUrl = $"{mediaMetadataToAddToCosmosDb.MediaAssetUri}/{AzureMediaServices.GetMP4FileName(asset)}";
                mediaMetadataToAddToCosmosDb.ManifestUrl = manifestUri;
                mediaMetadataToAddToCosmosDb.HLSUrl = hlsUri;
                mediaMetadataToAddToCosmosDb.MPEGDashUrl = mpegDashUri;

                AzureMediaServices.CreateStreamingEndpoint();
            }
            catch(Exception e)
            {
                log.Info($"Error: {e.Message}");
                asset?.Delete();

                throw e;
            }
            finally
            {
                log.Info($"{nameof(PublishMediaAsset)} completed");
            }
        }
    }
}
