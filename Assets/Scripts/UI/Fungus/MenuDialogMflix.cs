using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Linq;
using Fungus;
using TMPro;

public class MenuDialogMflix : MenuDialog
{
    [Header("Button Sprites")]
    public Sprite noCostSprite;
    public Sprite costSprite;      

    protected Image[] cachedButtonsImages;
    protected Transform[] cachedButtonsDiamondPanel;
    protected TextMeshProUGUI[] cachedButtonsText;
    protected TextMeshProUGUI[] cachedButtonsDiamondsCostText;

    protected override void Awake()
    {
        Button[] optionButtons = GetComponentsInChildren<Button>();
        cachedButtons = optionButtons;

        Array.Resize(ref cachedButtonsImages, cachedButtons.Length);
        Array.Resize(ref cachedButtonsDiamondPanel, cachedButtons.Length);
        Array.Resize(ref cachedButtonsDiamondsCostText, cachedButtons.Length);
        Array.Resize(ref cachedButtonsText, cachedButtons.Length);
        for (int i = 0; i < cachedButtons.Length; i++)
        {
            cachedButtonsImages[i] = cachedButtons[i].GetComponent<Image>();
            cachedButtonsText[i] = cachedButtons[i].transform.Find("Text (TMP)").GetComponentInChildren<TextMeshProUGUI>();
            cachedButtonsDiamondsCostText[i] = cachedButtons[i].transform.Find("DiamondCostPanel").GetComponentInChildren<TextMeshProUGUI>();

            cachedButtonsDiamondPanel[i] = cachedButtons[i].transform.Find("DiamondCostPanel");
        }

        Slider timeoutSlider = GetComponentInChildren<Slider>();
        cachedSlider = timeoutSlider;

        if (Application.isPlaying)
        {
            // Don't auto disable buttons in the editor
            Clear();
        }

        CheckEventSystem();
    }

    public bool AddOptionWithCost(string text, bool interactable, bool hideOption, Block targetBlock, bool hasDiamondCost, float diamondCost)
    {
        var block = targetBlock;

        UnityAction action = null;

        if (hasDiamondCost && FirebaseFirestoreHandler.instance != null)
        {
            action = delegate
            {
                if (FirebaseFirestoreHandler.instance.GetUserDiamondsAmountInt() < diamondCost)
                    return;

                FirebaseFirestoreHandler.instance.DebitDiamondsAmount(diamondCost);

#if UNITY_EDITOR
                Debug.Log("Firebase Firestore: Diamonds Deducted: " + FirebaseFirestoreHandler.instance.GetUserDiamondsAmount());
#endif

                EventSystem.current.SetSelectedGameObject(null);
                StopAllCoroutines();
                // Stop timeout
                Clear();
                HideSayDialog();
                if (block != null)
                {
                    var flowchart = block.GetFlowchart();
                    gameObject.SetActive(false);
                    // Use a coroutine to call the block on the next frame
                    // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                    flowchart.StartCoroutine(CallBlock(block));
                }
            };
        }
        else if (hasDiamondCost && FirebaseFirestoreHandler.instance == null)
        {
            action = delegate
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Firestore: No instance found. Control Returned");
#endif
                return;
            };
        }
        else
        {
            action = delegate
            {
                EventSystem.current.SetSelectedGameObject(null);
                StopAllCoroutines();
                // Stop timeout
                Clear();
                HideSayDialog();
                if (block != null)
                {
                    var flowchart = block.GetFlowchart();
                    gameObject.SetActive(false);
                    // Use a coroutine to call the block on the next frame
                    // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                    flowchart.StartCoroutine(CallBlock(block));
                }
            };
        }

        /*UnityAction action = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            Clear();
            HideSayDialog();
            if (block != null)
            {
                var flowchart = block.GetFlowchart();
                gameObject.SetActive(false);
                // Use a coroutine to call the block on the next frame
                // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                flowchart.StartCoroutine(CallBlock(block));
            }
        };*/        

        return AddOptionWithCost(text, interactable, hideOption, action, hasDiamondCost, diamondCost);
    }

    /*public override bool AddOption(string text, bool interactable, bool hideOption, Block targetBlock)
    {
        var block = targetBlock;
        UnityAction action = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            Clear();
            HideSayDialog();
            if (block != null)
            {
                var flowchart = block.GetFlowchart();
                gameObject.SetActive(false);
                // Use a coroutine to call the block on the next frame
                // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                flowchart.StartCoroutine(CallBlock(block));
            }
        };

        return AddOption(text, interactable, hideOption, action);
    }*/

    private bool AddOptionWithCost(string text, bool interactable, bool hideOption, UnityAction action, bool hasDiamondCost, float diamondCost)
    {
        if (nextOptionIndex >= CachedButtons.Length)
        {
            Debug.LogWarning("Unable to add menu item, not enough buttons: " + text);
            return false;
        }
        //if first option notify that a menu has started
        if (nextOptionIndex == 0)
            MenuSignals.DoMenuStart(this);

        var button = cachedButtons[nextOptionIndex];
        var buttonImage = cachedButtonsImages[nextOptionIndex];
        var buttonDiamondPanel = cachedButtonsDiamondPanel[nextOptionIndex];
        var buttonText = cachedButtonsText[nextOptionIndex];
        var buttonDiamondCostText = cachedButtonsDiamondsCostText[nextOptionIndex];

        if (hasDiamondCost)
        {
            buttonImage.sprite = costSprite;
            buttonDiamondPanel.gameObject.SetActive(true);
            buttonDiamondCostText.text = diamondCost.ToString();
            buttonText.color = Color.black;
        }
        else
        {
            buttonImage.sprite = noCostSprite;
            buttonDiamondPanel.gameObject.SetActive(false);
            buttonDiamondCostText.text = "";
            buttonText.color = Color.white;
        }

        //move forward for next call
        nextOptionIndex++;

        //don't need to set anything on it
        if (hideOption)
            return true;

        button.gameObject.SetActive(true);
        button.interactable = interactable;
        if (interactable && autoSelectFirstButton && !cachedButtons.Select(x => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        TextAdapter textAdapter = new TextAdapter();
        textAdapter.InitFromGameObject(button.gameObject, true);
        if (textAdapter.HasTextObject())
        {
            text = TextVariationHandler.SelectVariations(text);

            textAdapter.Text = text;
        }

        button.onClick.AddListener(action);

        //CheckButtonsGroupPosition();
        if (DisplayedOptionsCount == 3 && buttonsGroup != null)
            buttonsGroup.anchoredPosition = new Vector2(0, (float)DisplayedOptionsCount * 100f);

        return true;
    }
}
