## Part 2: ASP.NET MVC Real Time Search

In this 3 part tutorial, we are going to explore how to easily add Algolia search to Windows Phone 8.1 and ASP.NET MVC applications. To do this, we will take advantage of Algolia's [C# client](https://github.com/algolia/algoliasearch-client-csharp) to add indexing capabilities to the ASP.NET MVC application and search capabilities to the Windows Phone application.

The application we will be building is called PackageTrack. It is a simple web application where a user can create, read, update, and delete packages they like and use. The web application will be capable of managing the package information, indexing that data with Algolia, adding auto complete search to the UI, and providing a simple REST API for the data. We will also create a Windows Phone application with an auto suggest search box hooked into the Algolia search service to provide search results. Users will then be able to select one of the search results and have that information shown by requesting the data from the web application API.

## Implement real time search

With all of our indexing in place, we are ready to add a real time search experience to our web application.

### Download JavaScript libraries

We will need some JavaScript libraries in order to create the search feature. Download each of the following and save them in the ```Scripts``` directory with the names we indicate.

**typeahead.jquery.js**  
[https://cdn.jsdelivr.net/typeahead.js/0.10.5/typeahead.jquery.min.js](https://cdn.jsdelivr.net/typeahead.js/0.10.5/typeahead.jquery.min.js)

**hogan.common.js**  
[https://cdn.jsdelivr.net/hogan.js/3.0.2/hogan.common.js](https://cdn.jsdelivr.net/hogan.js/3.0.2/hogan.common.js)

**algoliasearch.min.js**  
[https://cdn.jsdelivr.net/algoliasearch/2.9/algoliasearch.min.js](https://cdn.jsdelivr.net/algoliasearch/2.9/algoliasearch.min.js)

### Bundle new JavaScript

With the new scripts added to our project, we need to update our bundles to include them. Open up ```\App_Start\BundleConfig.cs``` and add the following new bundle.

```c#
bundles.Add(new ScriptBundle("~/bundles/main").Include(
          "~/Scripts/typeahead.jquery.min.js",
          "~/Scripts/hogan.common.js",
          "~/Scripts/algoliasearch.min.js",
          "~/Scripts/main.js"));
```

### Update our layout

Now we need to update our layout to include a search box and the new script bundle we created.

Open up ```_Layout.cshtml``` and modify the code as follows.

```html
....

<div class="navbar navbar-inverse navbar-fixed-top">
    <div class="container">
        <div class="navbar-header">
            @Html.ActionLink("Package Track", "Index", "Home", new { area = "" }, new { @class = "navbar-brand" })
        </div>
        <div class="typeahead-container">
            <input id="typeahead-algolia" class="typeahead" type="text" placeholder="Search...">
        </div>
    </div>
</div>

...

@Scripts.Render("~/bundles/jquery")
@Scripts.Render("~/bundles/bootstrap")
@Scripts.Render("~/bundles/main")
@RenderSection("scripts", required: false)
```

### Create JavaScript to tie UI with Algolia

The following code needs to be added to a new file named ```main.js``` within the ```Scripts``` directory. This sets us up to perform searches with our packages index on Algolia. Be sure to update the application id and key using your search only key.

```javascript
$(document).ready(function () {
    var algolia = new AlgoliaSearch('<APPLICATION_ID>', '<SEARCH_ONLY_API_KEY>');
    var index = algolia.initIndex('packages');

    var template = Hogan.compile(
        '<a href="/home/details/{{{ Id }}}">' +
            '<div class="hit">' +
                '<div class="name">' +
                    '{{{ Name }}} ' +
                '</div>' +
                '{{#attributes}}' +
                '<div class="attribute">{{ attribute }}: {{{ value }}}</div>' +
                '{{/attributes}}' +
            '</div>' +
        '</a>');

    $('#typeahead-algolia').typeahead({
        highlight: false,
        hint: true,
        minLength: 1
    },
    {
        source: index.ttAdapter({ "hitsPerPage": 10 }),
        displayKey: 'Name',
        templates: {
            suggestion: function (hit) {
                // select matching attributes only
                hit.attributes = [];
                for (var attribute in hit._highlightResult) {
                    if (attribute === 'Name') {
                        // already handled by the template
                        continue;
                    }
                    // all others attributes that are matching should be added in the attributes array
                    // so we can display them in the dropdown menu. Non-matching attributes are skipped.
                    if (hit._highlightResult[attribute].matchLevel !== 'none') {
                        hit.attributes.push({ attribute: attribute, value: hit._highlightResult[attribute].value });
                    }
                }

                // render the hit using Hogan.js
                return template.render(hit);
            }
        }
    });
});
```

### Style the search

Finally, we want to style our search. Add the following CSS to ```\Content\Site.css```.

```css
.typeahead-container {
    float: right;
}

.typeahead {
    padding: 0 5px;
    margin-top: 10px;
    width: 300px;
    height: 30px;
    font-size: 14px;
    line-height: 30px;
    max-width: none;
}

.tt-input,
.tt-hint {
    width: 396px;
    height: 30px;
    padding: 8px 12px;
    font-size: 14px;
    line-height: 30px;
    border: 2px solid #ccc;
    border-radius: 8px;
    outline: none;
}

.tt-input {
    box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);
}

.tt-hint {
    color: #999;
}

.tt-dropdown-menu {
    width: 422px;
    margin-top: 12px;
    padding: 8px 0;
    background-color: #fff;
    border: 1px solid #ccc;
    border: 1px solid rgba(0, 0, 0, 0.2);
    border-radius: 8px;
    box-shadow: 0 5px 10px rgba(0,0,0,.2);
}

.tt-suggestion {
    padding: 3px 20px;
    font-size: 14px;
    line-height: 24px;
}

    .tt-suggestion.tt-cursor {
        color: #fff;
        background-color: #0097cf;
    }

    .tt-suggestion p {
        margin: 0;
    }

    .tt-suggestion em {
        font-weight: bold;
        font-style: normal;
    }

.name {
    font-weight: bold;
}

.attribute {
    margin-left: 10px;
}
```

If you would like to get a more in depth tutorial on the real time search piece we just added, you can read our other articles here.

[Getting Started With Algolia Real Time Search](https://github.com/algolia/tutorials/blob/master/front-end-javascript/Getting%20Started%20with%20Algolia%20Real%20Time%20Search.md)

[Algolia Real Time Search With Twitter typeahead.js](https://github.com/algolia/tutorials/blob/master/front-end-javascript/Algolia%20Real%20Time%20Search%20with%20Twitter%20typeahead.js.md)

Go ahead and test it out.

### Wrap up

That wraps up this tutorial on adding Algolia real time search to an ASP.NET MVC application. In the next tutorial, we will explore how to add Algolia search to a Windows Phone 8.1 application and hook it into our ASP.NET MVC application.

Source code for tutorials can be found [here on GitHub](https://github.com/algolia/tutorials/tree/master/csharp/src/PackageTrack).