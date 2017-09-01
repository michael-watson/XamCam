using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using FunctionApp3;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

/// <summary>
/// REFERENCE ONLY - please look at file ICCFUnctions
/// </summary>


namespace FunctionApp4
{
    public static class Function1
    {
        [FunctionName("HttpTriggerCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
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
        //[FunctionName("RequestForJSON")]
        //public static async Task<HttpResponseMessage> RunRequestForJSONList([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        //{
        //    string jsonConvert = await req.Content.ReadAsStringAsync();
        //    dynamic data = JsonConvert.DeserializeObject(jsonConvert);

        //    log.Info($"Webook was triggered! Comment {data.comment.body}");

        //    return req.CreateResponse(HttpStatusCode.OK, new { body = $"New Github Comment: {data.comment.body}" });


        //}

        [FunctionName("RequestForJSON")]
        public static async Task<HttpResponseMessage> RunRequestForJSONList([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var client = new System.Net.Http.HttpClient();

            client.BaseAddress = new System.Uri("http://iccapp1.azurewebsites.net/");

            var response = await client.GetAsync("api/PartialReturnListWithOneObject");

            var totalJsonResults = response.Content.ReadAsStringAsync().Result;

        //      Returns string version of JSON
        //    return req.CreateResponse(HttpStatusCode.OK, totalJsonResults);
        //      "[{\"id\":1,\"VideoName\":\"C# video\",\"lengthInSeconds\":90,\"imageURL\":\"http://espn.com\"},{\"id\":2,\"VideoName\":\"C# video two\",\"lengthInSeconds\":190,\"imageURL\":\"http://bing.com\"}]"


            var rootObject = JsonConvert.DeserializeObject<Result>(totalJsonResults);




            var jsonObject = JsonConvert.SerializeObject(rootObject);

            //string jsonConvert = await req.Content.ReadAsStringAsync();
            //dynamic data = JsonConvert.DeserializeObject(jsonConvert);
            //log.Info($"Webook was triggered! Comment {data.comment.body}");
            //return req.CreateResponse(HttpStatusCode.OK, new { body = $"New Github Comment: {data.comment.body}" });

            //return req.CreateResponse(HttpStatusCode.OK, jsonObject);
            //return req.CreateResponse(HttpStatusCode.OK, totalJsonResults);

            var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            httpRM.Content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");

            return httpRM;
        }


        [FunctionName("GetJSONReturnJSON")]
        public static async Task<HttpResponseMessage> RunGetJSONReturnJSON([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri("http://iccapp1.azurewebsites.net/");
            var response = await client.GetAsync("api/PartialReturnListWithOneObject");
            response.EnsureSuccessStatusCode();
            //RETURNS JSON
            return response;
        }

        [FunctionName("GetJSONDeserializeSerializeReturnJSON")]
        public static async Task<HttpResponseMessage> RunGetJSONDeserializeSerializeReturnJSON([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri("http://iccapp1.azurewebsites.net/");
            var response = await client.GetAsync("api/PartialReturnListWithOneObject");
            response.EnsureSuccessStatusCode();
            //RETURNS HTTPResponseRequest

            //RETURNS JSON
            //in STRING VERSION
            var result = response.Content.ReadAsStringAsync().Result;

            //JSON -> OBJECT (DESERIALIZE)

            //List<Result> myResult;
            var myResultArray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Result>>(result);

//            var s = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(result);




            //myResult is OBJECT or List of OBJECTS

            //OBJECT -> JSON (SERIALIZE)
            //string jsonString = JsonConvert.SerializeObject(myResult);
            string jsonString = JsonConvert.SerializeObject(myResultArray);


            //send out of HTTPResponse
            var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            httpRM.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            return httpRM;





            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            //httpRM.Content = new StringContent(response, System.Text.Encoding.UTF8, "application/json");

            //return response;


            //var totalJsonResults = response.Content.ReadAsStringAsync().Result;
            //return await Task.Run(() => JsonObject.Parse(response));


            //      Returns string version of JSON
            //    return req.CreateResponse(HttpStatusCode.OK, totalJsonResults);
            //      "[{\"id\":1,\"VideoName\":\"C# video\",\"lengthInSeconds\":90,\"imageURL\":\"http://espn.com\"},{\"id\":2,\"VideoName\":\"C# video two\",\"lengthInSeconds\":190,\"imageURL\":\"http://bing.com\"}]"


            //var rootObject = JsonConvert.DeserializeObject<Result>(totalJsonResults);




            //var jsonObject = JsonConvert.SerializeObject(rootObject);

            ////string jsonConvert = await req.Content.ReadAsStringAsync();
            ////dynamic data = JsonConvert.DeserializeObject(jsonConvert);
            ////log.Info($"Webook was triggered! Comment {data.comment.body}");
            ////return req.CreateResponse(HttpStatusCode.OK, new { body = $"New Github Comment: {data.comment.body}" });

            ////return req.CreateResponse(HttpStatusCode.OK, jsonObject);
            ////return req.CreateResponse(HttpStatusCode.OK, totalJsonResults);

            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            //httpRM.Content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");

            //return httpRM;
        }

        [FunctionName("PostByteArrayContent")]
        public static async Task<HttpResponseMessage> RunPostByteArrayContent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            var myByteArrayStream = req.Content.ReadAsStreamAsync();
            myByteArrayStream.Wait();
            Stream requestStream = myByteArrayStream.Result;

            var myByteArray = await req.Content.ReadAsByteArrayAsync();
  //          myByteArray.Wait();

            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ByteArrayContent(myByteArray);

//      ByteArrayContent imageByteArrayContent = new ByteArrayContent(result);
//     HttpResponseMessage httpResponse = await client2.PostAsync(fullRequestUri, imageByteArrayContent);

            var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            //httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
            return httpRM;



//            httpRM.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");


  //          return response;



            //log.Info("C# HTTP trigger function processed a request.");

            //// parse query parameter
            //string name = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            //    .Value;

            //// Get request body
            //dynamic data = await req.Content.ReadAsAsync<object>();

            //// Set name to query string or body data
            //name = name ?? data?.name;

            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);





            //var task = this.Request.Content.ReadAsStreamAsync();
            //task.Wait();
            //Stream requestStream = task.Result;

            //try
            //{
            //    Stream fileStream = File.Create(HttpContext.Current.Server.MapPath("~/" + filename));
            //    requestStream.CopyTo(fileStream);
            //    fileStream.Close();
            //    requestStream.Close();
            //}
            //catch (IOException)
            //{
            //    throw new HttpResponseException("A generic error occured. Please try again later.", HttpStatusCode.InternalServerError);
            //}

            HttpResponseMessage response2 = new HttpResponseMessage();
            response2.StatusCode = HttpStatusCode.Created;
            return response;


            //var client = new System.Net.Http.HttpClient();
            //client.BaseAddress = new System.Uri("http://iccapp1.azurewebsites.net/");
            //var response = await client.GetAsync("api/PartialReturnListWithOneObject");
            //response.EnsureSuccessStatusCode();
            ////RETURNS HTTPResponseRequest

            ////RETURNS JSON
            ////in STRING VERSION
            //var result = response.Content.ReadAsStringAsync().Result;

            ////JSON -> OBJECT (DESERIALIZE)

            ////List<Result> myResult;
            //var myResultArray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Result>>(result);

            ////            var s = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(result);




            ////myResult is OBJECT or List of OBJECTS

            ////OBJECT -> JSON (SERIALIZE)
            ////string jsonString = JsonConvert.SerializeObject(myResult);
            //string jsonString = JsonConvert.SerializeObject(myResultArray);


            ////send out of HTTPResponse
            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            //httpRM.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            //return httpRM;

        }

        //START GETTING THE APPROPRIATE TOKENS AND SUCH FROM AMS

        //        [FunctionName("GetAzureADAuthToken")]
        //        public static async Task<HttpResponseMessage> RunGetAzureADToken([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        //        {

        //            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        ////            String loginRequestURIString = String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId);
        //            String loginRequestURIString = String.Format("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token");

        //            var myloginRequestURI = new System.Uri(loginRequestURIString);

        //            //          HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

        //            //          request.Content = new StringContent("{\"name\":\"John Doe\", \"age\":33}" , Encoding.UTF8, "application/x-www-form-urlencoded");
        //            //            request.Content = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");

        //            //            HttpContent myContent = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");
        //                        HttpContent myContent = new StringContent("{\"grant_type\":\"client_credentials\"}", Encoding.UTF8, "application/x-www-form-urlencoded");

        //            HttpClient httpClient1 = new HttpClient();
        //            httpClient1.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //            //            HttpResponseMessage httpReponse2 = await httpClient1.SendAsync(myloginRequestURI, request);
        //            HttpResponseMessage httpReponse2 = await httpClient1.PostAsync(myloginRequestURI, myContent);

        //            return httpReponse2;

        //            //   //4 Send the stream up
        //            //  HttpClient client2 = new HttpClient();
        //            ////            client2.BaseAddress = new System.Uri("http://iccapp2.azurewebsites.net/");
        //            ////           var requestUri = new System.Uri("api/PostByteArrayContent");


        //            //var fullRequestUri = new System.Uri("http://iccapp2.azurewebsites.net/api/PostByteArrayContent");

        //            //ByteArrayContent imageByteArrayContent = new ByteArrayContent(result);
        //            ////HttpResponseMessage httpResponse = await client2.PostAsync(fullRequestUri, imageByteArrayContent);

        //            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
        //            ////httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
        //            //return httpRM;



        //        }
        [FunctionName("GetAzureADAuthTokenTest")]
        public static async Task<HttpResponseMessage> RunGetAzureADTokenTest([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
            //            String loginRequestURIString = String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId);
 //           String loginRequestURIString = String.Format("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token");

   //         var myloginRequestURI = new System.Uri(loginRequestURIString);

            //          HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            //          request.Content = new StringContent("{\"name\":\"John Doe\", \"age\":33}" , Encoding.UTF8, "application/x-www-form-urlencoded");
            //            request.Content = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");

            //            HttpContent myContent = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");
            //HttpContent myContent = new StringContent("{\"grant_type\":\"client_credentials\"}", Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpClient httpClient1 = new HttpClient();
            httpClient1.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            //            HttpResponseMessage httpReponse2 = await httpClient1.SendAsync(myloginRequestURI, request);

            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token");
            //myRequest.Content = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");
            //myRequest.Content = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\":\"8d631792-ed10-46aa-bd09-b8ca1641bc6f\",\"client_secret\":\"HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=\",\"resource\":\"https://rest.media.azure.net\"}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var ClientSecret = "HTUFvSAT2C8002nQA1FPfKEpUZkIUTxRRyDFaORVa38=";
                                
            myRequest.Content = new StringContent("grant_type=client_credentials"+"&client_id=8d631792-ed10-46aa-bd09-b8ca1641bc6f"+"&client_secret="+ClientSecret+"&resource=https://rest.media.azure.net", Encoding.UTF8, "application/x-www-form-urlencoded");

           

            HttpResponseMessage myhrm = await httpClient1.SendAsync(myRequest);


            //httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");

            //            var result = response.Content.ReadAsByteArrayAsync().Result;
            var result = myhrm.Content.ReadAsStringAsync().Result;
            var myResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(result);

      //      AzureADResult myAzureADResult = myResultArray[0];
            var ADToken = myResult.access_token;

            var mystring = "hi"; 

            return myhrm;

            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            //return httpRM;










            // HttpResponseMessage httpReponse2 = await httpClient1.PostAsync(myloginRequestURI, myContent);

            //return httpReponse2;

            //   //4 Send the stream up
            //  HttpClient client2 = new HttpClient();
            ////            client2.BaseAddress = new System.Uri("http://iccapp2.azurewebsites.net/");
            ////           var requestUri = new System.Uri("api/PostByteArrayContent");


            //var fullRequestUri = new System.Uri("http://iccapp2.azurewebsites.net/api/PostByteArrayContent");

            //ByteArrayContent imageByteArrayContent = new ByteArrayContent(result);
            ////HttpResponseMessage httpResponse = await client2.PostAsync(fullRequestUri, imageByteArrayContent);

            //var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
            ////httpRM.Content = new StringContent(jsonResult, System.Text.Encoding.UTF8, "application/json");
            //return httpRM;



        }







    }
}