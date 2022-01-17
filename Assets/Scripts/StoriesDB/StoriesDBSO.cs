#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "StoriesDB", menuName = "Stories/Create Stories Database", order = 2)]
public class StoriesDBSO : ScriptableObject
{
    public StoriesCategorySO[] storyCategories;

    private StoriesDB storiesDB;

    private void OnValidate()
    {
        for (int i = 0; i < storyCategories.Length && (storyCategories.Length > 0); i++)
        {
            if (storyCategories[i] != null)
                storyCategories[i].categoryIndex = i;
        }
    }

    public void SaveDBToJson()
    {
        if(storyCategories.Length > 0)
        {
            storiesDB = new StoriesDB();
            Array.Resize(ref storiesDB.storiesCategories, storyCategories.Length);
            for (int i = 0; i < storyCategories.Length; i++)
            {
                if(storyCategories[i] != null)
                {
                    StoriesCategory category = storyCategories[i].GetStoriesCategory();
                    storiesDB.storiesCategories[i] = category;
                }
            }
        }

        string saveString = JsonUtility.ToJson(storiesDB, true);
        SerializationManager.SaveAsTextFile(Application.dataPath, DataPaths.storyDatabaseFileName, saveString);

        AssetDatabase.Refresh();
    }

    public void DeleteDBJson()
    {
        SerializationManager.DeleteIfFileExists(Application.dataPath + "/" + DataPaths.storyDatabaseFileName);

        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(StoriesDBSO))]
public class StoriesDBSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StoriesDBSO storiesDBSO = target as StoriesDBSO;

        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Save To Json File"))
            storiesDBSO.SaveDBToJson();

        if (GUILayout.Button("Delete Json File"))
            storiesDBSO.DeleteDBJson();
    }
}
#endif