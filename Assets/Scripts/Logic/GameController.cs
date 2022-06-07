using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Fungus;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Flags")]
    [SerializeField] private bool devMode = false;

    public bool DevMode
    {
        get { return devMode; }
    }

    [Header("Stories Database Key")]
    public string storiesDBKey;     //This will download the json file

    private StoriesDB storiesDB;
    private StoriesDBItem storiesDBItemLoaded;   

    public static event Action<StoriesDB> OnStoryDBLoaded;

    private UIStoriesDetailsPanel detailsPanel;

    /*private void OnApplicationQuit()
    {
        //TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        //string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        string timeText = TimeUtils.GetTimeInFormat(Time.realtimeSinceStartup);

#if UNITY_EDITOR
        print(timeText);
#endif

        if (SDKManager.instance != null)
        {
            SDKEventStringData timeOnAppData;
            timeOnAppData.eventParameterName = SDKEventsNames.timeOnAppParameterName;
            timeOnAppData.eventParameterData = timeText;
            SDKManager.instance.SendEvent(SDKEventsNames.timeOnAppEventName, timeOnAppData);
        }
    }*/

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        detailsPanel = FindObjectOfType<UIStoriesDetailsPanel>();
        detailsPanel?.SetStoryBufferingImageStatus(true);        

        AsyncOperationHandle<TextAsset> storiesDBHandle = Addressables.LoadAssetAsync<TextAsset>(storiesDBKey);
        storiesDBHandle.Completed += OnStoriesDBLoaded;        
    }

    private void OnStoriesDBLoaded(AsyncOperationHandle<TextAsset> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:

                break;

            case AsyncOperationStatus.Succeeded:
                string databaseString = obj.Result.text;
                storiesDB = JsonUtility.FromJson<StoriesDB>(databaseString);

                if (storiesDB != null)
                {                    
                    OnStoryDBLoaded?.Invoke(storiesDB);
                    Debug.Log("GAMECONTROLLER: Stories Database successfully loaded!");

                    detailsPanel?.SetStoryBufferingImageStatus(false);
                }
                break;

            case AsyncOperationStatus.Failed:
                detailsPanel?.SetStoryBufferingImageStatus(false);
                Debug.LogError("GAMECONTROLLER: Failed to load Stories Database. Please make sure that JSON file exists and it is set as Addressables and it is stored in Cloud Server (CDN).");
                break;
        }
    }

    public void SaveStoryData(StoriesDBItem _storiesDBItem, Flowchart _loadedFlowchart, Action _storyDataLoadedCallback)
    {
        if (_storiesDBItem == null)
            return;

        storiesDBItemLoaded = _storiesDBItem;

        SaveStoryDataProgress(_loadedFlowchart);

        _storyDataLoadedCallback?.Invoke();
    }

    /*private IEnumerator SaveStoryDataRoutine(Action storyDataLoadedCallback)
    {
        AsyncOperationHandle<GameObject> flowchartHandle = Addressables.LoadAssetAsync<GameObject>(storiesDBItemLoaded.storyFlowchartKey);
        //flowchartHandle.Completed += OnFlowchartLoaded;

        yield return flowchartHandle;

        switch (flowchartHandle.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:

                GameObject flowchartGameobject = flowchartHandle.Result;
                storyFlowchartLoaded = flowchartGameobject.GetComponent<Flowchart>();

                if (storyFlowchartLoaded != null)
                {
                    SaveStoryDataProgress();
                    storyDataLoadedCallback?.Invoke();
                }
                else
                {
                    FindObjectOfType<UIStoriesDetailsPanel>().SetStoryBufferingImageStatus(false);
                    Debug.LogError("Unable To Loaded Flowchart in order to save StoryData!");
                }

                break;

            case AsyncOperationStatus.Failed:
                FindObjectOfType<UIStoriesDetailsPanel>().SetStoryBufferingImageStatus(false);
                Debug.LogError("Unable To Loaded Flowchart in order to save StoryData!");
                break;
        }        
    }*/

    /*private void OnFlowchartLoaded(AsyncOperationHandle<GameObject> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:

                GameObject flowchartGameobject = obj.Result;
                storyFlowchart = flowchartGameobject.GetComponent<Flowchart>();
                
                break;

            case AsyncOperationStatus.Failed:
                break;
        }
    }*/

    private void SaveStoryDataProgress(Flowchart _loadedFlowchart)
    {
        StoryData storyData = new StoryData();

        if (storiesDBItemLoaded.storyEpisodesKeys.Length > 0)
        {
            storyData.currentEpisodeIndex = 0;
            storyData.currentEpisodeKey = storiesDBItemLoaded.storyEpisodesKeys[0];

            /*if (storyFlowchart.HasVariable("GrandTotalBlocks"))
                storyData.blocksGrandTotal = storyFlowchart.GetIntegerVariable("GrandTotalBlocks");*/

            storyData.blocksGrandTotal = storiesDBItemLoaded.storyTotalBlocksCount;

            for (int i = 0; i < storiesDBItemLoaded.storyEpisodesKeys.Length; i++)
            {
                EpisodeData episodeData = new EpisodeData();
                episodeData.episodeAssetKey = storiesDBItemLoaded.storyEpisodesKeys[i];
                episodeData.isFinished = false;

                if (devMode)
                    episodeData.isUnlocked = true;
                else
                    episodeData.isUnlocked = (i == 0) ? true : false;
                
                episodeData.currentBlockID = 0;
                episodeData.currentCommandID = 0;
                episodeData.lastCameraPosition = Vector3.zero;
                episodeData.lastCameraRotation = Vector3.zero;
                episodeData.lastCameraOrthosize = 21f;

                List<Variable> flowchartVariables = _loadedFlowchart.Variables;
                if(flowchartVariables.Count > 0)
                {
                    for (int j = 0; j < flowchartVariables.Count; j++)
                    {
                        EpisodeVariablesData variableData = new EpisodeVariablesData();
                        variableData.variableKey = flowchartVariables[j].Key;
                        variableData.variableType = flowchartVariables[j].GetType().Name;

                        switch (flowchartVariables[j].GetType().Name)
                        {
                            case "BooleanVariable":
                                variableData.variableValue = _loadedFlowchart.GetBooleanVariable(flowchartVariables[j].Key).ToString();
                                break;

                            case "IntegerVariable":
                                variableData.variableValue = _loadedFlowchart.GetIntegerVariable(flowchartVariables[j].Key).ToString();
                                break;

                            case "FloatVariable":
                                variableData.variableValue = _loadedFlowchart.GetFloatVariable(flowchartVariables[j].Key).ToString();
                                break;

                            case "StringVariable":
                                variableData.variableValue = _loadedFlowchart.GetStringVariable(flowchartVariables[j].Key);
                                break;
                        }

                        episodeData.variableDatas.Add(variableData);
                    }
                }

                storyData.episodeDataList.Add(episodeData);
            }
        }

        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(storiesDBItemLoaded.storyProgressFileName, saveString);        

        //storyFlowchartLoaded = null;
        storiesDBItemLoaded = null;
    }
    
    public StoryData GetStoryDataFromFilename(string fileName)
    {
        StoryData storyData = null;

        string fullPath = DataPaths.loadProgressPath + fileName + DataPaths.loadProgressFileExtension;
        if(SerializationManager.FileExists(fullPath))
        {
            string saveString = SerializationManager.LoadFromTextFile(fullPath);
            if (saveString != null)
                storyData = JsonUtility.FromJson<StoryData>(saveString);
        }

        return storyData;
    }    

    public StoryData SaveAndLoadUpdatedStoryData(StoriesDBItem _storyDBItem, StoryData _previousStoryData, Flowchart _loadedFlowchart)
    {
        StoryData storyData = _previousStoryData;

        if (_storyDBItem.storyEpisodesKeys.Length > 0)
        {
            //Check for updated Total Blocks Count
            if(storyData.blocksGrandTotal != _storyDBItem.storyTotalBlocksCount)
                storyData.blocksGrandTotal = _storyDBItem.storyTotalBlocksCount;

            //Check if storyData Episodes List count is equal to story DB item Length. If not then save the new EpisodeData(s)
            if (storyData.episodeDataList.Count != _storyDBItem.storyEpisodesKeys.Length)
            {
                #region 1 - Search for Old VariableDatas If exists any...
                List<EpisodeVariablesData> oldVariablesData = null;
                for (int i = 0; i < storyData.episodeDataList.Count; i++)
                {
                    if (storyData.HasEpisodeDataKey(storyData.episodeDataList[i].episodeAssetKey))
                    {
                        if (storyData.episodeDataList[i].variableDatas != null)
                        {
                            if (storyData.episodeDataList[i].variableDatas.Count > 0)
                            {
                                oldVariablesData = new List<EpisodeVariablesData>(storyData.episodeDataList[i].variableDatas);
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 2 - Save Updated EpisodeData in Story Data
                for (int i = 0; i < _storyDBItem.storyEpisodesKeys.Length; i++)
                {
                    //New Episode Data was not found in StoryData so save it now.
                    if (!storyData.HasEpisodeDataKey(_storyDBItem.storyEpisodesKeys[i]))
                    {
                        EpisodeData episodeData = new EpisodeData();
                        episodeData.episodeAssetKey = _storyDBItem.storyEpisodesKeys[i];
                        episodeData.isFinished = false;
                        episodeData.isUnlocked = (i == 0) ? true : false;
                        episodeData.currentBlockID = 0;
                        episodeData.currentCommandID = 0;
                        episodeData.lastCameraPosition = Vector3.zero;
                        episodeData.lastCameraRotation = Vector3.zero;
                        episodeData.lastCameraOrthosize = 21f;

                        if (oldVariablesData != null && oldVariablesData.Count > 0) //If Old VariableDatas already exists then copy it to new Episode Data
                        {
#if UNITY_EDITOR
                            print(_storyDBItem.storyTitle + ": Old Variables Data found! Saving...");
#endif

                            episodeData.variableDatas.Clear();
                            episodeData.variableDatas.AddRange(oldVariablesData);
                        }
                        else if (_loadedFlowchart != null)   //Else Save it from Flowchart Variables
                        {
#if UNITY_EDITOR
                            print(_storyDBItem.storyTitle + ": Old Variables Data not found! Trying to save from Flowchart...");
#endif

                            List<Variable> flowchartVariables = _loadedFlowchart.Variables;
                            if (flowchartVariables.Count > 0)
                            {
                                for (int j = 0; j < flowchartVariables.Count; j++)
                                {
                                    EpisodeVariablesData variableData = new EpisodeVariablesData();
                                    variableData.variableKey = flowchartVariables[j].Key;
                                    variableData.variableType = flowchartVariables[j].GetType().Name;

                                    switch (flowchartVariables[j].GetType().Name)
                                    {
                                        case "BooleanVariable":
                                            variableData.variableValue = _loadedFlowchart.GetBooleanVariable(flowchartVariables[j].Key).ToString();
                                            break;

                                        case "IntegerVariable":
                                            variableData.variableValue = _loadedFlowchart.GetIntegerVariable(flowchartVariables[j].Key).ToString();
                                            break;

                                        case "FloatVariable":
                                            variableData.variableValue = _loadedFlowchart.GetFloatVariable(flowchartVariables[j].Key).ToString();
                                            break;

                                        case "StringVariable":
                                            variableData.variableValue = _loadedFlowchart.GetStringVariable(flowchartVariables[j].Key);
                                            break;
                                    }

                                    episodeData.variableDatas.Add(variableData);
                                }
                            }
                        }

                        storyData.episodeDataList.Insert(i, episodeData);
                    }
                    else if (storyData.HasEpisodeDataKey(_storyDBItem.storyEpisodesKeys[i]))    //If Episode Data has Key, then Check if VariablesData are empty or not. If Yes, then save it from loadedFlowchart
                    {
                        EpisodeData episodeData = storyData.GetEpisodeDataFromIndex(i);
                        if (episodeData.variableDatas != null && _loadedFlowchart != null)
                        {
                            if (episodeData.variableDatas.Count <= 0)
                            {
#if UNITY_EDITOR
                                print(_storyDBItem.storyTitle + ": Empty Variable Datas found in existing EpisodeData! Saving from Flowchart...");
#endif

                                List<Variable> flowchartVariables = _loadedFlowchart.Variables;
                                if (flowchartVariables.Count > 0)
                                {
                                    for (int j = 0; j < flowchartVariables.Count; j++)
                                    {
                                        EpisodeVariablesData variableData = new EpisodeVariablesData();
                                        variableData.variableKey = flowchartVariables[j].Key;
                                        variableData.variableType = flowchartVariables[j].GetType().Name;

                                        switch (flowchartVariables[j].GetType().Name)
                                        {
                                            case "BooleanVariable":
                                                variableData.variableValue = _loadedFlowchart.GetBooleanVariable(flowchartVariables[j].Key).ToString();
                                                break;

                                            case "IntegerVariable":
                                                variableData.variableValue = _loadedFlowchart.GetIntegerVariable(flowchartVariables[j].Key).ToString();
                                                break;

                                            case "FloatVariable":
                                                variableData.variableValue = _loadedFlowchart.GetFloatVariable(flowchartVariables[j].Key).ToString();
                                                break;

                                            case "StringVariable":
                                                variableData.variableValue = _loadedFlowchart.GetStringVariable(flowchartVariables[j].Key);
                                                break;
                                        }

                                        episodeData.variableDatas.Add(variableData);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 3 - Lastly Check for Last Finished Episode and unlock 1st newly added episode
                for (int i = 0; i < storyData.episodeDataList.Count; i++)
                {
                    if(storyData.episodeDataList[i].isUnlocked && storyData.episodeDataList[i].isFinished)
                    {
                        if (i + 1 < storyData.episodeDataList.Count)
                        {
                            if (!storyData.episodeDataList[i + 1].isUnlocked && !storyData.episodeDataList[i + 1].isFinished)
                            {
                                storyData.episodeDataList[i + 1].isFinished = false;
                                storyData.episodeDataList[i + 1].isUnlocked = true;

                                break;
                            }
                        }
                    }
                }
                #endregion
            }
        }

        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(_storyDBItem.storyProgressFileName, saveString);

        return storyData;
    }    
}