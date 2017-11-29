using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

using Windows.Storage;
using Windows.Storage.Streams;

namespace Recorder.IoT
{
    static class APIService
    {

        #region Constant Fields
        readonly static Lazy<HttpClient> httpClientHolder = new Lazy<HttpClient>(() =>
            new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            {
                BaseAddress = new Uri("https://homecamfunction.azurewebsites.net/api/")
            });

        const string postMediaFileAPI = "PostMediaFile";
        const string postMediaFileFunctionKey = "1E1q22hhjosmQhgOgpV88CbR8j4YIGrXG9ZFanTQGWu8BQn6cxROGw==";

        const string getIoTHubConenctionStringAPI = "AddDevice";
        #endregion

        #region Properties
        static HttpClient HttpClient => httpClientHolder.Value;
        #endregion

        #region Methods
        public static async Task<string> GetIoTHubConnectionString()
        {
            var postDeviceApiUrl = $"{getIoTHubConenctionStringAPI}/{DeviceInfo.Instance.Id}";
            var httpResponseMessage = await HttpClient.PostAsync(postDeviceApiUrl, null).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task UploadVideoAsync(DeviceClient deviceClient, Twin deviceTwin, string recordedFileName)
        {
            await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{deviceTwin.DeviceId}: Upload Started"))).ConfigureAwait(false);

            var storageFile = await KnownFolders.VideosLibrary.GetFileAsync(recordedFileName);
            if (storageFile == null)
                throw new NullReferenceException("Storage File Null");

            var videoToUpload = await ConvertFileToBytesAsync(storageFile).ConfigureAwait(false);
            var title = Uri.EscapeDataString(recordedFileName);
            var apiUrl = $"{postMediaFileAPI}/{deviceTwin?.DeviceId ?? Uri.EscapeDataString("No_Id")}/{title}?code={postMediaFileFunctionKey}";

            var content = new ByteArrayContent(videoToUpload);

            var postResult = await HttpClient.PostAsync(apiUrl, content).ConfigureAwait(false);
            var success = await postResult.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (postResult.IsSuccessStatusCode)
            {
                //Stub if we want to do handle post result
            }

            System.Diagnostics.Debug.WriteLine(success);

            await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{deviceTwin.DeviceId}: Upload Completed"))).ConfigureAwait(false);
        }

        async static Task<byte[]> ConvertFileToBytesAsync(StorageFile file)
        {
            var streamRef = RandomAccessStreamReference.CreateFromFile(file);
            var streamWithContent = await streamRef.OpenReadAsync();

            byte[] buffer = new byte[streamWithContent.Size];
            await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);

            return buffer;
        }
        #endregion
    }
}
