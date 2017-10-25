using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace XamCam
{
    public class HomePageViewModel : BaseViewModel
    {
        #region Constructors
        public HomePageViewModel()
        {
            RefreshCommand = new Command(async () => await GetAllVideosAsync());
            GetAllDevicesAvailableAsync();
        }
        #endregion

        #region Properties
        public ICommand RefreshCommand { get; private set; }
        public List<CameraDevice> CamerasAvailable { get; set; } = new List<CameraDevice>();
        public ObservableCollection<MediaMetadata> Videos { get; set; } = new ObservableCollection<MediaMetadata>();
        #endregion

        #region Methods
        public async Task GetAllVideosAsync()
        {
            if (!IsInternetConnectionActive)
                IsInternetConnectionActive = true;

            Videos.Clear();

            try
            {
                var intermediateList = await APIService.GetAllVideosAsync();

                if (intermediateList?.Count != Videos?.Count && intermediateList.Count != 0)
                {
                    foreach (var video in intermediateList)
                        Videos.Add(video);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                IsInternetConnectionActive = false;
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