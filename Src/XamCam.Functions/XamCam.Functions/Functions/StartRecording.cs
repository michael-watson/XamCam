using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace XamCam.Functions
{
	public static class StartRecording
	{
		[FunctionName(nameof(StartRecording))]
		async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "StartRecording/{id}")]
		HttpRequestMessage req, string id, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

			var recording = await IoTDeviceService.Instance.StartRecording(id).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(recording))
				return req.CreateResponse(HttpStatusCode.OK);

			return req.CreateResponse(HttpStatusCode.Conflict, "Already recording");
		}
	}
}