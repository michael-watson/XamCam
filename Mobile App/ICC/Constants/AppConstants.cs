using System;
namespace ICC.Constants
{
	public static class AppConstants
	{
		public static readonly string MediaAssetsUrl = "https://iccfunction.azurewebsites.net/api/GetMediaAssets";

		public static readonly string IotDeviceBaseUrl = "https://homecamfunction.azurewebsites.net/api/";
		public static readonly string GetDevicesUrl = $"{IotDeviceBaseUrl}GetDevices";
	}
}
