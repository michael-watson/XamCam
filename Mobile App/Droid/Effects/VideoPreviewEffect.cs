using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using ICC.Droid;
using Android.Graphics;
using Android.Media;
using System;
using Android.OS;
using System.Collections.Generic;
using Android.Widget;
using Android.App;

[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(VideoPreviewEffect), "VideoPreviewEffect")]
namespace ICC.Droid
{
	public class VideoPreviewEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			UpdateUI();
		}

		protected override void OnDetached()
		{
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(args);

			switch (args.PropertyName)
			{
				case nameof(VideoImagePreview):
					UpdateUI();
					break;
			}
		}

		void UpdateUI()
		{
			var uri = ((VideoImagePreview)Element).VideoUrl;
			MediaMetadataRetriever mediaMetadataRetriever = new MediaMetadataRetriever();

			try
			{
				mediaMetadataRetriever.SetDataSource(uri, new Dictionary<string, string>());
				//var bitmap = mediaMetadataRetriever.GetFrameAtTime(0);
				using (var bitmap = mediaMetadataRetriever.GetFrameAtTime(0))
					(Forms.Context as Activity).RunOnUiThread(() => ((ImageView)Control).SetImageBitmap(bitmap));

				//Device.BeginInvokeOnMainThread(() => ((ImageView)Control).SetImageBitmap(bitmap));
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			finally
			{
				mediaMetadataRetriever?.Release();
			}
		}
	}
}