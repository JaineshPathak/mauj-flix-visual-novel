using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BottomButton
{
    public Image buttonMain;
    public Sprite buttonOn;
    public Sprite buttonOff;

    public void SetButtonOn()
    {
        if (buttonMain == null || buttonOn == null)
            return;

        buttonMain.sprite = buttonOn;
    }

    public void SetButtonOff()
    {
        if (buttonMain == null || buttonOff == null)
            return;

        buttonMain.sprite = buttonOff;
    }
}

public class UIBottomPanel : MonoBehaviour
{
    public UIStoriesDetailsPanel detailsPanel;

    [Header("Buttons Images")]
    [SerializeField] private BottomButton[] bottomButtons;

    [Header("Panels")]
    [SerializeField] private CanvasGroup[] panelsList;

    private bool playSoundFirst = false;

    private void Start()
    {
        SelectPanel(0);     //Home Panel
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
}
