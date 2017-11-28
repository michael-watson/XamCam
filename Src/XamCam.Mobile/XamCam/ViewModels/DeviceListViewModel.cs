using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamCam
{
	public class DeviceListViewModel : BaseViewModel
	{
		#region Fields
		ICommand setCamerasAvailableCommand;
		#endregion

		#region Properties
		public ICommand SetCamerasAvailableCommand => setCamerasAvailableCommand ??
			(setCamerasAvailableCommand = new Command<IEnumerable<CameraDevice>>(camerasAvailable =>
												 CamerasAvailable = new ObservableCollection<CameraDevice>(camerasAvailable)));

		public ObservableCollection<CameraDevice> CamerasAvailable { get; set; }
		#endregion
	}
}