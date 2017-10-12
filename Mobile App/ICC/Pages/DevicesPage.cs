using System.Collections.Generic;

using Xamarin.Forms;

using ICC.Pages;
using ICC.ViewModels;

using Rg.Plugins.Popup.Extensions;

namespace ICC
{
	public class DevicesPage : BaseContentPage<DevicesPageViewModel>
	{
		public DevicesPage(IEnumerable<CameraDevice> cameraDevices)
		{
			ViewModel.SetCameraDevices(cameraDevices);

			var deviceList = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(CameraViewCell))
			};

			Content = deviceList;

			deviceList.SetBinding(ListView.ItemsSourceProperty, nameof(DevicesPageViewModel.CamerasAvailable));

			deviceList.ItemSelected += DeviceSelected;
		}

		void DeviceSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;

			var selectedCamera = e.SelectedItem as CameraDevice;
			if (selectedCamera?.ConnectionState == 1)
			{
				//Navigation.PushPopupAsync(new RecordingPopup());
			}
			else
				DisplayAlert("Error", "This device is currently offline. Please ensure it is plugged in.", "Ok");
		}
	}
}