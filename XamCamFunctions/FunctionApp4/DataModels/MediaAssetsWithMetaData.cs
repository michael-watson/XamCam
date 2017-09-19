using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp4.DataModels
{
    class MediaAssetsWithMetaData
    {

        //PROPERTIES ARE KEPT LOWERCASE TO KEEP COSMOS DB PROPERTIES LOWERCASE
        //PROPERTIES ARE NOT TRANSFOMRED BY JSONPROPERTY TAGS BY COSMOSDB SDK
        public string id { get; set; }
        public string mediaAssetUri { get; set; }
  
        public string email { get; set; }
        
        public string fileName { get; set; }

        public string uploadedAt { get; set; }

        public string title { get; set; }
        
        public string accountType { get; set; }



    }
}