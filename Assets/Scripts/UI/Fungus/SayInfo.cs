using UnityEngine;
using Fungus;

public class SayInfo : MonoBehaviourSingleton<SayInfo>
{
    //public static SayInfo instance;

    public SayDialog narrativeSayDialogue;
    public SayDialog narrativeTutorialSayDialogue;
    public SayDialog narrativeBlackSayDialogue;
    public SayDialog characterSayDialogue;
    public SayDialog characterSayDialogueWithMenu;
    public SayDialog characterNameDialogueMale;
    public SayDialog characterNameDialogueFemale;

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }*/

    public SayDialog GetSayDialogueByType(SayMflix.SayDialogStyle sayDialogStyle)
    {
        SayDialog sayDialogType = null;

        switch (sayDialogStyle)
        {
            case SayMflix.SayDialogStyle.Type_Narrative:

                if (narrativeSayDialogue)
                    sayDialogType = narrativeSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_NarrativeTutorial:

                if (narrativeTutorialSayDialogue)
                    sayDialogType = narrativeTutorialSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_NarrativeBlack:

                if (narrativeBlackSayDialogue)
                    sayDialogType = narrativeBlackSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_CharacterSay:

                if (characterSayDialogue)
                    sayDialogType = characterSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_CharacterSayWithMenu:

                if (characterSayDialogueWithMenu)
                    sayDialogType = characterSayDialogueWithMenu;

                break;
        }

        return sayDialogType;
    }

    public SayDialog GetSayDialogueByType(SayMflix.SayDialogStyle sayDialogStyle, CharacterGender characterGender, StringVariable nameVariableReference)
    {
        SayDialog sayDialogType = null;

        switch (sayDialogStyle)
        {
            case SayMflix.SayDialogStyle.Type_Narrative:

                if (narrativeSayDialogue)
                    sayDialogType = narrativeSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_NarrativeTutorial:

                if (narrativeTutorialSayDialogue)
                    sayDialogType = narrativeTutorialSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_NarrativeBlack:

                if (narrativeBlackSayDialogue)
                    sayDialogType = narrativeBlackSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_CharacterSay:

                if (characterSayDialogue)
                    sayDialogType = characterSayDialogue;

                break;

            case SayMflix.SayDialogStyle.Type_CharacterSayWithMenu:

                if (characterSayDialogueWithMenu)
                    sayDialogType = characterSayDialogueWithMenu;

                break;

            case SayMflix.SayDialogStyle.Type_CharacterName:

                switch (characterGender)
                {
                    case CharacterGender.Gender_None:
                        break;

                    case CharacterGender.Gender_Male:

                        sayDialogType = characterNameDialogueMale;
                        sayDialogType.GetComponent<MflixPickName>().nameVariableRef = nameVariableReference;

                        break;

                    case CharacterGender.Gender_Female:

                        sayDialogType = characterNameDialogueFemale;
                        sayDialogType.GetComponent<MflixPickName>().nameVariableRef = nameVariableReference;

                        break;
                }

                break;
        }

        return sayDialogType;
    }
}
