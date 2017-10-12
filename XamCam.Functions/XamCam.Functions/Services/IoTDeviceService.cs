using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

using XamCam.Functions.Models;

namespace XamCam.Functions
{
	public class IoTDeviceService
	{
		//Singleton implementation
		/// <summary>
		/// Gets the <c cref="IOTManager.DeviceManager">DeviceManager</c> instance.
		/// </summary>
		/// <value>The instance.</value>
		static IoTDeviceService _instance;
		public static IoTDeviceService Instance =>
			_instance ?? (_instance = new IoTDeviceService());


		RegistryManager _registryManager;

		IoTDeviceService()
		{
			if (_registryManager == null)
			{
				_registryManager = RegistryManager.CreateFromConnectionString(Constants.ConnectionString);
			}
		}

		/// <summary>
		/// Adds the device to IoT Hub if it isn't already. 
		/// </summary>
		/// <param name="deviceId">Device Identifier of IoT device</param>
		/// <returns>ConnectionString for device</returns>
		public async Task<string> AddDeviceAsync(string deviceId)
		{
			Device device;

			try
			{
				var d = new Device(deviceId) { Status = Microsoft.Azure.Devices.DeviceStatus.Enabled };
				device = await _registryManager?.AddDeviceAsync(d);
			}
			catch (DeviceAlreadyExistsException)
			{
				device = await _registryManager?.GetDeviceAsync(deviceId);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine($"Error creating the device: {e.Message}");
				return string.Empty;
			}

			var connectionString = $"HostName={Constants.HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
			//Get
			return connectionString;
		}

		/// <summary>
		/// Get <c cref="IOTManager.Models.DeviceConfig">DeviceConfig</c> from IoT hub based on the Device's Id. 
		/// </summary>
		/// <returns> DeviceConfig of specified device. If no device is specified, all devices will be returned</returns>
		public async Task<List<DeviceConfig>> GetDevicesAsync(string deviceId = null)
		{
			var deviceList = new List<Device>();

			if (!string.IsNullOrEmpty(deviceId))
			{
				var device = await _registryManager?.GetDeviceAsync(deviceId);
				if (device != null)
					deviceList.Add(device);
			}
			else
			{
				var devices = await _registryManager?.GetDevicesAsync(Constants.MAX_DEVICE_LIST);
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
					ConnectionState = (Models.DeviceConnectionState)(int)device.ConnectionState,
					Status = (Models.DeviceStatus)(int)device.Status,
					StatusReason = device.StatusReason,
					ConnectionStateUpdatedTime = device.ConnectionStateUpdatedTime,
					StatusUpdatedTime = device.ConnectionStateUpdatedTime,
					LastActivityTime = device.LastActivityTime,
					CloudToDeviceMessageCount = device.CloudToDeviceMessageCount
				});
			}

			return listOfDevices;
		}

		/// <summary>
		/// Remove a Device from IoT Hub
		/// </summary>
		/// <param name="deviceId">Device Identifier of IoT device</param>
		/// <returns>A boolean whether the operation was successful</returns>
		public async Task<bool> RemoveDeviceAsync(string deviceId)
		{
			try
			{
				await _registryManager.RemoveDeviceAsync(deviceId);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}