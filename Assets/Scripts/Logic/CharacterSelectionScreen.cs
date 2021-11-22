using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public enum CharacterGender
{
    Gender_Male,
    Gender_Female
};

[System.Serializable]
public class CharacterDataAssets
{
    public Character fungusCharacter;
    public List<Sprite> fungusCharacterPortraits = new List<Sprite>();
    public List<string> fungusPortraitsStrings = new List<string>();

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

        //episodesHandler.SelectCharacter(characterGender, characterFungus[currentCharacterIndex], characterVariableRef, currentCharacterIndex);
        episodesHandler.SelectCharacter(characterGender, characterDataAssets[currentCharacterIndex], characterVariableRef, currentCharacterIndex, true);
    }
}