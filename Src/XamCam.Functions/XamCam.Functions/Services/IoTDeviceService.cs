using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Net.Http;
using System.Net;

namespace XamCam.Functions
{
	public class IoTDeviceService
	{
		#region Constant Fields
		readonly static RegistryManager registryManager = RegistryManager.CreateFromConnectionString(EnvironmentVariables.IoTHubConnectionString);

		readonly static Lazy<HttpClient> httpClientHolder = new Lazy<HttpClient>(() =>
			new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
			{
				BaseAddress = new Uri("https://homecamfunction.azurewebsites.net/api/")
			});
		#endregion

		#region Fields
		static IoTDeviceService instance;
		#endregion

		#region Properties
		static HttpClient Client => httpClientHolder.Value;
		static ServiceClient serviceClient;

		public static IoTDeviceService Instance =>
			instance ?? (instance = new IoTDeviceService());
		#endregion

		#region Constructors
		IoTDeviceService()
		{
		}
		#endregion

		#region Methods
		public async Task<string> AddDeviceAsync(string deviceId)
		{
			Device device;

			try
			{
				var d = new Device(deviceId) { Status = Microsoft.Azure.Devices.DeviceStatus.Enabled };
				device = await registryManager?.AddDeviceAsync(d);
			}
			catch (DeviceAlreadyExistsException)
			{
				device = await registryManager?.GetDeviceAsync(deviceId);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine($"Error creating the device: {e.Message}");
				return string.Empty;
			}

			var connectionString = $"HostName={IoTConstants.HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

			return connectionString;
		}

		public async Task<string> StartRecording(string deviceId)
		{
			var connectionString = await GetConnectionString(deviceId);
			serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

			var methodInvocation = new CloudToDeviceMethod("StartRecording") { ResponseTimeout = TimeSpan.FromSeconds(30) };
			//methodInvocation.SetPayloadJson($"'{deviceId}'");

			var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

			if (response.Status == 200)
				return string.Empty;

			return "already recording";
		}

		public async Task<string> StopRecording(string deviceId)
		{
			var connectionString = await GetConnectionString(deviceId);
			serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

			var methodInvocation = new CloudToDeviceMethod("StopRecording") { ResponseTimeout = TimeSpan.FromSeconds(30) };
			//methodInvocation.SetPayloadJson("'{deviceId}'");

			var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

			if (response.Status == 200)
				return string.Empty;

			return "not recording";
		}

		public async Task<List<DeviceConfig>> GetDevicesAsync(string deviceId = null)
		{
			var deviceList = new List<Device>();

			if (!string.IsNullOrEmpty(deviceId))
			{
				var device = await registryManager?.GetDeviceAsync(deviceId);
				if (device != null)
					deviceList.Add(device);
			}
			else
			{
				var devices = await registryManager?.GetDevicesAsync(IoTConstants.MaxDeviceList);
				if (devices != null)
					deviceList.AddRange(devices);
			}

			List<DeviceConfig> listOfDevices = new List<DeviceConfig>();

			foreach (var device in deviceList)
			{
				listOfDevices.Add(new DeviceConfig
				{
					DeviceId = device.Id,
					GenerationId = device.GenerationId,
					ETag = device.ETag,
					ConnectionState = (DeviceConnectionState)(int)device.ConnectionState,
					Status = (DeviceStatus)(int)device.Status,
					StatusReason = device.StatusReason,
					ConnectionStateUpdatedTime = device.ConnectionStateUpdatedTime,
					StatusUpdatedTime = device.ConnectionStateUpdatedTime,
					LastActivityTime = device.LastActivityTime,
					CloudToDeviceMessageCount = device.CloudToDeviceMessageCount
				});
			}

			return listOfDevices;
		}

		public async Task<bool> RemoveDeviceAsync(string deviceId)
		{
			try
			{
				await registryManager.RemoveDeviceAsync(deviceId);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		async Task<string> GetConnectionString(string deviceId)
		{
			var postDeviceApiUrl = $"{APIEndpointUrls.GetIoTHubConenctionStringAPI}/{deviceId}";
			var httpResponseMessage = await Client.PostAsync(postDeviceApiUrl, null).ConfigureAwait(false);

			return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
		}
		#endregion
	}
}