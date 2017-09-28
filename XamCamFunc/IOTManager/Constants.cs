namespace IOTManager
{
	public static class Constants
	{
		public static class IotHubConfig
		{
			public static string HostName = "HomeCam-IoT.azure-devices.net";
			public static string IotHubD2CEndpoint = "messages/events";
			public static string ConnectionString = $"HostName={HostName};SharedAccessKeyName=iothubowner;SharedAccessKey=SF7KzXbqc+zq0l7YyVtvyQI2KR9OsGrEqzaXLWwq86c=";
			public static int MAX_DEVICE_LIST = 50;
		}
	}
}