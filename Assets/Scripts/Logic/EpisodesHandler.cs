using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System;
using underDOGS.SDKEvents;

public class EpisodesHandler : MonoBehaviour
{
    [Header("Fungus")]
    public Flowchart episodeFlowchart;
    public string episodeGameStartBlock = "01 - Game Start";

    [Header("Characters")]
    public CharacterSelectionScreen[] characterSelectionScreens;

    [Header("Music")]
    public AudioClip[] musicsList;

    [Header("UI End Screen")]
    public UIEpisodeEndPanel endPanel;

    [Header("Debug")]
    public List<Block> executingBlocks = new List<Block>();
    public List<string> executingBlocksName = new List<string>();
    public List<int> executingBlocksItemIDs = new List<int>();

    private EpisodesSpawner episodesSpawner;
    private Camera mainCamera;

    private StoryData storyData;
    private EpisodeData episodeData;

    private MusicManager musicManager;    

    private void OnEnable()
    {
        if(episodeFlowchart != null)        
            BlockSignals.OnBlockStart += OnBlockStarted;

        UIEpisodeEndPanel.OnNextEpisodePanelOpened += OnNextEpisodePanelOpened;
    }

    private void OnDisable()
    {
        if (episodeFlowchart != null)        
            BlockSignals.OnBlockStart -= OnBlockStarted;

        UIEpisodeEndPanel.OnNextEpisodePanelOpened -= OnNextEpisodePanelOpened;
    }

    private void OnDestroy()
    {
        if (episodeFlowchart != null)        
            BlockSignals.OnBlockStart -= OnBlockStarted;

        UIEpisodeEndPanel.OnNextEpisodePanelOpened -= OnNextEpisodePanelOpened;
    }    

    private void OnBlockStarted(Block block)
    {
        if (storyData == null || episodeData == null)
            return;

        if (!episodeData.currentBlocksVisited.Contains(block.ItemId) && !episodeData.isFinished)
            episodeData.currentBlocksVisited.Add(block.ItemId);

        episodeData.currentBlocksTotal = episodeData.currentBlocksVisited.Count;
    }

    private void OnNextEpisodePanelOpened()
    {
        if (episodesSpawner == null)
            return;

        if (EpisodesSpawner.instance == null)
            return;

        if (episodesSpawner.playerData != null)
        {
            if (!episodesSpawner.playerData.hasRatedGame)
            {
                int episodeIndex = storyData.GetIndexFromEpisodeData(episodeData);
                episodeIndex++;

#if UNITY_EDITOR
                Debug.Log("EPISODE INDEX: " +episodeIndex);
#endif

                if(episodeIndex % 2 == 0)
                    episodesSpawner.OpenRateUsWindow();
            }

            if (!episodesSpawner.playerData.HasStoryLiked(episodesSpawner.storyTitleEnglish))
                episodesSpawner.OpenLikeStoryWindow();
        }
    }

    public void Init(EpisodesSpawner spawner)
    {
        if (endPanel == null)
            endPanel = GetComponentInChildren<UIEpisodeEndPanel>();

        if (endPanel != null)
            endPanel.episodesHandler = this;

        episodesSpawner = spawner;

        mainCamera = Camera.main;

        if (FungusManager.Instance != null)
            musicManager = FungusManager.Instance.MusicManager;

        if (episodesSpawner.saveOrLoadData)
        {
            storyData = episodesSpawner.storyData;
            episodeData = storyData.GetEpisodeDataFromIndex(storyData.currentEpisodeIndex);
            LoadFromEpisodeData();

            /*
            string loadSettingsPath = DataPaths.loadProgressPath + episodesSpawner.currentEpisodeReferenceKey + DataPaths.loadProgressFileExtension;
            
            #region LOAD FROM TEXT FILE
            //If Progress File exists then load it
            if (SerializationManager.FileExists(loadSettingsPath))
            {
                LoadEpisodeDataProgress(loadSettingsPath);
            }
            //Else Create a new progress file
            else
            {
                SaveEpisodeProgress(true);
                episodeFlowchart.ExecuteBlock(episodeGameStartBlock);
            }
            #endregion
            */
        }
    }

    private void LoadFromEpisodeData()
    {
        if (episodeData == null)
            return;

        for (int i = 0; i < episodeData.variableDatas.Count; i++)
        {
            //print(EpisodeData.instance.variableDatas[i].variableKey + ": " + EpisodeData.instance.variableDatas[i].variableValue);                        

            //Set Flowchart Data
            if (episodeFlowchart.HasVariable(episodeData.variableDatas[i].variableKey))
            {
                switch (episodeData.variableDatas[i].variableType)
                {
                    case "BooleanVariable":
                        episodeFlowchart.SetBooleanVariable(episodeData.variableDatas[i].variableKey, bool.Parse(episodeData.variableDatas[i].variableValue));
                        break;

                    case "IntegerVariable":
                        episodeFlowchart.SetIntegerVariable(episodeData.variableDatas[i].variableKey, int.Parse(episodeData.variableDatas[i].variableValue));
                        break;

                    case "FloatVariable":
                        episodeFlowchart.SetFloatVariable(episodeData.variableDatas[i].variableKey, float.Parse(episodeData.variableDatas[i].variableValue));
                        break;

                    case "StringVariable":
                        episodeFlowchart.SetStringVariable(episodeData.variableDatas[i].variableKey, episodeData.variableDatas[i].variableValue);
                        break;
                }
            }            
        }

        //Adjust the characters
        if (characterSelectionScreens.Length > 0)
        {
            for (int i = 0; i < characterSelectionScreens.Length; i++)
            {
                if (characterSelectionScreens[i] != null)
                    characterSelectionScreens[i].SelectCharacterFromLoadedData();
            }
        }

        //Set Camera Position, Rotation and OrthographicSize from loaded data
        mainCamera.transform.position = episodeData.lastCameraPosition;
        mainCamera.transform.eulerAngles = episodeData.lastCameraRotation;
        mainCamera.orthographicSize = episodeData.lastCameraOrthosize;        

        //Get Block from BlockID and Execute that Block from Command index
        Block blockToExecute = null;
        if (episodeData.currentBlockID == -1)       //If this is -1, then load Block right from the start
        {
            blockToExecute = episodeFlowchart.FindBlockFromItemId(0);
            episodeFlowchart.ExecuteBlock(blockToExecute, 0);
        }
        else        //Else Get the block from BlockID and execute that block
        {
            blockToExecute = episodeFlowchart.FindBlockFromItemId(episodeData.currentBlockID);
            episodeFlowchart.ExecuteBlock(blockToExecute, episodeData.currentCommandID);
        }
        
        StartCoroutine("UpdateRoutine");

        //Play Last saved music index
        if (episodeData.allowMusic)
        {
            if (musicsList.Length > 0)
                if (musicsList[episodeData.currentMusicIndex] != null)
                    musicManager.PlayMusic(musicsList[episodeData.currentMusicIndex], true, 1f, 0);
        }
    }

    private void SaveEpisodeData()
    {
        if (episodeData == null)
            return;

        episodeData.isFinished = false;

        if (!storyData.hasStarted)
            storyData.hasStarted = true;

        episodeData.currentBlocksTotal = episodeData.currentBlocksVisited.Count;

        storyData.blocksGrandDone = 0;
        for (int i = 0; i < storyData.episodeDataList.Count; i++)
        {
            if(storyData.episodeDataList[i] != null)
            {
                storyData.blocksGrandDone += storyData.episodeDataList[i].currentBlocksTotal;
            }
        }

        episodeData.variableDatas.Clear();

        List<Variable> flowchartVariables = episodeFlowchart.Variables;
        for (int i = 0; i < flowchartVariables.Count; i++)
        {
            EpisodeVariablesData variableData = new EpisodeVariablesData();
            variableData.variableKey = flowchartVariables[i].Key;
            variableData.variableType = flowchartVariables[i].GetType().Name;

            switch (flowchartVariables[i].GetType().Name)
            {
                case "BooleanVariable":
                    variableData.variableValue = episodeFlowchart.GetBooleanVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "IntegerVariable":
                    variableData.variableValue = episodeFlowchart.GetIntegerVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "FloatVariable":
                    variableData.variableValue = episodeFlowchart.GetFloatVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "StringVariable":
                    variableData.variableValue = episodeFlowchart.GetStringVariable(flowchartVariables[i].Key);
                    break;
            }

            episodeData.variableDatas.Add(variableData);            
        }        

        //Get List of Executing Blocks
        executingBlocks.Clear();
        executingBlocks = episodeFlowchart.GetExecutingBlocks();

        executingBlocksName.Clear();
        executingBlocksItemIDs.Clear();
        for (int i = 0; i < executingBlocks.Count; i++)
        {
            executingBlocksName.Add(executingBlocks[i].BlockName);
            executingBlocksItemIDs.Add(executingBlocks[i].ItemId);
        }

        //Get BlockID from last Index
        episodeData.currentBlockID = executingBlocksItemIDs[executingBlocksItemIDs.Count - 1];

        //Get Block from BlockID and save its ActiveCommand index
        Block blockTemp = episodeFlowchart.FindBlockFromItemId(episodeData.currentBlockID);
        episodeData.currentCommandID = blockTemp.ActiveCommand.CommandIndex;

        //Save Camera's last position, rotation and Orthographic size
        episodeData.lastCameraPosition = mainCamera.transform.position;
        episodeData.lastCameraRotation = mainCamera.transform.eulerAngles;
        episodeData.lastCameraOrthosize = mainCamera.orthographicSize;

        SaveStoryData();

        //storyData.SaveEpisodeDataFromData(episodeData, OnEpisodeDataSaved);
    }

    private void SaveEpisodeDataFinishBranched()
    {
        StopCoroutine("UpdateRoutine");

        if (episodeData == null)
            return;

        //Send a "[StoryTitle]_episodeN_branchending_completed" event
        if(SDKManager.instance != null)
        {
            string storyTitleProper = episodesSpawner.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData branchEventData;
            branchEventData.eventParameterName = storyTitleProper;
            branchEventData.eventParameterData = "episode" + (storyData.GetIndexFromEpisodeData(episodeData) + 1).ToString();

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventName, branchEventData);
        }

        EpisodeData episodeFour = storyData.GetEpisodeDataFromIndex(3);

        storyData.currentEpisodeIndex = 3;
        storyData.currentEpisodeKey = episodeFour.episodeAssetKey;

        episodeData.currentBlockID = 0;
        episodeData.currentCommandID = 0;
        episodeData.currentBlocksTotal = 0;
        episodeData.currentBlocksVisited.Clear();
        episodeData.lastCameraPosition = Vector3.zero;
        episodeData.lastCameraRotation = Vector3.zero;
        episodeData.lastCameraOrthosize = 21f;

        SaveStoryData();
    }

    private void SaveEpisodeDataFinish()
    {
        StopCoroutine("UpdateRoutine");

        if (episodeData == null)
            return;

        //Send "episode_completed" Event
        if(SDKManager.instance != null)
        {
            string storyTitleProper2 = episodesSpawner.storyTitleEnglish;
            storyTitleProper2 = storyTitleProper2.Replace(" ", "_");

            SDKEventEpisodeData eventEpisodeData;
            eventEpisodeData.storyTitle = storyTitleProper2;
            eventEpisodeData.episodeNum = storyData.GetIndexFromEpisodeData(episodeData) + 1;
            
            SDKManager.instance.SendEvent(eventEpisodeData);
        }

        //Episode was finished. Save the data and copy this Data to next Episode Data if any
        episodeData.isFinished = true;
        episodeData.isUnlocked = true;

        episodeData.currentBlocksTotal = episodeData.currentBlocksVisited.Count;        

        storyData.blocksGrandDone = 0;
        for (int i = 0; i < storyData.episodeDataList.Count; i++)
        {
            if (storyData.episodeDataList[i] != null)
            {
                storyData.blocksGrandDone += storyData.episodeDataList[i].currentBlocksTotal;
            }
        }

        episodeData.currentBlockID = -1;
        episodeData.currentCommandID = -1;
        episodeData.lastCameraPosition = Vector3.zero;
        episodeData.lastCameraRotation = Vector3.zero;
        episodeData.lastCameraOrthosize = 21f;
        
        //Check if this is last episode. If not then copy data to next Episode Data
        if (storyData.currentEpisodeIndex < storyData.episodeDataList.Count - 1)
        {
            if (!endPanel.nextEpisodeBarTimer.gameObject.activeSelf)
                endPanel.nextEpisodeBarTimer.gameObject.SetActive(true);

            storyData.currentEpisodeIndex++;
            storyData.currentEpisodeKey = storyData.GetEpisodeKeyFromIndex(storyData.currentEpisodeIndex);

            EpisodeData nextEpisodeData = storyData.GetEpisodeDataFromIndex(storyData.currentEpisodeIndex);
            nextEpisodeData.isFinished = false;
            nextEpisodeData.isUnlocked = true;
            nextEpisodeData.currentBlockID = 0;
            nextEpisodeData.currentCommandID = 0;
            nextEpisodeData.lastCameraPosition = Vector3.zero;
            nextEpisodeData.lastCameraRotation = Vector3.zero;
            nextEpisodeData.lastCameraOrthosize = 21f;

            for (int i = 0; i < episodeData.variableDatas.Count; i++)
            {
                nextEpisodeData.variableDatas[i].variableKey = episodeData.variableDatas[i].variableKey;
                nextEpisodeData.variableDatas[i].variableValue = episodeData.variableDatas[i].variableValue;
                nextEpisodeData.variableDatas[i].variableType = episodeData.variableDatas[i].variableType;
            }   
        }
        else //All Episodes are completed
        {
            if(endPanel.nextEpisodeBarTimer.gameObject.activeSelf)
                endPanel.nextEpisodeBarTimer.gameObject.SetActive(false);

            //For UIPersonalProfile Section
            if (episodesSpawner.playerData.ContainsStoryStarted(episodesSpawner.storiesDBItem.storyTitleEnglish) && 
                !episodesSpawner.playerData.ContainsStoryCompleted(episodesSpawner.storiesDBItem.storyTitleEnglish))
            {
                episodesSpawner.playerData.RemoveStoryStarted(episodesSpawner.storiesDBItem.storyTitleEnglish);
                episodesSpawner.playerData.AddStoryCompleted(episodesSpawner.storiesDBItem.storyTitleEnglish);

                SaveLoadGame.SavePlayerData(episodesSpawner.playerData);
            }

            if (SDKManager.instance != null)
            {
                //Send "story_completed" Event
                string storyTitleProper = episodesSpawner.storyTitleEnglish;
                storyTitleProper = storyTitleProper.Replace(" ", "_");

                SDKEventStringData eventStoryCompleteData;
                eventStoryCompleteData.eventParameterName = SDKEventsNames.storyParameterName;
                eventStoryCompleteData.eventParameterData = storyTitleProper;
                SDKManager.instance.SendEvent(SDKEventsNames.storyCompleteEventName, eventStoryCompleteData);

                //Send "time_per_story" Event
                float totalStoryTime = storyData.GetTotalEpisodeTime();
                string timeText = TimeUtils.GetTimeInFormat(totalStoryTime);

                SDKEventStringData eventStoryTimeData;
                eventStoryTimeData.eventParameterName = SDKEventsNames.timeOnAppParameterName;
                eventStoryTimeData.eventParameterData = timeText;
                SDKManager.instance.SendEvent(SDKEventsNames.timePerStoryEventName, eventStoryTimeData);
            }

            storyData.currentEpisodeIndex = 0;  //Start of Story
            storyData.currentEpisodeKey = storyData.GetEpisodeKeyFromIndex(storyData.currentEpisodeIndex);
        }

        SaveStoryData();
    }

    public void PlayEpisodeAgain()
    {
        if (episodeData == null)
            return;

        StopCoroutine("UpdateRoutine");

        episodeData.currentBlockID = -1;
        episodeData.currentCommandID = -1;
        episodeData.currentBlocksTotal = 0;
        episodeData.currentBlocksVisited.Clear();
        episodeData.lastCameraPosition = Vector3.zero;
        episodeData.lastCameraRotation = Vector3.zero;
        episodeData.lastCameraOrthosize = 21f;
        episodeData.episodeTime = 0;        

        int currentIndex = storyData.GetIndexFromEpisodeData(episodeData);
        
        storyData.currentEpisodeIndex = currentIndex;
        storyData.currentEpisodeKey = storyData.GetEpisodeKeyFromIndex(storyData.currentEpisodeIndex);

        SaveStoryData();

        if (episodesSpawner == null)
            episodesSpawner = EpisodesSpawner.instance;

        episodesSpawner.StartLoadingStoryScene();
    }

    private void SaveStoryData()
    {
        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(episodesSpawner.storyDataKey, saveString);
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if (episodeData != null)
                episodeData.episodeTime += Time.deltaTime;

            yield return null;
        }
    }

    #region OLD SAVING PROGRESS
    /*
    private void LoadEpisodeDataProgress(string loadSettingsPath)
    {
        #region LOAD FROM TEXT FILE
        string saveString = SerializationManager.LoadFromTextFile(loadSettingsPath);
        if (saveString != null)
        {
            EpisodeData episodeSaveData = JsonUtility.FromJson<EpisodeData>(saveString);
            EpisodeData.instance = episodeSaveData;

            //print(EpisodeData.instance.episodeAssetKey);
            //print(EpisodeData.instance.isFinished);

            for (int i = 0; i < EpisodeData.instance.variableDatas.Count; i++)
            {
                //print(EpisodeData.instance.variableDatas[i].variableKey + ": " + EpisodeData.instance.variableDatas[i].variableValue);                        

                if (episodeFlowchart.HasVariable(EpisodeData.instance.variableDatas[i].variableKey))
                {
                    switch (EpisodeData.instance.variableDatas[i].variableType)
                    {
                        case "BooleanVariable":
                            episodeFlowchart.SetBooleanVariable(EpisodeData.instance.variableDatas[i].variableKey, bool.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                            break;

                        case "IntegerVariable":
                            episodeFlowchart.SetIntegerVariable(EpisodeData.instance.variableDatas[i].variableKey, int.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                            break;

                        case "FloatVariable":
                            episodeFlowchart.SetFloatVariable(EpisodeData.instance.variableDatas[i].variableKey, float.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                            break;

                        case "StringVariable":
                            episodeFlowchart.SetStringVariable(EpisodeData.instance.variableDatas[i].variableKey, EpisodeData.instance.variableDatas[i].variableValue);
                            break;
                    }
                }
            }

            //Adjust the characters
            if(characterSelectionScreens.Length > 0)
            {
                for (int i = 0; i < characterSelectionScreens.Length; i++)
                {
                    if (characterSelectionScreens[i] != null)
                        characterSelectionScreens[i].SelectCharacterFromLoadedData();
                }
            }

            //Set Camera Position, Rotation and OrthographicSize from loaded data
            mainCamera.transform.position = EpisodeData.instance.lastCameraPosition;
            mainCamera.transform.eulerAngles = EpisodeData.instance.lastCameraRotation;
            mainCamera.orthographicSize = EpisodeData.instance.lastCameraOrthosize;

            //Get Block from BlockID and Execute that Block from Command index
            Block blockToExecute = episodeFlowchart.FindBlockFromItemId(EpisodeData.instance.currentBlockID);
            episodeFlowchart.ExecuteBlock(blockToExecute, EpisodeData.instance.currentCommandID);
        }
        #endregion

        #region LOAD FROM OBJECT
        
        EpisodeData episodeSaveData = (EpisodeData)SerializationManager.Load(loadSettingsPath);
        EpisodeData.instance = episodeSaveData;

        print(EpisodeData.instance.episodeAssetKey);
        print(EpisodeData.instance.isFinished);

        for (int i = 0; i < EpisodeData.instance.variableDatas.Count; i++)
        {
            print(EpisodeData.instance.variableDatas[i].variableKey + ": " + EpisodeData.instance.variableDatas[i].variableValue);                        

            if (episodeFlowchart.HasVariable(EpisodeData.instance.variableDatas[i].variableKey))
            {
                switch (EpisodeData.instance.variableDatas[i].variableType)
                {
                    case "BooleanVariable":
                        episodeFlowchart.SetBooleanVariable(EpisodeData.instance.variableDatas[i].variableKey, bool.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                        break;

                    case "IntegerVariable":
                        episodeFlowchart.SetIntegerVariable(EpisodeData.instance.variableDatas[i].variableKey, int.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                        break;

                    case "FloatVariable":
                        episodeFlowchart.SetFloatVariable(EpisodeData.instance.variableDatas[i].variableKey, float.Parse(EpisodeData.instance.variableDatas[i].variableValue));
                        break;

                    case "StringVariable":
                        episodeFlowchart.SetStringVariable(EpisodeData.instance.variableDatas[i].variableKey, EpisodeData.instance.variableDatas[i].variableValue);
                        break;
                }
            }
        }

        if (characterSelectionScreens.Length > 0)
        {
            for (int i = 0; i < characterSelectionScreens.Length; i++)
            {
                if (characterSelectionScreens[i] != null)
                    characterSelectionScreens[i].SelectCharacterFromLoadedData();
            }
        }
        
        #endregion
    }

    
    private void SaveEpisodeProgress()
    {
        EpisodeData episodeSaveData = new EpisodeData();
        EpisodeData.instance = episodeSaveData;

        EpisodeData.instance.episodeAssetKey = episodesSpawner.currentEpisodeReferenceKey.ToString();
        EpisodeData.instance.isFinished = false;

        EpisodeData.instance.variableDatas.Clear();

        List<Variable> flowchartVariables = episodeFlowchart.Variables;
        for (int i = 0; i < flowchartVariables.Count; i++)
        {
            EpisodeVariablesData variableData = new EpisodeVariablesData();
            variableData.variableKey = flowchartVariables[i].Key;
            variableData.variableType = flowchartVariables[i].GetType().Name;

            switch (flowchartVariables[i].GetType().Name)
            {
                case "BooleanVariable":
                    variableData.variableValue = episodeFlowchart.GetBooleanVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "IntegerVariable":
                    variableData.variableValue = episodeFlowchart.GetIntegerVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "FloatVariable":
                    variableData.variableValue = episodeFlowchart.GetFloatVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "StringVariable":
                    variableData.variableValue = episodeFlowchart.GetStringVariable(flowchartVariables[i].Key);
                    break;
            }

            //print(flowchartVariables[i].Key + "(" + flowchartVariables[i].GetType().Name + ")" + ": " + flowchartVariables[i].GetValue().ToString());
            //print(variableData.variableKey + "(" + variableData.variableType + ")" + ": " + variableData.variableValue);

            EpisodeData.instance.variableDatas.Add(variableData);
        }

        executingBlocks = episodeFlowchart.GetExecutingBlocks();
        
        executingBlocksName.Clear();
        executingBlocksItemIDs.Clear();
        for (int i = 0; i < executingBlocks.Count; i++)
        {
            executingBlocksName.Add(executingBlocks[i].BlockName);
            executingBlocksItemIDs.Add(executingBlocks[i].ItemId);
        }

        //Get BlockID       
        EpisodeData.instance.currentBlockID = executingBlocksItemIDs[executingBlocksItemIDs.Count - 1];

        //Get Block from BlockID
        Block blockTemp = episodeFlowchart.FindBlockFromItemId(EpisodeData.instance.currentBlockID);
        EpisodeData.instance.currentCommandID = blockTemp.ActiveCommand.CommandIndex;

        //Save Camera's last position, rotation and Orthographic size
        EpisodeData.instance.lastCameraPosition = mainCamera.transform.position;
        EpisodeData.instance.lastCameraRotation = mainCamera.transform.eulerAngles;
        EpisodeData.instance.lastCameraOrthosize = mainCamera.orthographicSize;

        //Save into Text File
        string saveString = JsonUtility.ToJson(EpisodeData.instance);
        SerializationManager.SaveAsTextFile(EpisodeData.instance.episodeAssetKey, saveString);

        //SerializationManager.Save(EpisodeData.instance.episodeAssetKey, EpisodeData.instance);
    }

    private void SaveEpisodeProgress(bool newSaveFile = false)
    {
        if (newSaveFile)
        {
            EpisodeData episodeSaveData = new EpisodeData();
            EpisodeData.instance = episodeSaveData;
        }
        
        EpisodeData.instance.episodeAssetKey = episodesSpawner.currentEpisodeReferenceKey.ToString();
        EpisodeData.instance.isFinished = false;

        EpisodeData.instance.variableDatas.Clear();

        List<Variable> flowchartVariables = episodeFlowchart.Variables;
        for (int i = 0; i < flowchartVariables.Count; i++)
        {
            EpisodeVariablesData variableData = new EpisodeVariablesData();
            variableData.variableKey = flowchartVariables[i].Key;            
            variableData.variableType = flowchartVariables[i].GetType().Name;

            switch (flowchartVariables[i].GetType().Name)
            {
                case "BooleanVariable":
                    variableData.variableValue = episodeFlowchart.GetBooleanVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "IntegerVariable":
                    variableData.variableValue = episodeFlowchart.GetIntegerVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "FloatVariable":
                    variableData.variableValue = episodeFlowchart.GetFloatVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "StringVariable":
                    variableData.variableValue = episodeFlowchart.GetStringVariable(flowchartVariables[i].Key);
                    break;
            }

            //print(flowchartVariables[i].Key + "(" + flowchartVariables[i].GetType().Name + ")" + ": " + flowchartVariables[i].GetValue().ToString());
            //print(variableData.variableKey + "(" + variableData.variableType + ")" + ": " + variableData.variableValue);

            EpisodeData.instance.variableDatas.Add(variableData);
        }

        //Save Camera's last position, rotation and Orthographic size
        EpisodeData.instance.lastCameraPosition = mainCamera.transform.position;
        EpisodeData.instance.lastCameraRotation = mainCamera.transform.eulerAngles;
        EpisodeData.instance.lastCameraOrthosize = mainCamera.orthographicSize;

        string saveString = JsonUtility.ToJson(EpisodeData.instance);
        SerializationManager.SaveAsTextFile(EpisodeData.instance.episodeAssetKey, saveString);

        //SerializationManager.Save(EpisodeData.instance.episodeAssetKey, EpisodeData.instance);
    }

    //This method is called from "CallMethod" command from Flowchart"
    private void SaveEpisodeProgressFinished()
    {
        EpisodeData episodeSaveData = new EpisodeData();
        EpisodeData.instance = episodeSaveData;

        EpisodeData.instance.episodeAssetKey = episodesSpawner.currentEpisodeReferenceKey.ToString();
        EpisodeData.instance.isFinished = true;

        EpisodeData.instance.variableDatas.Clear();

        List<Variable> flowchartVariables = episodeFlowchart.Variables;
        for (int i = 0; i < flowchartVariables.Count; i++)
        {
            EpisodeVariablesData variableData = new EpisodeVariablesData();
            variableData.variableKey = flowchartVariables[i].Key;
            variableData.variableType = flowchartVariables[i].GetType().Name;

            switch (flowchartVariables[i].GetType().Name)
            {
                case "BooleanVariable":
                    variableData.variableValue = episodeFlowchart.GetBooleanVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "IntegerVariable":
                    variableData.variableValue = episodeFlowchart.GetIntegerVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "FloatVariable":
                    variableData.variableValue = episodeFlowchart.GetFloatVariable(flowchartVariables[i].Key).ToString();
                    break;

                case "StringVariable":
                    variableData.variableValue = episodeFlowchart.GetStringVariable(flowchartVariables[i].Key);
                    break;
            }

            //print(flowchartVariables[i].Key + "(" + flowchartVariables[i].GetType().Name + ")" + ": " + flowchartVariables[i].GetValue().ToString());
            //print(variableData.variableKey + "(" + variableData.variableType + ")" + ": " + variableData.variableValue);

            EpisodeData.instance.variableDatas.Add(variableData);
        }

        executingBlocks = episodeFlowchart.GetExecutingBlocks();

        executingBlocksName.Clear();
        executingBlocksItemIDs.Clear();
        for (int i = 0; i < executingBlocks.Count; i++)
        {
            executingBlocksName.Add(executingBlocks[i].BlockName);
            executingBlocksItemIDs.Add(executingBlocks[i].ItemId);
        }

        //Get BlockID       
        EpisodeData.instance.currentBlockID = executingBlocksItemIDs[executingBlocksItemIDs.Count - 1];

        //Get Block from BlockID
        Block blockTemp = episodeFlowchart.FindBlockFromItemId(EpisodeData.instance.currentBlockID);
        EpisodeData.instance.currentCommandID = blockTemp.ActiveCommand.CommandIndex;

        //Save Camera's last position, rotation and Orthographic size
        EpisodeData.instance.lastCameraPosition = mainCamera.transform.position;
        EpisodeData.instance.lastCameraRotation = mainCamera.transform.eulerAngles;
        EpisodeData.instance.lastCameraOrthosize = mainCamera.orthographicSize;

        string saveString = JsonUtility.ToJson(EpisodeData.instance);
        SerializationManager.SaveAsTextFile(EpisodeData.instance.episodeAssetKey, saveString);

        //Save the data for next episode if exists
        if(episodesSpawner.nextEpisodeReferenceKey.Length > 0)
        {
            EpisodeData nextEpisodeSaveData = new EpisodeData();
            nextEpisodeSaveData.episodeAssetKey = episodesSpawner.nextEpisodeReferenceKey;
            nextEpisodeSaveData.isFinished = false;

            nextEpisodeSaveData.variableDatas.Clear();

            for (int i = 0; i < flowchartVariables.Count; i++)
            {
                EpisodeVariablesData variableData = new EpisodeVariablesData();
                variableData.variableKey = flowchartVariables[i].Key;
                variableData.variableType = flowchartVariables[i].GetType().Name;

                switch (flowchartVariables[i].GetType().Name)
                {
                    case "BooleanVariable":
                        variableData.variableValue = episodeFlowchart.GetBooleanVariable(flowchartVariables[i].Key).ToString();
                        break;

                    case "IntegerVariable":
                        variableData.variableValue = episodeFlowchart.GetIntegerVariable(flowchartVariables[i].Key).ToString();
                        break;

                    case "FloatVariable":
                        variableData.variableValue = episodeFlowchart.GetFloatVariable(flowchartVariables[i].Key).ToString();
                        break;

                    case "StringVariable":
                        variableData.variableValue = episodeFlowchart.GetStringVariable(flowchartVariables[i].Key);
                        break;
                }

                //print(flowchartVariables[i].Key + "(" + flowchartVariables[i].GetType().Name + ")" + ": " + flowchartVariables[i].GetValue().ToString());
                //print(variableData.variableKey + "(" + variableData.variableType + ")" + ": " + variableData.variableValue);

                nextEpisodeSaveData.variableDatas.Add(variableData);
            }

            nextEpisodeSaveData.currentBlockID = 0;
            nextEpisodeSaveData.currentCommandID = 0;

            nextEpisodeSaveData.lastCameraPosition = Vector3.zero;
            nextEpisodeSaveData.lastCameraRotation = Vector3.zero;
            nextEpisodeSaveData.lastCameraOrthosize = 20;

            string nextSaveString = JsonUtility.ToJson(nextEpisodeSaveData);
            SerializationManager.SaveAsTextFile(nextEpisodeSaveData.episodeAssetKey, nextSaveString);
        }

        //SerializationManager.Save(EpisodeData.instance.episodeAssetKey, EpisodeData.instance);
    }
    */
    #endregion

    public void SelectCharacter(CharacterGender characterGender, CharacterDataAssets selectedCharacterDataAsset, VariableReference variableReference, int value, bool saveProgress = false)
    {
        if (episodeFlowchart == null)
            return;

        switch (characterGender)
        {
            case CharacterGender.Gender_Male:

                foreach (var portraitCommand in episodeFlowchart.GetComponentsInChildren<Portrait>())
                {
                    //print(portraitCommand._Character.ToString() + portraitCommand.ItemId.ToString());
                    if ( portraitCommand.enabled && portraitCommand._Character != null && portraitCommand._Character.NameMatch(selectedCharacterDataAsset.fungusCharacter.NameText /*"{$MalePlayerName}"*/))
                    {                        
                        //portrait._Character = maleCharacter;
                        //portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                        if (portraitCommand.Display == DisplayType.Show)
                        {
                            #region OLD HARD CODED CODES
                            //print(portraitCommand._Portrait.name);
                            /*if (portraitCommand._Portrait.name.Contains("Confuse"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[0];
                            else if (portraitCommand._Portrait.name.Contains("Happy"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[1];
                            else if (portraitCommand._Portrait.name.Contains("Idle"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[2];
                            else if (portraitCommand._Portrait.name.Contains("Sad"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[3];*/

                            /*for (int i = 0; i < selectedCharacterDataAsset.fungusPortraitsStrings.Count; i++)
                            {
                                if (portraitCommand._Portrait.name.Contains(selectedCharacterDataAsset.fungusPortraitsStrings[i]))
                                {
                                    print(portraitCommand._Portrait.name + ": "+ selectedCharacterDataAsset.fungusPortraitsStrings[i]);
                                    portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[i];
                                }
                            }*/
                            #endregion

                            for (int i = 0; i < selectedCharacterDataAsset.fungusCharacterPortraits.Count; i++)
                            {
                                if (portraitCommand._Portrait == portraitCommand._Character.Portraits[i])
                                {
                                    portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                                    portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[i];
                                }
                            }                            
                        }
                        else if(portraitCommand.Display == DisplayType.Hide)
                            portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                        //portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                    }
                }

                break;

            case CharacterGender.Gender_Female:
                //femaleCharacter = selectedCharacter;

                foreach (var portraitCommand in episodeFlowchart.GetComponentsInChildren<Portrait>())
                {
                    if (portraitCommand.enabled && portraitCommand._Character != null && portraitCommand._Character.NameMatch(selectedCharacterDataAsset.fungusCharacter.NameText /*"{$FemalePlayerName}"*/))
                    {
                        #region OLD HARD CODED CODES
                        //portrait._Character = maleCharacter;
                        /*portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                        if (portraitCommand.Display == DisplayType.Show)
                        {
                            //print(portraitCommand._Portrait.name);
                            if (portraitCommand._Portrait.name.Contains("Confuse"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[0];
                            else if (portraitCommand._Portrait.name.Contains("Happy"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[1];
                            else if (portraitCommand._Portrait.name.Contains("Idle"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[2];
                            else if (portraitCommand._Portrait.name.Contains("Sad"))
                                portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[3];
                            //print("ITS THERE!");                            
                        }*/
                        #endregion

                        if (portraitCommand.Display == DisplayType.Show)
                        {
                            for (int i = 0; i < selectedCharacterDataAsset.fungusCharacterPortraits.Count; i++)
                            {
                                if (portraitCommand._Portrait == portraitCommand._Character.Portraits[i])
                                {
                                    portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                                    portraitCommand._Portrait = selectedCharacterDataAsset.fungusCharacterPortraits[i];
                                }
                            }
                        }
                        else if (portraitCommand.Display == DisplayType.Hide)
                            portraitCommand._Character = selectedCharacterDataAsset.fungusCharacter;
                    }
                }

                break;
        }

        if (episodeFlowchart.HasVariable(variableReference.variable.Key))
            episodeFlowchart.SetIntegerVariable(variableReference.variable.Key, value);

        if (saveProgress)
            Invoke("SaveEpisodeData", 0.1f);
    }

    private void SetAllowMusicTrue()
    {
        if (episodeData == null)
            return;

        episodeData.allowMusic = true;

        SaveStoryData();
    }

    private void SetAllowMusicFalse()
    {
        if (episodeData == null)
            return;

        episodeData.allowMusic = false;
        musicManager.StopMusic();

        SaveStoryData();
    }

    private void PlayMusicAtIndex(int index)
    {
        if (episodeData == null)
            return;

        if (!episodeData.allowMusic)
            return;

        if (musicManager == null)
            return;

        episodeData.currentMusicIndex = index;
        musicManager.PlayMusic(musicsList[index], true, 1f, 0);

        SaveStoryData();
    }

    #region OLD CODES
    /*public void SelectCharacter(CharacterGender characterGender, Character selectedCharacter, VariableReference variableReference, int value)
    {
        if (episodeFlowchart == null)
            return;

        switch (characterGender)
        {
            case CharacterGender.Gender_Male:
                maleCharacter = selectedCharacter;

                foreach (var portraitCommand in episodeFlowchart.GetComponentsInChildren<Portrait>())
                {
                    if (portraitCommand._Character.NameMatch("{$MalePlayerName}"))
                    {
                        //portrait._Character = maleCharacter;
                        if (portraitCommand.Display == DisplayType.Show)
                        {
                            print(portraitCommand._Portrait.name);
                            if(portraitCommand._Portrait.name.Contains("Happy"))
                            {
                                print("ITS THERE!");
                            }
                        }
                    }
                }

                break;

            case CharacterGender.Gender_Female:
                femaleCharacter = selectedCharacter;
                break;
        }

        if (episodeFlowchart.HasVariable(variableReference.variable.Key))
            episodeFlowchart.SetIntegerVariable(variableReference.variable.Key, value);        
    }*/

    /*private void LevelBackgroundLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;
            case AsyncOperationStatus.Succeeded:
                //levelBgSpriteRenderer.material.shader = Shader.Find("Sprites/Default");                
                //levelBgSpriteRenderer.sprite = obj.Result;

                //narrativeText.material.shader = Shader.Find("TextMeshPro/Distance Field");
                //characterText.material.shader = Shader.Find("TextMeshPro/Distance Field");                
                break;
            case AsyncOperationStatus.Failed:
                break;
        }

        episodeFlowchart.ExecuteBlock("Game Start");
    }*/
    #endregion
}