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

                log.Info($"Publishing Media");
                var locator = AzureMediaServices.PublishMedia(asset, TimeSpan.FromDays(999));

                log.Info($"Generating Streaming Endpoint");
                AzureMediaServices.CreateStreamingEndpoint();

                log.Info($"Generating Urls");
                var (manifestUri, hlsUri, mpegDashUri) = AzureMediaServices.BuildStreamingURLs(asset, locator);

                mediaMetadataToAddToCosmosDb.BlobStorageMediaUrl = $"{mediaMetadataToAddToCosmosDb.MediaAssetUri}/{AzureMediaServices.GetMP4FileName(asset)}";
                mediaMetadataToAddToCosmosDb.ManifestUrl = manifestUri;
                mediaMetadataToAddToCosmosDb.HLSUrl = hlsUri;
                mediaMetadataToAddToCosmosDb.MPEGDashUrl = mpegDashUri;
            }
            catch (Exception e)
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
