using Xamarin.Forms;

namespace XamCam
{
	public class VideoImagePreview : Image
	{
		public VideoImagePreview() =>
			Effects.Add(Effect.Resolve("Xamarin.VideoPreviewEffect"));

		public string VideoUrl { get; set; }
	}
}