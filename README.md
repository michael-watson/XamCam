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

## XamCam Functions

## Visual Studio Requirements

### Visual Studio for PC

This solution requires Visual Studio for PC Version 15.3 or later.
Earlier versions of VS don't support Azure Functions.

### 2. Create Azure Function App

![](https://user-images.githubusercontent.com/13558917/29196481-756d88bc-7de9-11e7-9d81-33c14d1077b0.png)

1. In the Azure portal, click on New -> Enter `Function App` into the Search Bar -> Selected `Function App` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29196973-ea5fb796-7dec-11e7-92d3-fda7ba5a6f6b.png)

2. Name the Function App
    - I named mine XamListFunctionApp
3. Select Consumption for the Hosting Plan
4. Select the XamList Resource Group
    - We created this resource group when we made our API App, above
5. Select the Location closest to you
6. Under Storage, Select Create New
    - I named my storage "xamlistfunctionapp"
7. Click Create

