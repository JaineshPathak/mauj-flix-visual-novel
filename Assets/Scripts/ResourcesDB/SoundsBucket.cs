using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundsBucket : MonoBehaviourSingleton<SoundsBucket>
{
    //public static SoundsBucket instance;

    public bool testMode;

    [SerializeField] private AudioClip[] musicListTest;
    [SerializeField] private AudioClip[] soundsListTest;

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

        if(testMode)
            callback?.Invoke(musicListTest[index]);
        else
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

        if (testMode)
            callback?.Invoke(soundsListTest[index]);
        else
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

#if UNITY_EDITOR
[CustomEditor(typeof(SoundsBucket)), DisallowMultipleComponent, CanEditMultipleObjects]
public class SoundsBucketEditor : Editor
{
    private SerializedProperty testModeSerial;

    private SerializedProperty musicListTestSerial;
    private SerializedProperty soundsListTestSerial;

    private SerializedProperty musicListsSerial;
    private SerializedProperty soundsListSerial;

    private SoundsBucket soundsBucket;

    private void OnEnable()
    {
        testModeSerial = serializedObject.FindProperty("testMode");

        musicListTestSerial = serializedObject.FindProperty("musicListTest");
        soundsListTestSerial = serializedObject.FindProperty("soundsListTest");

        musicListsSerial = serializedObject.FindProperty("musicLists");
        soundsListSerial = serializedObject.FindProperty("soundsList");
    }

    public override void OnInspectorGUI()
    {
        soundsBucket = target as SoundsBucket;

        serializedObject.Update();

        EditorGUILayout.PropertyField(testModeSerial);

        EditorGUILayout.Space(15);

        if (testModeSerial.boolValue)
        {
            EditorGUILayout.PropertyField(musicListTestSerial);
            EditorGUILayout.PropertyField(soundsListTestSerial);
        }
        else
        {
            EditorGUILayout.PropertyField(musicListsSerial);
            EditorGUILayout.PropertyField(soundsListSerial);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif