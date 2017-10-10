using System;

namespace XamCamFunctions
{
    public static class Constants
    {
        //BLOB STORAGE STRINGS
        public static string BlobURLAndKey = Environment.GetEnvironmentVariable("BlobURLAndKey");       
        public static string DirectUploadBlobURLAndKey = Environment.GetEnvironmentVariable("DirectUploadBlobURLAndKey");

        //COSMOS DB STRING
        public const string CosmosDBEndPoint = "https://xamcamcosmosdb.documents.azure.com:443/";
        public static string CosmosDBMyKey =  "mMyIRtdLejDrhTkpBBgKBXyZyzMCi02sU0xajBwygaQHHndxTC1VYvxHygB0EoDtqBbJEtmIRLxBTAVKVtzg1g==";
        

        //CONSTANTS NEEDED FOR AZURE AD
        public static string tenantId = Environment.GetEnvironmentVariable("tenantId");    
        public static string GrantType = Environment.GetEnvironmentVariable("GrantType"); 
        public static string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret"); 
        public static string ClientID = Environment.GetEnvironmentVariable("ClientID"); 
        public static string RequestedResource = "https://rest.media.azure.net";
        public static string MediaServiceRestEndpoint = "https://xamcammediaservice.restv2.westus.media.azure.net/api/";

        //BLOB STORAGE ACCOUNT
        public static string StorageAccountName = "xamcamstorage";
        public static string StorageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey"); 

        //WEBHOOK AND SIGNING KEY
        public static string WebHookEndpoint = Environment.GetEnvironmentVariable("WebHookEndpoint"); 
        public static string WebHookSigningKey = Environment.GetEnvironmentVariable("WebHookSigningKey"); 
        
        //AMS CONSTANTS WITH URL
        public static string AMSUrlWithSlash = "http://xamcammediaservice.streaming.mediaservices.windows.net/";
    }
}
