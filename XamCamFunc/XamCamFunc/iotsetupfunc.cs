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
    public static class iotsetupfunc
    {
        [FunctionName("AddDevice")]

        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "AddDevice/{id}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            
            var res = await DeviceManager.Instance.AddDeviceAsync(id);
      
            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}