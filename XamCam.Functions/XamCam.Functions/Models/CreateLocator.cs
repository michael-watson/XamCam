using System;

namespace XamCam.Functions.Models
{
	class CreateLocatorBody
	{
		public string AccessPolicyId { get; set; }
		public string AssetId { get; set; }
		public DateTime StartTime { get; set; }
		public int Type { get; set; }
	}

	public class AccessPolicy
	{
		public Deferred __deferred { get; set; }
	}

	public class Asset
	{
		public Deferred __deferred { get; set; }
	}

	public class Locator
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

	public class CreatedLocatorRoot
	{
		public Locator d { get; set; }
	}
}