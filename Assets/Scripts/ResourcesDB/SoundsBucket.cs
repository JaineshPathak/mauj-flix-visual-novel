using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundsBucket : MonoBehaviourSingleton<SoundsBucket>
{
    //public static SoundsBucket instance;

    [SerializeField] private AssetReference[] musicLists;
    [SerializeField] private AssetReference[] soundsList;

    private int currentMusicIndex = -1;
    private int currentSoundIndex = -1;
    private AsyncOperationHandle<AudioClip> musicHandle;
    private AsyncOperationHandle<AudioClip> soundHandle;

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }*/

    public void GetMusicAtIndex(int index, Action<AudioClip> callback, bool unloadPrevious = false)
    {
        currentMusicIndex = index;
        StartCoroutine(GetMusicAtIndexRoutine(index, callback, unloadPrevious));
    }

    private IEnumerator GetMusicAtIndexRoutine(int index, Action<AudioClip> callback, bool unloadPrevious)
    {
        if (unloadPrevious)
            Addressables.Release(musicHandle);

        musicHandle = Addressables.LoadAssetAsync<AudioClip>(musicLists[index]);

        yield return musicHandle;

        if (musicHandle.Status == AsyncOperationStatus.Succeeded)
            callback?.Invoke(musicHandle.Result);
    }

    //-------------------------------------------------------------------------------------------------------------------

    public void GetSoundAtIndex(int index, Action<AudioClip> callback, bool unloadPrevious = false)
    {
        currentSoundIndex = index;
        StartCoroutine(GetSoundAtIndexRoutine(index, callback, unloadPrevious));
    }

    private IEnumerator GetSoundAtIndexRoutine(int index, Action<AudioClip> callback, bool unloadPrevious)
    {
        if (unloadPrevious)
            Addressables.Release(soundHandle);

        soundHandle = Addressables.LoadAssetAsync<AudioClip>(soundsList[index]);

        yield return soundHandle;

        if (soundHandle.Status == AsyncOperationStatus.Succeeded)
            callback?.Invoke(soundHandle.Result);
    }

    /*public AudioClip GetMusicAtIndex(int index)
    {
        return musicsList[index];
    }

    public AudioClip GetSoundAtIndex(int index)
    {
        return soundsList[index];
    }*/
}
