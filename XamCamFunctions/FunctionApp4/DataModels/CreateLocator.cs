using System;

namespace FunctionApp4.DataModels.CreateLocator
{
    class CreateLocatorBody
    {
        public string AccessPolicyId { get; set; }
        public string AssetId { get; set; }
        public DateTime StartTime { get; set; }
        public int Type { get; set; }
    }

    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class Deferred
    {
        public string uri { get; set; }
    }

    public class AccessPolicy
    {
        public Deferred __deferred { get; set; }
    }

    public class Deferred2
    {
        public string uri { get; set; }
    }

    public class Asset
    {
        public Deferred2 __deferred { get; set; }
    }

    public class D
    {
        public Metadata __metadata { get; set; }
        public AccessPolicy AccessPolicy { get; set; }
        public Asset Asset { get; set; }
        public string Id { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public int Type { get; set; }
        public string Path { get; set; }
        public string BaseUri { get; set; }
        public string ContentAccessComponent { get; set; }
        public string AccessPolicyId { get; set; }
        public string AssetId { get; set; }
        public DateTime StartTime { get; set; }
        public object Name { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}