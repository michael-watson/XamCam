using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace XamCam
{
	public class VideoListViewModel : BaseViewModel
	{
		#region Fields
		ICommand refreshCommand;
		#endregion

		#region Constructors
		public VideoListViewModel() =>
			Task.Run(async () => await GetAllDevicesAvailableAsync());
		#endregion

		#region Properties
		public ICommand RefreshCommand => refreshCommand ??
			(refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

		public List<CameraDevice> CamerasAvailable { get; set; } = new List<CameraDevice>();
		public ObservableCollection<MediaMetadata> Videos { get; set; } = new ObservableCollection<MediaMetadata>();

		public bool DisplayNoVideosIndicator
		{
			get => Videos.Count == 0;
		}
		#endregion

		#region Methods
		async Task ExecuteRefreshCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			Videos.Clear();

			try
			{
				var videos = await APIService.GetAllVideosAsync();
				foreach (var video in videos)
					Videos.Add(video);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			finally
			{
				IsBusy = false;

				OnPropertyChanged(nameof(DisplayNoVideosIndicator));
			}
		}

		public async Task GetAllDevicesAvailableAsync()
		{
			try
			{
				CamerasAvailable = await APIService.GetAllDevicesAsync();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
		#endregion
	}
}