namespace XamCam
{
    public static class AppConstants
    {
        public static string GetDevicesUrl => $"{baseUrl}GetDevices";
        public static string MediaAssetsUrl => $"{baseUrl}GetMediaMetadata";

        const string baseUrl = "https://homecamfunction.azurewebsites.net/api/";
    }
}
