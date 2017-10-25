using Newtonsoft.Json;

namespace XamCam
{
	public class CameraDevice
	{
		[JsonProperty("DeviceId")]
		public string DeviceId { get; set; }

		[JsonProperty("Nickname")]
		public object Nickname { get; set; }

		[JsonProperty("ConnectionState")]
		public long ConnectionState { get; set; }

		[JsonProperty("CloudToDeviceMessageCount")]
		public long CloudToDeviceMessageCount { get; set; }

		[JsonProperty("ConnectionStateUpdatedTime")]
		public string ConnectionStateUpdatedTime { get; set; }

		[JsonProperty("GenerationId")]
		public string GenerationId { get; set; }

		[JsonProperty("ETag")]
		public string ETag { get; set; }

		[JsonProperty("LastActivityTime")]
		public string LastActivityTime { get; set; }

		[JsonProperty("StatusReason")]
		public object StatusReason { get; set; }

		[JsonProperty("Status")]
		public long Status { get; set; }

		[JsonProperty("StatusUpdatedTime")]
		public string StatusUpdatedTime { get; set; }
	}

	public partial class GettingStarted
	{
		public static GettingStarted FromJson(string json) => JsonConvert.DeserializeObject<GettingStarted>(json, Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this GettingStarted self) => JsonConvert.SerializeObject(self, Converter.Settings);
	}

	public static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
		};
	}
}