using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICC.ViewModels
{
	public class DevicesPageViewModel : BaseViewModel
	{
		public DevicesPageViewModel()
		{
		}

		public ObservableCollection<CameraDevice> CamerasAvailable { get; set; }

		public void SetCameraDevices(IEnumerable<CameraDevice> cameras)
		{
			CamerasAvailable = new ObservableCollection<CameraDevice>(cameras);
		}
	}
}