# Importing Data from C# 

## 1. Introduction

In a few simple steps, this tutorial will teach you how to import your existing data, index new data as it is added, and remove indexed data when it is removed from C# based applications.

The [Algolia Search API Client for C#](https://github.com/algolia/algoliasearch-client-csharp) simplifies the integration of your C# based applications with Algolia's real time search service.

## 2. Add Agolia Real Time Search to the Project

In order to integrate Algolia within your C# application, you will need to add the Algolia NuGet Package. Open up the Package Manager Console (Tools → NuGet Package Manager → Package Manager Console) and enter the command ```Install-Package Algolia.Search```

## 3. Import Existing Data

In many cases, you may already have data within your C# application. In order to integrate with Algolia, you will want to index that data. We will use contact information being stored within a DB and accessed with Entity Framework as our example.

```csharp
private void IndexData()
```

You can now use this function within your C# application to index your existing data.

Be sure to change 'YourApplicationID' and 'YourAPIKey' to your account values [here](https://www.algolia.com/licensing). Because we are making calls that require more than read access, you will need to create a new key or use an existing one that can Add Records, Delete Records, and Delete Index (for reindexing example). If you create a new key, you will need to make sure it can Add Records and Delete Records.

To ensure the reindexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

## 4. Reindex Data

Sometimes, you may have the need to completely reindex your data. This means removing data from the index that may not longer exist, adding new data, and updating existing data. The following code can be used within your C# application to perform a reindexing.

```csharp
private void ReIndexData()
```

To ensure the reindexing performs well it is suggested you limit the number of items indexed per call between 1,000 and 10,000 depending on the object size.

## 5. Add or Update Data

Now, we need to handle the case where data is being added or updated. We can easily setup our code to add or update data to our search index.

```csharp
private void AddOrUpdateData(Contact contact)
```

This code can be used to add or update contacts within our search index.

## 6. Delete Data

Next, we need to handle the case where data is deleted from your C# application.

```csharp
private void DeleteData(Contact contact)
```

Now, whenever contact data is removed, it will also get removed from Algolia.

## 7. Next Steps

1. [Read the C# documentation](https://www.algolia.com/doc/csharp)
2. [Dive into the C# command reference](https://github.com/algolia/algoliasearch-client-csharp#commands-reference)
3. [Explore the C# API client source code](https://github.com/algolia/algoliasearch-client-csharp)












