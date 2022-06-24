using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ThumbnailType
{
    Small,
    Big,
    Loading,
    Title,
    Trending
};

public class ThumbnailsBucket : MonoBehaviourSingletonPersistent<ThumbnailsBucket>
{
    //public static ThumbnailsBucket instance;

    public AssetReference thumbnailsSmallAtlasRef;
    public AssetReference thumbnailsBigAtlasRef;
    public AssetReference thumbnailsLoadAtlasRef;
    public AssetReference thumbnailsTitleAtlasRef;
    public AssetReference thumbnailsTrendingAtlasRef;

    private SpriteAtlas thumbnailsSmallAtlas;
    private SpriteAtlas thumbnailsBigAtlas;
    private SpriteAtlas thumbnailsLoadAtlas;
    private SpriteAtlas thumbnailsTitleAtlas;
    private SpriteAtlas thumbnailsTrendingAtlas;

    private int successCount = 0;   //If 4 - Means all Atlas fully loaded. Less than 4, then something went wrong downloading

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }*/

    public void StartLoadingAtlas(Action<bool> callback)
    {
        StartCoroutine(StartLoadingAtlasRoutine(callback));
    }

    private IEnumerator StartLoadingAtlasRoutine(Action<bool> callback)
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Thumbnails Bucket: Atlas Thumbnails Loading - STARTED!");

        //-------------------------------------------------------------------------------
        //Small Atlas

        AsyncOperationHandle<SpriteAtlas> atlasSmallHandle = Addressables.LoadAssetAsync<SpriteAtlas>(thumbnailsSmallAtlasRef);

        yield return atlasSmallHandle;

        if (atlasSmallHandle.Status == AsyncOperationStatus.Succeeded)
        {
            successCount++;
            thumbnailsSmallAtlas = atlasSmallHandle.Result;

#if UNITY_EDITOR
            Debug.Log("Thumbnails Bucket: Small Atlas Thumbnails Loading DONE!");
#endif
        }
        else if(atlasSmallHandle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.LogError("Thumbnails Bucket: Small Atlas Thumbnails Loading ERROR!");
#endif
        }

        //-------------------------------------------------------------------------------
        //Big Atlas

        AsyncOperationHandle<SpriteAtlas> atlasBigHandle = Addressables.LoadAssetAsync<SpriteAtlas>(thumbnailsBigAtlasRef);

        yield return atlasBigHandle;

        if (atlasBigHandle.Status == AsyncOperationStatus.Succeeded)
        {
            successCount++;
            thumbnailsBigAtlas = atlasBigHandle.Result;

#if UNITY_EDITOR
            Debug.Log("Thumbnails Bucket: Big Atlas Thumbnails Loading DONE!");
#endif
        }
        else if (atlasBigHandle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.LogError("Thumbnails Bucket: Big Atlas Thumbnails Loading ERROR!");
#endif
        }

        //-------------------------------------------------------------------------------
        //Loading Atlas

        AsyncOperationHandle<SpriteAtlas> atlasLoadHandle = Addressables.LoadAssetAsync<SpriteAtlas>(thumbnailsLoadAtlasRef);

        yield return atlasLoadHandle;

        if (atlasLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            successCount++;
            thumbnailsLoadAtlas = atlasLoadHandle.Result;

#if UNITY_EDITOR
            Debug.Log("Thumbnails Bucket: Loading Atlas Thumbnails Loading DONE!");
#endif
        }
        else if (atlasLoadHandle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.LogError("Thumbnails Bucket: Loading Atlas Thumbnails Loading ERROR!");
#endif
        }

        //-------------------------------------------------------------------------------
        //Title Atlas

        AsyncOperationHandle<SpriteAtlas> atlasTitleHandle = Addressables.LoadAssetAsync<SpriteAtlas>(thumbnailsTitleAtlasRef);

        yield return atlasTitleHandle;

        if (atlasTitleHandle.Status == AsyncOperationStatus.Succeeded)
        {
            successCount++;
            thumbnailsTitleAtlas = atlasTitleHandle.Result;

#if UNITY_EDITOR
            Debug.Log("Thumbnails Bucket: Title Atlas Thumbnails Loading DONE!");
#endif
        }
        else if (atlasTitleHandle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.LogError("Thumbnails Bucket: Title Atlas Thumbnails Loading ERROR!");
#endif
        }

        //-------------------------------------------------------------------------------

        //Title Atlas

        AsyncOperationHandle<SpriteAtlas> atlasTrendingHandle = Addressables.LoadAssetAsync<SpriteAtlas>(thumbnailsTrendingAtlasRef);

        yield return atlasTrendingHandle;

        if (atlasTrendingHandle.Status == AsyncOperationStatus.Succeeded)
        {
            successCount++;
            thumbnailsTrendingAtlas = atlasTrendingHandle.Result;

#if UNITY_EDITOR
            Debug.Log("Thumbnails Bucket: Trending Atlas Thumbnails Loading DONE!");
#endif
        }
        else if (atlasTrendingHandle.Status == AsyncOperationStatus.Failed)
        {
#if UNITY_EDITOR
            Debug.LogError("Thumbnails Bucket: Trending Atlas Thumbnails Loading ERROR!");
#endif
        }

        Debug.Log($"Thumbnails Bucket: Atlas Thumbnails Loading - ENDED! SCORE - {successCount}");

        if(successCount >= 5)
            callback?.Invoke(true);
        else
            callback?.Invoke(false);
    }

    //------------------------------------------------------------------------------------------------------------------

    public Sprite GetThumbnailSprite(string spriteName, ThumbnailType thumbnailType)
    {
        switch (thumbnailType)
        {
            case ThumbnailType.Small:
                return thumbnailsSmallAtlas.GetSprite(spriteName);

            case ThumbnailType.Big:
                return thumbnailsBigAtlas.GetSprite(spriteName);

            case ThumbnailType.Loading:
                return thumbnailsLoadAtlas.GetSprite(spriteName);

            case ThumbnailType.Title:
                return thumbnailsTitleAtlas.GetSprite(spriteName);

            case ThumbnailType.Trending:
                return thumbnailsTrendingAtlas.GetSprite(spriteName);
        }

        return null;
    }

    //------------------------------------------------------------------------------------------------------------------
}
