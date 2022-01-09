using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

[Serializable]
public class EpisodeBranches
{
    public GameObject episodeItem;
    public string episodeStartBlock = "01 - Game Start";
    public string episodeNormalEndBlock;
    public string episodeBranchEndBlock;
}

[Serializable]
public class EpisodeCurrentData
{
    public GameObject episodeItem;
    public string episodeStartBlock = "01 - Game Start";
    public string episodeNormalEndBlock;
    public string episodeBranchEndBlock;

    public void WriteData(GameObject _episodeItem, string _startBlock, string _normalEndBlock, string _branchEndBlock)
    {
        episodeItem = _episodeItem;
        episodeStartBlock = _startBlock;
        episodeNormalEndBlock = _normalEndBlock;
        episodeBranchEndBlock = _branchEndBlock;
    }

    public void ClearData()
    {
        episodeItem = null;
        episodeStartBlock = string.Empty;
        episodeNormalEndBlock = string.Empty;
        episodeBranchEndBlock = string.Empty;
    }
}

public class EpisodesTesting : MonoBehaviour
{
    [Header("Episode Current Data")]
    public EpisodeCurrentData episodeCurrentData;

    [Header("Episode Branches")]
    public bool foundBranches;
    public GameObject branchEndingsScreen;
    public Button branchStartButton;
    public Button branchNormalEndButton;
    public Button branchBranchEndButton;
    public EpisodeBranches[] episodeBranches;

    [Header("Episode Buttons")]
    public EpisodesLoadButton episodeLoadButtonPrefab;
    public GameObject titleObject;
    public GameObject backButton;
    public GameObject choicesButton;
    public Transform episodeButtonGroupLayout;
        
    [Header("Episodes List")]
    public GameObject[] episodesToTest;

    [Header("Episode Prefab Current")]
    public GameObject currentEpisodePrefab;

    [Header("Episode Choices Prefab")]
    public GameObject episodeChoicesScreen;
    public Transform episodeChoicesParent;
    public EpisodeMenuChoiceButton episodeChoiceButtonPrefab;

    private List<EpisodeMenuChoiceButton> menuChoiceButtonsList = new List<EpisodeMenuChoiceButton>();

    private void Start()
    {
        branchEndingsScreen.SetActive(false);
        backButton.SetActive(false);

        choicesButton.SetActive(false);
        episodeChoicesScreen.SetActive(false);

        branchStartButton.onClick.AddListener(OnEpisodeBranchNormalStart);
        branchNormalEndButton.onClick.AddListener(OnEpisodeBranchNormalEnd);
        branchBranchEndButton.onClick.AddListener(OnEpisodeBranchEnd);

        LoadEpisodes();
    }

    private void LoadEpisodes()
    {
        if (episodeLoadButtonPrefab == null || episodesToTest.Length <= 0)
            return;

        for (int i = 0; i < episodesToTest.Length; i++)
        {
            EpisodesLoadButton buttonGameObject = Instantiate(episodeLoadButtonPrefab, episodeButtonGroupLayout);
            buttonGameObject.episodeText.text = "Episode " + (i + 1);
            buttonGameObject.episodesTesting = this;
            buttonGameObject.episodePrefab = episodesToTest[i];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentEpisodePrefab != null)
            OnBackButton();
    }

    public void SpawnEpisode(GameObject episodePrefab)
    {
        currentEpisodePrefab = Instantiate(episodePrefab);

        for (int i = 0; i < episodeBranches.Length && (episodeBranches.Length > 0); i++)
        {
            if ((episodeBranches[i].episodeItem != null) &&
                episodePrefab == episodeBranches[i].episodeItem)
            {
                foundBranches = true;
                branchEndingsScreen.SetActive(true);
                titleObject.SetActive(false);
                episodeButtonGroupLayout.gameObject.SetActive(false);

                episodeCurrentData.WriteData(episodeBranches[i].episodeItem, episodeBranches[i].episodeStartBlock, episodeBranches[i].episodeNormalEndBlock, episodeBranches[i].episodeBranchEndBlock);
                break;
            }
            else
                foundBranches = false;
        }

        if (!foundBranches)
        {
            currentEpisodePrefab.GetComponentInChildren<Flowchart>().ExecuteBlock("01 - Game Start");

            backButton.SetActive(true);
            choicesButton.SetActive(true);
            titleObject.SetActive(false);
            episodeButtonGroupLayout.gameObject.SetActive(false);
        }

        SpawnMenuChoices();
    }

    private void SpawnMenuChoices()
    {
        if (currentEpisodePrefab == null)
            return;

        Flowchart chart = currentEpisodePrefab.GetComponentInChildren<Flowchart>();
        foreach (Menu menuCommand in chart.GetComponentsInChildren<Menu>())
        {
            if(menuCommand != null)
            {
                EpisodeMenuChoiceButton menuChoiceButton = Instantiate(episodeChoiceButtonPrefab, episodeChoicesParent);
                menuChoiceButton.episodesTesting = this;
                menuChoiceButton.myTargetBlock = menuCommand.TargetBlock;

                string fixedText = menuChoiceButton.replacerHindi.GetFixedText(menuCommand.Text);
                menuChoiceButton.choicebuttonText.text = fixedText;

                menuChoiceButtonsList.Add(menuChoiceButton);
            }
        }
    }

    //============================================================================================================    

    public void OnBackBranchButton()
    {
        foundBranches = false;

        episodeCurrentData.ClearData();

        if(currentEpisodePrefab != null)
        {
            Destroy(currentEpisodePrefab);
            currentEpisodePrefab = null;

            LeanTween.cancelAll();
        }

        branchEndingsScreen.SetActive(false);
        titleObject.SetActive(true);
        episodeButtonGroupLayout.gameObject.SetActive(true);
    }

    public void OnBackButton()
    {
        if (currentEpisodePrefab == null)
            return;

        Destroy(currentEpisodePrefab);
        currentEpisodePrefab = null;

        LeanTween.cancelAll();

        foundBranches = false;

        episodeCurrentData.ClearData();

        branchEndingsScreen.SetActive(false);
        backButton.SetActive(false);
        choicesButton.SetActive(false);
        titleObject.SetActive(true);
        episodeButtonGroupLayout.gameObject.SetActive(true);

        if(menuChoiceButtonsList.Count > 0)
        {
            for (int i = 0; i < menuChoiceButtonsList.Count; i++)
            {
                Destroy(menuChoiceButtonsList[i].gameObject);
            }

            menuChoiceButtonsList.Clear();
        }
    }

    public void OnChoicesButton()
    {
        if (currentEpisodePrefab == null)
            return;

        episodeChoicesScreen.SetActive(true);
        choicesButton.SetActive(false);
    }

    public void OnBackChoicesButton()
    {
        episodeChoicesScreen.SetActive(false);
        choicesButton.SetActive(true);
    }

    //============================================================================================================

    private void OnEpisodeBranchNormalStart()
    {
        if (currentEpisodePrefab == null)
            return;

        currentEpisodePrefab.GetComponentInChildren<Flowchart>().ExecuteBlock(episodeCurrentData.episodeStartBlock);

        backButton.SetActive(true);
        branchEndingsScreen.SetActive(false);
        choicesButton.SetActive(true);
    }

    private void OnEpisodeBranchNormalEnd()
    {
        if (currentEpisodePrefab == null)
            return;

        currentEpisodePrefab.GetComponentInChildren<Flowchart>().ExecuteBlock(episodeCurrentData.episodeNormalEndBlock);

        backButton.SetActive(true);
        branchEndingsScreen.SetActive(false);
        choicesButton.SetActive(true);
    }

    private void OnEpisodeBranchEnd()
    {
        if (currentEpisodePrefab == null)
            return;

        currentEpisodePrefab.GetComponentInChildren<Flowchart>().ExecuteBlock(episodeCurrentData.episodeBranchEndBlock);

        backButton.SetActive(true);
        branchEndingsScreen.SetActive(false);
        choicesButton.SetActive(true);
    }

    //============================================================================================================

    public void ExecuteMenuChoice(Block targetBlock)
    {
        if (targetBlock == null || currentEpisodePrefab == null)
            return;

        choicesButton.SetActive(true);
        episodeChoicesScreen.SetActive(false);

        MoveToView moveToViewTmp = null;
        MoveToView[] moveToViewsTmp = targetBlock.GetComponentsInChildren<MoveToView>();
        if (moveToViewsTmp.Length > 0)
        {
            moveToViewTmp = moveToViewsTmp[0];

            Camera.main.transform.position = moveToViewTmp.TargetView.transform.position;
            Camera.main.transform.rotation = moveToViewTmp.TargetView.transform.rotation;
            Camera.main.orthographicSize = moveToViewTmp.TargetView.ViewSize;
        }
        else
        {
            FadeToView fadeToViewTmp = null;
            FadeToView[] fadeToViews = targetBlock.GetComponentsInChildren<FadeToView>();
            if(fadeToViews.Length > 0)
            {
                fadeToViewTmp = fadeToViews[0];

                Camera.main.transform.position = fadeToViewTmp.TargetView.transform.position;
                Camera.main.transform.rotation = fadeToViewTmp.TargetView.transform.rotation;
                Camera.main.orthographicSize = fadeToViewTmp.TargetView.ViewSize;
            }
        }

        currentEpisodePrefab.GetComponentInChildren<Flowchart>().StopAllBlocks();
        currentEpisodePrefab.GetComponentInChildren<Flowchart>().ExecuteBlock(targetBlock);
    }
}
