using System;

namespace XamCamFunctions
{
    public static class Constants
    {
        //BLOB STORAGE STRINGS
        public const string BlobURLAndKey = "DefaultEndpointsProtocol=https;AccountName=xamcamstorage;AccountKey=N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==;EndpointSuffix=core.windows.net";

        public const string DirectUploadBlobURLAndKey = "DefaultEndpointsProtocol=https;AccountName=directuploadxamcam;AccountKey=kR5ejiqV1K4GfzCtQjB8Qp+1Ktdj1ktrvnJlesZ3EzjJHAtSKQmJbm7nHnZo91GWZ5/RlXaHT3O7JNgHU7vLog==;EndpointSuffix=core.windows.net";

        //COSMOS DB STRING
        public const string CosmosDBEndPoint = "https://xamcamcosmosdb.documents.azure.com:443/";
        public const string CosmosDBMyKey = "mMyIRtdLejDrhTkpBBgKBXyZyzMCi02sU0xajBwygaQHHndxTC1VYvxHygB0EoDtqBbJEtmIRLxBTAVKVtzg1g==";

        //CONSTANTS NEEDED FOR AZURE AD
        public static string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        public static string GrantType = "client_credentials";
        public static string ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
        public static string ClientID = "8d631792-ed10-46aa-bd09-b8ca1641bc6f";
        public static string RequestedResource = "https://rest.media.azure.net";
        public static string MediaServiceRestEndpoint = "https://xamcammediaservice.restv2.westus.media.azure.net/api/";

        //BLOB STORAGE ACCOUNT
        public static string StorageAccountName = "xamcamstorage";
        public static string StorageAccountKey = "N0cfqGOzaWIkSUNfiUxodYEmD1yHLAFexLw6YG8hg2368MBho3MsiC6BLbeoyfjUodNjOzax1vZEGDprHrK3aQ==";

        //WEBHOOK AND SIGNING KEY

        public static string WebHookEndpoint = "https://iccfunction.azurewebsites.net/api/NewXamCamWebHook?code=FDKntb18eg2ZrnJWeN/ySSNEc99ha9a5uTqpwKfRZrUDW5HztvlY2A==&clientId=default";
        //public static string WebHookEndpoint = "https://iccfunction.azurewebsites.net/api/NewXamCamWebHook?code=FDKntb18eg2ZrnJWeN/ySSNEc99ha9a5uTqpwKfRZrUDW5HztvlY2A==";

        public static string WebHookSigningKey = "FDKntb18eg2ZrnJWeN/ySSNEc99ha9a5uTqpwKfRZrUDW5HztvlY2A==";

    }
}
