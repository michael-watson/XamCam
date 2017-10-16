using System;

namespace XamCam.Functions
{
	public static class Constants
	{
		//BLOB STORAGE STRINGS
		public static readonly string BlobURLAndKey = Environment.GetEnvironmentVariable("BlobURLAndKey");
        public static readonly string DirectUploadBlobURLAndKey = Environment.GetEnvironmentVariable("DirectUploadBlobURLAndKey");

		//COSMOS DB STRING
		public const string CosmosDBEndPoint = "https://xamcamcosmosdb.documents.azure.com:443/";
        public static readonly string CosmosDBKey = Environment.GetEnvironmentVariable("CosmosDBMyKey");

		//CONSTANTS NEEDED FOR AZURE AD
        public static readonly string TenantId = Environment.GetEnvironmentVariable("TenantId");
        public static readonly string GrantType = Environment.GetEnvironmentVariable("GrantType");
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        public static readonly string ClientID = Environment.GetEnvironmentVariable("ClientID");
		public const string RequestedResource = "https://rest.media.azure.net";
        public const string MediaServiceRestEndpoint = "https://icchomecam.restv2.westus.media.azure.net/api/";

        //BLOB STORAGE ACCOUNT
        public const string StorageAccountName = "xamcamstorage";
        public static readonly string StorageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
        public static readonly string BlobStorageAccountConnection = Environment.GetEnvironmentVariable("BlobStorageAccountConnection");

		//WEBHOOK AND SIGNING KEY
		public static readonly string WebHookEndpoint = Environment.GetEnvironmentVariable("WebHookEndpoint");
		public static readonly string WebHookSigningKey = Environment.GetEnvironmentVariable("WebHookSigningKey");

		//AMS CONSTANTS WITH URL
		public const string AMSUrlWithSlash = "http://xamcammediaservice.streaming.mediaservices.windows.net/";

		public const string HostName = "HomeCam-IoT.azure-devices.net";
		public const string IotHubD2CEndpoint = "messages/events";
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
		//$"HostName={HostName};SharedAccessKeyName=iothubowner;SharedAccessKey=SF7KzXbqc+zq0l7YyVtvyQI2KR9OsGrEqzaXLWwq86c=";
		public const int MaxDeviceList = 50;
	}
}