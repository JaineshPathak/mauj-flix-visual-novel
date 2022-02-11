using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class BackendDownloadAssets : MonoBehaviour
{
    public bool clearCache;
    private const string storyEpisodesLabel = "StoryEpisode";

    private const string catalogPath = "https://storage.googleapis.com/mauj-flix-stories-bundles-production/Android/catalog_2021.08.18.19.45.34.json";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        if (clearCache)
            Caching.ClearCache();

        yield return null;

        /*AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
        initHandle.Completed += OnInitDone;
        yield return initHandle;

        AsyncOperationHandle<IResourceLocator> catalogHandle = Addressables.LoadContentCatalogAsync(catalogPath, true);
        catalogHandle.Completed += OnCatalogDone;
        yield return catalogHandle;*/

        /*AsyncOperationHandle<long> sizeDownloadHandle = Addressables.GetDownloadSizeAsync(storyEpisodesLabel);
        yield return sizeDownloadHandle;
        
        if(sizeDownloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            if (sizeDownloadHandle.Result > 0)       //Download size greater than 0, so download updates
            {
#if UNITY_EDITOR
                Debug.Log("** BACKGROUND DOWNLOAD: UPDATES FOUND! **", this);
#endif
                AsyncOperationHandle backgroundDownloadHandle = Addressables.DownloadDependenciesAsync(storyEpisodesLabel, false);
                backgroundDownloadHandle.Completed += OnBackgroundDownloadComplete;

                while (!backgroundDownloadHandle.IsDone)
                {
#if UNITY_EDITOR
                    Debug.Log($"** BACKGROUND DOWNLOAD PROGRESS: {backgroundDownloadHandle.PercentComplete}% **", this);
#endif
                    yield return new WaitForEndOfFrame();
                }
                //yield return backgroundDownloadHandle;

#if UNITY_EDITOR
                Debug.Log("** BACKGROUND DOWNLOAD: UPDATES COMPLETE! **", this);
#endif
                Addressables.Release(backgroundDownloadHandle);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("** BACKGROUND DOWNLOAD: NO UPDATES REQUIRED! **", this);
#endif
            }
        }

        Addressables.Release(sizeDownloadHandle);*/

        #region OLD codes
        /*AsyncOperationHandle<IList<IResourceLocation>> episodeLocations = Addressables.LoadResourceLocationsAsync(storyEpisodesLabel);
        yield return episodeLocations;*/

        //print(episodeLocations.Result.Count);
        /*foreach(IResourceLocation resource in episodeLocations.Result)
        {
            print(resource.PrimaryKey + " " + resource.HasDependencies);            
        }*/

        /*if (episodeLocations.Status == AsyncOperationStatus.Succeeded)
        {
            if (clearCache)
                Addressables.ClearDependencyCacheAsync(episodeLocations.Result);

            AsyncOperationHandle<long> downloadSizeHandle = Addressables.GetDownloadSizeAsync(episodeLocations.Result);
            yield return downloadSizeHandle;

            if (downloadSizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                print(downloadSizeHandle.Result);

                if (downloadSizeHandle.Result > 0)
                {
                    AsyncOperationHandle backgroundDownloadHandle = Addressables.DownloadDependenciesAsync(episodeLocations, false);
                    backgroundDownloadHandle.Completed += OnBackgroundDownloadComplete;
                    
                    while (!backgroundDownloadHandle.IsDone)
                    {
                        float percent = backgroundDownloadHandle.PercentComplete;
                        Debug.Log($"Background Progress: {backgroundDownloadHandle.PercentComplete * 100f} %");
                        yield return new WaitForEndOfFrame();
                    }
                    //yield return backgroundDownloadHandle;

                    Addressables.Release(backgroundDownloadHandle);
                }
            }

            Addressables.Release(downloadSizeHandle);
            Addressables.Release(episodeLocations);
        }*/
        #endregion
    }

    private void OnInitDone(AsyncOperationHandle<IResourceLocator> handle)
    {
        switch (handle.Status)
        {
            case AsyncOperationStatus.None:
                print("INIT: IDK!!");
                break;
            case AsyncOperationStatus.Succeeded:
                print("INIT: YOOOOO!!");
                print(handle.Result.LocatorId);
                break;
            case AsyncOperationStatus.Failed:
                print("INIT: NAADA!!");
                break;
        }
    }

    private void OnCatalogDone(AsyncOperationHandle<IResourceLocator> handle)
    {
        switch (handle.Status)
        {
            case AsyncOperationStatus.None:
                print("CATALOG: IDK!!");
                break;

            case AsyncOperationStatus.Succeeded:                
                print("CATALOG: YOOOOO!!");

                foreach (var locator in Addressables.ResourceLocators)
                {
                    switch (locator)
                    {                        
                        case ResourceLocationMap rlm:
                            {
                                Debug.Log(locator.LocatorId);
                                foreach (var kvp in rlm.Locations)
                                {
                                    foreach(var internalid in kvp.Value)
                                    {
                                        Debug.Log(internalid.PrimaryKey + ": " + internalid.InternalId);
                                    }
                                }
                                break;
                            }
                    }
                }

                break;

            case AsyncOperationStatus.Failed:
                print("CATALOG: NAADA!!");
                break;
        }
    }

    private void OnResourceDone(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        switch (handle.Status)
        {
            case AsyncOperationStatus.None:
                print("CATALOG: IDK!!");
                break;

            case AsyncOperationStatus.Succeeded:
                print("CATALOG: YOOOOO!!");

                foreach(var loc in handle.Result)
                {
                    Debug.Log($"{loc.PrimaryKey}: {loc.ProviderId}");
                }

                break;

            case AsyncOperationStatus.Failed:
                print("CATALOG: NAADA!!");
                break;
        }
    }

    private void OnBackgroundDownloadComplete(AsyncOperationHandle handle)
    {
        if (handle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.Log("** BACKGROUND DOWNLOAD: SOMETHING WENT WRONG **", this);
#endif
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
#if UNITY_EDITOR
            Debug.Log("** BACKGROUND DOWNLOAD: LOOKS ALRIGHT **", this);
#endif
        }
    }

    private static float BToKb(long bytes)
    {
        return bytes / 1000f;
    }    
}