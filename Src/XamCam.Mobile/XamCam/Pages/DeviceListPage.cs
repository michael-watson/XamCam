using System.Collections.Generic;

using Xamarin.Forms;

namespace XamCam
{
	public class DeviceListPage : BaseContentPage<DeviceListViewModel>
	{
		#region Constructors
		public DeviceListPage(IEnumerable<CameraDevice> cameraDevices)
		{
			ViewModel.SetCamerasAvailableCommand?.Execute(cameraDevices);

			var deviceList = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(DeviceViewCell))
			};

			Content = deviceList;

			deviceList.SetBinding(ListView.ItemsSourceProperty, nameof(DeviceListViewModel.CamerasAvailable));

			deviceList.ItemSelected += DeviceSelected;
		}
		#endregion

		#region Methods
		void DeviceSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;

			var selectedCamera = e.SelectedItem as CameraDevice;
			if (selectedCamera?.ConnectionState == 1)
			{
				//Navigation.PushPopupAsync(new RecordingPopup());
			}
			//else
			//DisplayAlert("Error", "This device is currently offline. Please ensure it is plugged in.", "Ok");

			((ListView)sender).SelectedItem = null;
		}
		#endregion
	}
}