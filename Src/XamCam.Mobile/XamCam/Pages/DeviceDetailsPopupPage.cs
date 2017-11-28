using System;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Animations;

namespace XamCam
{
	public class DeviceDetailsPopupPage : PopupPage
	{
		public DeviceDetailsPopupPage()
		{
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

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

