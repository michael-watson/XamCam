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

All code should be formatted to our code guidelines. This will be considered in all pull requests and is a best practice to develop as a team. Below are the guidelines:

1. `using` statements should be ordered outside in, meaning that they should be ordered starting with libraries that are furthest away from your code like below:
    ```csharp
    using System;
    using System.Linq;
    using System.Collections.Generic

    using UIKit;
    using Coregraphics;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.iOS;

    using XamCam.UWP;
    ```
    Notice how the `iOS` references are before the Xamarin.Forms references. That is because the Xamarin.Forms classes are built on top of the iOS classes. We start with the `System` libhraries first because the IDEs produce this code first.
2. Code should be bracket formatted in the following order: `private` Properties, `public` Properties, `Constructors`, `private` Methods, `public` Methods. Below is a code snippet example:
    ```csharp
    namespace XamCam
    {
	    public class MyClass 
	    {
            static object locker = new object ();

            public string Path;
            public double Value;
   
            public MyClass() { }
            public MyClass (string parameter1) 
            {
                ...
            }

            double doSomething (string r) 
            {
                ...
            }
            
            public double GetSomething ()
            {
                ...
            }
        }
    }
    ```
3. We will also be using a camel case style when creating our classes. The following applies:
    * All `public` properties and methods should start with a capital letter
    * All `private` properties and methods should start with a lower case letter
    * Any following words after the first word should start with a capital letter (i.e. `doSomething`, `GetSomething`)
