using Android.OS;
using Android.App;
using Android.Content.PM;

using Plugin.MediaManager.Forms.Android;

namespace XamCam.Droid
{
    [Activity(Label = "ICC.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			VideoViewRenderer.Init();
            global::Xamarin.Forms.Forms.SetFlags("FastRenderers_Experimental");
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			var density = Resources.DisplayMetrics.Density;

			App.ScreenWidth = (double)((Resources.DisplayMetrics.WidthPixels - 0.5f) / density);
			App.ScreenHeight = (double)((Resources.DisplayMetrics.HeightPixels - 0.5f) / density);

			LoadApplication(new App());
		}
	}
}