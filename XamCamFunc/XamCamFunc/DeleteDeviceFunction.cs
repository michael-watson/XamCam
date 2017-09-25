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
    public static class DeleteDeviceFunction
    {
        [FunctionName("DeleteDevice")]

        async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "delete", "put", Route = "DeleteDevice/id/{id}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if (string.IsNullOrEmpty(id))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an id for a device");
            }

            await DeviceManager.Instance.RemoveDeviceAsync(id);

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, "Removed Device with Device Id: " + id);
        }
    }
}
