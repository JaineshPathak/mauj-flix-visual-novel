using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveStoriesDB : MonoBehaviour
{
    public StoriesDB storiesDB;

    private void OnValidate()
    {
        if(storiesDB != null)
        {
            if(storiesDB.storiesCategories.Length > 0)
            {
                for (int i = 0; i < storiesDB.storiesCategories.Length; i++)
                {
                    storiesDB.storiesCategories[i].categoryIndex = i;
                }
            }
        }
    }

    public void SaveDBToJson()
    {
        string saveString = JsonUtility.ToJson(storiesDB, true);
        SerializationManager.SaveAsTextFile(Application.dataPath, DataPaths.storyDatabaseFileName, saveString);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public void DeleteDBJson()
    {
        SerializationManager.DeleteIfFileExists(Application.dataPath + "/" + DataPaths.storyDatabaseFileName);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveStoriesDB))]
public class SaveStoriesDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveStoriesDB saveStoryData = target as SaveStoriesDB;

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Save To Json"))
            saveStoryData.SaveDBToJson();

        if (GUILayout.Button("Delete Json File"))
            saveStoryData.DeleteDBJson();        
    }
}
#endif