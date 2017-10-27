using System;
using System.Threading;
using System.Threading.Tasks;

using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;

using Xamarin.Forms;

namespace XamCam
{
    public class NativeVideoPlayerViewModel : BaseViewModel
    {
        #region Fields
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        string buttonImageSource;
        #endregion

        #region Properties
        public CancellationTokenSource CancelTokenSource
        {
            get
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource?.Dispose();
                cancelTokenSource = null;
                cancelTokenSource = new CancellationTokenSource();

                return cancelTokenSource;
            }
        }

        public string ButtonImageSource
        {
            get => buttonImageSource;
            set => SetProperty(ref buttonImageSource, value);
        }
        #endregion

        #region MethodsS
        public async Task<bool> TogglePlayback()
        {
            switch (CrossMediaManager.Current.VideoPlayer.Status)
            {
                case MediaPlayerStatus.Playing:
                    await CrossMediaManager.Current.PlaybackController.Pause();
                    break;

                default:
                    await CrossMediaManager.Current.PlaybackController.Play();
                    break;
            }

            UpdateButtonSource();

            return CrossMediaManager.Current.VideoPlayer.Status == MediaPlayerStatus.Playing;
        }

        public async Task SeekTo(double videoPercentComplete)
        {
            double totalVideoSeconds = CrossMediaManager.Current.VideoPlayer.Duration.TotalSeconds;

            if (Device.RuntimePlatform == Device.Android)
                totalVideoSeconds = totalVideoSeconds / 1000;

            double secondsToSkip = videoPercentComplete * totalVideoSeconds;

            await CrossMediaManager.Current.VideoPlayer.Seek(TimeSpan.FromSeconds(secondsToSkip));
        }

        void UpdateButtonSource()
        {
            switch (CrossMediaManager.Current.VideoPlayer.Status)
            {
                case MediaPlayerStatus.Paused:
                case MediaPlayerStatus.Buffering:
                case MediaPlayerStatus.Loading:
                    ButtonImageSource = "ic_play_arrow_white_48.png";
                    break;

                case MediaPlayerStatus.Stopped:
                case MediaPlayerStatus.Playing:
                    ButtonImageSource = "ic_pause_white_48.png";
                    break;

                case MediaPlayerStatus.Failed:
                    ButtonImageSource = string.Empty;
                    break;

                default:
                    throw new NotImplementedException($"This enumeration has not been implemented{CrossMediaManager.Current.VideoPlayer.Status}");
            }
        }
        #endregion
    }
}