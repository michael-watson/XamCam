using System;
using System.Net;
using System.Text;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace XamCam.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class PostMediaFile
    {
        [FunctionName(nameof(PostMediaFile))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostMediaFile/{deviceId}/{mediaTitle}/")]HttpRequestMessage req, string deviceId, string mediaTitle,
            [Queue(QueueNames.MediaToEncode)] out MediaMetadata mediaMetadataToEncode,
            TraceWriter log)
        {
            log.Info($"Webhook was triggered!");
            var mediaBlobAsByteArrary = req.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            var badRequestMessageStringBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(deviceId))
                badRequestMessageStringBuilder.AppendLine("Device Id Empty");

            if (string.IsNullOrWhiteSpace(mediaTitle))
                badRequestMessageStringBuilder.AppendLine("Video Title Empty");

            if (badRequestMessageStringBuilder.Length > 0)
            {
                mediaMetadataToEncode = null;
                return req.CreateResponse(HttpStatusCode.BadRequest, badRequestMessageStringBuilder.ToString());
            }

            log.Info($"Using Azure Media Service Rest API Endpoint : {APIEndpointUrls.MediaServiceRestEndpoint}");

            IAsset newAzureMediaServicesAsset = null;

            try
            {
                mediaMetadataToEncode = new MediaMetadata
                {
                    FileName = $"{deviceId}_{DateTime.UtcNow.Ticks}.mp4",
                    Title = mediaTitle,
                    UploadedAt = DateTimeOffset.UtcNow
                };

                log.Info("Context object created.");

                newAzureMediaServicesAsset = AzureMediaServices.CreateAssetAndUploadSingleFile(AssetCreationOptions.None, mediaTitle, mediaMetadataToEncode.FileName, mediaBlobAsByteArrary, log);
                mediaMetadataToEncode.MediaServicesAssetId = newAzureMediaServicesAsset.Id;
                mediaMetadataToEncode.MediaAssetUri = newAzureMediaServicesAsset.Uri;

                log.Info("new asset created.");
            }
            catch (Exception ex)
            {
                log.Info($"Exception {ex}");

                mediaMetadataToEncode = null;
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }

            log.Info("asset Id: " + newAzureMediaServicesAsset.Id);
            log.Info("container Path: " + newAzureMediaServicesAsset.Uri.Segments[1]);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                containerPath = newAzureMediaServicesAsset.Uri.Segments[1],
                assetId = newAzureMediaServicesAsset.Id
            });
        }
    }
}
