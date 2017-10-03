using System;
using Newtonsoft.Json;

namespace XamCamFunctions.DataModels
{
    class MediaAssetsWithMetaData
    {
        //PROPERTIES ARE KEPT LOWERCASE TO KEEP COSMOS DB KEYS LOWERCASE
        //PROPERTIES ARE NOT TRANSFOMRED BY JSONPROPERTY TAGS BY COSMOSDB SDK
        public string id { get; set; }
        public string mediaAssetUri { get; set; }
        public string email { get; set; }
        public string fileName { get; set; }
        public DateTime uploadedAt { get; set; }
        public string title { get; set; }
        public string accountType { get; set; }
        
    }
}