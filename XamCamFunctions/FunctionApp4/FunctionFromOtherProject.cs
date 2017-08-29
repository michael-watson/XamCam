using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using FunctionApp3;
using Newtonsoft.Json;



/// <summary>
/// REFERENCE ONLY - please look at file ICCFUnctions
/// </summary>


namespace FunctionApp4
{
    public static class FunctionFromOtherProject
    {
        [FunctionName("HttpTriggerCSharp2")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }




            [FunctionName("HttpTriggerCSharp4")]
            public static async Task<HttpResponseMessage> Run4([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
            {
                log.Info("C# HTTP trigger function processed a request.");

                // parse query parameter
                string name = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                    .Value;

                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();

                // Set name to query string or body data
                name = name ?? data?.name;

                return name == null
                    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
            }

            [FunctionName("PartialPlayableURL")]
            public static async Task<HttpResponseMessage> RunPartialPlayableURL([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
            {
                log.Info("PartialPlayableURL processed a request.");

                string name;
                name = string.Format("http://espn.com");

                return name == null
                    ? req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong - you should see get an ESPN URL.   ")
                    : req.CreateResponse(HttpStatusCode.OK, name);
            }

            [FunctionName("PartialReturnListWithOneObject")]
            public static async Task<HttpResponseMessage> RunPartialReturnListWithOneObject([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
            {
                log.Info("Partial Return List with One Object processed a request.");

                List<Result> MyResults;

                string jsonResult = JsonConvert.SerializeObject
                    (MyResults = new List<Result>()
                        {
                        new Result { id=1, VideoName="C# video", lengthInSeconds=90, imageURL = "http://espn.com"},
                        new Result { id=2, VideoName="C# video two", lengthInSeconds=190, imageURL = "http://bing.com"}

                        }

                    );

                //            return req.CreateResponse(HttpStatusCode.OK, jsonResult);


                var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
                httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
                return httpRM;

            }

            [FunctionName("PartialUploadBitArray")]
            public static async Task<HttpResponseMessage> RunPartialUploadBitArray([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
            {
                log.Info("Partial Return List with One Object processed a request.");

                //List<Result> MyResults;
                //string jsonResult = JsonConvert.SerializeObject
                //    (MyResults = new List<Result>()
                //        {
                //            new Result { id=1, VideoName="C# video", lengthInSeconds=90, imageURL = "http://espn.com"},
                //            new Result { id=2, VideoName="C# video two", lengthInSeconds=190, imageURL = "http://bing.com"}
                //        }
                //    );
                ////            return req.CreateResponse(HttpStatusCode.OK, jsonResult);
                //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
                //httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
                //return httpRM;

                //1 grab the image
                //2 change the image into a byte array
                //3 make the byte array into a stream
                //4 Send the stream up


                //1 grab the image
                HttpClient client = new HttpClient();

                client.BaseAddress = new System.Uri("https://upload.wikimedia.org/");
                var response = await client.GetAsync("wikipedia/commons/8/8a/Too-cute-doggone-it-video-playlist.jpg");
                response.EnsureSuccessStatusCode();
                //THIS RESPONDS AN IMAGE
                //return response;  


                //2 change the image into a byte array
                //RETURNS JSON
                //in Byte Array 
                var result = response.Content.ReadAsByteArrayAsync().Result;

                //https://stackoverflow.com/questions/4736155/how-do-i-convert-byte-to-stream-in-c
                //3 make the byte array into a stream
                //Stream stream = new MemoryStream(result);


                //4 Send the stream up
                HttpClient client2 = new HttpClient();
                //            client2.BaseAddress = new System.Uri("http://iccapp2.azurewebsites.net/");
                //           var requestUri = new System.Uri("api/PostByteArrayContent");

                var fullRequestUri = new System.Uri("http://iccapp2.azurewebsites.net/api/PostByteArrayContent");

                ByteArrayContent imageByteArrayContent = new ByteArrayContent(result);
                HttpResponseMessage httpResponse = await client2.PostAsync(fullRequestUri, imageByteArrayContent);

                var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
                //httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
                return httpRM;


            }


        }
    }
