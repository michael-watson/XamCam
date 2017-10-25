using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace XamCam
{
    public static class APIService
    {
        #region Constant Fields
        static readonly Lazy<HttpClient> clientHolder = new Lazy<HttpClient>(CreateHttpClient);
        static readonly Lazy<JsonSerializer> serializerHolder = new Lazy<JsonSerializer>();
        #endregion

        #region Properties
        static HttpClient Client => clientHolder.Value;
        static JsonSerializer Serializer => serializerHolder.Value;
        #endregion

        #region Methods
        public static async Task<List<MediaMetadata>> GetAllVideosAsync() =>
            await GetDataObjectFromAPI<List<MediaMetadata>>(AppConstants.MediaAssetsUrl).ConfigureAwait(false);

        public static async Task<List<CameraDevice>> GetAllDevicesAsync() =>
            await GetDataObjectFromAPI<List<CameraDevice>>(AppConstants.GetDevicesUrl).ConfigureAwait(false);

        static async Task<T> GetDataObjectFromAPI<T>(string apiUrl)
        {
            try
            {
                using (var stream = await Client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    if (json == null)
                        return default(T);

                    return await Task.Run(() => Serializer.Deserialize<T>(json)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return default(T);
            }
        }

        static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
        #endregion
    }
}
