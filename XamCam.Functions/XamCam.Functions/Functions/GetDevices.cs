using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
	public static class GetDevices
	{
		[FunctionName(nameof(GetDevices))]
		async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetDevices/{id?}")]HttpRequestMessage req, string id, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

            var deviceConfigurationList = await IoTDeviceService.Instance.GetDevicesAsync(id).ConfigureAwait(false);

            return req.CreateResponse(HttpStatusCode.OK, deviceConfigurationList);
		}
	}
}