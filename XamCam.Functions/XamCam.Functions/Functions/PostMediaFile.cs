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

namespace XamCam.Functions.Functions
{
    public static class PostMediaFile
    {
        static readonly string _aadTenantDomain = Constants.TenantId;
        static readonly string _restAPIEndpoint = Constants.MediaServiceRestEndpoint;

        static readonly string _mediaservicesClientId = Constants.ClientID;
        static readonly string _mediaservicesClientSecret = Constants.ClientSecret;

        static CloudMediaContext _context = null;

        [FunctionName("PostMediaFile")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostMediaFile/{deviceId}/{mediaTitle}/{mediaFileExtension}")]HttpRequestMessage req, string deviceId, string mediaTitle, string mediaFileExtension, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");
            var mediaBlobAsByteArrary = await req.Content.ReadAsByteArrayAsync();

            if (string.IsNullOrWhiteSpace(mediaFileExtension))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Media File Extension Empty");

            if (string.IsNullOrWhiteSpace(deviceId))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Device Id Empty");

            if (string.IsNullOrWhiteSpace(mediaTitle))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Video Title Empty");

            log.Info($"Using Azure Media Service Rest API Endpoint : {_restAPIEndpoint}");

            IAsset newAzureMediaServicesAsset = null;

            try
            {
                var tokenCredentials = new AzureAdTokenCredentials(_aadTenantDomain,
                                                                new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                                                AzureEnvironments.AzureCloudEnvironment);

                var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                _context = new CloudMediaContext(new Uri(_restAPIEndpoint), tokenProvider);

                log.Info("Context object created.");

                newAzureMediaServicesAsset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, mediaTitle, mediaFileExtension, mediaBlobAsByteArrary);

                log.Info("new asset created.");

            }
            catch (Exception ex)
            {
                log.Info($"Exception {ex}");
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

        static public IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string fileName, string mediaFileExtension, byte[] mediaFile)
        {
            IAsset inputAsset = _context.Assets.Create(fileName, assetCreationOptions);

            var assetFile = inputAsset.AssetFiles.Create($"{fileName}.{mediaFileExtension}");

            Console.WriteLine("Upload {0}", assetFile.Name);

            using (var memoryStream = new MemoryStream(mediaFile))
                assetFile.Upload(memoryStream);

            Console.WriteLine("Done uploading {0}", assetFile.Name);

            return inputAsset;
        }
    }
}
