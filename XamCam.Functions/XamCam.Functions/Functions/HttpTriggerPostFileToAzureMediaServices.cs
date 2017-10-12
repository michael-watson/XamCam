using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using XamCam.Functions.Models;

namespace XamCam.Functions
{
	class HttpTriggerPostFileToAzureMediaServices
	{
		//CONSTANTS NEEDED FOR AZURE AD
		static string tenantId = Constants.tenantId;
		static string GrantType = Constants.GrantType;
		static string ClientSecret = Constants.ClientSecret;
		static string ClientID = Constants.ClientID;
		static string RequestedResource = Constants.RequestedResource;

		[FunctionName("PostMediaFileToAzureMediaServices")]
		public static async Task<HttpResponseMessage> RunPostMediaFileToAzureMediaServices([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PostMediaFileToAzureMediaServices/{deviceId}/{videoTitle}")]HttpRequestMessage req, string deviceId, string videoTitle, TraceWriter log)
		{

			var fileAsBytes = await req.Content.ReadAsByteArrayAsync();
			var myUploadedFile = new UploadedFile
			{
				Title = videoTitle,
				FileName = $"{deviceId}_{DateTime.UtcNow.Ticks}.mp4",
				File = fileAsBytes,
				UploadedAt = DateTime.UtcNow
			};
			HttpClient httpClient = new HttpClient();

			//CREATE HTTP REQUEST
			HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId));
			myRequest.Content = new StringContent("grant_type=" + GrantType + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&resource=" + RequestedResource, Encoding.UTF8, "application/x-www-form-urlencoded");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage httpResponseMessageWithADToken = await httpClient.SendAsync(myRequest);

			//EXTRACT AD ACCESS TOKEN FROM HTTP RESPONSE MESSAGE
			var stringResult = httpResponseMessageWithADToken.Content.ReadAsStringAsync().Result;
			var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADResult>(stringResult);
			var azureADToken = resultObject.access_token;

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateAnAsset
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateAssetRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Assets"));

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createAnAssetBody = new CreateAnAssetBody { Name = "TestAsset7", Options = "0" };
			string PostCreateAnAssetjsonBody = JsonConvert.SerializeObject(createAnAssetBody);
			myPostCreateAssetRequest.Content = new StringContent(PostCreateAnAssetjsonBody, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateAssetResponseMessage = await httpClient.SendAsync(myPostCreateAssetRequest);

			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var PostCreateAssetStringResult = await myPostCreateAssetResponseMessage.Content.ReadAsStringAsync();

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateAssetResponseMessageResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaAsset>(PostCreateAssetStringResult);

			var myPostCreateAssetRequestdObjectResults = myPostCreateAssetResponseMessageResultObject.d;
			var myPostCreateAssetRequestdObjectResultsResultId = myPostCreateAssetRequestdObjectResults.Id;

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateAnAssetFile
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "3.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateAnAssetFileRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Files"));

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createdAssetFileBody = new CreateAssetFileBody
			{
				IsEncrypted = "false",
				IsPrimary = "true",
				MimeType = "video/mp4",
				Name = "TestVideo.mp4",
				ParentAssetId = myPostCreateAssetRequestdObjectResultsResultId
			};

			string myPostCreateAnAssetFilejsonBody = JsonConvert.SerializeObject(createdAssetFileBody);
			myPostCreateAnAssetFileRequest.Content = new StringContent(myPostCreateAnAssetFilejsonBody, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateAnAssetFileResponseMessage = await httpClient.SendAsync(myPostCreateAnAssetFileRequest);

			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var myPostCreateAnAssetFileResponseMessagestringResult = myPostCreateAnAssetFileResponseMessage.Content.ReadAsStringAsync().Result;

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateAnAssetFileResponseMessageresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<CreatedAssetRoot>(myPostCreateAnAssetFileResponseMessagestringResult);

			var myPostCreateAnAssetFiledObjectResults = myPostCreateAnAssetFileResponseMessageresultObject.d;
			var myPostCreateAnAssetFilecreateAssetFileId = myPostCreateAnAssetFiledObjectResults.Id;

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateAccessPolicy
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateAccessPolicyRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies"));

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createdAccessPolicyBody = new CreateAccessPolicyBody { Name = "NewUploadPolicy", DurationInMinutes = "440", Permissions = "2" };
			string myPostCreateAccessPolicyjsonBody = JsonConvert.SerializeObject(createdAccessPolicyBody);
			myPostCreateAccessPolicyRequest.Content = new StringContent(myPostCreateAccessPolicyjsonBody, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateAccessPolicyResponseMessage = await httpClient.SendAsync(myPostCreateAccessPolicyRequest);


			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var myPostCreateAccessPolicystringResult = myPostCreateAccessPolicyResponseMessage.Content.ReadAsStringAsync().Result;

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateAccessPolicyresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<CreatedAssetRoot>(myPostCreateAccessPolicystringResult);

			var myPostCreateAccessPolicydObjectResults = myPostCreateAccessPolicyresultObject.d;
			var myPostCreateAccessPolicyaccessPolicyIdResults = myPostCreateAccessPolicydObjectResults.Id;

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateLocator
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateLocatorRequest = new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createdLocatorBody = new CreateLocatorBody
			{
				AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults,
				AssetId = myPostCreateAssetRequestdObjectResultsResultId,
				StartTime = DateTime.Now,
				Type = 1
			};
			string myPostCreateLocatorjsonBody = JsonConvert.SerializeObject(createdLocatorBody);
			myPostCreateLocatorRequest.Content = new StringContent(myPostCreateLocatorjsonBody, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateLocatorResponseMessage = await httpClient.SendAsync(myPostCreateLocatorRequest);

			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var myPostCreateLocatorstringResult = myPostCreateLocatorResponseMessage.Content.ReadAsStringAsync().Result;

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateLocatorresultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<CreatedLocatorRoot>(myPostCreateLocatorstringResult);

			var myPostCreateLocatordObjectResults = myPostCreateLocatorresultObject.d;
			var myPostCreateLocatorlocatorResults = myPostCreateLocatordObjectResults.Id;

			const string assetIDSearchString = "blob.core.windows.net/";
			var indexOfContainerNameStart = myPostCreateLocatordObjectResults.BaseUri.IndexOf(assetIDSearchString) + assetIDSearchString.Length;
			var containerName = myPostCreateLocatordObjectResults.BaseUri.Substring(indexOfContainerNameStart);

			string firstLocator = myPostCreateLocatordObjectResults.__metadata.uri;
			string preFirstAssetId = myPostCreateLocatordObjectResults.AccessPolicyId;

			var indexOfLocatorWord = firstLocator.IndexOf("Locator");
			string shortFirstLocator = firstLocator.Substring(0, indexOfLocatorWord);

			string pattern = ":";
			string replacement = "%3A";
			Regex rgx = new Regex(pattern);
			string htmlSafeFirstAssetId = rgx.Replace(preFirstAssetId, replacement);

			var finalAccessPolicyId = String.Format("{0}AccessPolicies('{1}')", shortFirstLocator, htmlSafeFirstAssetId);

			///////////////////////////////////
			///// UPLOAD TO BLOB STORAGE
			//////////////////////////////////

			//ALTERNATIVE INSTANTIATION VIA CONFIGURATION FILE
			//CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			//CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constants.BlobURLAndKey);
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// RETRIEVE REFERENCE TO CONTAINER
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			// CREATE CONTAINER IF IT DOES NOT EXIST
			container.CreateIfNotExists();

			//DEFAULT BEHAVIOR MAKES BLOCKS PRVIATE (YOU MUST USE SPECIFIC STORAGE
			//ACCESS KEYS TO DOWNLOAD BLBOS FROM THE CONTAINER.)
			//MAKE FILES AVAILABLE TO PUBLIC VIA THE FOLLOWING CODE:
			container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

			//RETRIEVE REFERENCE TO BLOB BY FILENAME
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(myUploadedFile.FileName);

			//IN CASE YOU NEED TO SET THE MEDIA TYPE
			//https://stackoverflow.com/questions/24621664/uploading-blockblob-and-setting-contenttype

			//UPLOAD THE BYTE ARRAY
			blockBlob.UploadFromByteArray(myUploadedFile.File, 0, myUploadedFile.File.Length);

			///////////////////////////////////
			///// OPTIONAL: UPDATE THE ASSET FILE WITH ANY PROPERTIES
			//////////////////////////////////

			///////////////////////////////////
			///// OPTIONAL: ENCODE SOURCE
			//////////////////////////////////

			///////////////////////////////////
			///// DELETE THE LOCATOR
			//////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  Bearer Token
			httpClient.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("MaxDataServiceVersion", "3.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.11");

			//CREATE HTTP REQUEST
			HttpRequestMessage myDeleteLocatorRequest =
				new HttpRequestMessage
				(
					HttpMethod.Delete,
					String.Format("{0}", firstLocator)
				);

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myDeleteTheLocatorReponseMessage =
				await httpClient.SendAsync(myDeleteLocatorRequest);

			///////////////////////////////////
			///// DELETE THE ASSET FILE
			//////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  Bearer Token
			httpClient.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("MaxDataServiceVersion", "3.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.11");

			//CREATE HTTP REQUEST
			HttpRequestMessage myDeleteAccessPolicyRequest =
				new HttpRequestMessage
				(
					HttpMethod.Delete,
					String.Format("{0}", finalAccessPolicyId)
				);

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myDeleteAccessPolicyResponseMessage =
				await httpClient.SendAsync(myDeleteAccessPolicyRequest);

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateAccessPolicy
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateAccessPolicyRequest2 =
				new HttpRequestMessage
				(
					HttpMethod.Post,
					String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/AccessPolicies")
				);

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createdAccessPolicyBody2 = new CreateAccessPolicyBody
			{
				Name = "DownloadPolicy",
				DurationInMinutes = "144000",
				Permissions = "1"
			};
			string myPostCreateAccessPolicyjsonBody2 = JsonConvert.SerializeObject(createdAccessPolicyBody2);
			myPostCreateAccessPolicyRequest2.Content =
				new StringContent(myPostCreateAccessPolicyjsonBody2, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateAccessPolicyResponseMessage2 =

				await httpClient.SendAsync(myPostCreateAccessPolicyRequest2);
			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var myPostCreateAccessPolicystringResult2 =
				myPostCreateAccessPolicyResponseMessage2.Content.ReadAsStringAsync().Result;

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateAccessPolicyresultObject2 =
				Newtonsoft.Json.JsonConvert.DeserializeObject<CreatedAccessPolicy>(myPostCreateAccessPolicystringResult2);

			var myPostCreateAccessPolicydObjectResults2 = myPostCreateAccessPolicyresultObject2.d;
			var myPostCreateAccessPolicyaccessPolicyIdResults2 = myPostCreateAccessPolicydObjectResults2.Id;

			////////////////////////////////////////////////////////////////////////////////
			// PostCreateLocator
			////////////////////////////////////////////////////////////////////////////////

			if (httpClient.DefaultRequestHeaders != null)
			{
				httpClient.DefaultRequestHeaders.Clear();
			}

			//  BEARER TOKEN AND HEADERS
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureADToken);
			httpClient.DefaultRequestHeaders.Add("x-ms-version", "2.15");
			httpClient.DefaultRequestHeaders.Add("DataServiceVersion", "1.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

			//CREATE HTTP REQUEST
			HttpRequestMessage myPostCreateLocatorRequest2 =
				new HttpRequestMessage(HttpMethod.Post, String.Format("https://xamcammediaservice.restv2.westus.media.azure.net/api/Locators"));

			//ASSEMBLE THE CONTENT OF THE REQUEST INCLUDING JSON BODY FOR REQUEST
			var createdLocatorBody2 = new CreateLocatorBody
			{
				AccessPolicyId = myPostCreateAccessPolicyaccessPolicyIdResults2,
				AssetId = myPostCreateAssetRequestdObjectResultsResultId,
				StartTime = DateTime.Now,
				Type = 1
			};
			string myPostCreateLocatorjsonBody2 = JsonConvert.SerializeObject(createdLocatorBody2);
			myPostCreateLocatorRequest2.Content =
				new StringContent(myPostCreateLocatorjsonBody2, Encoding.UTF8, "application/json");

			//SEND HTTP REQUEST AND RECEIVE HTTP RESPONSE MESSAGE
			HttpResponseMessage myPostCreateLocatorResponseMessage2 =
				await httpClient.SendAsync(myPostCreateLocatorRequest2);

			//EXTRACT RESPONSE FROM HTTP RESPONSE MESSAGE
			var myPostCreateLocatorstringResult2 =
				myPostCreateLocatorResponseMessage2.Content.ReadAsStringAsync().Result;

			//DESERIALIZE RESPONSE FROM HTTP RESPONSE MESSAGE (JSON->OBJECT)
			var myPostCreateLocatorresultObject2 = Newtonsoft.Json.JsonConvert.DeserializeObject<CreatedLocatorRoot>
				(myPostCreateLocatorstringResult2);

			var myPostCreateLocatordObjectResults2 = myPostCreateLocatorresultObject2.d;
			var myPostCreateLocatorlocatorResults2 = myPostCreateLocatordObjectResults2.Id;

			var firstHalfLocatorAMS = myPostCreateLocatorresultObject2.d.BaseUri;
			var uploadFileName = myUploadedFile.FileName;
			var myContentAccessComponent = myPostCreateLocatorresultObject2.d.ContentAccessComponent;
			string newLocator = String.Format("{0}/{1}{2}", firstHalfLocatorAMS, uploadFileName, myContentAccessComponent);

			////////////////////////////////////////////////////////////////////////////////
			//  SAVE TO COSMOS DB
			////////////////////////////////////////////////////////////////////////////////

			MediaAssetsWithMetaData uploadMediaAssetsWithMetaData = new MediaAssetsWithMetaData()
			{
				id = Guid.NewGuid().ToString(),
				email = "user@xamarin.com",
				mediaAssetUri = newLocator,
				title = myUploadedFile.Title,
				fileName = myUploadedFile.FileName,
				uploadedAt = myUploadedFile.UploadedAt
			};

			await CosmosDBService.PostMediaAssetAsync(uploadMediaAssetsWithMetaData);

			var httpRM = new HttpResponseMessage(HttpStatusCode.OK);
			return httpRM;
		}
	}
}