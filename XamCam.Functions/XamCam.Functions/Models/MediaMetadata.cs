using System;

namespace XamCam.Functions
{
	public class MediaMetadata
	{
        public MediaMetadata() => Id = Guid.NewGuid().ToString();
    
		public string Id { get; set; }
        public string MediaServicesAssetId { get; set; }
		public Uri MediaAssetUri { get; set; }
		public string FileName { get; set; }
		public DateTimeOffset UploadedAt { get; set; }
		public string Title { get; set; }
		public Uri ManifestUri{ get; set; }
		public Uri HLSUri { get; set; }
		public Uri MPEGDashUri { get; set; }
	}
}