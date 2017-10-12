## XamCam - GBB TSP Intelligent Cloud Competition 

## Branching and Pull Request guidelines:

* Every person should work on their own branch
* Branches should be matched to a active Github issue.
* Always merge from master into your branch before creating a pull request. This will show you if you have any conflicts with the existing code base
* Never commit to master, you will be shamed
* Branches will be deleted after a Pull Request has been approved into Master. 

General flow for creating a new branch

1. Github issues is assigned to you
2. Create a new branch base off master
    * All spaces should be replaced with a '-'
    * Ensure the name is similar to the associated Github issue
3. Perform work in chunks and commit to your branch
    * Make sure commit titles are meaningful
    * Be descriptive in the title and the notes
4. Format code before creating pull request
5. Create pull request and reference issue in the comments. Assign all pull requests to [@IanLeatherbury](https://github.com/IanLeatherbury).

## Code formatting guidelines:

All code should be formatted to our code guidelines. This will be considered in all pull requests and is a best practice to develop as a team. Please use [David Siegel's chsarp styling](https://github.com/dvdsgl/csharp-in-style)

# XamCam Functions

## Visual Studio Requirements

### Visual Studio for PC

This solution requires Visual Studio for PC Version 15.3 or later.
Earlier versions of VS don't support Azure Functions.

### 1. Create Azure Function App

![](https://user-images.githubusercontent.com/13558917/29196481-756d88bc-7de9-11e7-9d81-33c14d1077b0.png)

1. In the Azure portal, click on New -> Enter `Function App` into the Search Bar -> Selected `Function App` from the search results -> Click Create
2. Name the Function App
3. Select Consumption for the Hosting Plan
4. Select the your Resource Group
5. Select the Location closest to you
6. Click Create


### 2. Install Visual Studio Azure Functions Extension

1. Open using Visual Studio for PC (Version 15.3 or later)

![](https://user-images.githubusercontent.com/13558917/29254393-8a1b69e8-8049-11e7-8426-5e1d3ccb3193.png)

2. Ensure the following Extensions are installed
    - Visual Studio 2017 Tools for Azure Functions
    - Azure Functions and Web Jobs Tools

### 3. Download the source from this Github code repository

1. Download the code from this Github code repository

2. Open the project on Visual Studio

4. Open the file Constants in the folder Constants - you'll need to fill in the details here from various Azure services; we'll go step-by-step through these including 1) Azure Blob Storage, 2) Azure Cosmos DB, constants needed for Azure AD from 3) Azure AD and 4) Azure Media Services, and 5) a URL for a Azure WebHook associated with your Azure Media Services

### 3. Create an Azure Blob Storage Account

1) Create an Azure Blob Storage instance in the Azure portal
![] (https://user-images.githubusercontent.com/3628580/31473265-35c4b40c-aea8-11e7-89ec-4903c214a0aa.png)
![](https://user-images.githubusercontent.com/3628580/31473163-9f245156-aea7-11e7-9042-19248f5be84d.png)

2) Give it a unique name (it will only lowercase letter and numbers)

3) Once you've created the account take note of the account keys.  Look on the left menu bar for Settings > Access Keys.  Make a note of the KEY and CONNECTION STRING for either key1 or key2

### 3. Create an Azure Cosmos DB Account

1) Create an Azure Cosmos DB instance in the Azure Portal. 

![](https://user-images.githubusercontent.com/3628580/31473020-ebbabec0-aea6-11e7-8d4b-2108a99865c6.png)

2) When entering your information, make sure you select **SQL (DocumentDB)**

3) Navigate to your Cosmos DB instance after it has been created

4) Select **Data Explorer (Preview)** in the menu options

5) Click **New Collection** in the top left corner

6) Name the Database Id *Xamarin* for this example and name the Collection Id *XamCamAccount3*.  You can change these but you must make the corresponding changes in your code under XamCamFunctions > CosmosDB > CosmosDBService.cs in the DatabaseId and CollectionId fields.

7) You will now see that the left menu is populated with our new database and collection. Select **XamCamAccount3** or whatever you called your collection and click on **Documents** -- it's empty now but this is where you'll see the videos you've taken from your IoT Device.

8) Once you've created the account take note of the account keys.  Look on the left menu bar for Settings > Keys.  Make a note of the URI, PRIMARY KEY, and PRIMARY CONNECTION STRING



### 4. Publish Function App to Azure

1. In Visual Studio, right-click on XamCamFunctions and select Publish

2. Choose AzureFunctionApp -> Select Existing -> Publish
![](https://user-images.githubusercontent.com/3628580/31465342-940ae0ec-ae88-11e7-840d-763e840fbc79.png)

3. After you are logged in, you'll see the Function App you created earlier.  Select that Functions App App and click OK 

4. Visual Studio is now publishing the XamCamFunctions code to your Azure Functions App
