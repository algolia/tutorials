# Integrating with the Parse Cloud Platform

## 1. Introduction

One key feature of [Parse](https://parse.com/) is for applications to use Parse Core as their data store. In a few simple steps, this tutorial will teach you how to import your existing data, index new data as it is added to Parse, and remove indexed data when it is removed from Parse.

The [Algolia Parse Module](https://github.com/algolia/algoliasearch-client-node#setup-with-parse) simplifies the integration of your Parse based applications with Algolia's real time search service. The module makes it easy for you to use Algolia's search capabilities in a manner that will be familiar to those already using the Algolia Node.js client APIs. 

## 2. Prerequisites

### Familiar with Parse

This tutorial assumes you are familiar with Parse, how it works, and how to build Cloud Code applications. If you would like to learn more before continuing with this tutorial, we suggest reading the following documentation and tutorials:

1. [Getting started with Cloud Code](https://parse.com/docs/cloud_code_guide#started)
2. [Parse quickstart web project](https://parse.com/apps/quickstart#parse_data/web/new)
3. [Parse quickstart Cloud Code project on Mac/Linux](https://parse.com/apps/quickstart#cloud_code/unix)
4. [Parse quickstart Cloud Code project on Windows](https://parse.com/apps/quickstart#cloud_code/windows)

## 3. Add Agolia Real Time Search to the Project

In order to integrate Algolia within your Parse application, you will need to add the Algolia Node.js client. Copy [algoliasearch-node.js](https://raw.githubusercontent.com/algolia/algoliasearch-client-node/master/src/algoliasearch-node.js) to ```cloud/algoliasearch-node.js``` within your Parse Cloud Code directory.

## 4. Import Existing Data

In many cases, you may already have data within your Parse application. In order to integrate with Algolia, you will want to index that data. We will use contact information being stored within Parse as our example.

```javascript
var Algolia = require('cloud/algoliasearch-node');
var client  = new Algolia('YourApplicationID', 'YourAPIKey');
var index   = client.initIndex('contacts');

var indexData = function() {
  //Array of data to index
  var toIndex = [];

  //Create a new query for Contacts
  var query = new Parse.Query('Contact');

  //Find all items
  query.find({
    success: function(contacts) {
      //Process each contact
      for (var i = 0; i < contacts.length; i++) {
        //Convert Parse.Object to JSON
        var parseObject = contacts[i].toJSON();

        //Override Algolia object ID with the Parse.Object unique ID
        parseObject.objectID = parseObject.objectId;

        //Save object for indexing
        toIndex.push(parseObject);
      }

      //Add new indices (if no matching parseObject.objectID) or update
      index.saveObjects(toIndex, function(error, content) {
        if (error)
          console.log('Got an error: ' + content.message);
      });
    },
    error: function(error) {
      console.error('Got an error: ' + error.code + ': ' + error.message);
    }
  });
};
```

You can now use this function within your own Parse Cloud Code functions in order to index your existing data.

Be sure to change 'YourApplicationID' and 'YourAPIKey' to your account values [here](https://www.algolia.com/licensing). Because we are making calls that require more than read access, you will need to create a new key or use an existing one that can Add Records, Delete Records, and Delete Index (for reindexing example). If you create a new key, you will need to make sure it can Add Records and Delete Records.

To ensure the reindexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

## 5. Reindex Data

Sometimes, you may have the need to completely reindex your data. This means removing data from the index that may not longer exist, adding new data, and updating existing data. The following code can be used within your own Parse Cloud Code functions to perform a reindexing.

```javascript
var tempIndex = 'contacts_temp';
var mainIndex = 'contacts';
var Algolia   = require('cloud/algoliasearch-node');
var client    = new Algolia('YourApplicationID', 'YourAPIKey');
var index     = client.initIndex(tempIndex);

var reindexData = function() {
  //Array of data to index
  var toIndex = [];

  //Create a temp index
  var index = client.initIndex(tempIndex);

  //Create a new query for Contacts
  var query = new Parse.Query('Contact');

  //Find all items
  query.find({
    success: function(contacts) {
      //Process each contact
      for (var i = 0; i < contacts.length; i++) {
        //Convert Parse.Object to JSON
        var parseObject = contacts[i].toJSON();

        //Override Algolia object ID with the Parse.Object unique ID
        parseObject.objectID = parseObject.objectId;

        //Save object for indexing
        toIndex.push(parseObject);
      }

      //Add new indices to temp index
      index.saveObjects(toIndex, function(error, content) {
        if (error)
          console.log('Got an error: ' + content.message);
        else {
          //Overwrite main index with temp index
          client.moveIndex(tempIndex, mainIndex, function(error, content) {
            if (error)
              console.log('Got an error: ' + content.message);
          });
        }
      });
    },
    error: function(error) {
      console.error('Got an error: ' + error.code + ': ' + error.message);
    }
  });
};
```
To ensure the reindexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

## 6. Add or Update Data

Now, we need to handle the case where data is being added or updated. We can easily setup our code to automatically add or update data to our search index by using the ```afterSave``` Parse function. This will allow us to define code that will be called after data is stored in Parse.

```javascript
var Algolia = require('cloud/algoliasearch-node');
var client  = new Algolia('YourApplicationID', 'YourAPIKey');
var index   = client.initIndex('contacts');

Parse.Cloud.afterSave("Contact", function(request) {
  //Convert Parse.Object to JSON
  var parseObject = request.object.toJSON();

  //Override Algolia object ID with the Parse.Object unique ID
  parseObject.objectID = parseObject.objectId;

  //Add new index (if no matching parseObject.objectID) or update
  index.saveObject(parseObject, function(error, content) {
    if (error)
     console.log('Got an error: ' + content.message);
  });
});
```

Now, whenever contact data is saved in Parse, it will automatically be indexed with Algolia.

## 7. Delete Data

Next, we need to handle the case where data is deleted from your Parse application. In order to do this, we can use the ```afterDelete``` Parse function. This will allow us to define code that will be called after data is removed from Parse.

```javascript
var Algolia = require('cloud/algoliasearch-node');
var client  = new Algolia('YourApplicationID', 'YourAPIKey');
var index   = client.initIndex('contacts');

Parse.Cloud.afterDelete('Contact', function(request) {
  //Get the Parse/Algolia objectId
  var objectId = request.object.id;

  //Remove the index from Algolia
  index.deleteObject(objectId, function(error, content) {
    if (error)
      console.log('Got an error: ' + content.message);
  });
});
```

Now, whenever contact data is removed from Parse, it will automatically get removed from Algolia.

## 8. Next Steps

1. [Read the Node.js documentation](https://www.algolia.com/doc/node)
2. [Dive into the Node.js command reference](https://github.com/algolia/algoliasearch-client-node#commands-reference)
3. [Explore the Node.js API client source code](https://github.com/algolia/algoliasearch-client-node)













