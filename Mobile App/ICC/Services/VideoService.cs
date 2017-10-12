using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using ICC.Models;
using ICC.Constants;

namespace ICC
{
	public class VideoService
	{
		static HttpClient client = new HttpClient();
		static readonly VideoService instance = new VideoService();

		public static VideoService Instance
		{
			get { return instance; }
		}

		VideoService()
		{
			client.DefaultRequestHeaders
			  .Accept
			  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<VideoData>> GetAllVideosAsync()
		{
			var videosToReturn = new List<VideoData>();

			try
			{
				var url = AppConstants.MediaAssetsUrl;
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var intermediateList = JsonConvert.DeserializeObject<List<VideoData>>(json);

					if (intermediateList?.Count != videosToReturn?.Count)
					{
						videosToReturn.Clear();

						foreach (var video in intermediateList)
							videosToReturn.Add(video);
					}
				}
			}
			catch (JsonException e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}

			return videosToReturn;
		}
	}
}
