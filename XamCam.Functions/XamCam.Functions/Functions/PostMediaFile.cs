using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.IO;
using XamCam.Functions.Models;
using System.Text;

namespace XamCam.Functions.Functions
{
    public static class PostMediaFile
    {
        static readonly string _aadTenantDomain = Constants.TenantId;
        static readonly string _restAPIEndpoint = Constants.MediaServiceRestEndpoint;

        static readonly string _mediaservicesClientId = Constants.ClientID;
        static readonly string _mediaservicesClientSecret = Constants.ClientSecret;

        static CloudMediaContext _context = null;

        [FunctionName(nameof(PostMediaFile))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostMediaFile/{deviceId}/{mediaTitle}/{mediaFileExtension}")]HttpRequestMessage req, string deviceId, string mediaTitle, string mediaFileExtension,
            [Queue(Constants.AddToCosmosDbQueueName)] out MediaMetadata mediaMetadata,
            TraceWriter log)
        {
            log.Info($"Webhook was triggered!");
            var mediaBlobAsByteArrary = req.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            var badRequestMessageStringBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(mediaFileExtension))
                badRequestMessageStringBuilder.AppendLine("Media File Extension Empty");

            if (string.IsNullOrWhiteSpace(deviceId))
                badRequestMessageStringBuilder.AppendLine("Device Id Empty");

            if (string.IsNullOrWhiteSpace(mediaTitle))
                badRequestMessageStringBuilder.AppendLine("Video Title Empty");

            if (badRequestMessageStringBuilder.Length > 0)
            {
                mediaMetadata = null;
                return req.CreateResponse(HttpStatusCode.BadRequest, badRequestMessageStringBuilder.ToString());
            }
            
            log.Info($"Using Azure Media Service Rest API Endpoint : {_restAPIEndpoint}");

            IAsset newAzureMediaServicesAsset = null;

            try
            {
                mediaMetadata = new MediaMetadata
                {
                    FileName = $"{deviceId}_{DateTime.UtcNow.Ticks}.{mediaFileExtension}",
                    Title = mediaTitle,
                    UploadedAt = DateTimeOffset.UtcNow
                };

                var tokenCredentials = new AzureAdTokenCredentials(_aadTenantDomain,
                                                                new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                                                AzureEnvironments.AzureCloudEnvironment);

                var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                _context = new CloudMediaContext(new Uri(_restAPIEndpoint), tokenProvider);

                log.Info("Context object created.");

                newAzureMediaServicesAsset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, mediaTitle, mediaMetadata.FileName, mediaBlobAsByteArrary, log);

                log.Info("new asset created.");

            }
            catch (Exception ex)
            {
                log.Info($"Exception {ex}");

                mediaMetadata = null;
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

        static public IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string mediaTitle, string fileName, byte[] mediaFile, TraceWriter log)
        {
            IAsset inputAsset = _context.Assets.Create(mediaTitle, assetCreationOptions);

            var assetFile = inputAsset.AssetFiles.Create(fileName);

            log.Info($"Upload {assetFile.Name}");

            using (var memoryStream = new MemoryStream(mediaFile))
                assetFile.Upload(memoryStream);

            log.Info($"Done uploading {assetFile.Name}");

            return inputAsset;
        }
    }
}
