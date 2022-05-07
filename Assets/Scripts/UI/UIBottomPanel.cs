﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BottomButton
{
    public Image buttonMain;
    public Sprite buttonOn;
    public Sprite buttonOff;
    public TextMeshProUGUI buttonText;

    [Space(10)]

    public Color buttonTextColorOn = Color.white;
    public Color buttonTextColorOff;

    public void SetButtonOn()
    {
        if (buttonMain == null || buttonOn == null)
            return;

        buttonMain.sprite = buttonOn;

        if (buttonText)
            buttonText.color = buttonTextColorOn;
    }

    public void SetButtonOff()
    {
        if (buttonMain == null || buttonOff == null)
            return;

        buttonMain.sprite = buttonOff;

        if (buttonText)
            buttonText.color = buttonTextColorOff;
    }
}

public class UIBottomPanel : MonoBehaviour
{
    public UIStoriesDetailsPanel detailsPanel;

    [Header("Buttons Images")]
    [SerializeField] private BottomButton[] bottomButtons;

    [Header("Panels")]
    [SerializeField] private CanvasGroup[] panelsList;

    private EpisodesSpawner episodesSpawner;    
    private bool playSoundFirst = false;
    private int lastButtonIndex;

    public int LastButtonIndex
    {
        get { return lastButtonIndex; }
    }


    private void OnEnable()
    {
        EpisodesSpawner.OnRateUsWindowClosed += OnRateUsWindowClosed;
    }

    private void OnDisable()
    {
        EpisodesSpawner.OnRateUsWindowClosed -= OnRateUsWindowClosed;
    }

    private void OnDestroy()
    {
        EpisodesSpawner.OnRateUsWindowClosed -= OnRateUsWindowClosed;
    }

    private void Start()
    {
        if (EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        SelectPanel(0);     //Home Panel

        /*if (EpisodesSpawner.instance != null && EpisodesSpawner.instance.playerData != null)
        {
            if (EpisodesSpawner.instance.playerData.hasRatedGame && bottomButtons[2].buttonMain != null)
                if (bottomButtons[2].buttonMain.gameObject.activeSelf)
                    bottomButtons[2].buttonMain.gameObject.SetActive(false);
        }*/
    }

    /*private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            if(EpisodesSpawner.instance != null && EpisodesSpawner.instance.playerData != null)
            {
                if(EpisodesSpawner.instance.playerData.hasRatedGame && bottomButtons[2].buttonMain != null)
                    if(bottomButtons[2].buttonMain.gameObject.activeSelf)
                        bottomButtons[2].buttonMain.gameObject.SetActive(false);                
            }
        }
    }*/

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (lastButtonIndex != 0)
                SelectPanel(0);
            else if (detailsPanel.IsShown)
                detailsPanel.MoveHidePanel();
            else
                episodesSpawner.ShowAskToQuitPanel();
        }
    }

    public void SelectPanel(int index)
    {
        SelectWindow(index);
        SelectButton(index);
    }

    private void SelectWindow(int index)
    {
        if (panelsList.Length <= 0)
            return;

        if (index > panelsList.Length)
            return;

        /*if (index == 0)
            detailsPanel.episodesSpawner.topPanel.ShowTopPanel();
        else
            detailsPanel.episodesSpawner.topPanel.HideTopPanel();*/

        if(EpisodesSpawner.instance != null)
        {
            if(index == 0)
                EpisodesSpawner.instance.topPanel.ShowTopPanel();
            else
                EpisodesSpawner.instance.topPanel.HideTopPanel();
        }

        for (int i = 0; i < panelsList.Length; i++)
        {
            if (panelsList[i] != null && panelsList[i] != panelsList[index])
            {
                LeanTween.alphaCanvas(panelsList[i], 0, 0.3f).setEase(LeanTweenType.easeInOutSine);
                panelsList[i].interactable = false;
                panelsList[i].blocksRaycasts = false;
            }
        }

        LeanTween.alphaCanvas(panelsList[index], 1f, 0.3f).setEase(LeanTweenType.easeInOutSine);
        panelsList[index].interactable = true;
        panelsList[index].blocksRaycasts = true;
    }

    private void SelectButton(int index)
    {
        if (bottomButtons.Length <= 0)
            return;

        lastButtonIndex = index;

        for (int i = 0; i < bottomButtons.Length; i++)
        {
            bottomButtons[i].SetButtonOff();
        }

        bottomButtons[index].SetButtonOn();

        if(!playSoundFirst)
        {
            playSoundFirst = true;
            return;
        }

        detailsPanel.PlayButtonClickSound();
    }

    public void OnStarRateUsClicked()
    {
        if (EpisodesSpawner.instance == null)
            return;

        detailsPanel?.PlayButtonClickSound();

        for (int i = 0; i < bottomButtons.Length; i++)
            bottomButtons[i].SetButtonOff();

        if (bottomButtons[2].buttonMain != null)
            bottomButtons[2].SetButtonOn();

        EpisodesSpawner.instance.OpenRateUsWindow();
    }

    private void OnRateUsWindowClosed()
    {
        if (bottomButtons[2].buttonMain == null)
            return;

        detailsPanel?.PlayButtonClickSound();

        for (int i = 0; i < bottomButtons.Length; i++)
            bottomButtons[i].SetButtonOff();

        bottomButtons[2].SetButtonOff();
        bottomButtons[lastButtonIndex].SetButtonOn();
    }
}
