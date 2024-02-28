# AspireTodo

This repo contains the sample code we'll use for the .NET + Azure (via Aspire) training day. 

> Note, I'm just iterating over the steps in here quickly and will go back and refine. If you look at the repo's commit history you'll see these steps embodied, commit-by-commit, in the actual source code for the app. 

## Day 1

These are the high-level steps you'll perform on Day 1. 

### Creating a new Aspire project

You'l create a basic project and then learn how to run it and examine how the app is running using the .NET Aspire dashboard. 

* Create a new Aspire Starter project without the Redis Output Caching enabled 
* Run the app to see the Aspire Dashboard 
* Run through the various ways of looking at console logs for each project 
* Observe how the Trace node provides distributed tracing as users hit the frontend of the site
* Stop the debugger

### Change the code to be a "Todo" app

In this phase, you'll make some basic modifications to the backend API and frontend Web project to turn the template's content into a real Todo app. 

* Change `WeatherApiClient` to `TodoApiClient`
* Reflect the type name change in the `Web` project's `Program.cs` during build-up
* Change the `WeatherForecast` record types in both the `ApiService` project and the `Web` project into `TodoItem` record types, with a `Description` string property and a `IsCompleted` bool property
* Change the `ApiService` project's `Program.cs` such that it returns a `List<ToDoItem>` at the endpoint `/todos` and build the list in the `Program.cs` file with a sample list of `TodoItem` objects
* Rename the `Web` project's `Weather.razor` in the `Pages` directory to `Todos.razor`
* Delete the `Counter.razor` and `Home.razor` files from the `Web` project's `Pages` folder
* Change the `Web` project's `Layout/NavMenu.razor` so that it only has the `Home` link, deleting `Weather` and `Counter`, but change the link text to `Todo`
* Edit the `Web` project's `Pages/Todo.razor` file to display `TodoItem` objects instead of the old `WeatherForecast` objects, using the `TodoApiClient` class instead of the `WeatherApiClient` class
* Edit the `Web` project's `Pages/Todo.razor` file such that it is what renders in the browser when the `/` (root) endpoint is called from a web browser

### Deploy the app to Azure

In this phase, you'll publish your new AspireTodo app to Azure using the Azure Developer CLI or, Visual Studio (which uses the Azure Developer CLI as an underlying dependency). 

* Make sure AZD is defaulting to the .NET R&D subscription so you know you have everything you need to party today

    ```azd config set defaults.subscription <.NETR&DSubscriptionIdHere>```

* If you had Visual Studio open when you performed the `azd config set`, you may need to restart Visual Studio
* Right-click the `AppHost` project and select `Publish`, then go through the .NET Aspire Azure Container Apps publish flow to publish the app to Azure 
* If you aren't using Visual Studio, you can simply drop out a command prompt, `cd` into the root directory of your `.sln` file, and execute these commands:

    ```
    azd init
    azd provision
    azd deploy
    ```

* Browse to the Azure portal and see the variety of resources you've created
* Click on the `webfrontend` Azure Container App resource to view the overview of the app 
* Explore the `Ingress` area for both the `backendapi` and `webfrontend` container apps to take note of how the frontend is available via the open Internet, whereas the backend API app is private and secure

### Push your app into a GitHub repo

At this point your code should be ready to save, so get it into a GitHub repo (public or private, doesn't matter).

* Use Visual Studio's GitHub features, the `gh` CLI tool, or your favorite method of creating Git repositories to create a new local Git repository and remote GitHub repository
* Commit and push your code to the remote repository

This should be the end of the stuff we get through in Day 1. We'll kick off Day 2 by automating the deployment of your app when you commit code to the repo in which you've saved your code. 

## Day 2

During Day 2, you'll add Continuous Integration / Continuous Deploy (CI/CD) capabilities to your GitHub repository, add database support, and use asynchrous messaging rather than direct HTTP calls between your frontend and backend apps. 

