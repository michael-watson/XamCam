using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using ICC.Models;
using ICC.Constants;

namespace ICC
{
	public class IoTDeviceService
	{
		static HttpClient client = new HttpClient();
		static readonly IoTDeviceService instance = new IoTDeviceService();

		public static IoTDeviceService Instance
		{
			get { return instance; }
		}

		IoTDeviceService()
		{
			client.DefaultRequestHeaders
			  .Accept
			  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<CameraDevice>> GetAllDevicesAsync()
		{
			var devicesToReturn = new List<CameraDevice>();

			try
			{
				var url = AppConstants.GetDevicesUrl;
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var intermediateList = JsonConvert.DeserializeObject<List<CameraDevice>>(json);

					if (intermediateList?.Count != devicesToReturn?.Count)
					{
						devicesToReturn.Clear();

						foreach (var video in intermediateList)
							devicesToReturn.Add(video);
					}
				}
			}
			catch (JsonException e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}

			return devicesToReturn;
		}
	}
}