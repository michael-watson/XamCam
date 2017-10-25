using Xamarin.Forms;

namespace XamCam
{
	public class VideoViewCell : ViewCell
	{
		Label titleLabel;
		Label durationLabel;
		VideoImagePreview videoPreview;

		public VideoViewCell()
		{
			titleLabel = new Label
			{
				Text = "This",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				FontAttributes = FontAttributes.Bold,
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
			};
			durationLabel = new Label
			{
				Text = "0:00",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				Margin = new Thickness(0, 0, 10, 0),
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.End,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
			};
			videoPreview = new VideoImagePreview
			{
				WidthRequest = App.ScreenWidth,
				HeightRequest = App.ScreenWidth * (9.0 / 16.0),
				Source = "camera.png"
			};
			var titleStack = new StackLayout
			{
				Padding = 5,
				BackgroundColor = Color.Black,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				Orientation = StackOrientation.Horizontal,
				Children = { titleLabel, durationLabel }
			};

			var gridLayout = new Grid
			{
				RowSpacing = 0,
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = App.ScreenWidth * (9.0 / 16.0) }
				}
			};

			gridLayout.Children.Add(videoPreview, 0, 0);
			gridLayout.Children.Add(titleStack, 0, 0);

			View = gridLayout;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var video = BindingContext as MediaMetadata;

            videoPreview.VideoUrl = "Error, No Video Url Provided";
            durationLabel.Text = "Error, No Duration Provided";

			if (video?.Title?.Length > 15)
				titleLabel.Text = $"{video?.Title?.Substring(0, 15)}...";
			else
				titleLabel.Text = video?.Title ?? "No Title Entered";
		}
	}
}