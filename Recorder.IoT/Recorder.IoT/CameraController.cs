using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using Newtonsoft.Json;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

using Windows.Storage;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.System.Profile;
using Windows.Storage.Streams;
using Windows.Graphics.Display;
using Windows.Devices.Enumeration;
using Windows.Media.MediaProperties;

namespace Recorder.IoT
{
    public class CameraController
    {
        static CameraController instance;

        public static CameraController Instance
        {
            get
            {
                if (instance == null)
                    instance = new CameraController();

                return instance;
            }
        }

        //Recording Properties
        bool isRecording;
        bool isInitialized;
        string recordedFileName = "No name provided";
        string recordedFileStorageName = string.Empty;

        //IoT Hub Device Info Properties
        Twin deviceTwin;
        HttpClient httpClient = new HttpClient();
        DeviceClient deviceClient;
        List<DeviceInformation> deviceList;

        //Media Capture/Profile Properties
        MediaCapture mediaCapture;
        MediaEncodingProfile profile;

        public async Task Initialize()
        {
            try
            {
                var url = $"https://homecamfunction.azurewebsites.net/api/AddDevice/id/{DeviceInfo.Instance.Id}";
                var connectionString = await httpClient.GetStringAsync(url);
                connectionString = connectionString.Trim('"');

                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
                deviceTwin = await deviceClient.GetTwinAsync();

                await deviceClient.SetMethodHandlerAsync("StartRecording", startRecordingAsync, null);
                await deviceClient.SetMethodHandlerAsync("StopRecording", stopRecordingAsync, null);

                await initializeCameraAsync();

                await startRecordingAsync(null, null);
                await Task.Delay(5000);
                await stopRecordingAsync(null, null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                isInitialized = true;
            }
        }

        async Task<MethodResponse> startRecordingAsync(MethodRequest methodRequest, object userContext)
        {
            if (isRecording)
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Already Recording"), (int)HttpStatusCode.Conflict));
            if (!isInitialized)
                await initializeCameraAsync();

            var filename = methodRequest?.DataAsJson;
            if (string.IsNullOrWhiteSpace(filename))
                recordedFileName = "No name provided";
            else
                recordedFileName = filename;

            isRecording = true;

            var storageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync($"{DeviceInfo.Instance.Id}-{DateTime.UtcNow.Ticks}.mp4", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            recordedFileStorageName = storageFile.Name;

            await mediaCapture.StartRecordToStorageFileAsync(profile, storageFile);
            await reportUpdatesAsync();

            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'Success'"), 200));
        }

        async Task<MethodResponse> stopRecordingAsync(MethodRequest methodRequest, object userContext)
        {
            if (!isRecording) return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Not currently recording"), (int)HttpStatusCode.Conflict));

            await mediaCapture.StopRecordAsync();

            //Send video to Azure Function
            await uploadVideoAsync();

            isRecording = false;

            await reportUpdatesAsync();

            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'Success'"), 200));
        }

        async Task uploadVideoAsync()
        {
            await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{deviceTwin.DeviceId}: Upload Started")));

            var storageFile = await Windows.Storage.KnownFolders.VideosLibrary.GetFileAsync(recordedFileStorageName);
            if (storageFile != null)
            {
                var videoToUpload = await fileToBytesAsync(storageFile);
                using (var httpClient = new HttpClient())
                {
                    //var stringContent = JsonConvert.SerializeObject(uploadContent);

                    var title = Uri.EscapeDataString(recordedFileName);
                    var url = $"http://iccfunction.azurewebsites.net/api/PostMediaAssetToSpecifiedBlobContainer/{DeviceInfo.Instance.Id}/{title}";

                    var content = new ByteArrayContent(videoToUpload);
                    //var content = new StringContent(stringContent, Encoding.UTF8, "application/json");
                    var postResult = await httpClient.PostAsync(url, content);
                    var success = await postResult.Content.ReadAsStringAsync();

                    if (postResult.IsSuccessStatusCode)
                    {
                        //Stub if we want to do handle post result

                    }

                    System.Diagnostics.Debug.WriteLine(success);
                }

                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{deviceTwin.DeviceId}: Upload Completed")));
            }
        }

        async Task reportUpdatesAsync()
        {
            var updatedTwin = await deviceClient.GetTwinAsync();

            if (updatedTwin != null) deviceTwin = updatedTwin;

            if (isRecording)
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{DeviceInfo.Instance.Id}: Recording Started")));
            else
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{DeviceInfo.Instance.Id}: Recording Stopped")));

            TwinCollection props = new TwinCollection();
            props["IsRecording"] = isRecording;
            await deviceClient.UpdateReportedPropertiesAsync(props);

            await deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>($"{{\"IsRecording\":\"{isRecording}\"}}"));
        }

        async Task initializeCameraAsync()
        {
            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);
            deviceList = new List<Windows.Devices.Enumeration.DeviceInformation>();

            if (devices.Count > 0)
            {
                for (var i = 0; i < devices.Count; i++)
                {
                    deviceList.Add(devices[i]);
                }

                await initMediaCaptureAsync();
            }
        }

        async Task initMediaCaptureAsync()
        {
            var captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
            captureInitSettings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video;
            captureInitSettings.PhotoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.VideoPreview;

            if (deviceList.Count > 0)
            {
                captureInitSettings.VideoDeviceId = deviceList[0].Id;
            }

            mediaCapture = new Windows.Media.Capture.MediaCapture();
            await mediaCapture.InitializeAsync();

            // Add video stabilization effect during Live Capture
            Windows.Media.Effects.VideoEffectDefinition def = new Windows.Media.Effects.VideoEffectDefinition(Windows.Media.VideoEffects.VideoStabilization);
            await mediaCapture.AddVideoEffectAsync(def, MediaStreamType.VideoRecord);

            profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Qvga);

            // Use MediaEncodingProfile to encode the profile
            System.Guid MFVideoRotationGuild = new System.Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

            int MFVideoRotation = convertVideoRotationToMFRotation(VideoRotation.None);
            profile.Video.Properties.Add(MFVideoRotationGuild, PropertyValue.CreateInt32(MFVideoRotation));

            // add the mediaTranscoder 
            var transcoder = new Windows.Media.Transcoding.MediaTranscoder();
            transcoder.AddVideoEffect(Windows.Media.VideoEffects.VideoStabilization);

            // wire to preview XAML element
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }

        int convertVideoRotationToMFRotation(VideoRotation rotation)
        {
            int MFVideoRotation = 0;

            switch (rotation)
            {
                case VideoRotation.Clockwise90Degrees:
                    MFVideoRotation = 90;
                    break;
                case VideoRotation.Clockwise180Degrees:
                    MFVideoRotation = 180;
                    break;
                case VideoRotation.Clockwise270Degrees:
                    MFVideoRotation = 270;
                    break;
            }

            return MFVideoRotation;
        }

        async static Task<byte[]> fileToBytesAsync(StorageFile file)
        {
            RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromFile(file);
            IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();

            byte[] buffer = new byte[streamWithContent.Size];
            await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);

            return buffer;
        }
    }
}