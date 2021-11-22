using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Fungus;

#if UNITY_EDITOR
using UnityEditor;
#endif

//For Testing Episodes purpose only
public class SaveStoryData : MonoBehaviour
{
    public Flowchart anyEpisodeFlowchart;
    public AssetReference[] assetReferences;

    [Space(15f)]

    public string storyFileName;
    public StoryData storyData;

    private void OnValidate()
    {
        if (assetReferences.Length > 0)
        {
            storyFileName = "";
            for (int i = 0; i < assetReferences.Length; i++)
            {
                if (assetReferences[i] != null)
                    storyFileName += assetReferences[i].RuntimeKey.ToString();
            }
        }
        else
            storyFileName = "";
    }

    public void SaveStoryEpisodesData()
    {
        storyData = new StoryData();

        if (assetReferences.Length > 0)
        {
            storyData.currentEpisodeKey = assetReferences[0].RuntimeKey.ToString();

            for (int i = 0; i < assetReferences.Length; i++)
            {
                EpisodeData episodeData = new EpisodeData();
                episodeData.episodeAssetKey = assetReferences[i].RuntimeKey.ToString();
                episodeData.isFinished = false;
                episodeData.isUnlocked = (i == 0) ? true : false;
                episodeData.currentBlockID = 0;
                episodeData.currentCommandID = 0;
                episodeData.lastCameraPosition = Vector3.zero;
                episodeData.lastCameraRotation = Vector3.zero;
                episodeData.lastCameraOrthosize = 20f;

                List<Variable> flowchartVariables = anyEpisodeFlowchart.Variables;
                for (int j = 0; j < flowchartVariables.Count; j++)
                {
                    EpisodeVariablesData variableData = new EpisodeVariablesData();
                    variableData.variableKey = flowchartVariables[j].Key;
                    variableData.variableType = flowchartVariables[j].GetType().Name;

                    switch (flowchartVariables[j].GetType().Name)
                    {
                        case "BooleanVariable":
                            variableData.variableValue = anyEpisodeFlowchart.GetBooleanVariable(flowchartVariables[j].Key).ToString();
                            break;

                        case "IntegerVariable":
                            variableData.variableValue = anyEpisodeFlowchart.GetIntegerVariable(flowchartVariables[j].Key).ToString();
                            break;

                        case "FloatVariable":
                            variableData.variableValue = anyEpisodeFlowchart.GetFloatVariable(flowchartVariables[j].Key).ToString();
                            break;

                        case "StringVariable":
                            variableData.variableValue = anyEpisodeFlowchart.GetStringVariable(flowchartVariables[j].Key);
                            break;
                    }

                    //print(flowchartVariables[i].Key + "(" + flowchartVariables[i].GetType().Name + ")" + ": " + flowchartVariables[i].GetValue().ToString());
                    //print(variableData.variableKey + "(" + variableData.variableType + ")" + ": " + variableData.variableValue);

                    episodeData.variableDatas.Add(variableData);
                }

                storyData.episodeDataList.Add(episodeData);
            }
        }

        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(storyFileName, saveString);
    }

    public void DeleteStoryEpisodesData()
    {
        if (storyFileName == "")
            return;

        SerializationManager.DeleteIfFileExists(DataPaths.loadProgressPath + storyFileName + DataPaths.loadProgressFileExtension);
    }    
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveStoryData))]
public class SaveStoryDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveStoryData saveStoryData = target as SaveStoryData;

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Save Stories Episodes Data"))        
            saveStoryData.SaveStoryEpisodesData();

        if (GUILayout.Button("Delete Save File"))
            saveStoryData.DeleteStoryEpisodesData();
    }
}
#endif