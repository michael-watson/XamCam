using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FunctionApp4.DataModels.CreateAnAsset
{
    public class CreateAnAssetBody
    {
        public string Name { get; set; }
        public string Options { get; set; }
    }

    public class HttpsXamcammediaserviceRestv2WestusMediaAzureNetApiMetadataWindowsAzureMediaServicesPublish
    {
        public string title { get; set; }
        public string target { get; set; }
    }

    public class Actions
    {
        [JsonProperty("https://xamcammediaservice.restv2.westus.media.azure.net/api/$metadata#WindowsAzureMediaServices.Publish")]
        public List<HttpsXamcammediaserviceRestv2WestusMediaAzureNetApiMetadataWindowsAzureMediaServicesPublish> WindowsAzureMediaServicesPublish { get; set; }
    }

    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
        public Actions actions { get; set; }
    }

    public class Deferred
    {
        public string uri { get; set; }
    }

    public class Locators
    {
        public Deferred __deferred { get; set; }
    }

    public class Deferred2
    {
        public string uri { get; set; }
    }

    public class ContentKeys
    {
        public Deferred2 __deferred { get; set; }
    }

    public class Deferred3
    {
        public string uri { get; set; }
    }

    public class Files
    {
        public Deferred3 __deferred { get; set; }
    }

    public class Deferred4
    {
        public string uri { get; set; }
    }

    public class ParentAssets
    {
        public Deferred4 __deferred { get; set; }
    }

    public class Deferred5
    {
        public string uri { get; set; }
    }

    public class DeliveryPolicies
    {
        public Deferred5 __deferred { get; set; }
    }

    public class Deferred6
    {
        public string uri { get; set; }
    }

    public class AssetFilters
    {
        public Deferred6 __deferred { get; set; }
    }

    public class Deferred7
    {
        public string uri { get; set; }
    }

    public class StorageAccount
    {
        public Deferred7 __deferred { get; set; }
    }

    public class D
    {
        public Metadata __metadata { get; set; }
        public Locators Locators { get; set; }
        public ContentKeys ContentKeys { get; set; }
        public Files Files { get; set; }
        public ParentAssets ParentAssets { get; set; }
        public DeliveryPolicies DeliveryPolicies { get; set; }
        public AssetFilters AssetFilters { get; set; }
        public StorageAccount StorageAccount { get; set; }
        public string Id { get; set; }
        public int State { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public object AlternateId { get; set; }
        public string Name { get; set; }
        public int Options { get; set; }
        public int FormatOption { get; set; }
        public string Uri { get; set; }
        public string StorageAccountName { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}
