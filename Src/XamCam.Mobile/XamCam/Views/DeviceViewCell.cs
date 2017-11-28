using Xamarin.Forms;
using System.Security.Cryptography;

namespace XamCam
{
	public class DeviceViewCell : ViewCell
	{
		readonly Label cameraName;
		readonly Button cameraOnline;

		public DeviceViewCell()
		{
			cameraOnline = new Button
			{
				BorderRadius = 10,
				WidthRequest = 20,
				HeightRequest = 20,
				BackgroundColor = Color.Red,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};

			cameraName = new Label
			{
				Margin = new Thickness(10, 0, 0, 0),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalTextAlignment = TextAlignment.Center
			};

			View = new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				Children =
				{
					cameraName,
					cameraOnline
				}
			};
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var camera = BindingContext as CameraDevice;

			cameraName.Text = camera?.GenerationId;

			if (camera?.ConnectionState == 1)
				cameraOnline.BackgroundColor = Color.Green;
			else
				cameraOnline.BackgroundColor = Color.Red;
		}
	}
}