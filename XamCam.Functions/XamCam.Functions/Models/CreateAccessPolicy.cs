using System;

namespace XamCam.Functions.Models
{
	class CreateAccessPolicyBody
	{
		public string Name { get; set; }
		public string DurationInMinutes { get; set; }
		public string Permissions { get; set; }
	}

	public class PolicyDetails
	{
		public Metadata __metadata { get; set; }
		public string Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastModified { get; set; }
		public string Name { get; set; }
		public int DurationInMinutes { get; set; }
		public int Permissions { get; set; }
	}

	public class CreatedAccessPolicy
	{
		public PolicyDetails d { get; set; }
	}
}
