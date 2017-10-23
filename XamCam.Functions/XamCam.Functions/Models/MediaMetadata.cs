using System;

namespace XamCam.Functions.Models
{
	public class MediaMetadata
	{
        public MediaMetadata() => Id = Guid.NewGuid().ToString();

		public string Id { get; set; }
		public string MediaAssetUri { get; set; }
		public string FileName { get; set; }
		public DateTimeOffset UploadedAt { get; set; }
		public string Title { get; set; }
		public string HLSUrl { get; set; }
		public string SmoothStreamingUrl{ get; set; }
		public string MPEGDashUrl { get; set; }
	}
}