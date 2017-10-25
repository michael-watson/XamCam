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
        static string _encoderName = "Media Encoder Standard";
        static string _encoderPreset = "Content Adaptive Multiple Bitrate MP4";

        [FunctionName(nameof(EncodeMediaAsset))]
        public static void Run(
            [QueueTrigger(QueueNames.MediaToEncode)]MediaMetadata mediaMetadataFromQueue,
            [Queue(QueueNames.MediaToPusblish)] out MediaMetadata mediaMetadataToPublish,
            TraceWriter log)
        {
            log.Info($"{nameof(EncodeMediaAsset)} triggered");

            var asset = AzureMediaServices.GetAsset(mediaMetadataFromQueue);
            var job = AzureMediaServices.CloudMediaContext.Jobs.CreateWithSingleTask(_encoderName, _encoderPreset, asset, mediaMetadataFromQueue.Title, AssetCreationOptions.None);

            job.Submit();
            job = job.StartExecutionProgressTask(j => log.Info($"Encoding Job Id: {job.Id}  State: {job.State}  Progress: {j.GetOverallProgress().ToString("P")}"), CancellationToken.None).GetAwaiter().GetResult();

            switch (job.State)
            {
                case JobState.Finished:
                    log.Info($"Encoding Job Id: {job.Id} is complete.");
                    break;
                case JobState.Error: throw new Exception($"Encoding Job Id: {job.Id} failed.");
            }

            mediaMetadataToPublish = new MediaMetadata
            {
                FileName = job.OutputMediaAssets.FirstOrDefault()?.AssetFiles.FirstOrDefault().Name,
                MediaServicesAssetId = job.OutputMediaAssets.FirstOrDefault()?.Id,
                MediaAssetUri = job.OutputMediaAssets.FirstOrDefault()?.Uri,
                Title = mediaMetadataFromQueue.Title,
                UploadedAt = mediaMetadataFromQueue.UploadedAt,
            };

            asset.Delete(false);
        }
    }
}
