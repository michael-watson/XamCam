using Xamarin.Forms;

namespace ICC
{
	public partial class NativeVideoPlayerPageXaml : ContentPage
	{
		public NativeVideoPlayerPageXaml()
		{
			InitializeComponent();

			gridLayout.RowDefinitions = new RowDefinitionCollection
			{
				new RowDefinition { Height = App.ScreenWidth * (9.0 / 16.0) },
				new RowDefinition { Height = GridLength.Star }
			};
			gridLayout.ColumnDefinitions = new ColumnDefinitionCollection
			{
				new ColumnDefinition { Width = 20 },
				new ColumnDefinition { Width = GridLength.Star },
				new ColumnDefinition { Width = 20 }
			};
		}
	}
}
