using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace XamCam.Functions
{
    public class IoTDeviceService
    {
        #region Constant Fields
        readonly RegistryManager registryManager =
            RegistryManager.CreateFromConnectionString(EnvironmentVariables.IoTHubConnectionString);
        #endregion

        #region Fields
        static IoTDeviceService instance;
        #endregion

        #region Properties
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
        #endregion
    }
}