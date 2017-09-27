using Xamarin.Forms;

using ICC.Views;
using ICC.Models;
using ICC.ViewModels;

namespace ICC.Pages
{
	public partial class HomePage : BaseContentPage<HomePageViewModel>
	{
		NoVideosLayout noVideoLayout = new NoVideosLayout();
		VideosListLayout videosListLayout = new VideosListLayout();

		public HomePage()
		{
			InitializeComponent();

			Content = noVideoLayout;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			videosListLayout.ItemSelected += displayVideoSelected;

			await ViewModel.GetAllVideosAsync();

			Device.BeginInvokeOnMainThread(() =>
			{
				if (ViewModel.Videos.Count <= 0)
					Content = noVideoLayout;
				else
					Content = videosListLayout;
			});
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			videosListLayout.ItemSelected -= displayVideoSelected;
		}

		void displayVideoSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var videoSelected = e.SelectedItem as VideoData;
			if (videoSelected == null)
				return;

			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new NativeVideoPlayerPage(videoSelected.mediaAssetUri)));

			((ListView)sender).SelectedItem = null;
		}
	}
}