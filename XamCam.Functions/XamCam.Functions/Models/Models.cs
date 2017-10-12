using System;

namespace XamCam.Functions.Models
{
	public class DeviceConfig
	{
		public string Nickname { get; set; }
		public string DeviceId { get; set; }
		public string GenerationId { get; set; }
		public string ETag { get; set; }
		public DeviceConnectionState ConnectionState { get; set; }
		public DeviceStatus Status { get; set; }
		public string StatusReason { get; set; }
		public DateTime ConnectionStateUpdatedTime { get; set; }
		public DateTime StatusUpdatedTime { get; set; }
		public DateTime LastActivityTime { get; set; }
		public int CloudToDeviceMessageCount { get; set; }
	}

	public enum DeviceConnectionState { Disconnected, Connected }
	public enum DeviceStatus { Enabled, Disabled }
}
