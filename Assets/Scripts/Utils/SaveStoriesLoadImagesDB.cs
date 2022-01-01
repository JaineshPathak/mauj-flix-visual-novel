using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveStoriesLoadImagesDB : MonoBehaviour
{
    public StoriesLoadImagesDB storiesLoadImagesDB;


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

#if UNITY_EDITOR
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