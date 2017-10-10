using System;
using System.Threading;
using System.Threading.Tasks;

using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;

using ICC.ViewModels;
using Xamarin.Forms;

namespace ICC
{
	public class NativeVideoPlayerViewModel : BaseViewModel
	{
		CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
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
			get
			{
				switch (CrossMediaManager.Current.VideoPlayer.Status)
				{
					case MediaPlayerStatus.Paused:
					case MediaPlayerStatus.Buffering:
					case MediaPlayerStatus.Loading:
						return "ic_play_arrow_white_48.png";

					case MediaPlayerStatus.Stopped:
					case MediaPlayerStatus.Playing:
						return "ic_pause_white_48.png";

					case MediaPlayerStatus.Failed:
						return string.Empty;

					default:
						throw new NotImplementedException($"This enumeration has not been implemented{CrossMediaManager.Current.VideoPlayer.Status}");
				}
			}
		}

		public void Initialize()
		{
			OnPropertyChanged(nameof(ButtonImageSource));
		}

		public async Task<bool> TogglePlayback()
		{
			if (CrossMediaManager.Current.VideoPlayer.Status == MediaPlayerStatus.Playing)
			{
				await CrossMediaManager.Current.PlaybackController.Pause();
			}
			else
			{
				await CrossMediaManager.Current.PlaybackController.Play();
			}

			OnPropertyChanged(nameof(ButtonImageSource));

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
	}
}