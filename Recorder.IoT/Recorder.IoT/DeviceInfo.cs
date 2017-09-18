using System;

using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Recorder.IoT
{
    public sealed class DeviceInfo
    {
        static DeviceInfo instance;

        public static string OSName { get; set; }

        public static DeviceInfo Instance {
            get
            {
                if (instance == null)
                    instance = new DeviceInfo();

                return instance;
            }
        }

        private static string GetId()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
            {
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = token.Id;
                var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                byte[] bytes = new byte[hardwareId.Length];
                dataReader.ReadBytes(bytes);

                return BitConverter.ToString(bytes).Replace("-", "");
            }

            throw new Exception("NO API FOR DEVICE ID PRESENT!");
        }

        private DeviceInfo()
        {
            var deviceInformation = new EasClientDeviceInformation();

            Id = GetId();
            Model = deviceInformation.SystemProductName;
            Manufracturer = deviceInformation.SystemManufacturer;
            Name = deviceInformation.FriendlyName;
            OSName = deviceInformation.OperatingSystem;
        }

        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Manufracturer { get; private set; }
        public string Name { get; private set; }
    }
}