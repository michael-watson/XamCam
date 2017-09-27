using Xamarin.Forms;

using ICC.Models;

namespace ICC.Views
{
	public class VideoViewCell : ViewCell
	{
		Label titleLabel;
		VideoImagePreview videoPreview;

		public VideoViewCell()
		{
			titleLabel = new Label
			{
				Text = "This",
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				LineBreakMode = LineBreakMode.WordWrap
			};
			videoPreview = new VideoImagePreview
			{
				WidthRequest = App.ScreenWidth,
				HeightRequest = App.ScreenWidth * (9.0 / 16.0),
				Source = "camera.png"
			};
			var titleWrapper = new ContentView
			{
				Padding = 5,
				Content = titleLabel,
				BackgroundColor = Color.Black,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End
			};

			var gridLayout = new Grid
			{
				RowSpacing = 0,
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = App.ScreenWidth * (9.0 / 16.0) },
				}
			};

			gridLayout.Children.Add(videoPreview, 0, 0);
			gridLayout.Children.Add(titleWrapper, 0, 0);

			//makeCardUI(gridLayout);
			View = gridLayout;
		}

		void makeCardUI(Grid gridLayout)
		{
			var frame = new Frame
			{
				Margin = 5,
				Padding = 5,
				CornerRadius = 5,
				BackgroundColor = Color.LightGray,
				Content = gridLayout
			};

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					frame.HasShadow = false;
					break;
			}

			View = frame;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var video = BindingContext as VideoData;

			videoPreview.VideoUrl = video?.mediaAssetUri ?? string.Empty;
			titleLabel.Text = video?.title ?? "No Title Entered";
		}
	}
}