using System;

namespace XamCam.Functions
{
    static class EnvironmentVariables
    {
        #region Blob Storage
        public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
        public static readonly string BlobStorageConnectionString = Environment.GetEnvironmentVariable(nameof(BlobStorageConnectionString));
        #endregion

        #region CosmosDb
        public static readonly string CosmosDBConnectionString = Environment.GetEnvironmentVariable(nameof(CosmosDBConnectionString));
        public const string CosmosDbCollectionId = nameof(MediaMetadata);
        public const string CosmosDbDatabaseId = "MediaMetadataDatabase";
        #endregion

        #region AzureAD
        public static readonly string TenantId = Environment.GetEnvironmentVariable(nameof(TenantId));
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable(nameof(ClientSecret));
        public static readonly string ClientId = Environment.GetEnvironmentVariable(nameof(ClientId));
        #endregion

        #region IoT Hub
        public static readonly string IoTHubConnectionString = Environment.GetEnvironmentVariable(nameof(IoTHubConnectionString));
        #endregion
    }
}
