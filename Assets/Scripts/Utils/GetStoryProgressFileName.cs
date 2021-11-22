using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GetStoryProgressFileName : MonoBehaviour
{
    public AssetReference[] episodesReferences;

    [Space(15f)]

    public string progressFileName;

    public void GetProgressFileName()
    {
        if (episodesReferences.Length <= 0)
            return;

        progressFileName = GetFileName();
    }

    private string GetFileName()
    {
        string finalName = null;

        for (int i = 0; i < episodesReferences.Length; i++)
        {
            if (episodesReferences[i] != null)
            {
                char[] episodeKeyArr = episodesReferences[i].RuntimeKey.ToString().ToCharArray();
                if (episodeKeyArr.Length > 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        finalName = finalName + episodeKeyArr[j];
                    }
                }
            }
        }

        return finalName;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GetStoryProgressFileName))]
public class GetStoryProgressFileNameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GetStoryProgressFileName storyProgressFileName = target as GetStoryProgressFileName;

        EditorGUILayout.Space(5f);

        if (GUILayout.Button("Get Progress File Name"))
        {
            storyProgressFileName.GetProgressFileName();
        }
    }
}
#endif