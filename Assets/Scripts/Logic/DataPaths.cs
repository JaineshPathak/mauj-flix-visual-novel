using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataPaths
{
    public static readonly string progressDirectoryPath = Application.persistentDataPath + "/progress";
    public static readonly string loadProgressPath = Application.persistentDataPath + "/progress/";
    public static readonly string loadProgressFileExtension = ".json";

    public static readonly string storyDatabaseFileName = "StoriesDB.json";
    public static readonly string storyDatabaseFileNameTest = "StoriesDBTest.json";
    public static readonly string storyImagesLoadDatabaseName = "StoriesLoadImagesDB.json";

    public static readonly string firebaseGoogleProviderID = "google.com";
    public static readonly string firebaseAnonymousProviderID = "(anonymous)";
}
