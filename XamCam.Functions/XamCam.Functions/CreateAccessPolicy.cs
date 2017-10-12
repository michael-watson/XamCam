using System;

namespace XamCamFunctions.DataModels.CreateAccessPolicy
{
    class CreateAccessPolicyBody
    {
        public string Name { get; set; }
        public string DurationInMinutes { get; set; }
        public string Permissions { get; set; }
    }

    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class D
    {
        public Metadata __metadata { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public int DurationInMinutes { get; set; }
        public int Permissions { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}
