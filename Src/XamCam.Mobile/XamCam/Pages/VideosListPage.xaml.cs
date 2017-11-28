using Xamarin.Forms;

namespace XamCam
{
	public partial class VideosListPage : BaseContentPage<VideoListViewModel>
	{
		#region VideosListPage
		public VideosListPage()
		{
			InitializeComponent();
		}
		#endregion

		#region Methods
		protected override void OnAppearing()
		{
			base.OnAppearing();

			ViewModel.RefreshCommand?.Execute(null);
		}

		void OnDevicesClicked(object sender, System.EventArgs e) =>
			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new DeviceListPage(ViewModel.CamerasAvailable)));

		void DisplayVideoSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var videoSelected = e.SelectedItem as MediaMetadata;
			if (videoSelected == null)
				return;

			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new NativeVideoPlayerPage(videoSelected.HLSUrl)));

			((ListView)sender).SelectedItem = null;
		}
		#endregion
	}
}