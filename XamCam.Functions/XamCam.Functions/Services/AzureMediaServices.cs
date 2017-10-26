using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace XamCam.Functions
{
    static class AzureMediaServices
    {
        #region Constant Fields
        const string encoderName = "Media Encoder Standard";
        const string encoderPreset = "Content Adaptive Multiple Bitrate MP4";
        #endregion

        #region Fields
        static CloudMediaContext _cloudMediaContext;
        #endregion

        #region Properties
        public static CloudMediaContext CloudMediaContext => _cloudMediaContext ??
                (_cloudMediaContext = GetCloudMediaContext());
        #endregion

        #region Methods
        public static IAsset EncodeToAdaptiveBitrateMP4Set(IAsset asset, string outputAssetName)
        {
            var job = CloudMediaContext.Jobs.Create("Media Encoder Standard Job");
            var processor = GetLatestMediaProcessorByName(encoderName);

            var task = job.Tasks.AddNew($"Encoding {asset.Name}", processor, encoderPreset, TaskOptions.None);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew(outputAssetName, AssetCreationOptions.None);

            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).GetAwaiter().GetResult();

            return job.OutputMediaAssets.FirstOrDefault();
        }

        public static string GetMP4FileName(IAsset asset) =>
            Uri.EscapeDataString(asset.AssetFiles.Where(x => x.Name.ToLower().EndsWith(".mp4")).FirstOrDefault()?.Name)?.ToString();

        public static (string manifestUri, string hlsUri, string mpegDashUri) BuildStreamingURLs(IAsset asset)
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
            manifestUrl = manifestUrl.Replace(@"http://", @"https://");

            while (manifestUrl.Contains(" "))

            var hlsUrl = $"{manifestUrl}(format=m3u8-aapl)";
            var dashUrl = $"{manifestUrl}(format=mpd-time-csf)";

            return (manifestUrl, hlsUrl, dashUrl);
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

        static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = CloudMediaContext
                .MediaProcessors
                .Where(p => p.Name.Equals(mediaProcessorName))
                .ToList()
                .OrderBy(p => new Version(p.Version))
                .LastOrDefault();

            return processor ?? throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));
        }
        #endregion
    }
}
