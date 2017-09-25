using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;


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
                var d = new Device(deviceId) { Status = DeviceStatus.Enabled };
                device = await _registryManager?.AddDeviceAsync(d);
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager?.GetDeviceAsync(deviceId);
            }

            var connectionString = $"HostName={Constants.IotHubConfig.HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

            return connectionString;
        }

        public async Task<Device> GetDeviceAsync(string deviceId)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId);

            return device;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var deviceList = await _registryManager.GetDevicesAsync(Constants.IotHubConfig.MAX_DEVICE_LIST);

            return new List<Device>(deviceList);
        }

        public async Task RemoveDeviceAsync(string deviceId)
        {
            await _registryManager.RemoveDeviceAsync(deviceId);
        }

    }
}
