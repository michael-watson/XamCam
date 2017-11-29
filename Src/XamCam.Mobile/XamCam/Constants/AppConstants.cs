namespace XamCam
{
	public static class AppConstants
	{
		public static string GetDevicesUrl => $"{baseUrl}GetDevices";
		public static string MediaAssetsUrl => $"{baseUrl}GetMediaMetadata";
		public static string StartRecordingUrl => $"{baseUrl}StartRecording/";
		public static string StopRecordingUrl => $"{baseUrl}StopRecording/";

		const string baseUrl = "https://homecamfunction.azurewebsites.net/api/";
	}
}