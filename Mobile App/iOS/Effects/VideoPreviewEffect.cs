using UIKit;
using CoreMedia;
using Foundation;
using AVFoundation;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using ICC.iOS;

[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(VideoPreviewEffect), "VideoPreviewEffect")]
namespace ICC.iOS
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
			var formsControl = Element as VideoImagePreview;

			if (formsControl == null) return;

			if (!string.IsNullOrWhiteSpace(formsControl.VideoUrl))
			{
				var outputTime = new CMTime();
				var error = new NSError();

				using (var asset = AVAsset.FromUrl(new NSUrl(formsControl.VideoUrl)))
				using (var assetGenerator = AVAssetImageGenerator.FromAsset(asset))
				{
					var image = assetGenerator.CopyCGImageAtTime(new CMTime(0, 1), out outputTime, out error);

					if (image != null)
						Control?.InvokeOnMainThread(() => ((UIImageView)Control).Image = new UIImage(image));
				}
			}
			else
				formsControl.Source = "camera.png";
		}
	}
}