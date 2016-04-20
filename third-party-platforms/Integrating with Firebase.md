# Integrating with Firebase

## 1. Introduction

One key feature of [Firebase](https://www.firebase.com/) is building realtime applications by using their backend to store and sync data. Algolia further enhances that by providing realtime search capabilities. In a few simple steps, this tutorial will teach you how to import your existing data, index new data as it is added to Firebase, and remove indexed data when it is removed from Firebase.

Algolia's [Node.js client](https://github.com/algolia/algoliasearch-client-node) simplifies the integration of your Firebase applications with Algolia's real time search service. The module makes it easy for you to use Algolia's search capabilities in a manner that will be familiar to those already developing Firebase and Node.js applications.

## 2. Prerequisites

### Familiar with Firebase

This tutorial assumes you are familiar with Firebase, how it works, and how to build Firebase applications. If you would like to learn more before continuing with this tutorial, we suggest reading the following documentation and tutorials:

1. [Getting started](https://www.firebase.com/how-it-works.html)
2. [Quickstart web tutorial](https://www.firebase.com/docs/web/quickstart.html)
3. [Web development guide](https://www.firebase.com/docs/web/guide/)

## 3. Create a Node.js Application

In order to index your Firebase data and continually add/update index information, you will need to create a Node.js application. This application will be responsible for getting data out of Firebase and indexing it with Algolia. It can then be run anywhere like Heroku, Nodejitsu, AWS, Azure, etc.

In this tutorial, we will be indexing contact information in a Firebase application. Be sure to change 'YourApplicationID' and 'YourAPIKey' to your account values [here](https://www.algolia.com/licensing). Because we are making calls that require more than read access, you will need to create a new key or use an existing one that can Add Records, Delete Records, and Delete Index (for reindexing example). You will also need to set your Firebase 'INSTANCE' to the one your application uses.

Here is the intial portion of the Node.js application. 

```javascript
var Firebase = require('firebase');
var Algolia  = require('algolia-search');
var client   = new Algolia('YourApplicationID', 'YourAPIKey');
var index    = client.initIndex('contacts');

//Connect to our Firebase contacts data
var fb = new Firebase('<INSTANCE>.firebaseio.com/contacts');
```

Be sure to install the necessary packages so your application will run.

```tcsh
npm install firebase --save
npm install algolia-search --save
```

## 4. Import Existing Data

In many cases, you may already have data within your Firebase application. In order to integrate with Algolia, you will want to index that data. We will use contact information being stored within Firebaseio as our example. Add the following code to your Node.js application.

```javascript
//Get all data from Firebase
fb.on('value', initIndex);

function initIndex(snap) {
  //Array of data to index
  var toIndex = [];

  //Store values
  var values = snap.val();

  //Process each Firebase ojbect
  for (var key in values) {
    if (values.hasOwnProperty(key)) {
      //Get Firebase object
      var firebaseObject = values[key];

      //Override Algolia object ID with the Firebase object key
      firebaseObject.objectID = key;

      //Save object for indexing
      toIndex.push(firebaseObject);
    }
  }

  //Add new indices (if no matching firebaseObject.objectID) or update
  index.saveObjects(toIndex, function(error, content) {
    if (error)
      console.log('Got an error: ' + content.message);
  });
}
```

To ensure the indexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

Once you run this code, you will have all of your existing Firebase data indexed with Algolia. You will want to remove this code once is is done because the event will continue to fire each time data is added.

## 5. Reindex Data

Sometimes, you may have the need to completely reindex your data. This means removing data from the index that may not longer exist, adding new data, and updating existing data. The following code can be added to the Node.js application to perform a reindexing. You will want to remove or comment out the initial index code if currently present.

```javascript
//Get all data from Firebase
fb.on('value', reindexIndex);

function reindexIndex(snap) {
  //Array of data to index
  var toIndex = [];

  //Create a temp index
  var tempIndex = 'contacts_temp';
  var tmpIndex = client.initIndex(tempIndex);

  //Store values
  var values = snap.val();

  //Process each Firebase ojbect
  for (var key in values) {
    if (values.hasOwnProperty(key)) {
      //Get Firebase object
      var firebaseObject = values[key];

      //Override Algolia object ID with the Firebase object key
      firebaseObject.objectID = key;

      //Save object for indexing
      toIndex.push(firebaseObject);
    }
  }

  //Add new indices to temp index
  tmpIndex.saveObjects(toIndex, function(error, content) {
    if (error)
      console.log('Got an error: ' + content.message);
    else {
      //Overwrite main index with temp index
      client.moveIndex(tempIndex, 'contacts', function(error, content) {
        if (error)
          console.log('Got an error: ' + content.message);
      });
    }
  });
}
```

To ensure the reindexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

Once you run this code, you will have all of your existing Firebase data reindexed with Algolia. You will want to remove this code once it is done because the event will continue to fire each time data is added.

## 6. Add or Update Data

Now, we need to handle the case where data is being added or updated. We can easily setup our code to automatically add or update data to our search index by attaching to the 'child_added' and 'child_changed' events. This will allow us to define code that will be called after data is stored in Firebase. Add the following code to your Node.js application.

```javascript
//Listen for changes to Firebase data
fb.on('child_added',   addOrUpdateIndex);
fb.on('child_changed', addOrUpdateIndex);

function addOrUpdateIndex(snap) {
  //Get Firebase object
  var firebaseObject = snap.val();

  //Override Algolia object ID with the Firebase object key
  firebaseObject.objectID = snap.key();

  //Add new index (if no matching firebaseObject.objectID) or update
  index.saveObject(firebaseObject, function(error, content) {
    if (error)
     console.log('Got an error: ' + content.message);
  });
}
```

Now, whenever contact data is saved in Firebase, it will automatically be indexed with Algolia.

## 7. Delete Data

Next, we need to handle the case where data is deleted from your Firebase application. In order to do this, we can attach to the 'child_removed' event. This will allow us to define code that will be called after data is removed from Firebase. Add the following code to your Node.js application.

```javascript
//Listen for changes to Firebase data
fb.on('child_removed', removeIndex);

function removeIndex(snap) {
  //Get the Firebase/Algolia objectId
  var objectId = snap.key();

  //Remove the index from Algolia
  index.deleteObject(objectId, function(error, content) {
    if (error)
      console.log('Got an error: ' + content.message);
  });
}
```

Now, whenever contact data is removed from Firebase, it will automatically get removed from Algolia.

## 8. Next Steps

1. [Read the Node.js documentation](https://www.algolia.com/doc/node)
2. [Dive into the Node.js command reference](https://github.com/algolia/algoliasearch-client-node#commands-reference)
3. [Explore the Node.js API client source code](https://github.com/algolia/algoliasearch-client-node)













