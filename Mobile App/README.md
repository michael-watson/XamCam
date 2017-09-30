# XamCam
XamCam is a mobile application that will enable users to control a remote wifi camera for recording purposes. 
<br>
A simple **Win 10 UWP application**, a Raspberry Pi 3 running **IoT Core** in our case, will record the video content and upload to **Azure Media Services** through an **Azure Function**. The media content will be optimized through Azure Media Services and easily accessible with a bitrate that is responsive to the users internet connection. The **Xamarin.Forms mobile application** will stream the video content from Azure Media Services while also being able to control the recording functionality of the IoT device.
