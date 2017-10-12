using Xamarin.Forms;

namespace ICC
{
	public class VideoImagePreview : Image
	{
		public VideoImagePreview()
		{
			Effects.Add(Effect.Resolve("Xamarin.VideoPreviewEffect"));
		}

		public string VideoUrl { get; set; }
	}
}