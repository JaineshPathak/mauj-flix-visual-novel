using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class StoryData
{    
    public bool hasStarted;
    public int blocksGrandDone;
    public int blocksGrandTotal;
    public int currentEpisodeIndex = 0;
    public string currentEpisodeKey;
    public List<EpisodeData> episodeDataList = new List<EpisodeData>();

    public EpisodeData GetEpisodeDataFromIndex(int index)
    {
        return episodeDataList[index];
    }

    public bool HasEpisodeDataKey(string key)
    {
        if (episodeDataList.Count <= 0)
            return false;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            if (episodeDataList[i].episodeAssetKey == key)
                return true;
        }

        return false;
    }

    public int GetIndexFromEpisodeData(EpisodeData episodeData)
    {
        if (episodeDataList.Count <= 0)
            return -1;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            if (episodeDataList[i] == episodeData)
                return i;
        }

        return -1;
    }

    public int GetIndexFromEpisodeKey(string key)
    {
        if (episodeDataList.Count <= 0)
            return -1;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            if (episodeDataList[i].episodeAssetKey == key)
                return i;
        }

        return -1;
    }

    public string GetEpisodeKeyFromIndex(int index)
    {
        return episodeDataList[index].episodeAssetKey;
    }
    
    public EpisodeData GetEpisodeDataFromKey(string key)
    {
        if (episodeDataList.Count <= 0)
            return null;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            if (episodeDataList[i].episodeAssetKey == key)
                return episodeDataList[i];
        }

        return null;
    }

    public float GetTotalEpisodeTime()
    {
        float sum = 0;

        if (episodeDataList.Count <= 0)
            return sum;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            sum += episodeDataList[i].episodeTime;
        }

        return sum;
    }

    //Useless method - Don't use this
    /*
    public void SaveEpisodeDataFromData(EpisodeData episodeData, UnityAction callBack)
    {
        if (episodeDataList.Count <= 0)
            return;

        for (int i = 0; i < episodeDataList.Count; i++)
        {
            if(episodeDataList[i].episodeAssetKey == episodeData.episodeAssetKey)
            {
                EpisodeData lastEpisodeData = episodeDataList[i];
                lastEpisodeData.episodeAssetKey = episodeData.episodeAssetKey;
                lastEpisodeData.isFinished = episodeData.isFinished;
                lastEpisodeData.isUnlocked = episodeData.isUnlocked;
                lastEpisodeData.currentBlockID = episodeData.currentBlockID;
                lastEpisodeData.currentCommandID = episodeData.currentCommandID;
                lastEpisodeData.lastCameraPosition = episodeData.lastCameraPosition;
                lastEpisodeData.lastCameraRotation = episodeData.lastCameraRotation;
                lastEpisodeData.lastCameraOrthosize = episodeData.lastCameraOrthosize;

                lastEpisodeData.variableDatas.Clear();
                for (int k = 0; k < episodeData.variableDatas.Count; k++)
                {
                    EpisodeVariablesData episodeVariablesData = new EpisodeVariablesData();
                    episodeVariablesData.variableKey = episodeData.variableDatas[k].variableKey;
                    episodeVariablesData.variableValue = episodeData.variableDatas[k].variableValue;
                    episodeVariablesData.variableType = episodeData.variableDatas[k].variableType;
                    lastEpisodeData.variableDatas.Add(episodeVariablesData);
                }                

                callBack.Invoke();
                break;
            }
        }
    }
    */
}