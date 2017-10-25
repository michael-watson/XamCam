using Xamarin.Forms;

namespace XamCam
{
	public class CameraViewCell : ViewCell
	{
		Label cameraName;

		public CameraViewCell()
		{
			cameraName = new Label
			{
				Margin = new Thickness(10, 0, 0, 0),
				VerticalOptions = LayoutOptions.Center
			};

			View = cameraName;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var camera = BindingContext as CameraDevice;

			cameraName.Text = camera?.GenerationId;
		}
	}
}