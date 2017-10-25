using System;
using System.Linq;
using System.Threading;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace XamCam.Functions.Functions
{
    public static class EncodeMediaAsset
    {

        [FunctionName(nameof(EncodeMediaAsset))]
        public static void Run(
            [QueueTrigger(QueueNames.MediaToEncode)]MediaMetadata mediaMetadataFromQueue,
            [Queue(QueueNames.MediaToPublish)] out MediaMetadata mediaMetadataToPublish,
            TraceWriter log)
        {
            log.Info($"{nameof(EncodeMediaAsset)} triggered");

            var asset = AzureMediaServices.GetAsset(mediaMetadataFromQueue);
            var newAsset = AzureMediaServices.EncodeToAdaptiveBitrateMP4Set(asset, mediaMetadataFromQueue.Title);

            mediaMetadataToPublish = new MediaMetadata
            {
                FileName = newAsset.Name,
                MediaServicesAssetId = newAsset.Id,
                MediaAssetUri = newAsset.Uri,
                Title = mediaMetadataFromQueue.Title,
                UploadedAt = mediaMetadataFromQueue.UploadedAt,
            };

            asset.Delete(false);
        }
    }
}
