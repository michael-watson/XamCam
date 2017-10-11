# XamCam IoT Functions Readme

The Following will show you how to create an IoT Hub in Azure and create Azure functions to create/register/delete iot devices registrations from your Hub. 

The IOTManager project contains a DeviceManager class which communicates with our Azure IoT Hub. Through this class we can provision new IOT Devices and get their connectionstrings, which we then use (from the client side, or the IoT device) to interact with the "Device Twin" - a digital representation of the various states of the IoT Device. 

## Setup

### Creating the IoT Hub in Azure
1. In the azure portal, navigate to New. Search for IoT Hub and click Create
2. Name your IoT Hub, fill out the resource group (either new or existing).
Leave all other fields untouched
3. Click Create

### Getting the HostName and ConnectionString for the IOTManager
The file Constants.cs in the IOTManager project contains a few properties that we need to get from our newly created IotHub.

1. Navigate to your ResourceGroup that contains the newly created IoT Hub, and click on the IoT Hub
2. Under "Overview" - copy and paste the value for "HostName" to the <HostName> property in Constants.cs

3. Under "Shared access policies" - click on "iothubowner" and copy the value of "Connection string—primary key" into the <ConnectionString> property in Constants.cs

### Creating our Azure Functions 







