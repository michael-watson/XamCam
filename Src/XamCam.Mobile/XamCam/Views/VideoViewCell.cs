using Xamarin.Forms;

namespace XamCam
{
	public class VideoViewCell : ViewCell
	{
		#region Constant Fields
		readonly Label titleLabel;
		readonly Label durationLabel;
		readonly VideoImagePreview videoPreview;
		#endregion

		#region Constructors
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
		#endregion

		#region Methods
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext == null) return;

			var videoMetadata = BindingContext as MediaMetadata;

			videoPreview.VideoUrl = videoMetadata.BlobStorageMediaUrl;
			durationLabel.Text = "Error, No Duration Provided";

			switch (videoMetadata?.Title?.Length > 15)
			{
				case true:
					titleLabel.Text = $"{videoMetadata?.Title?.Substring(0, 15)}...";
					break;

				default:
					titleLabel.Text = videoMetadata?.Title ?? string.Empty;
					break;
			}
		}
		#endregion
	}
}