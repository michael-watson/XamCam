using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
	public static class AddDeviceFunction
	{
		[FunctionName("AddDevice")]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "AddDevice/id/{id}")]HttpRequestMessage req, string id, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

			if (string.IsNullOrEmpty(id))
			{
				return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide an id for a device");
			}

			var res = await DeviceManager.Instance.AddDeviceAsync(id);

			if (string.IsNullOrEmpty(res))
			{
				return req.CreateResponse(HttpStatusCode.InternalServerError, "There was an error with your request, please check the service logs or try again");
			}

			return req.CreateResponse(HttpStatusCode.OK, res);
		}
	}
}