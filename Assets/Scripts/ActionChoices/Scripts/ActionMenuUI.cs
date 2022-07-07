using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Fungus;

public class ActionMenuUI : MonoBehaviourSingleton<ActionMenuUI>
{
    [Header("Components")]
    public CanvasGroup mainCanvasGroup;
    public CanvasGroup blackBgCanvasGroup;
    public TextMeshProUGUI centerText;
    public CharReplacerHindi centerTextReplacer;

    [Header("Action Items UI Buttons")]
    public List<ActionMenuUIItem> actionMenuUIItems = new List<ActionMenuUIItem>();

    [Header("Choices Buttons")]
    public Button[] cachedChoiceButtons;

    private ActionItem[] actionItemsList;
    private ActionMenu actionMenu;

    private ActionMenuUIItem currentActionUIItem;
    private List<TextMeshProUGUI> cachedChoiceTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        mainCanvasGroup.alpha = 0;
        mainCanvasGroup.interactable = false;
        mainCanvasGroup.blocksRaycasts = false;

        cachedChoiceTexts.Clear();
        for (int i = 0; i < cachedChoiceButtons.Length && (cachedChoiceButtons.Length > 0); i++)
        {
            cachedChoiceButtons[i].transform.localScale = Vector3.zero;
            cachedChoiceButtons[i].onClick.RemoveAllListeners();

            cachedChoiceTexts.Add(cachedChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>());
        }

        currentActionUIItem = null;
    }

    private void OnValidate()
    {
        actionMenuUIItems.Clear();
        actionMenuUIItems.AddRange(GetComponentsInChildren<ActionMenuUIItem>());

        for (int i = 0; i < actionMenuUIItems.Count && (actionMenuUIItems.Count > 0); i++)        
            actionMenuUIItems[i].id = i;

        mainCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetupActionItems(string _centerTitleText, ActionMenu _actionMenu, ActionItem[] _actionItemsList)
    {
        if (_actionItemsList.Length <= 0)
            return;

        if (_actionMenu == null)
            return;

        centerText.text = _centerTitleText;
        centerTextReplacer.UpdateMe();

        actionMenu = _actionMenu;
        actionItemsList = _actionItemsList;

        LTSeq startSeq = LeanTween.sequence();
        startSeq.append(() => 
        {
            mainCanvasGroup.interactable = true;
            mainCanvasGroup.blocksRaycasts = true;
        });
        startSeq.append(LeanTween.alphaCanvas(mainCanvasGroup, 1f, 0.4f).setEaseInOutSine());
        startSeq.append( () => 
        {
            for (int i = 0; i < actionItemsList.Length; i++)
            {
                actionMenuUIItems[i].SetupItem(actionItemsList[i].itemSprite, this);
                actionMenuUIItems[i].ItemShow();
            }
        });        
    }

    public void OnItemFocusClicked(int id)
    {
        actionMenu.currentActionItemSelected = actionItemsList[id];

        if (blackBgCanvasGroup.alpha == 0)
            LeanTween.alphaCanvas(blackBgCanvasGroup, 1f, 0.4f).setEaseInOutSine();

        for (int i = 0; i < actionMenuUIItems.Count && (actionMenuUIItems.Count > 0); i++)
        {
            if (i == id)
            {
                currentActionUIItem = actionMenuUIItems[i];
                actionMenuUIItems[i].ItemFocus();
            }
            else
                actionMenuUIItems[i].ItemUnfocus();
        }

        ShowItemChoices();
    }

    private void ShowItemChoices()
    {
        if (cachedChoiceButtons.Length <= 0)
            return;

        for (int i = 0; i < actionMenu.currentActionItemSelected.actionItemChoices.Count; i++)        
            InitChoiceButton(cachedChoiceButtons[i], cachedChoiceTexts[i], actionMenu.currentActionItemSelected.actionItemChoices[i]);        
    }

    private void InitChoiceButton(Button cachedButton, TextMeshProUGUI cachedText, ActionItemChoice actionItemChoice)
    {
        LeanTween.scale(cachedButton.gameObject, Vector3.one, 0.4f).setEaseOutBack();
        cachedText.text = actionItemChoice.choiceText;

        var block = actionItemChoice.targetBlock;
        UnityAction buttonEvent = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            cachedButton.onClick.RemoveAllListeners();

            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;

            currentActionUIItem.ItemHide();
            LeanTween.alphaCanvas(mainCanvasGroup, 0, 1f).setEaseInOutSine().setOnComplete( () => 
            {
                if (block != null)
                {
                    var flowchart = block.GetFlowchart();
                    gameObject.SetActive(false);
                    // Use a coroutine to call the block on the next frame
                    // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                    flowchart.StartCoroutine(CallBlock(block));
                }
            });            
        };

        cachedButton.onClick.RemoveAllListeners();
        cachedButton.onClick.AddListener(buttonEvent);
    }

    protected IEnumerator CallBlock(Block block)
    {
        yield return new WaitForEndOfFrame();
        block.StartExecution();
    }
}
