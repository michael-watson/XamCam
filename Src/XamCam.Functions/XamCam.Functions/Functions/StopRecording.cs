using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
	public static class StopRecording
	{
		[FunctionName(nameof(StopRecording))]
		async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "StopRecording/{id}")]
		HttpRequestMessage req, string id, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

			var recording = await IoTDeviceService.Instance.StopRecording(id).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(recording))
				return req.CreateResponse(HttpStatusCode.OK);

			return req.CreateResponse(HttpStatusCode.Conflict, "Already recording");
		}
	}
}
