using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using ICC.Models;
using System.Collections.Generic;

namespace ICC.ViewModels
{
	public class HomePageViewModel : BaseViewModel
	{
		public HomePageViewModel()
		{
			RefreshCommand = new Command(async () => await GetAllVideosAsync());
			GetAllDevicesAvailableAsync();
		}

		public ICommand RefreshCommand { get; private set; }
		public List<CameraDevice> CamerasAvailable { get; set; } = new List<CameraDevice>();
		public ObservableCollection<VideoData> Videos { get; set; } = new ObservableCollection<VideoData>();

		public async Task GetAllVideosAsync()
		{
			if (!IsBusy)
				IsBusy = true;

			Videos.Clear();

			try
			{
				var intermediateList = await VideoService.Instance.GetAllVideosAsync();

				if (intermediateList?.Count != Videos?.Count && intermediateList.Count != 0)
				{
					foreach (var video in intermediateList)
						Videos.Add(video);
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

		public async Task GetAllDevicesAvailableAsync()
		{
			try
			{
				CamerasAvailable = await IoTDeviceService.Instance.GetAllDevicesAsync();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}
	}
}