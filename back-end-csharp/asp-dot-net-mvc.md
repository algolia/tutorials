## Part 1: ASP.NET MVC

In this 3 part tutorial, we are going to explore how to easily add Algolia search to Windows Phone 8.1 and ASP.NET MVC applications. To do this, we will take advantage of Algolia's [C# client](https://github.com/algolia/algoliasearch-client-csharp) to add indexing capabilities to the ASP.NET MVC application and search capabilities to the Windows Phone application.

The application we will be building is called PackageTrack. It is a simple web application where a user can create, read, update, and delete packages they like and use. The web application will be capable of managing the package information, indexing that data with Algolia, adding auto complete search to the UI, and providing a simple REST API for the data. We will also create a Windows Phone application with an auto suggest search box hooked into the Algolia search service to provide search results. Users will then be able to select one of the search results and have that information shown by requesting the data from the web application API.

## Prerequisites

In order to follow along well, it will be helpful to share versions of tooling, operating system, and technology used for the tutorial. You will need to use (at the very least) Visual Studio 2013 with update 4. Also, in order to develop Windows Phone 8.1 applications, you will need to be developing on Windows 8 or greater. Finally, the application is being built using .NET framework 4.5.

## Create a new ASP.NET Web Application

With Visual Studio running, create a new solution and choose an ASP.NET Web Application for your initial project. If you want to follow along exactly, name your solution ```PackageTrack``` and your project ```PackageTrack.Web```.

![Screenshot 01](/images/algolia_csharp_01.png)

To keep this tutorial simple, change the authentication to ```No Authentication```.

![Screenshot 02](/images/algolia_csharp_02.png)

You will also want to make sure MVC and Web API are selected.

![Screenshot 03](/images/algolia_csharp_03.png)

## Install necessary NuGet packages

This application requires two new packages in order to work.

**Entity Framework**

Install the Entity Framework package. The version used in this tutorial is 6.1.3.

```tcsh
PM> Install-Package EntityFramework 
```

**Algolia Search**

Install the Algolia Search package. The version used in this tutorial is 3.0.5.

```tcsh
PM> Install-Package Algolia.Search
```

Finally, we need to update all installed packages to make sure we are using the latest libraries and scripts. You can do this by right clicking the web project in Solution Explore and choosing ```Manage NuGet Packages...```. 

## Cleanup

Now that we have our packages installed and updated, we need to perform a little cleanup in our project.

First, let's delete the Home Controller, ```HomeController.cs```, and the ```Home``` directory within ```Views```. We are removing the controller and its views because we will be creating our own later.

## Update our layout

Our layout needs a little updating to improve the look and feel a bit. Update ```\Vieews\Shared\_Layout.cshtml``` with the following.

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Package Track</title>
    @Styles.Render("~/Content/css")
    @Scripts.Render("~/bundles/modernizr")
</head>
<body>
    <div class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                @Html.ActionLink("Package Track", "Index", "Home", new { area = "" }, new { @class = "navbar-brand" })
            </div>
        </div>
    </div>
    <div class="container body-content">
        @RenderBody()
        <hr />
        <footer>
            <p>&copy; @DateTime.Now.Year - <a href="https://www.algolia.com/" target="_blank">Algolia</a></p>
        </footer>
    </div>

    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/bootstrap")
    @RenderSection("scripts", required: false)
</body>
</html>
```

## Entity Framework and Database

We are now ready to create our model and DbContext in order to take advantage of Entity Framework. What we will do is a create a simple model to represent our package that we want to store in the database. After that, we will create our DbContext which will allow us to easily add, remove, update, and more on our packages stored in the database.

By default, the project is setup to use a local MDF database. You are welcome to change it but for simplicity this tutorial will not.

To create our package model, create a new file named ```Package.cs``` inside the ```Models``` directory. Update the created class to look like the following.

```c#
public class Package
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
    public int Count { get; set; }
}
```

Next, we need to create our DbContext. To do this, create a new directory called ```Data``` at the root of the project. Then create a new file named ```PackageTrackContext.cs``` inside this new directory. Update the code to the following.

```c#
using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PackageTrack.Web.Data
{
    public class PackageTrackDbContext : DbContext
    {
        public DbSet<Package> Packages { get; set; }
    }
}
```

Before moving on, be sure to build your solution. This will be necessary for the next steps where we use Visual Studio tooling to scaffold our CRUD controller and views for us.

## Package CRUD

We now need to create the controller, actions, and views in order to create, read, update, and delete packages. To do this we will use Visual Studio tooling. This will make it super simple. The following 3 screenshots show what to do.

First, right click the ```Controller``` directory and choose ```Add``` followed by ```Controller```.

![Screenshot 04](/images/algolia_csharp_04.png)

Next, choose ```MVC 5 Controller with views, using Entity Framework```.

![Screenshot 05](/images/algolia_csharp_05.png)

Finally, be sure to choose the Package model we created for the ```Model class```, the PackageTrackDbContext we created for the ```Data context class```, check ```Use async controller actions```, and set the ```Controller name``` to HomeController.

![Screenshot 06](/images/algolia_csharp_06.png)

Visual Studio should create a new HomeController along with a set of views. You can now build and run your project. You should be able to create, read, update, and delete packages. Go head and do this now and create a few packages. We will need them later.


## Build an Admin controller

We now need to create an Admin controller that will provide us the ability to issue index, re-index, and delete commands to our Algolia index.

Inside the ```Controllers``` directory, create a file named ```AdminController.cs```. Update it to contain the following code.

```c#
using Algolia.Search;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PackageTrack.Web.Controllers
{
    public class AdminController : Controller
    {
        private PackageTrackDbContext db = new PackageTrackDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReIndexData()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexData()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteData()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
```

Inside the ```Views``` directory, create a new directory named ```Admin```. Now that we have a new place for our admin views, create a new view there named ```Index.cshml```. This is where we will setup our ability to issue index commands. Update the view with the following code.

```html
<h2>Admin</h2>

<div>
    <hr />

    <div class="form-actions no-color">
        @using (Html.BeginForm("reindexdata", "admin"))
        {
            @Html.AntiForgeryToken()
            <input type="submit" value="Re-Index Data" class="btn btn-default" />
        }
        <br />
        @using (Html.BeginForm("indexdata", "admin"))
        {
            @Html.AntiForgeryToken()
            <input type="submit" value="Index Data" class="btn btn-default" />
        }
        <br />
        @using (Html.BeginForm("deletedata", "admin"))
        {
            @Html.AntiForgeryToken()
            <input type="submit" value="Delete Data" class="btn btn-default" />
        }
    </div>
</div>
```

Now we need to create 3 views that can be used to show when our index commands have completed. Create the following views with their markup inside the ```Views\Admin``` directory.

**ReIndexData.cshtml**

```html
<h2>Data Re-Indexed</h2>
```

**IndexData.cshtml**

```html
<h2>Data Indexed</h2>
```

**DeleteData.cshtml**

```html
<h2>Data Deleted</h2>
```

## Algolia time

Up to this point, we have been doing standard .NET development and nothing with Algolia (except adding the NuGet package). We are now ready to start.

Since our web application allows our users to create, update, and delete packages, we need to be able to do the same things to our Algolia index. On top of that, there are cases where we need to perform an index on all of our data, do a full re-index where data is removed and then added, and also remove all data from our index. We do all this from our web application rather than the Windows Phone application because it is much more secure to have our server talking to Algolia rather than each client. We don't want our credentials that allow changing index data to fall into a user's hand.

### Create reusable Algolia client 

The AlgoliaClient allows our application to easily interact with our Algolia index. In order to keep performance optimal, we don't want to create an instance of this client each time we need it. To address this, we will create one AlgoliaClient and store it in an Application Variable.

Let's go ahead and do this now.

Open up your ```Global.asax.cs``` file and update it as follows.

```c#
using Algolia.Search;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PackageTrack.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create our Algolia client
            var algoliaClient = new AlgoliaClient("<APPLICATION_ID>", "<ADMIN_API_KEY>");

            // Create our index helper
            var indexHelper = new IndexHelper<Package>(algoliaClient, "packages", "Id");

            // Store our index helper in an application variable.
            // We don't want to create a new one each time
            // because it will impact performance.
            Application.Add("PackageIndexHelper", indexHelper);
        }
    }
}
```

What we are doing here is first creating an AlgoliaClient using our application id and key. You will need to update these two values using your own credentials. You can either use the Admin key already created for you or create your own key and choose the permissions allowed.

Next, we create an IndexHelper and inject our AlgoliaClient into it along with the name of the index and the value of our Package model identifier that should be mapped to the Algolia index object Id. The reason we want to specify the identifier in our model is because the IndexHelper will automatically build and  map our model to the Algolia index object.

Finally, we add our IndexHlper to the application variable so we can use it elsewhere in our application.

One thing to note is the index we are using in Algolia is called ```packages```. You can either create it ahead of time or it will be automatically created when you issue index commands.

### Update our Admin controller to work with Algolia

Now that we have an Algolia IndexHelper to use, we need to update our Admin controller to talk to Algolia when someone issues an indexing command.

Update each of the 3 following actions inside ```AdminController.cs```. Make sure and add a using statement for Algolia.Search.

```c#
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> ReIndexData()
{
    // Get the package index helper from Application variable
    var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
    await packageIndexHelper.OverwriteIndexAsync(db.Packages, 1000);

    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> IndexData()
{
    // Get the package index helper from Application variable
    var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
    await packageIndexHelper.SaveObjectsAsync(db.Packages, 1000);

    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> DeleteData()
{
    // Get the package index helper from Application variable
    var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
    await packageIndexHelper.DeleteObjectsAsync(db.Packages, 1000);

    return View();
}
```

The IndexHelper is very nice. It will automatically handle the steps needed to perform indexing, re-indexing, and deleting of data. The way it works is it already knows the identifier to look for in your model so when you make an OverwriteIndex, SaveObjects, or DeleteObjects call it automatically converts your model to JSON, adds an objectId field, sets the objectId field to the identifier you told it to use, and makes the necessary calls to Algolia.

Now is a great time to test things out. You should already have packages in your database that are not in your Algolia index. Try out the different functions under the ```/admin``` path. You should see your index get populated, re-populated, and cleared out depending on which function you use.

### Update our CRUD to work with Algolia

Having administrative ability to manage our index is great, but we really need to have our index kept up to date as users create, update, and delete packages from our database.

We will do this by updating the actions inside our Home controller we created earlier to support our CRUD operations on packages.

Update each of the 3 following actions inside ```HomeController.cs```. Make sure and add a using statement for Algolia.Search.

```c#
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Create([Bind(Include = "Id,Name,Link,Count")] Package package)
{
    if (ModelState.IsValid)
    {
        db.Packages.Add(package);
        await db.SaveChangesAsync();

        // Get the package index helper from Application variable
        var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
        await packageIndexHelper.SaveObjectAsync(package);

        return RedirectToAction("Index");
    }

    return View(package);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Link,Count")] Package package)
{
    if (ModelState.IsValid)
    {
        db.Entry(package).State = EntityState.Modified;
        await db.SaveChangesAsync();

        // Get the package index helper from Application variable
        var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
        await packageIndexHelper.SaveObjectAsync(package);

        return RedirectToAction("Index");
    }
    return View(package);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<ActionResult> DeleteConfirmed(int id)
{
    Package package = await db.Packages.FindAsync(id);
    db.Packages.Remove(package);
    await db.SaveChangesAsync();

    // Get the package index helper from Application variable
    var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
    await packageIndexHelper.DeleteObjectAsync(package);

    return RedirectToAction("Index");
}
```

What we did here was whenever a package is created, updated, or deleted, we use the IndexHelper to add, update, or remove the data within our index.

Go ahead and try it out.

### Wrap up

That wraps up this tutorial on adding Algolia to an ASP.NET MVC application. In the next tutorial, we will explore how to add real time search to our MVC application.

Source code for tutorials can be found [here on GitHub](https://github.com/algolia/tutorials/tree/master/csharp/src/PackageTrack).