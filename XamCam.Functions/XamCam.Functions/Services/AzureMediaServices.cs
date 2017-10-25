using System;
using System.IO;
using System.Linq;

using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace XamCam.Functions
{
    static class AzureMediaServices
    {
        static CloudMediaContext _cloudMediaContext;

        public static CloudMediaContext CloudMediaContext => _cloudMediaContext ??
                (_cloudMediaContext = GetCloudMediaContext());

        public static (Uri manifestUri, Uri hlsUri, Uri mpegDashUri) BuildStreamingURIs(IAsset asset)
        {
            var accessPolicy = CloudMediaContext.AccessPolicies.Create(
                "Streaming policy",
                TimeSpan.FromMinutes(30),
                AccessPermissions.Read);

            var originLocator = CloudMediaContext.Locators.CreateLocator(
                LocatorType.OnDemandOrigin,
                asset,
                accessPolicy,
                DateTime.UtcNow.AddMinutes(-5));

            var manifestFile = asset.AssetFiles.Where(x => x.Name.ToLower().EndsWith(".ism")).FirstOrDefault();

            var manifestUrl = originLocator.Path + manifestFile.Name + "/manifest";
            manifestUrl.Replace("http://", "https://");

            var hlsUrl = manifestFile + "(format=m3u8-aapl)";
            var dashUrl = manifestUrl + "(format=mpd-time-csf)";

            return (new Uri(manifestUrl), new Uri(hlsUrl), new Uri(dashUrl));
        }

        public static IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string mediaTitle, string fileName, byte[] mediaFile, TraceWriter log)
        {
            var inputAsset = CloudMediaContext.Assets.Create(mediaTitle, assetCreationOptions);

            var assetFile = inputAsset.AssetFiles.Create(fileName);

            log.Info($"Upload {assetFile.Name}");

            using (var memoryStream = new MemoryStream(mediaFile))
                assetFile.Upload(memoryStream);

            log.Info($"Done uploading {assetFile.Name}");

            return inputAsset;
        }

        public static IAsset GetAsset(MediaMetadata mediaMetadata) => 
            CloudMediaContext.Assets.Where(x => x.Id.Equals(mediaMetadata.MediaServicesAssetId)).FirstOrDefault();

        static CloudMediaContext GetCloudMediaContext()
        {
            var tokenCredentials = new AzureAdTokenCredentials(EnvironmentVariables.TenantId,
                                                                new AzureAdClientSymmetricKey(EnvironmentVariables.ClientId, EnvironmentVariables.ClientSecret),
                                                                AzureEnvironments.AzureCloudEnvironment);

            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            return new CloudMediaContext(new Uri(APIEndpointUrls.MediaServiceRestEndpoint), tokenProvider);
        }
    }
}
