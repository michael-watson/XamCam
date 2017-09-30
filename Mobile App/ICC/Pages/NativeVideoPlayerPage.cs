using System;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.MediaManager;
using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace ICC.Pages
{
	public class NativeVideoPlayerPage : BaseContentPage<NativeVideoPlayerViewModel>
	{
		Grid grid;
		Button playButton;
		Slider progressBar;
		VideoView videoView;
		Label transcriptLabel;
		ContentView clickContainer;

		static double videoProgress = 0;

		public NativeVideoPlayerPage(string url)
		{
			Title = "Native Video Player";

			videoView = new VideoView
			{
				Source = url
			};
			transcriptLabel = new Label
			{
				LineBreakMode = LineBreakMode.WordWrap,
				Text = "This is where our transcript would go if we had one"
			};
			playButton = new Button
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			grid = new Grid
			{
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = App.ScreenWidth * (9.0 / 16.0) },
					new RowDefinition { Height = GridLength.Star }
				},
				ColumnDefinitions = new ColumnDefinitionCollection
				{
					new ColumnDefinition { Width = 20 },
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = 20 }
				}
			};
			clickContainer = new ContentView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent
			};
			progressBar = new Slider
			{
				Minimum = 0,
				Maximum = 1,
				Margin = new Thickness(10, 0, 10, 10),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End
			};

			grid.Children.Add(videoView, 0, 3, 0, 1);
			grid.Children.Add(clickContainer, 0, 3, 0, 1);
			grid.Children.Add(progressBar, 1, 2, 0, 1);
			grid.Children.Add(playButton, 1, 2, 0, 1);
			grid.Children.Add(transcriptLabel, 1, 2, 1, 2);

			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += userResetFade;

			clickContainer.GestureRecognizers.Add(tapGesture);
			playButton.SetBinding(Button.ImageProperty, nameof(NativeVideoPlayerViewModel.ButtonImageSource));

			Content = grid;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			if (width > height)
			{
				setLandscapeMode();
			}
			else
			{
				setPortraitMode();
			}
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			playButton.Clicked += togglePlaybackButtonClicked;
			progressBar.ValueChanged += progressBarValueChanged;
			CrossMediaManager.Current.PlayingChanged += videoViewProgressChanged;

			ViewModel.Initialize();
			await beginToFadeSlider();
		}

		protected override async void OnDisappearing()
		{
			base.OnDisappearing();

			playButton.Clicked -= togglePlaybackButtonClicked;
			progressBar.ValueChanged -= progressBarValueChanged;
			CrossMediaManager.Current.PlayingChanged -= videoViewProgressChanged;

			ViewModel.CancelTokenSource.Cancel();
			await CrossMediaManager.Current.Stop();
			ViewExtensions.CancelAnimations(progressBar);
		}

		void userResetFade(object sender, EventArgs e) => resetFade();

		void videoViewProgressChanged(object sender, PlayingChangedEventArgs e)
		{
			//Need to cached video progress to prevent infinite loop when user drags Slider
			videoProgress = e.Progress;

			Device.BeginInvokeOnMainThread(() => progressBar.Value = videoProgress);
		}

		async void progressBarValueChanged(object sender, ValueChangedEventArgs e)
		{
			if (e.NewValue == videoProgress)
				return;

			videoProgress = e.NewValue;

			resetFade();

			await ViewModel.SeekTo(videoProgress);
		}

		async void togglePlaybackButtonClicked(object sender, EventArgs e)
		{
			var isPlaying = await ViewModel.TogglePlayback();

			await resetFade(isPlaying);
		}

		async Task resetFade(bool restartFade = true)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				progressBar.Opacity = 1;
				playButton.Opacity = 1;

				clickContainer.BackgroundColor = Color.Black;
				clickContainer.FadeTo(0.6);
			});

			ViewExtensions.CancelAnimations(progressBar);
			ViewExtensions.CancelAnimations(clickContainer);
			ViewExtensions.CancelAnimations(playButton);

			if (restartFade)
				await beginToFadeSlider();
		}

		async Task beginToFadeSlider()
		{
			int fadeDelay = 1000;
			uint fadeDuration = 1000;

			try
			{
				await Task.Delay(fadeDelay);

				if (CrossMediaManager.Current.VideoPlayer.Status != MediaPlayerStatus.Paused)
					Device.BeginInvokeOnMainThread(async () =>
					{
						await Task.WhenAll(
							playButton.FadeTo(0, fadeDuration),
							progressBar.FadeTo(0, fadeDuration),
							clickContainer.FadeTo(0, fadeDuration));
					});
			}
			catch (TaskCanceledException)
			{
			}
			finally
			{
				if (CrossMediaManager.Current.VideoPlayer.Status != MediaPlayerStatus.Paused)
					Device.BeginInvokeOnMainThread(() =>
					{
						clickContainer.BackgroundColor = Color.Transparent;
						clickContainer.FadeTo(1, 1);
					});
			}
		}

		void setPortraitMode()
		{
			transcriptLabel.IsVisible = true;

			NavigationPage.SetHasNavigationBar(this, true);

			Grid.SetRowSpan(videoView, 1);
			Grid.SetRowSpan(playButton, 1);
			Grid.SetRowSpan(progressBar, 1);
			Grid.SetRowSpan(clickContainer, 1);
		}

		void setLandscapeMode()
		{
			transcriptLabel.IsVisible = false;

			NavigationPage.SetHasNavigationBar(this, false);

			Grid.SetRowSpan(videoView, 2);
			Grid.SetRowSpan(playButton, 2);
			Grid.SetRowSpan(progressBar, 2);
			Grid.SetRowSpan(clickContainer, 2);
		}
	}
}