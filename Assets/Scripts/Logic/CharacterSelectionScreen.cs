using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public enum CharacterGender
{
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

    private void Awake()
    {
        if (charactersListFromUI.Count == 0)
            charactersListFromUI = characterSelectionContent.GetComponentsInChildren<Image>().ToList();

        if (episodesHandler == null)
            GetEpisodeHandler();

        if(characterDataAssets.Count > 0)
        {
            for (int i = 0; i < characterDataAssets.Count; i++)
            {
                characterDataAssets[i].PopulatePortraitsList();
            }
        }
    }

    private void Start()
    {
        SelectCharacter(0);
    }

    public void GetEpisodeHandler()
    {
        episodesHandler = transform.parent.GetComponent<EpisodesHandler>();
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

        if(characterDataAssets[currentCharacterIndex].hasDiamondCost)
        {
            diamondCostPanel.gameObject.SetActive(true);
            diamondCostText.text = Mathf.RoundToInt(characterDataAssets[currentCharacterIndex].diamondCost).ToString();
        }
        else
        {
            diamondCostPanel.gameObject.SetActive(false);
            diamondCostText.text = "";
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

        if (FirebaseFirestoreHandler.instance == null)
            return;

        //If the current character costs Diamonds
        if (characterDataAssets[currentCharacterIndex].hasDiamondCost)
        {
            //If users Diamond amounts is less than current diamond cost, then return
            if (FirebaseFirestoreHandler.instance.GetUserDiamondsAmountInt() < characterDataAssets[currentCharacterIndex].diamondCost)
                return;

            //Debit the diamonds and select the character
            FirebaseFirestoreHandler.instance.DebitDiamondsAmount(characterDataAssets[currentCharacterIndex].diamondCost);
            episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);
        }
        else
            episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);

        //episodesHandler.SelectCharacter(characterGender, characterFungus[currentCharacterIndex], characterVariableRef, currentCharacterIndex);
    }
}