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

4. Open the file Constants in the folder Constants - you'll need to fill in the details here from various Azure services; we'll go step-by-step through these including 1) Azure Blob Storage, 2) Azure Cosmos DB, 3) Azure AD Application registration,  4) Azure Media Services, and 5) a URL for a Azure WebHook associated with your Azure Media Services.

### 4. Create an Azure Blob Storage instance

1) Create an Azure Blob Storage instance in the Azure portal
![] (https://user-images.githubusercontent.com/3628580/31473265-35c4b40c-aea8-11e7-89ec-4903c214a0aa.png)
![](https://user-images.githubusercontent.com/3628580/31473163-9f245156-aea7-11e7-9042-19248f5be84d.png)

2) Give it a unique name (it will only lowercase letter and numbers)

3) Once you've created the account take note of the account keys.  Look on the left menu bar for Settings > Access Keys.  Make a note of the KEY and CONNECTION STRING for either key1 or key2

### 5. Create an Azure Cosmos DB instance

1) Create an Azure Cosmos DB instance in the Azure Portal. 

![](https://user-images.githubusercontent.com/3628580/31473020-ebbabec0-aea6-11e7-8d4b-2108a99865c6.png)

2) When entering your information, make sure you select **SQL (DocumentDB)**

3) Navigate to your Cosmos DB instance after it has been created

4) Select **Data Explorer (Preview)** in the menu options

5) Click **New Collection** in the top left corner

6) Name the Database Id *Xamarin* for this example and name the Collection Id *XamCamAccount3*.  You can change these but you must make the corresponding changes in your code under XamCamFunctions > CosmosDB > CosmosDBService.cs in the DatabaseId and CollectionId fields.

7) You will now see that the left menu is populated with our new database and collection. Select **XamCamAccount3** or whatever you called your collection and click on **Documents** -- it's empty now but this is where you'll see the videos you've taken from your IoT Device.

8) Once you've created the account take note of the account keys.  Look on the left menu bar for Settings > Keys.  Make a note of the URI, PRIMARY KEY, and PRIMARY CONNECTION STRING

### 6. Register an application in Azure Active Directory

1) Click to your Azure Active Directory.  On the left menu panel click to Manage > App Registrations.  Then in the resulting panel click, on the upper-left, + New application registration

2) Enter in a name, select Native as the Application Type, and type in a Redirect URI

8a) Once you've registered the app take note of the account keys.  On the overview panel look for the application ID - we'll call this the Client ID in our application.  Click on the Settings, and click on Keys.  Create a Key and take a note of it as it will be called Client Secret in our application.  

8b)Also depending on how we choose to do our Authentication - we will need the Directory Id of our Azure Active Directory Account.  You can find this by clicking back into the general Azure Active Directory section within the Azure portal.  On the left menu bar to go Manage > Properties > DirectoryID.  Take note of the DirectoryID - this will be called tenantID in our application.

### 7. Create an Azure Media Services Instance

1) Create an Azure Media Services Instance in the Azure portal
![](https://user-images.githubusercontent.com/3628580/31474064-5d163df0-aead-11e7-829d-4ceeb8c0164f.png)

2) Give it a unique name (it will only lowercase letter and numbers) and connect it with the Storage Account in step 4

3) Click down to Streaming endpoints > take note of the hostname (you'll need to reference this in the application and add a slash at the end of the URL)

4) Click into the streaming endpoint, to press Play.  Click Settings and click on Premium we've set our Streaming Units to 2 for up to 400Mbs egress.

### 8. Publish Function App to Azure

1a.  Return to the final step in Section 3: Download the source from this Github code repository - fill in the constants as appropriate from the above steps.  Optionally, you can store these values as Key/Value pairs under your Function > Application settings preventing the need to check in sensitive values in your source control. 

1b. For the Webhook constants, you'll need to check back into the Azure portal under your Function after you published your function. Once you complete the below steps, go to the Azure Portal and click on the function called "NewXamCamWebHookThree". 

![](https://user-images.githubusercontent.com/3628580/31475091-a793acea-aeb3-11e7-8d45-ea87425b0e63.png)

Notice on the upper-right you see </> Get function URL.  That URL will be represented in the Constants class in your code as WebHookEndpoint.  Also, the WebHookSigningKey will be the portion of that URL between the "code=" and the "&clientId=default"

2. In Visual Studio, right-click on XamCamFunctions and select Publish

3. Choose AzureFunctionApp -> Select Existing -> Publish
![](https://user-images.githubusercontent.com/3628580/31465342-940ae0ec-ae88-11e7-840d-763e840fbc79.png)

4. After you are logged in, you'll see the Function App you created earlier.  Select that Functions App App and click OK 

5. Visual Studio is now publishing the XamCamFunctions code to your Azure Functions App
