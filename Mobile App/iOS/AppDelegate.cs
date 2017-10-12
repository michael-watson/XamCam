using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.MediaManager.Forms.iOS;
using UIKit;

namespace ICC.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			VideoViewRenderer.Init();

			App.ScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
			App.ScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
