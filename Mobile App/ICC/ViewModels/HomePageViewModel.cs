using System;
using System.Net.Http;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

using Xamarin.Forms;

using ICC.Models;
using ICC.Constants;
using System.Net.Http.Headers;

namespace ICC.ViewModels
{
	public class HomePageViewModel : BaseViewModel
	{
		public HomePageViewModel()
		{
			RefreshCommand = new Command(async () => await GetAllVideosAsync());
		}

		static HttpClient client = new HttpClient();

		public ICommand RefreshCommand { get; private set; }
		public ObservableCollection<VideoData> Videos { get; set; } = new ObservableCollection<VideoData>();

		public async Task GetAllVideosAsync()
		{
			if (!IsBusy)
				IsBusy = true;

			client.DefaultRequestHeaders
			  .Accept
			  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

			try
			{
				var url = AppConstants.FunctionGetUrl;
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var intermediateList = JsonConvert.DeserializeObject<List<VideoData>>(json);

					if (intermediateList?.Count != Videos?.Count)
					{
						Videos.Clear();

						foreach (var video in intermediateList)
							Videos.Add(video);
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}