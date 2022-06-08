using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class StoriesDBLoader : MonoBehaviourSingletonPersistent<StoriesDBLoader>
{
    //public static StoriesDBLoader instance;

    public AssetReference storiesDBKey;

    [HideInInspector] public StoriesDB storiesDB;

    private IEnumerator Start()
    {
        AsyncOperationHandle<TextAsset> storiesDownloadHandle = Addressables.LoadAssetAsync<TextAsset>(storiesDBKey);

        yield return storiesDownloadHandle;

        switch (storiesDownloadHandle.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:

                string databaseString = storiesDownloadHandle.Result.text;
                storiesDB = JsonUtility.FromJson<StoriesDB>(databaseString);

#if UNITY_EDITOR
                Debug.Log("Stories Loader: Stories DB Successfully Downloaded!");
#endif

                break;

            case AsyncOperationStatus.Failed:

#if UNITY_EDITOR
                Debug.LogError("Stories Loader: Stories DB Download Failed!");
#endif

                break;
        }
    }
}
