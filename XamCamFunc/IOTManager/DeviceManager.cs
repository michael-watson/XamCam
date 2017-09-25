﻿using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using IOTManager.Models;

namespace IOTManager
{
    public class DeviceManager
    {

        RegistryManager _registryManager;
        static DeviceManager _instance;

        //Singleton implementation
        public static DeviceManager Instance =>
            _instance ?? (_instance = new DeviceManager());

        DeviceManager()
        {
            if (_registryManager == null)
            {
                _registryManager = RegistryManager.CreateFromConnectionString(Constants.IotHubConfig.connectionString);
            }
        }

        /// <summary>
        /// returns a connectionstring to the newly created device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
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
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating the device: {e.Message}");
                return string.Empty;
            }
            
            var connectionString = $"HostName={Constants.IotHubConfig.HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

            return connectionString;
        }

        /// <summary>
        /// Get All Devices 
        /// </summary>
        /// <returns> List of Devices</returns>
        public async Task<List<DeviceConfig>> GetAllDevicesAsync(string deviceId = null)
        {
            var deviceList = new List<Device>();

            if (!string.IsNullOrEmpty(deviceId))
            {
                var device = await _registryManager?.GetDeviceAsync(deviceId);
                if(device!=null)
                    deviceList.Add(device);
            }
            else
            {
                var devices = await _registryManager?.GetDevicesAsync(Constants.IotHubConfig.MAX_DEVICE_LIST);
                if(devices!=null)
                    deviceList.AddRange(devices);
            }

            List<DeviceConfig> listOfDevices = new List<DeviceConfig>();

            foreach(var device in deviceList)
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
        /// Remove a Device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task RemoveDeviceAsync(string deviceId)
        {
            await _registryManager.RemoveDeviceAsync(deviceId);
        }

    }
}
