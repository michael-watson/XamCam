using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

using Windows.Storage;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Graphics.Display;
using Windows.Devices.Enumeration;
using Windows.Media.MediaProperties;

namespace Recorder.IoT
{
    public class CameraController
    {
        #region Fields
        static CameraController instance;
        bool isRecording;
        bool isInitialized;
        string recordedFileName = "No name provided";
        string recordedFileStorageName = string.Empty;
        Twin deviceTwin;
        DeviceClient deviceClient;
        List<DeviceInformation> deviceList;
        MediaCapture mediaCapture;
        MediaEncodingProfile profile;
        #endregion

        #region Constructors
        CameraController() { }
        #endregion

        #region Properties
        public static CameraController Instance => instance ?? (instance = new CameraController());
        #endregion

        #region Methods
        public async Task Initialize()
        {
            try
            {
                var connectionString = await APIService.GetIoTHubConnectionString().ConfigureAwait(false);
                connectionString = connectionString.Trim('"');

                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
                deviceTwin = await deviceClient.GetTwinAsync().ConfigureAwait(false);

                await deviceClient.SetMethodHandlerAsync("StartRecording", StartRecordingAsync, null).ConfigureAwait(false);
                await deviceClient.SetMethodHandlerAsync("StopRecording", StopRecordingAsync, null).ConfigureAwait(false);

                await InitializeCameraAsync().ConfigureAwait(false);

                await StartRecordingAsync(null, null).ConfigureAwait(false);
                await Task.Delay(5000).ConfigureAwait(false);
                await StopRecordingAsync(null, null).ConfigureAwait(false);
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

        async Task<MethodResponse> StartRecordingAsync(MethodRequest methodRequest, object userContext)
        {
            if (isRecording)
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Already Recording"), (int)HttpStatusCode.Conflict)).ConfigureAwait(false);

            if (!isInitialized)
                await InitializeCameraAsync().ConfigureAwait(false);

            var filename = methodRequest?.DataAsJson;

            if (string.IsNullOrWhiteSpace(filename))
                recordedFileName = "No name provided";
            else
                recordedFileName = filename;

            isRecording = true;

            var storageFile = await KnownFolders.VideosLibrary.CreateFileAsync($"{DeviceInfo.Instance.Id}-{DateTime.UtcNow.Ticks}.mp4", CreationCollisionOption.GenerateUniqueName);
            recordedFileStorageName = storageFile.Name;

            await mediaCapture.StartRecordToStorageFileAsync(profile, storageFile);
            await ReportUpdatesAsync().ConfigureAwait(false);

            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'Success'"), 200)).ConfigureAwait(false);
        }

        async Task<MethodResponse> StopRecordingAsync(MethodRequest methodRequest, object userContext)
        {
            if (!isRecording) return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Not currently recording"), (int)HttpStatusCode.Conflict));

            await mediaCapture.StopRecordAsync();

            //Send video to Azure Function
            await APIService.UploadVideoAsync(deviceClient, deviceTwin, recordedFileName).ConfigureAwait(false);

            isRecording = false;

            await ReportUpdatesAsync().ConfigureAwait(false);

            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'Success'"), 200)).ConfigureAwait(false);
        }

        async Task ReportUpdatesAsync()
        {
            var updatedTwin = await deviceClient.GetTwinAsync().ConfigureAwait(false);

            if (updatedTwin != null) deviceTwin = updatedTwin;

            if (isRecording)
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{DeviceInfo.Instance.Id}: Recording Started"))).ConfigureAwait(false);
            else
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes($"{DeviceInfo.Instance.Id}: Recording Stopped"))).ConfigureAwait(false);

            TwinCollection props = new TwinCollection { ["IsRecording"] = isRecording };
            await deviceClient.UpdateReportedPropertiesAsync(props).ConfigureAwait(false);

            await deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>($"{{\"IsRecording\":\"{isRecording}\"}}")).ConfigureAwait(false);
        }

        async Task InitializeCameraAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            deviceList = new List<DeviceInformation>();

            if (devices.Count > 0)
            {
                for (var i = 0; i < devices.Count; i++)
                    deviceList.Add(devices[i]);

                await InitMediaCaptureAsync().ConfigureAwait(false);
            }
        }

        async Task InitMediaCaptureAsync()
        {
            var captureInitSettings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            };

            if (deviceList.Count > 0)
                captureInitSettings.VideoDeviceId = deviceList[0].Id;

            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            await AddVideoStabilization().ConfigureAwait(false);

            profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);

            var MFVideoRotationGuild = GetMediaEncodingProfileGUID();

            int MFVideoRotation = ConvertVideoRotationToMFRotation(VideoRotation.None);
            profile.Video.Properties.Add(MFVideoRotationGuild, PropertyValue.CreateInt32(MFVideoRotation));

            var transcoder = new Windows.Media.Transcoding.MediaTranscoder();
            transcoder.AddVideoEffect(Windows.Media.VideoEffects.VideoStabilization);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }

        Guid GetMediaEncodingProfileGUID() => new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        async Task AddVideoStabilization()
        {
            var def = new Windows.Media.Effects.VideoEffectDefinition(Windows.Media.VideoEffects.VideoStabilization);
            await mediaCapture.AddVideoEffectAsync(def, MediaStreamType.VideoRecord);
        }

        int ConvertVideoRotationToMFRotation(VideoRotation rotation)
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
        #endregion
    }
}