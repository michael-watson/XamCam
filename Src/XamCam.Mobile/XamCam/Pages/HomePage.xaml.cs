using Xamarin.Forms;

namespace XamCam
{
    public partial class HomePage : BaseContentPage<HomePageViewModel>
    {
        #region Constant Fields
        readonly NoVideosLayout noVideoLayout = new NoVideosLayout();
        readonly VideosListLayout videosListLayout = new VideosListLayout();
        #endregion

        #region Constructors
        public HomePage()
        {
            InitializeComponent();

            Content = noVideoLayout;
        }
        #endregion

        #region Methods
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

        async void onDevicesClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DevicesPage(ViewModel.CamerasAvailable));
        }

        void displayVideoSelected(object sender, SelectedItemChangedEventArgs e)
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