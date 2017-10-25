using Xamarin.Forms;

namespace XamCam
{
    public partial class NativeVideoPlayerPageXaml : ContentPage
    {
        #region Constructors
        public NativeVideoPlayerPageXaml()
        {
            InitializeComponent();

            GridLayout.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = App.ScreenWidth * (9.0 / 16.0) },
                new RowDefinition { Height = GridLength.Star }
            };
            GridLayout.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = 20 },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = 20 }
            };
        }
        #endregion
    }
}
