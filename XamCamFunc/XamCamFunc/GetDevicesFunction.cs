using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using IOTManager;
using System.Threading.Tasks;

namespace XamCamFunc
{
    public static class GetDevicesFunction
    {
        [FunctionName("GetDevices")]

        async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetDevices/{id?}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var res = await DeviceManager.Instance.GetAllDevicesAsync(id);
            return req.CreateResponse(HttpStatusCode.OK, res);
            
        }
    }
}
