using System;

namespace XamCam.Functions
{
    static class EnvironmentVariables
    {
        #region Blob Storage
        public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
        #endregion

        #region CosmosDb
        public static readonly string CosmosDBConnectionString = Environment.GetEnvironmentVariable(nameof(CosmosDBConnectionString));
        public const string CosmosDbCollectionId = nameof(MediaMetadata);
        public const string DatabaseId = "MediaMetadataDatabase";
        #endregion

        #region AzureAD
        public static readonly string TenantId = Environment.GetEnvironmentVariable(nameof(TenantId));
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable(nameof(ClientSecret));
        public static readonly string ClientId = Environment.GetEnvironmentVariable(nameof(ClientId));
        #endregion

        #region Webhook and Signing Key
        public static readonly string WebHookEndpoint = Environment.GetEnvironmentVariable(nameof(WebHookEndpoint));
        public static readonly string WebHookSigningKey = Environment.GetEnvironmentVariable(nameof(WebHookSigningKey));
        #endregion

        #region IoT Hub
        public static readonly string IoTHubConnectionString = Environment.GetEnvironmentVariable(nameof(IoTHubConnectionString));
        #endregion
    }
}
