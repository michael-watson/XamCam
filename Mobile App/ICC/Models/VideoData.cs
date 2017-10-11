using System;

namespace ICC.Models
{
	public class VideoData
	{
		public string id { get; set; }
		public string mediaAssetUri { get; set; }
		public string email { get; set; }
		public string fileName { get; set; }
		public object uploadedAt { get; set; }
		public string title { get; set; }
		public TimeSpan duration { get; set; } = TimeSpan.Zero;
		public object accountType { get; set; }
	}
}