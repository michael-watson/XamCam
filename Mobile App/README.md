# XamCam
XamCam is a mobile application that will enable users to control a remote wifi camera for recording purposes. 
<br>
A simple **Win 10 UWP application**, a Raspberry Pi 3 running **IoT Core** in our case, will record the video content and upload to **Azure Media Services** through an **Azure Function**. The media content will be optimized through Azure Media Services and easily accessible with a bitrate that is responsive to the users internet connection. The **Xamarin.Forms mobile application** will stream the video content from Azure Media Services while also being able to control the recording
functionality of the IoT device.

## Getting Started

# Visual Studio Mobile Center
[Mobile Center](https://www.visualstudio.com/vs/mobile-center/) is the next generation of [HockeyApp](https://www.hockeyapp.net/), [Xamarin Test Cloud](https://testcloud.xamarin.com/login), [Xamarin Insights](https://www.xamarin.com/insights), and a plethora of new services like automated builds. It is a single platform to manage all aspects of continuous integration and continuous deployment, including build, test, distribution, crash reporting, and analytics.

Setting up Mobile Center is completely optional. If you'd like to use it, you can set it up by following the steps listed below. However, if you'd rather skip this step for now, simply leave the two values of AppSecret as empty strings.

    * Register for new a Mobile Center account (or login using GitHub, Microsoft, or an existing Mobile Center account)
    * Create a new app for both iOS and Android
    * In PrivateKeys.cs set the two values of AppSecret your new iOS and Android app's respective App Secrets.

