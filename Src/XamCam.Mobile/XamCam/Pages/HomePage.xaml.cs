using System;
using Xamarin.Forms;

namespace XamCam
{
    public partial class VideoListPage : BaseContentPage<VideoListViewModel>
    {
        #region Constant Fields
        readonly NoVideosLayout noVideoLayout = new NoVideosLayout();
        readonly VideosListLayout videosListLayout = new VideosListLayout();
        #endregion

        #region Constructors
        public VideoListPage()
        {
            InitializeComponent();

            Content = noVideoLayout;
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            videosListLayout.ItemSelected += DisplayVideoSelected;
            ViewModel.RefreshCompleted += HandleRefreshCompleted;

            ViewModel.RefreshCommand?.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            videosListLayout.ItemSelected -= DisplayVideoSelected;
            ViewModel.RefreshCompleted -= HandleRefreshCompleted;
        }

        void OnDevicesClicked(object sender, System.EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new DevicesPage(ViewModel.CamerasAvailable)));

        void DisplayVideoSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var videoSelected = e.SelectedItem as MediaMetadata;
            if (videoSelected == null)
                return;

            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new WebViewVideoPage(videoSelected.HLSUrl)));

            ((ListView)sender).SelectedItem = null;
        }

        void HandleRefreshCompleted(object sender, int numberOfVideos)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (numberOfVideos)
                {
                    case int totalVideos when totalVideos > 0:
                        Content = videosListLayout;
                        break;

                    default:
                        Content = noVideoLayout;
                        break;
                }
            });
        }
        #endregion
    }
}