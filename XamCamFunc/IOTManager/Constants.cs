using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTManager
{
    public static class Constants
    {
        public static class IotHubConfig
        {
            //public static string HostName = "xamcamiothub.azure-devices.net";
            public static string HostName = "HomeCam-IoT.azure-devices.net";
            public static string iotHubD2CEndpoint = "messages/events";
            //public static string connectionString = "HostName=xamcamiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=jqD+sXsypBbXMy/Zjt6/zq6Bb27HbH21x1z3Lk4NnZg=";
            public static string connectionString = $"HostName={HostName};SharedAccessKeyName=iothubowner;SharedAccessKey=SF7KzXbqc+zq0l7YyVtvyQI2KR9OsGrEqzaXLWwq86c=";
            public static int MAX_DEVICE_LIST = 50;
        }


    }
}
