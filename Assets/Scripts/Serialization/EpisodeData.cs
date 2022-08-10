using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EpisodeData
{
    private static EpisodeData m_Instance;
    public static EpisodeData instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new EpisodeData();
            }
            return m_Instance;
        }
        set
        {
            if (value != null)
            {
                m_Instance = value;
            }
        }
    }

    public string episodeAssetKey;

    public bool isFinished;
    public bool isUnlocked = false;
#if UNITY_EDITOR
    public bool allowClothesChange;
#endif

    public int currentBlocksTotal;
    public List<int> currentBlocksVisited = new List<int>();

    public int currentBlockID;
    public int currentCommandID;

    public bool allowMusic = true;
    public int currentMusicIndex;

    public Vector3 lastCameraPosition;
    public Vector3 lastCameraRotation;
    public float lastCameraOrthosize;

    public List<EpisodeVariablesData> variableDatas = new List<EpisodeVariablesData>();

    public float episodeTime;
}

[System.Serializable]
public class EpisodeVariablesData
{
    public string variableKey;
    public string variableValue;
    public string variableType;
}