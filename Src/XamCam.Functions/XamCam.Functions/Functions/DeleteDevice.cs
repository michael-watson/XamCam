using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
    public static class DeleteDevice
    {
        [FunctionName(nameof(DeleteDevice))]
        async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteDevice/{id}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if (string.IsNullOrWhiteSpace(id))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an id for a device");

            var wasDeviceRemovedSuccessfully = await IoTDeviceService.Instance.RemoveDeviceAsync(id).ConfigureAwait(false);

            if (!wasDeviceRemovedSuccessfully)
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"Device with Id: {id} not found.");

            return req.CreateResponse(HttpStatusCode.OK, "Removed Device with Device Id: " + id);
        }
    }
}