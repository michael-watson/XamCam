using System;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Animations;

namespace XamCam
{
	public class DeviceDetailsPopupPage : PopupPage
	{
		//DeviceDetailsViewModel viewModel;

		bool isRecording;

		public DeviceDetailsPopupPage(CameraDevice selectedDevice)
		{
			//viewModel = new DeviceDetailsViewModel(selectedDevice);
			Animation = new ScaleAnimation(Rg.Plugins.Popup.Enums.MoveAnimationOptions.Center, Rg.Plugins.Popup.Enums.MoveAnimationOptions.Center)
			{
				ScaleIn = 1.2,
				ScaleOut = 0.8,
				DurationIn = 400,
				DurationOut = 300,
				EasingIn = Easing.SinOut,
				EasingOut = Easing.SinIn,
				HasBackgroundAnimation = true,
			};

			var frameLayout = new Frame
			{
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			var recordButton = new Button
			{
				WidthRequest = 50,
				HeightRequest = 50,
				BorderRadius = 25,
				BackgroundColor = Color.Red,
				Opacity = 0.5,
				BorderColor = Color.Black,
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.Center,
				CommandParameter = selectedDevice.DeviceId
			};

			var deviceIdLabel = new Label
			{
				Text = selectedDevice.DeviceId
			};
			var lastUpdatedLabel = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				Text = $"Last updated: {DateTime.Parse(selectedDevice.LastActivityTime).ToString("G")}"
			};

			frameLayout.Content = new StackLayout
			{
				Spacing = 10,
				Padding = 10,
				Children =
				{
					deviceIdLabel,
					lastUpdatedLabel,
					recordButton
				}
			};

			Padding = new Thickness(20, 0);
			Content = frameLayout;

			recordButton.Clicked += RecordButton_Clicked;
		}

		async void RecordButton_Clicked(object sender, EventArgs e)
		{
			string response = string.Empty;
			var button = sender as Button;

			if (isRecording)
			{
				response = await APIService.StopRecordingAsync((string)button.CommandParameter);
				if (string.IsNullOrEmpty(response))
				{
					button.Opacity = 0.5;
					isRecording = false;
				}
				else
				{
					button.Opacity = 1;
					isRecording = true;
				}
			}
			else
			{
				response = await APIService.StartRecordingAsync((string)button.CommandParameter);
				if (string.IsNullOrEmpty(response))
				{
					button.Opacity = 1;
					isRecording = true;
				}
				else
				{
					button.Opacity = 0.5;
					isRecording = false;
				}

			}
		}
	}
}