## PackageTrack Part 3: Windows Phone 8.1

In this 3 part tutorial, we are going to explore how to easily add Algolia search to Windows Phone 8.1 and ASP.NET MVC applications. To do this, we will take advantage of Algolia's [C# client](https://github.com/algolia/algoliasearch-client-csharp) to add indexing capabilities to the ASP.NET MVC application and search capabilities to the Windows Phone application.

The application we will be building is called PackageTrack. It is a simple web application where a user can create, read, update, and delete packages they like and use. The web application will be capable of managing the package information, indexing that data with Algolia, adding auto complete search to the UI, and providing a simple REST API for the data. We will also create a Windows Phone application with an auto suggest search box hooked into the Algolia search service to provide search results. Users will then be able to select one of the search results and have that information shown by requesting the data from the web application API.

## Create REST API for packages

The last thing we need to do in our web application is to build a REST based API for our package information. This will be used by our Windows Phone application.

First, create a new directory within the ```Controllers``` directory and name it ```Api```.

Right click on the Api folder and choose ```Add``` followed by ```Controller```. Follow the next few screenshots to see what settings to use and values to set.

![Screenshot 07](/images/algolia_csharp_07.png)

![Screenshot 08](/images/algolia_csharp_08.png)

![Screenshot 09](/images/algolia_csharp_09.png)

Because we want to only return JSON even when the request header is not ```application/json``` we need to update our ```WebApiConfig.cs``` file in the ```App_Start``` directory.

```c#
using System.Net.Http.Headers;

  ...

  config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
```

We are now ready to create our Windows Phone application that will perform searches using Algolia and show data from our web application.

## Create a new Windows Phone Application

In our current solution, create a new project and choose ```Blank App (Windows Phone)```. You can name it whatever you like, but we went with PackageTrack.Phone.

![Screenshot 10](/images/algolia_csharp_10.png)

## Install necessary NuGet package

This application requires one new package in order to work.

**Algolia Search**

Install the Algolia Search package. The version used in this tutorial is 3.0.5.

```tcsh
PM> Install-Package Algolia.Search
```

## Configure startup projects

In order for both applications to work, we need to configure our solution to start the web and phone application when we build and run. The following screenshots show how to do this.

![Screenshot 11](/images/algolia_csharp_11.png)

![Screenshot 12](/images/algolia_csharp_12.png)

Debug or run your applications now to make sure both projects start.

## Build our application UI

Now that our project and solution are ready to go, we can add some components to our application to build out our UI.

If ```MainPage.xaml``` is not open, you will want to open it now. This will load the designer view. The next two steps and screenshots show the two components we want to add to our view. Just drag each of the components from the Toolbox within Visual Studio into your UI as shown below.

### Add AutoSuggestBox

![Screenshot 13](/images/algolia_csharp_13.png)

### Add WebView

![Screenshot 14](/images/algolia_csharp_14.png)

![Screenshot 15](/images/algolia_csharp_15.png)

### Configure WebView

We need to give our WebView a name so we can reference it within our code. You will need to name it ```SearchWebView```. The following code shows the updated XAML for this change.

```xml
<WebView Name="SearchWebView" HorizontalAlignment="Left" Height="576" Margin="10,54,0,0" VerticalAlignment="Top" Width="380"/>
```

### Configure AutoSuggestBox events

Our AutoSuggestBox is where the user will start typing a search query and will be responsible for searching our index on Algolia. We need to setup two events to handle when the text changes and when a suggestion is selected.

Update the MainPage XAML to the following for the AutoSuggestBox.

```xml
<AutoSuggestBox HorizontalAlignment="Left" Margin="10,10,0,0" VerticalAlignment="Top" Width="380" TextChanged="AutoSuggestBox_TextChanged" SuggestionChosen="AutoSuggestBox_SuggestionChosen"/>
```

When adding new event handlers in this fashion, the code behind is automatically created in ```MainPage.xaml.cs```. If they are not there, be sure to add them as follows.

```c#
private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
{

}

private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
{

}
```

## Add Algolia to our application

If you don't have ```MainPage.xaml.cs``` open, open it now.

We are going to add the necessary code to add Algolia search capabilities to our application.

The first thing we need is to add two private members to our MainPage class. These will be an AlgoliaClient and Index.

```c#
private AlgoliaClient algoliaClient = null;
private Index algoliaIndex = null;
```

Next, we need to instantiate these objects within the constructor. Be sure to change the application id and search only key to the ones you used in the web project.

```c#
public MainPage()
{
    this.InitializeComponent();

    this.NavigationCacheMode = NavigationCacheMode.Required;

    algoliaClient = new AlgoliaClient("<APPLICATION_ID>", "<SEARCH_ONLY_API_KEY>");
    algoliaIndex = algoliaClient.InitIndex("packages");
}
```

Finally, be sure to add the necessary using statement.

```c#
using Algolia.Search;
```

## Tie it all together

We now need to create a class that can be used to deserialize the Algolia search responses. Inside ```MainPage.xaml.cs``` create the following class. You will notice it defines Name, Link, Count, and objectID. The first 3 are fields we defined in our web application. The last one is the ID that Algolia uses in its indexed data. This is the field we mapped our models ID field to.

Also, you will see we are overriding ```ToString()```. This is because we will be adding each ```Hit``` object to our AutoSuggestBox and want to control how the data is shown.

```c#
private class PackagesResult
{
    public class Hit
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Count { get; set; }
        public string objectID { get; set; }

        public override string ToString()
        {
            return Name + Environment.NewLine +
                "  Link:\t" + Link + Environment.NewLine +
                "  Count:\t" + Count;
        }
    }

    public List<Hit> hits { get; set; }
}
```

Next we need to implement our search code each time a user enters a character in our AutoSuggestBox. Update the AutoSuggestBox event handler for TextChanged as follows.

```c#
private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
{
    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
    {
        var searchTerm = sender.Text;
        var query = new Query(searchTerm);
        var result = algoliaIndex.SearchAsync(query).Result;
        var packagesResult = result.ToObject<PackagesResult>();

        sender.ItemsSource = packagesResult.hits;
    }
}
```

What we are doing here is creating a new Algolia Query using the entered text. We then issue a search, get back the results as JSON, and convert it to a PackagesResult object. The last thing we do is add the list of Hits as the ItemsSource of our AutoSuggestBox. This will automatically show the results to the user.

Finally, we need to update the SuggestionChosen event for our AutoSuggestBox when a user selects one of the results we showed them. Update it as follows.

```c#
private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
{
    var hit = (PackagesResult.Hit)args.SelectedItem;
    SearchWebView.Navigate(new Uri("http://localhost:8671/api/packages/" + hit.objectID));
}
```

All we are doing here is taking the selected item, reference that items object ID, and then navigating our WebView to that page. The page we are showing is the API endpoint we created in our web application. This is just a simple example of how to tie the two applications together. A more robust approach would be to process the data from the API and build a UI capable of viewing, editing, and deleting the data.

![Screenshot 16](/images/algolia_csharp_16.png)

### Wrap up

That wraps up this tutorial on adding Algolia to an ASP.NET MVC application along with a Windows Phone 8.1 application. There was a lot going on here, but if you take out the standard stuff for our web and phone application, the Algolia part is very simple and straightforward.

Source code for tutorials can be found [here on GitHub](https://github.com/algolia/tutorials/tree/master/csharp/src/PackageTrack).