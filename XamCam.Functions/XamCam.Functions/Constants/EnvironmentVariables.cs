using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamCam.Functions.Constants
{
    static class EnvironmentVariables
    {
         #region Blob Storage
        public static readonly string BlobURLAndKey = Environment.GetEnvironmentVariable("BlobURLAndKey");
        public static readonly string DirectUploadBlobURLAndKey = Environment.GetEnvironmentVariable("DirectUploadBlobURLAndKey");
        public static readonly string StorageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
        public static readonly string BlobStorageAccountConnection = Environment.GetEnvironmentVariable("BlobStorageAccountConnection");
        #endregion

        #region CosmosDb
        public static readonly string CosmosDBKey = Environment.GetEnvironmentVariable("CosmosDBMyKey");
        #endregion

        #region AzureAD
        public static readonly string TenantId = Environment.GetEnvironmentVariable("TenantId");
        public static readonly string GrantType = Environment.GetEnvironmentVariable("GrantType");
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        public static readonly string ClientID = Environment.GetEnvironmentVariable("ClientID");
        #endregion

        #region Webhook and Signing Key
        public static readonly string WebHookEndpoint = Environment.GetEnvironmentVariable("WebHookEndpoint");
        public static readonly string WebHookSigningKey = Environment.GetEnvironmentVariable("WebHookSigningKey");
        #endregion

        #region IoT Hub
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
        #endregion
    }
}
