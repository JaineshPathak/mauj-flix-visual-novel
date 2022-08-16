using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class UICharacterSelection : MonoBehaviourSingleton<UICharacterSelection>
{
    //public static UICharacterSelection instance;

    [Header("Top Texts")]
    public TextMeshProUGUI topText;
    public CharReplacerHindi topTextHindiFixer;

    [Header("Canvas")]
    public CanvasGroup canvasGroup;
    public ScrollSnapRect snapRect;
    public Image[] characterImagesList;

    [Header("Character Data Assets")]
    public List<CharacterDataAssets> characterDataAssets = new List<CharacterDataAssets>();

    [Header("Buttons")]
    public Button submitCharacterButton;
    public Transform diamondCostPanel;
    public TextMeshProUGUI diamondCostText;
    public Animator buttonShineAnim;

    [Space(15)]

    public Button prevButton;
    public Button nextButton;

    [Header("Debugging")]
    public int currentCharacterIndex;
    public EpisodesHandler episodesHandler;
    public IntegerVariable integerVariableRef;

    private Action<int, Transform> callback;

    private EpisodesSpawner episodesSpawner;    

    private void OnValidate()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        /*if (instance == null)
            instance = this;
        else
            Destroy(gameObject);*/

        if(canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (snapRect == null)
            snapRect = GetComponentInChildren<ScrollSnapRect>();

        if (nextButton)
            nextButton.onClick.AddListener(NextCharacter);

        if (prevButton)
            prevButton.onClick.AddListener(PreviousCharacter);

        if (submitCharacterButton)
            submitCharacterButton.onClick.AddListener(OnCharacterSubmit);
    }

    public void ShowSelectionScreen(string _topText, Sprite[] _characterSprites, List<CharacterDataAssets> _characterDataAssets, IntegerVariable _integerVarRef, Action<int, Transform> _callback)
    {
        characterDataAssets.Clear();
        characterDataAssets = _characterDataAssets;

        if(canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            LeanTween.alphaCanvas(canvasGroup, 1f, 1f).setEaseInOutSine();

            StartCoroutine("SelectUpdateRoutine");
        }

        if(_topText.Length > 0 && topText)
        {
            topText.text = _topText;
            topTextHindiFixer.UpdateMe();
        }

        for (int i = 0; i < characterImagesList.Length && (_characterSprites.Length > 0); i++)
            characterImagesList[i].sprite = _characterSprites[i];

        integerVariableRef = _integerVarRef;

        callback = _callback;
    }

    public void HideSelectionScreen()
    {
        if (canvasGroup == null)
            return;

        if (canvasGroup.alpha <= 0)
            return;

        StopCoroutine("SelectUpdateRoutine");

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(canvasGroup, 0, 1f).setEaseInOutSine();
    }

    public void HideSelectionScreen(Action onComplete)
    {
        if (canvasGroup == null)
            return;

        if (canvasGroup.alpha <= 0)
            return;

        StopCoroutine("SelectUpdateRoutine");

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(canvasGroup, 0, 1f).setEaseInOutSine().setOnComplete( () => onComplete?.Invoke() );
    }

    private void NextCharacter()
    {
        SelectCharacter(currentCharacterIndex + 1);
    }

    private void PreviousCharacter()
    {
        SelectCharacter(currentCharacterIndex - 1);
    }

    private void SelectCharacter(int characterIndex)
    {
        characterIndex = Mathf.Clamp(characterIndex, 0, characterImagesList.Length - 1);
        currentCharacterIndex = characterIndex;

        if (characterDataAssets.Count <= 0)
            return;

        if (characterDataAssets[currentCharacterIndex].hasDiamondCost)
        {
            if (episodesSpawner == null && EpisodesSpawner.instance != null)
                episodesSpawner = EpisodesSpawner.instance;

            if (episodesSpawner != null)
                episodesSpawner.topPanel.ShowTopPanel();

            /*if (EpisodesSpawner.instance != null)
                EpisodesSpawner.instance.ShowHideDiamondPanel(true, 0.3f);*/

            diamondCostPanel.gameObject.SetActive(true);
            diamondCostText.text = Mathf.RoundToInt(characterDataAssets[currentCharacterIndex].diamondCost).ToString();

            if (buttonShineAnim != null)
            {
                buttonShineAnim.enabled = true;
                buttonShineAnim.Rebind();
                buttonShineAnim.Update(0);
            }
        }
        else
        {
            /*if (EpisodesSpawner.instance != null)
                EpisodesSpawner.instance.ShowHideDiamondPanel(false, 0.3f);*/

            if (episodesSpawner != null)
                episodesSpawner.topPanel.HideTopPanel();

            diamondCostPanel.gameObject.SetActive(false);
            diamondCostText.text = "";

            if (buttonShineAnim != null)
            {
                buttonShineAnim.Rebind();
                buttonShineAnim.Update(0);
                buttonShineAnim.enabled = false;
            }
        }
    }

    private void OnCharacterSubmit()
    {
        callback?.Invoke(currentCharacterIndex, diamondCostPanel);

#if UNITY_EDITOR
        if(UITopMenuButton.instance != null && EpisodesSpawner.instance != null)
        {
            //Disable to Wardrobe button if not allowed. (This is used for old Stories)
            /*if (EpisodesSpawner.instance.storiesDBItem == null || !EpisodesSpawner.instance.storiesDBItem.allowWardrobeChange)
            {
                UITopMenuButton.instance.WardrobeButtonStatus = false;
                return;
            }*/

            if (episodesHandler == null)
                episodesHandler = FindObjectOfType<EpisodesHandler>();

            if (episodesHandler != null && episodesHandler.EpisodeDataCurrent != null)
            {
                if(!episodesHandler.EpisodeDataCurrent.allowClothesChange)
                {
                    episodesHandler.EpisodeDataCurrent.allowClothesChange = true;
                    episodesHandler.SaveStoryData();

                    UITopMenuButton.instance.WardrobeButtonStatus = episodesHandler.EpisodeDataCurrent.allowClothesChange;
                }
            }
        }
#endif
        //HideSelectionScreen();
    }

    private IEnumerator SelectUpdateRoutine()
    {
        while (true)
        {
            if (canvasGroup != null && canvasGroup.alpha == 1 && canvasGroup.interactable && canvasGroup.blocksRaycasts)
            {
                if (SwipeManager.IsSwipingLeft())
                {
                    if (snapRect)
                    {
                        snapRect.NextScreen();
                        NextCharacter();
                    }
                }

                if (SwipeManager.IsSwipingRight())
                {
                    if (snapRect)
                    {
                        snapRect.PreviousScreen();
                        PreviousCharacter();
                    }
                }
            }

            yield return null;
        }
    }
}
