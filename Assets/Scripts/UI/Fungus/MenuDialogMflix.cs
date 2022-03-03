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
    protected Animator[] cachedShineEffectAnim;

    protected override void Awake()
    {
        Button[] optionButtons = GetComponentsInChildren<Button>();
        cachedButtons = optionButtons;

        Array.Resize(ref cachedButtonsImages, cachedButtons.Length);
        Array.Resize(ref cachedButtonsDiamondPanel, cachedButtons.Length);
        Array.Resize(ref cachedButtonsDiamondsCostText, cachedButtons.Length);
        Array.Resize(ref cachedButtonsText, cachedButtons.Length);
        Array.Resize(ref cachedShineEffectAnim, cachedButtons.Length);
        for (int i = 0; i < cachedButtons.Length; i++)
        {
            cachedButtonsImages[i] = cachedButtons[i].GetComponent<Image>();
            cachedButtonsText[i] = cachedButtons[i].transform.Find("Text (TMP)").GetComponentInChildren<TextMeshProUGUI>();
            cachedButtonsDiamondsCostText[i] = cachedButtons[i].transform.Find("DiamondCostPanel").GetComponentInChildren<TextMeshProUGUI>();

            cachedButtonsDiamondPanel[i] = cachedButtons[i].transform.Find("DiamondCostPanel");

            cachedShineEffectAnim[i] = cachedButtons[i].GetComponent<Animator>();
            cachedShineEffectAnim[i].enabled = false;
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

        if (hasDiamondCost && FirebaseFirestoreOffline.instance != null)
        {
            if (EpisodesSpawner.instance != null)
            {
                //EpisodesSpawner.instance.ShowHideDiamondPanel(true);
                EpisodesSpawner.instance.topPanel.ShowTopPanel();
            }

            action = delegate
            {
                #region Old code
                /*if (FirebaseFirestoreHandler.instance.GetUserDiamondsAmountInt() < diamondCost)
                    return;

                FirebaseFirestoreHandler.instance.DebitDiamondsAmount(diamondCost);*/

                //FirebaseFirestoreOffline.instance.DebitDiamondsAmount(diamondCost);

                /*if (EpisodesSpawner.instance != null)
                    EpisodesSpawner.instance.ShowHideDiamondPanel(false, 0.5f, 1f);*/
                #endregion

                if (FirebaseFirestoreOffline.instance.GetDiamondsAmountInt() < diamondCost)
                    return;

                Transform buttonDiamondPanelStart = cachedButtonsDiamondPanel[nextOptionIndex];
                StartCoroutine(ActionWithDiamondCostRoutine(block, diamondCost, buttonDiamondPanelStart));               
            };
        }
        else if (hasDiamondCost && FirebaseFirestoreOffline.instance == null)
        {
            /*if (EpisodesSpawner.instance != null)
                EpisodesSpawner.instance.ShowHideDiamondPanel(true);*/

            action = delegate
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Firestore Offline: No instance found. Control Returned!");
#endif
                return;
            };
        }
        else
        {
            action = delegate
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Firestore Offline: No Diamond Cost, Go Ahead!");
#endif

                if (EpisodesSpawner.instance != null)
                {
                    //EpisodesSpawner.instance.ShowHideDiamondPanel(false, 0.5f, 1f);
                    EpisodesSpawner.instance.topPanel.HideTopPanel();
                }

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

        #region Backup action code
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
        #endregion

        return AddOptionWithCost(text, interactable, hideOption, action, hasDiamondCost, diamondCost);
    }    

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
        var buttonShineAim = cachedShineEffectAnim[nextOptionIndex];

        if (hasDiamondCost)
        {
            buttonImage.sprite = costSprite;
            buttonDiamondPanel.gameObject.SetActive(true);
            buttonDiamondCostText.text = diamondCost.ToString();
            buttonText.color = Color.black;
            buttonShineAim.enabled = true;
        }
        else
        {
            buttonImage.sprite = noCostSprite;
            buttonDiamondPanel.gameObject.SetActive(false);
            buttonDiamondCostText.text = "";
            buttonText.color = Color.white;
            buttonShineAim.enabled = false;
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

    private IEnumerator ActionWithDiamondCostRoutine(Block block, float diamondCost, Transform buttonDiamondPanelStart)
    {
        // Stop timeout

#if UNITY_EDITOR
        Debug.Log("STEP 1 - Diamond Cost Coroutine Started for Block: " + block.BlockName);
#endif

        //StopAllCoroutines();

        yield return null;

        if (EpisodesSpawner.instance != null)
        {
            EpisodesSpawner.instance.diamondsPool.PlayDiamondsAnimationDebit(buttonDiamondPanelStart, EpisodesSpawner.instance.topPanel.diamondsPanelIcon, (int)diamondCost, () =>
            {
#if UNITY_EDITOR
                Debug.Log("STEP 2 - Diamond Cost Coroutine Completed for Block: " + block.BlockName);
#endif

                EpisodesSpawner.instance.topPanel.HideTopPanel(0.3f, 0.7f);

                EventSystem.current.SetSelectedGameObject(null);
                StopAllCoroutines();

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

                //StartCoroutine(OnDiamondsDebitCompletedRoutine(block));
            }, 200f, Color.red);
        }
    }

    /*private IEnumerator OnDiamondsDebitCompletedRoutine(Block block)
    {
        Debug.Log("3 - Diamond Cost Coroutine Finishing Up for Block: " + block.BlockName);

        yield return new WaitForSecondsRealtime(0.35f);

        Clear();
        HideSayDialog();

        EpisodesSpawner.instance.topPanel.HideTopPanel();
        
        //yield return new WaitForSecondsRealtime(0.4f);

        Debug.Log("4 - Diamond Cost Coroutine Calling Block: " + block.BlockName);

        var flowchart = block.GetFlowchart();
        flowchart.StartCoroutine(CallBlock(block));
        gameObject.SetActive(false);
        // Use a coroutine to call the block on the next frame
        // Have to use the Flowchart gameobject as the MenuDialog is now inactive
    }*/
}
