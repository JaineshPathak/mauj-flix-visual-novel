#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class SaveStoriesLoadImagesDB : MonoBehaviour
{
    public StoriesDBItemSO[] storiesDBItems;

    [Space(15)]

    public StoriesLoadImagesDB storiesLoadImagesDB;

    private void OnValidate()
    {
        if(storiesDBItems.Length > 0)
        {
            Array.Resize(ref storiesLoadImagesDB.storiesLoadKeys, storiesDBItems.Length);
            for (int i = 0; i < storiesDBItems.Length && (storiesDBItems.Length > 0); i++)
            {
                if(storiesDBItems[i] != null)
                    storiesLoadImagesDB.storiesLoadKeys[i] = storiesDBItems[i].storyThumbnailLoadingName;
            }
        }        
    }

    public void SaveStoryLoadImagesDB()
    {
        string saveString = JsonUtility.ToJson(storiesLoadImagesDB, true);
        SerializationManager.SaveAsTextFile(Application.dataPath, DataPaths.storyImagesLoadDatabaseName, saveString);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public void DeleteStoryLoadImagesDB()
    {
        SerializationManager.DeleteIfFileExists(Application.dataPath + "/" + DataPaths.storyImagesLoadDatabaseName);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}

[CustomEditor(typeof(SaveStoriesLoadImagesDB))]
public class SaveStoriesLoadImagesDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveStoriesLoadImagesDB saveStoriesLoadImagesDB = target as SaveStoriesLoadImagesDB;

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Save to Json"))
            saveStoriesLoadImagesDB.SaveStoryLoadImagesDB();

        if (GUILayout.Button("Delete Json"))
            saveStoriesLoadImagesDB.DeleteStoryLoadImagesDB();
    }
}
#endif