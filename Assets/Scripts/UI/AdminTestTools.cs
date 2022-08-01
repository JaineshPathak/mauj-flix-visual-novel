using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class AdminTestTools : MonoBehaviour
{
    [Header("Bottom Button")]
    public RectTransform arrowImage;
    public Button testToolsMainButton;

    [Header("Test Tools Panel")]
    public GameObject testToolsMainPanel;
    public Button testToolsMainPanelCloseButton;

    public Slider speedSlider;
    public Button choicesMainButton;

    [Header("Choices Panel")]
    public GameObject choicesMainPanel;
    public Button choicesCloseButton;
    public Transform choicesPanelParent;
    public EpisodeMenuChoiceButton choiceButtonPrefab;

    private List<EpisodeMenuChoiceButton> menuChoiceButtonsList = new List<EpisodeMenuChoiceButton>();

    private bool adminToolPanelOn;
    private EpisodesHandler epHandler;

    private void OnEnable()
    {
        EpisodesSpawner.OnEpisodeLoaded += OnEpisodeLoaded;
    }

    private void OnDisable()
    {
        EpisodesSpawner.OnEpisodeLoaded -= OnEpisodeLoaded;
    }

    private void OnDestroy()
    {
        EpisodesSpawner.OnEpisodeLoaded -= OnEpisodeLoaded;        
    }

    private void OnEpisodeLoaded(EpisodesHandler episodesHandler)
    {
        epHandler = episodesHandler;

        Time.timeScale = 1f;

        if (UserControl.instance.AdminMode)
        {
            testToolsMainButton.gameObject.SetActive(true);
            SpawnChoices();
        }
        else if(testToolsMainButton)
            testToolsMainButton.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (testToolsMainButton)
        {
            testToolsMainButton.gameObject.SetActive(false);
            if (UserControl.instance.AdminMode)            
                testToolsMainButton.onClick.AddListener(ShowHideTestingToolsPanel);            
            else
                testToolsMainButton.gameObject.SetActive(false);
        }

        if (testToolsMainPanelCloseButton)
        {
            testToolsMainPanelCloseButton.onClick.AddListener(() =>
            {
                adminToolPanelOn = false;
                CheckArrow();
                testToolsMainPanel.SetActive(false);
            });
        }

        if (choicesMainButton)
            choicesMainButton.onClick.AddListener(() => choicesMainPanel.SetActive(true));

        if (choicesCloseButton)
            choicesCloseButton.onClick.AddListener(() => choicesMainPanel.SetActive(false));
    }   

    private void ShowHideTestingToolsPanel()
    {
        if (UserControl.instance == null)
            return;

        if (!UserControl.instance.AdminMode)
            return;

        adminToolPanelOn = !adminToolPanelOn;

        CheckArrow();

        testToolsMainPanel.SetActive(adminToolPanelOn);
    }

    private void CheckArrow()
    {
        if (!adminToolPanelOn)
        {
            Quaternion arrowRotation = arrowImage.rotation;
            arrowRotation.eulerAngles = new Vector3(0, 0, 180f);
            arrowImage.rotation = arrowRotation;
        }
        else
            arrowImage.rotation = Quaternion.identity;
    }

    public void SpawnChoices()
    {
        if (epHandler == null)
            return;

        Flowchart chart = epHandler.episodeFlowchart;
        if(chart != null)
        {
            foreach (Menu menuCommand in chart.GetComponentsInChildren<Menu>())
            {
                if (menuCommand != null)
                {
                    EpisodeMenuChoiceButton menuChoiceButton = Instantiate(choiceButtonPrefab, choicesPanelParent);
                    menuChoiceButton.adminTesting = this;
                    menuChoiceButton.myTargetBlock = menuCommand.TargetBlock;

                    string fixedText = menuChoiceButton.replacerHindi.GetFixedText(menuCommand.Text);
                    menuChoiceButton.choicebuttonText.text = fixedText;

                    menuChoiceButtonsList.Add(menuChoiceButton);
                }
            }
        }        
    }

    public void ExecuteMenuChoice(Block targetBlock)
    {
        if (targetBlock == null)
            return;

        adminToolPanelOn = false;
        choicesMainPanel.SetActive(false);
        testToolsMainPanel.SetActive(false);

        CheckArrow();

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
            if (fadeToViews.Length > 0)
            {
                fadeToViewTmp = fadeToViews[0];

                Camera.main.transform.position = fadeToViewTmp.TargetView.transform.position;
                Camera.main.transform.rotation = fadeToViewTmp.TargetView.transform.rotation;
                Camera.main.orthographicSize = fadeToViewTmp.TargetView.ViewSize;
            }
        }

        epHandler.episodeFlowchart.StopAllBlocks();
        epHandler.episodeFlowchart.ExecuteBlock(targetBlock);
    }

    public void SetTimeScale(float val)
    {
        Time.timeScale = val;
    }
}
