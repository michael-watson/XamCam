using Xamarin.Forms;

namespace XamCam
{
    public class WebViewVideoPage : BaseContentPage<BaseViewModel>
    {
        public WebViewVideoPage(string url) => Content = new WebView { Source = url };
    }
}
