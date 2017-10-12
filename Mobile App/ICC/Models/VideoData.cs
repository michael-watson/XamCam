using System;

using Newtonsoft.Json;

namespace ICC.Models
{
	public class VideoData
	{
		[JsonProperty("fileName")]
		public string FileName { get; set; }

		[JsonProperty("mediaAssetUri")]
		public string MediaAssetUri { get; set; }

		[JsonProperty("duration")]
		public TimeSpan Duration { get; set; }

		[JsonProperty("accountType")]
		public object AccountType { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("hLS")]
		public string HLS { get; set; }

		[JsonProperty("mPEGDash")]
		public string MPEGDash { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("smoothStreaming")]
		public string SmoothStreaming { get; set; }

		[JsonProperty("uploadedAt")]
		public DateTime UploadedAt { get; set; }
	}
}