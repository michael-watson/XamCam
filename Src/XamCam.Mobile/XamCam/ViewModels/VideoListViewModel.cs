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

        #region Events
        public event EventHandler<int> RefreshCompleted;
        #endregion

        #region Properties
        public ICommand RefreshCommand => refreshCommand ??
            (refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

        public List<CameraDevice> CamerasAvailable { get; set; } = new List<CameraDevice>();
        public ObservableCollection<MediaMetadata> Videos { get; set; } = new ObservableCollection<MediaMetadata>();
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            if (!IsInternetConnectionActive)
                IsInternetConnectionActive = true;

            try
            {
                Videos = new ObservableCollection<MediaMetadata>(await APIService.GetAllVideosAsync());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                IsInternetConnectionActive = false;
                OnRefreshCompleted(Videos?.Count ?? 0);
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

        void OnRefreshCompleted(int numberOfVideos) =>
            RefreshCompleted?.Invoke(this, numberOfVideos);
        #endregion
    }
}