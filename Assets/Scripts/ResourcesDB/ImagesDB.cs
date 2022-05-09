using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class ImagesDB : MonoBehaviour
{
    public bool testMode;
    public SpriteAtlas spriteAtlas;                 //When testMode = true;
    public AssetReference spriteAtlasRef;           //When testMode = false;

    public static event Action<SpriteAtlas> OnDBSpriteAtlasLoaded;

    public Sprite[] atlasTexturesList;

    private void Start()
    {
        if (!testMode)            
            StartCoroutine(LoadSpriteAtlasRef());
    }

    private void OnValidate()
    {
        if(testMode && spriteAtlas && atlasTexturesList.Length <= 0)
        {
            Array.Resize(ref atlasTexturesList, spriteAtlas.spriteCount);
            spriteAtlas.GetSprites(atlasTexturesList);
        }
    }

    //imageLoader.currentTextureIndexToLoad = EditorGUILayout.Popup(imageLoader.currentTextureIndexToLoad, atlasSpritesNamesList);

    private IEnumerator LoadSpriteAtlasRef()
    {
        var loadAtlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasRef);

        yield return loadAtlasHandle;

        if (loadAtlasHandle.Status == AsyncOperationStatus.Succeeded)        
            OnDBSpriteAtlasLoaded?.Invoke(loadAtlasHandle.Result);        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ImagesDB)), CanEditMultipleObjects]
public class ImagesDBEditor : Editor
{
    private SerializedProperty testModeSerial;
    private SerializedProperty spriteAtlasSerial;
    private SerializedProperty spriteAtlasRefSerial;

    private void OnEnable()
    {
        testModeSerial = serializedObject.FindProperty("testMode");
        spriteAtlasSerial = serializedObject.FindProperty("spriteAtlas");
        spriteAtlasRefSerial = serializedObject.FindProperty("spriteAtlasRef");
    }

    public override void OnInspectorGUI()
    {
        ImagesDB imagesDB = target as ImagesDB;

        serializedObject.Update();

        AddPropertyField(testModeSerial, "Test Mode?", "For testing purpose, uncheck when shipping for Production");
        if (imagesDB.testMode)
            AddPropertyField(spriteAtlasSerial, "Sprite Atlas", "Assign a Sprite Atlas from Project");
        else
            AddPropertyField(spriteAtlasRefSerial, "Sprite Atlas Ref", "Assign a Sprite Atlas checked as Addressable");

        serializedObject.ApplyModifiedProperties();
    }

    private void AddPropertyField(SerializedProperty property, string label, string tooltip)
    {
        if (property == null)
            return;

        GUIContent propertyContent = new GUIContent(label, tooltip);
        EditorGUILayout.PropertyField(property, propertyContent, GUILayout.ExpandHeight(false));
    }
}
#endif