using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class BackendDownloadAssets : MonoBehaviour
{
    public bool clearCache;
    private const string storyEpisodesLabel = "StoryEpisode";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        if (clearCache)
            Caching.ClearCache();

        AsyncOperationHandle<long> sizeDownloadHandle = Addressables.GetDownloadSizeAsync(storyEpisodesLabel);
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

        Addressables.Release(sizeDownloadHandle);

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
