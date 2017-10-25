using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
	public static class AddDevice
	{
		[FunctionName(nameof(AddDevice))]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AddDevice/id/{id}")]HttpRequestMessage req, string id, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

			if (string.IsNullOrWhiteSpace(id))
				return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an id for a device");

            var addDeviceResult = await IoTDeviceService.Instance.AddDeviceAsync(id);

			if (string.IsNullOrWhiteSpace(addDeviceResult))
				return req.CreateResponse(HttpStatusCode.InternalServerError, "There was an error with your request, please check the service logs or try again");

			return req.CreateResponse(HttpStatusCode.OK, addDeviceResult);
		}
	}
}