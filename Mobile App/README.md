# XamCam
XamCam is a mobile application that will enable users to control a remote WiFi camera for recording purposes. 

## Description 
A [Win 10 UWP application](https://docs.microsoft.com/en-us/windows/uwp/get-started/whats-a-uwp), a Raspberry Pi 3 running [IoT Core](https://developer.microsoft.com/en-us/windows/iot) in our case, will record the video content and upload to [Azure Media Services](https://azure.microsoft.com/en-us/services/media-services/) through an [Azure Function](https://azure.microsoft.com/en-us/services/functions/). The media content will be optimized through Azure Media Services and easily accessible with a bitrate that is responsive to the users internet connection. The [Xamarin.Forms](https://www.xamarin.com/forms) mobile application will *stream* the video content from **Azure Media Services** while also being able to control the recording functionality of the **IoT device**.

# Getting Started

## Setup AppConstants.cs
There is a ```AppConstants.cs``` file in the Constants directory located at Xamam/XamCam/Constants that contains the **Azure Functions** Url.

It is imperative to change this ```FunctionGetUrl``` to **your Azure Function Url** before you run the application.

If needed, here is more information on setting up [Azure Functions.](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)

# Visual Studio Mobile Center
[Mobile Center](https://www.visualstudio.com/vs/mobile-center/) is the next generation of [HockeyApp](https://www.hockeyapp.net/), [Xamarin Test Cloud](https://testcloud.xamarin.com/login), [Xamarin Insights](https://www.xamarin.com/insights), and a plethora of new services like automated builds. It is a single platform to manage all aspects of continuous integration and continuous deployment, including build, test, distribution, crash reporting, and analytics.

## Getting Started with Mobile Center

  * [Register](https://mobile.azure.com/) for new a Mobile Center account (or [login](https://mobile.azure.com/) using GitHub, Microsoft, or an existing Mobile Center account)
  * [Create a new app](https://mobile.azure.com/apps/create) for both iOS and Android
  * In ```PrivateKeys.cs``` set the two values of ```AppSecret``` your new iOS and Android app's respective App Secrets.

## Setting up Services in Mobile Center
This app uses [Mobile Center](https://www.visualstudio.com/vs/mobile-center/) for Continuous Integration and Continuous Deployment, by taking advantage of the functionality provided by each of the following Services:

## Build

## Test

## Distribute

## Crash Reporting

## Analytics 
