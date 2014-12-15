# Algolia Real Time Search with Twitter typeahead.js

In our [previous article](/front-end-javascript/Getting%20Started%20with%20Algolia%20Real%20Time%20Search.md) we learned how to easily add [Algolia's real time search](https://www.algolia.com) to our web application. We kept it simple and showed the results in a functional but not very pretty result list.

In this article, we will see how to quickly and easily add some awesomesauce to our application by integrating our search results with Twitter's [typeahead.js](https://github.com/twitter/typeahead.js). If you haven't read the [previous article](/front-end-javascript/Getting%20Started%20with%20Algolia%20Real%20Time%20Search.md), you will want to so you can create your Algolia account and initial data index which will be used in this article.

### Initial code

To help start this tutorial, we will start with a basic application structure. Create the following directory structure and files.

```
user-search-typeahead/
  css/
    styles.css
  js/
    main.js
  index.html
```

Add the following code to ```index.html```, ```styles.css```, and ```main.js```.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <title>Algolia | User Search with typeahead.js</title>
  <link rel="stylesheet" type="text/css" href="/css/styles.css">
</head>
<body>
  <div class="search">
    <h1>Search users in real time with typeahead.js</h1>
    <input id="typeahead-algolia" class="typeahead" type="text">
  </div>
  <script src="https://cdn.jsdelivr.net/jquery/2.1.1/jquery.min.js"></script>
  <script src="https://cdn.jsdelivr.net/algoliasearch/latest/algoliasearch.min.js"></script>
  <script src="/js/main.js"></script>
</body>
</html>
```

```css
.typeahead {
  width: 300px;
  height: 30px;
  font-size: 24px;
  line-height: 30px;
}
```

```javascript
$(document).ready(function() {
  var algolia = new AlgoliaSearch('<APPLICATION ID>', '<SEARCH-ONLY API KEY>');
  var index = algolia.initIndex('<INDEX NAME>');
});
```

You will need to update the JavaScript file and set your own application id, search-only api key, and index name. These will be the same ones you used for the previous tutorial we did.

### Algolia's support for Twitter typeahead.js

One of the many nice things about Algolia's APIs is the built in adapter for Twitter typeahead.js.

```javascript
/*
 * Get a Typeahead.js adapter
 * @param searchParams contains an object with query parameters (see search for details)
 */
ttAdapter: function(params) {
    var self = this;
    return function(query, cb) {
        self.search(query, function(success, content) {
            if (success) {
                cb(content.hits);
            }
        }, params);
    };
}
```

This adapter makes it very simple to hook into typeahead.js functionality for building beautiful auto complete UI components. In essence it is building the necessary hooks to automatically initiate a search and return the results as a user types within the typeahead.js component.

### Add typeahead.js to our application

In order to add typeahead.js to our application, we need to include the script. In the ```index.html``` file, add the following typeahead.js script tag just before the Algolia script tag.

```html
<script src="https://cdn.jsdelivr.net/typeahead.js/0.10.5/typeahead.jquery.min.js"></script>
```

Now that we have the necessary scripts, we can tie the typeahead.js functionality with our Algolia search code. Update ```main.js``` to look like the following.

```javascript
$(document).ready(function() {
  var algolia = new AlgoliaSearch('<APPLICATION ID>', '<SEARCH-ONLY API KEY>');
  var index = algolia.initIndex('<INDEX NAME>');

  $('#typeahead-algolia').typeahead(null, {                                
    source: index.ttAdapter({ "hitsPerPage": 10 }),
    displayKey: 'name'
  });
});
```

So that is all we need in order to have nice type ahead/auto complete functionality tied to our Algolia real time search. What is happening here is as we type in the input field, the typeahead component is firing off searches using the ```ttAdapter``` we hooked in as the source. As results come back, the typeahead component pulls out the ```name``` field from each ```hit``` and shows that in the dropdown.

### Make it a little prettier

Some final touches will be to make the results look a little nicer with some CSS. Open up ```styles.css``` and update it to the following.

```css
.typeahead {
  width: 300px;
  height: 30px;
  font-size: 24px;
  line-height: 30px;
}

.tt-input,
.tt-hint {
  width: 396px;
  height: 30px;
  padding: 8px 12px;
  font-size: 24px;
  line-height: 30px;
  border: 2px solid #ccc;
  border-radius: 8px;
  outline: none;
}

.tt-input {
  box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);
}

.tt-hint {
  color: #999
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
  font-size: 18px;
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

Your results should look like this now if everything was done correctly.

![Algolia Typeahead Example](/images/Algolia_typeahead.png)

### Customize our typeahead.js options

There are three options we can control when setting up typeahead.js.

1. highlight - If true, when suggestions are rendered, pattern matches for the current query in text nodes will be wrapped in a strong element with tt-highlight class. Defaults to false.
2. hint - If false, the typeahead will not show a hint. Defaults to true.
3. minLength - The minimum character length needed before suggestions start getting rendered. Defaults to 1.

Right now we are not supplying options when creating our typeahead object. Let's change that by updating our code in ```main.js``` to look like this.

```javascript
$(document).ready(function() {
  var algolia = new AlgoliaSearch('<APPLICATION ID>', '<SEARCH-ONLY API KEY>');
  var index = algolia.initIndex('<INDEX NAME>');

  $('#typeahead-algolia').typeahead({
    highlight: false,
    hint: true,
    minLength: 1
  }, 
  {                                
    source: index.ttAdapter({ "hitsPerPage": 10 }),
    displayKey: 'name'
  });
});
```

What we have done is passed in an object with the options explicitly set to the defaults. This will give you and idea of what you can tweak and play with. As the code stands now, the application will behave the same. Feel free to try different settings out to see how they behave.

### Enhancing our dataset

The second parameter to typeahead.js is an object that is a dataset. Datasets in typeahead.js have 4 fields that can be set.

1. source – The backing data source for suggestions. Expected to be a function with the signature (query, cb). It is expected that the function will compute the suggestion set (i.e. an array of JavaScript objects) for query and then invoke cb with said set. cb can be invoked synchronously or asynchronously. A Bloodhound suggestion engine can be used here, to learn how, see Bloodhound Integration. Required.
2. name – The name of the dataset. This will be appended to tt-dataset- to form the class name of the containing DOM element. Must only consist of underscores, dashes, letters (a-z), and numbers. Defaults to a random number.
3. displayKey – For a given suggestion object, determines the string representation of it. This will be used when setting the value of the input control after a suggestion is selected. Can be either a key string or a function that transforms a suggestion object into a string. Defaults to value.
4. templates – A hash of templates to be used when rendering the dataset. Note a precompiled template is a function that takes a JavaScript object as its first argument and returns a HTML string.

We have already used source and displayKey. We will now take advantage of templates to further enhance our application.

### Typeahead.js templates

For our template, we will use [Hogan.js](http://twitter.github.io/hogan.js/). What we essentially need is a function that can take a JavaScript object and return an HTML string. Hogan will give us that functionality.

First thing we need to do is include Hogan.js in our application. Open up ```index.html``` and add the following script tag before the Algolia script tag.

```html
<script src="https://cdn.jsdelivr.net/hogan.js/3.0.2/hogan.common.js"></script>
```

Next we will need to create our template for use by typeahead.js. Open up ```main.js``` and add the following code before the typeahead.js code.

```javascript
var template = Hogan.compile('<div class="hit">' +
  '<div class="name">' +
    '{{{ name }}} ' +
  '</div>' +
  '{{#attributes}}' +
    '<div class="attribute">{{ attribute }}: {{{ value }}}</div>' +
  '{{/attributes}}' +
  '</div>');
```

This template will serve to show our results by displaying the name along with a sub list of attributes that match out search criteria.

The final piece we need is to tell typeahead.js to use our template. Update the typeahead.js code like the following.

```javascript
$('#typeahead-algolia').typeahead({
  highlight: false,
  hint: true,
  minLength: 1
}, 
{                                
  source: index.ttAdapter({ "hitsPerPage": 10 }),
  displayKey: 'name',
  templates: {
    suggestion: function(hit) {
      hit.attributes = [];

      // check each hit for highlighted results
      for (var attribute in hit._highlightResult) {
        if (attribute === 'name') {
          // already shown, no need to add a second time
          continue;
        }

        // all highlighted results with match level not equal to non should
        // be added in order to show up in the results drop down
        if (hit._highlightResult[attribute].matchLevel !== 'none') {
          hit.attributes.push({ 
            attribute: attribute, 
            value: hit._highlightResult[attribute].value 
          });
        }
      }

      // render a template for each hit
      return template.render(hit);
    }
  }
});
```

What we have done is defined what we want to show for the suggestion in the drop down by building our template. The really cool thing going on here is that we are taking advantage of the ```_highlightResult``` object for each of the hits provided by Algolia. For each of the highlighted results that have a match level not equal to none, we add it to our attributes object which will end up being shown as part of our template. Here is an example of a hit with the highlighted results. You will notice that email, web, and name have highlighted results when searching for the letter 'k'.

```javascript
{
  "company": "Frances Meyer Inc",
  "address": "2505 Congress St",
  "email": "kelli@varrato.com",
  "web": "http://www.kellivarrato.com",
  "name": "Kelli Varrato",
  "objectID": "3769410",
  "_highlightResult": {
    "company": {
      "value": "Frances Meyer Inc",
      "matchLevel": "none",
      "matchedWords": []
    },
    "address": {
      "value": "2505 Congress St",
      "matchLevel": "none",
      "matchedWords": []
    },
    "email": {
      "value": "<em>k</em>elli@varrato.com",
      "matchLevel": "full",
      "matchedWords": ["k"]
    },
    "web": {
      "value": "http://www.<em>k</em>ellivarrato.com",
      "matchLevel": "full",
      "matchedWords": ["k"]
    },
    "name": {
      "value": "<em>K</em>elli Varrato",
      "matchLevel": "full",
      "matchedWords": ["k"]
    }
  }
}
```

### Final result

If everything is working properly, this is what your results should look like.

![Algolia Advanced Example](/images/Algolia_hogan.png)