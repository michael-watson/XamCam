using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp4.DataModels.CreateLocator
{
    class CreateLocator
    {
    }

    class CreateLocatorBody
    {
   //"AccessPolicyId":"nb:pid:UUID:e10a3717-cd60-417b-8c8f-6de9157e769b",
   //"AssetId":"nb:cid:UUID:e7c7ce9e-c127-43ea-8457-4d81f4852a29",
   //"StartTime":"2017-08-15T23:25:53",
   //"Type":1

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
