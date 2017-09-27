using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.MediaManager.Forms.Android;
using Android.Util;

namespace ICC.Droid
{
	[Activity(Label = "ICC.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			var density = Resources.DisplayMetrics.Density;

			//App.ScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
			//App.ScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;



			VideoViewRenderer.Init();

			LoadApplication(new App());
		}
	}
}
