using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;
using System;

public enum CharacterGender
{
    Gender_None,
    Gender_Male,
    Gender_Female
};

[System.Serializable]
public class CharacterDataAssets
{
    [Header("Diamonds Cost")]
    public bool hasDiamondCost;
    public float diamondCost;

    [Header("Characters Data")]
    public Character fungusCharacter;
    public string characterSpriteName;
    public List<Sprite> fungusCharacterPortraits = new List<Sprite>();    
    //public List<string> fungusPortraitsStrings = new List<string>();

    public void PopulatePortraitsList()
    {
        if (fungusCharacter == null)
            return;

        fungusCharacterPortraits.Clear();
        fungusCharacterPortraits = fungusCharacter.Portraits;        
    }
}

public class CharacterSelectionScreen : MonoBehaviour
{
    [Header("Episode Handler")]
    public EpisodesHandler episodesHandler;

    [Header("Scroll UI Contents")]
    public CharacterGender characterGender;
    public Transform characterSelectionContent;
    public List<Image> charactersListFromUI = new List<Image>();
    
    [Header("Load/Save Characters Data")]
    public int currentCharacterIndex = 0;
    public VariableReference characterVariableRef;
    public List<CharacterDataAssets> characterDataAssets = new List<CharacterDataAssets>();
    //public List<Character> characterFungus;

    [Header("UI")]
    public Transform diamondCostPanel;
    public TextMeshProUGUI diamondCostText;
    public Animator buttonShineAnim;

    private EpisodesSpawner episodesSpawner;
    private ScrollSnapRect snapRect;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (charactersListFromUI.Count == 0 && characterSelectionContent)
            charactersListFromUI = characterSelectionContent.GetComponentsInChildren<Image>().ToList();

        if (snapRect == null)
            snapRect = GetComponentInChildren<ScrollSnapRect>();

        if (episodesHandler == null)
            GetEpisodeHandler();        
    }

    private void Start()
    {
        if (EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner != null && episodesSpawner.testModeSingle)
            PopulateCharactersData();

        /*if (!episodesSpawner.storiesDBItem.isReworked)
        {
            if (characterDataAssets.Count > 0)
            {
                for (int i = 0; i < characterDataAssets.Count; i++)
                {
                    characterDataAssets[i].PopulatePortraitsList();
                }
            }
        }*/

        StartCoroutine("SelectUpdateRoutine");

        //SelectCharacter(0);
    }

    public void PopulateCharactersData()
    {
        if (characterDataAssets.Count <= 0)
            return;

        for (int i = 0; i < characterDataAssets.Count; i++)
        {
            characterDataAssets[i].PopulatePortraitsList();
        }
    }

    public void GetEpisodeHandler()
    {
        try
        {
            episodesHandler = transform.parent.GetComponent<EpisodesHandler>();
        }
        catch(NullReferenceException e)
        {
#if UNITY_EDITOR
            Debug.LogError("Cannot get Episode Handler as it is NULL: " + e.Message);
#endif
        }
    }

    //From UI Button
    public void NextCharacter()
    {
        SelectCharacter(currentCharacterIndex + 1);
    }

    //From UI Button
    public void PreviousCharacter()
    {
        SelectCharacter(currentCharacterIndex - 1);
    }

    private void SelectCharacter(int characterIndex)
    {
        characterIndex = Mathf.Clamp(characterIndex, 0, charactersListFromUI.Count - 1);
        currentCharacterIndex = characterIndex;

        if (characterDataAssets.Count <= 0)
            return;

        if(characterDataAssets[currentCharacterIndex].hasDiamondCost)
        {
            if (episodesSpawner == null && EpisodesSpawner.instance != null)
                episodesSpawner = EpisodesSpawner.instance;

            if (episodesSpawner != null)
                episodesSpawner.topPanel.ShowTopPanel();

            /*if (EpisodesSpawner.instance != null)
                EpisodesSpawner.instance.ShowHideDiamondPanel(true, 0.3f);*/

            diamondCostPanel.gameObject.SetActive(true);
            diamondCostText.text = Mathf.RoundToInt(characterDataAssets[currentCharacterIndex].diamondCost).ToString();

            if(buttonShineAnim != null)
                buttonShineAnim.enabled = true;
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
                buttonShineAnim.enabled = false;
        }
    }
    
    public void SelectCharacterFromLoadedData()
    {
        if (episodesHandler == null)
            GetEpisodeHandler();

        Flowchart episodeFlowchart = episodesHandler.episodeFlowchart;

        if (episodeFlowchart == null)
            return;

        if (episodeFlowchart.HasVariable(characterVariableRef.variable.Key))
        {            
            int keyValue = episodeFlowchart.GetIntegerVariable(characterVariableRef.variable.Key);            
            currentCharacterIndex = keyValue;
            CharacterDataAssets characterDataAsset = GetCharacterDataAssetsFromIndex(keyValue);            
            episodesHandler.SelectCharacter(characterGender, characterDataAsset, characterVariableRef, keyValue, false);
        }
    }

    public CharacterDataAssets GetCharacterDataAssetsFromIndex(int index)
    {
        if (characterDataAssets.Count <= 0)
            return null;

        return characterDataAssets[index];
    }

    public void OnCharacterSubmit()
    {
        if (episodesHandler == null)
            return;

        if (EpisodesSpawner.instance && EpisodesSpawner.instance.testModeSingle)
        {
            StopCoroutine("SelectUpdateRoutine");

            episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);

            MessageReceivedMFlix[] messageReceiversMaujflixTest = episodesHandler.episodeFlowchart.GetComponentsInChildren<MessageReceivedMFlix>();
            if (messageReceiversMaujflixTest.Length > 0)
            {
                for (int i = 0; i < messageReceiversMaujflixTest.Length; i++)
                {
                    if (messageReceiversMaujflixTest[i] != null)
                        messageReceiversMaujflixTest[i].OnFungusMessageReceivedForCharacter("CharacterSelected", this);
                }
            }

            EpisodesSpawner.instance.topPanel.HideTopPanel(0.3f, 0.7f);

            return;
        }

        if ( /*FirebaseFirestoreHandler.instance == null*/ FirebaseFirestoreOffline.instance == null)
            return;

        MessageReceivedMFlix[] messageReceiversMaujflix = episodesHandler.episodeFlowchart.GetComponentsInChildren<MessageReceivedMFlix>();

        //If the current character costs Diamonds
        if (characterDataAssets[currentCharacterIndex].hasDiamondCost)
        {
            if (episodesSpawner == null && EpisodesSpawner.instance != null)
                episodesSpawner = EpisodesSpawner.instance;

            /*if (FirebaseFirestoreHandler.instance.GetUserDiamondsAmountInt() < characterDataAssets[currentCharacterIndex].diamondCost)
                return;*/

            //If users Diamond amounts is less than current diamond cost, then return
            if (FirebaseFirestoreOffline.instance.GetDiamondsAmountInt() < characterDataAssets[currentCharacterIndex].diamondCost)
                return;

            //Debit the diamonds and select the character
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDebit(diamondCostPanel, episodesSpawner.topPanel.diamondsPanelIcon, (int)characterDataAssets[currentCharacterIndex].diamondCost, (int)characterDataAssets[currentCharacterIndex].diamondCost, () =>
            {
                if (episodesSpawner == null && EpisodesSpawner.instance != null)
                    episodesSpawner = EpisodesSpawner.instance;

                if (episodesSpawner != null)
                    episodesSpawner.topPanel.HideTopPanel(0.3f, 0.7f);

                episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);

                if(messageReceiversMaujflix.Length > 0)
                {
                    for (int i = 0; i < messageReceiversMaujflix.Length; i++)
                    {
                        if (messageReceiversMaujflix[i] != null)
                            messageReceiversMaujflix[i].OnFungusMessageReceivedForCharacter("CharacterSelected", this);
                    }
                }
            }, 200f, Color.red);

            //FirebaseFirestoreOffline.instance.DebitDiamondsAmount(characterDataAssets[currentCharacterIndex].diamondCost);
            //episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);
        }
        else
        {
            //if (EpisodesSpawner.instance != null)
                //EpisodesSpawner.instance.ShowHideDiamondPanel(false, 0.3f);

            if (episodesSpawner != null)
                episodesSpawner.topPanel.HideTopPanel(0.3f, 0.7f);

            episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);

            if (messageReceiversMaujflix.Length > 0)
            {
                for (int i = 0; i < messageReceiversMaujflix.Length; i++)
                {
                    if (messageReceiversMaujflix[i] != null)
                        messageReceiversMaujflix[i].OnFungusMessageReceivedForCharacter("CharacterSelected", this);
                }
            }
        }

        StopCoroutine("SelectUpdateRoutine");

        //episodesHandler.SelectCharacter(characterGender, characterFungus[currentCharacterIndex], characterVariableRef, currentCharacterIndex);
    }

    private IEnumerator SelectUpdateRoutine()
    {
        while(true)
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

    //=======================================================================================================================================

    public void ShowSelectionScreen(string _topText)
    {
        if (UICharacterSelection.instance == null)
            return;

        if (AtlasDB.instance == null)
            return;

        List<Sprite> characterSprites = new List<Sprite>();
        for (int i = 0; i < characterDataAssets.Count && (characterDataAssets.Count > 0); i++)
        {
            if (characterDataAssets[i].characterSpriteName.Length > 0)
                characterSprites.Add(AtlasDB.instance.charactersAtlas.GetSprite(characterDataAssets[i].characterSpriteName));
        }

        IntegerVariable integerVariable = characterVariableRef.variable as IntegerVariable;
        UICharacterSelection.instance.ShowSelectionScreen(_topText, characterSprites.ToArray(), characterDataAssets, integerVariable, (int index) =>
        {
            currentCharacterIndex = index;
            OnCharacterSubmit();

            /*MessageReceivedMFlix[] messageReceiversMaujflixTest = episodesHandler.episodeFlowchart.GetComponentsInChildren<MessageReceivedMFlix>();
            if (messageReceiversMaujflixTest.Length > 0)
            {
                for (int i = 0; i < messageReceiversMaujflixTest.Length; i++)
                {
                    if (messageReceiversMaujflixTest[i] != null)
                        messageReceiversMaujflixTest[i].OnFungusMessageReceivedForCharacter("CharacterSelected", this);
                }
            }*/
        });
    }

    //=======================================================================================================================================
}