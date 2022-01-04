#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "StoriesDB", menuName = "Stories/Create Stories Database", order = 2)]
public class StoriesDBSO : ScriptableObject
{
    public StoriesCategorySO[] storyCategories;

    private StoriesDB storiesDB;

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
        SerializationManager.SaveAsTextFile(Application.dataPath, DataPaths.storyDatabaseFileNameTest, saveString);

        AssetDatabase.Refresh();
    }

    public void DeleteDBJson()
    {
        SerializationManager.DeleteIfFileExists(Application.dataPath + "/" + DataPaths.storyDatabaseFileNameTest);

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