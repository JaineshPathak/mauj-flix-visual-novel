using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
                 "Say (Maujflix)",
                 "Writes text in a dialog box. (Modified for Maujflix)")]
[AddComponentMenu("")]
public class SayMflix : Say
{
    public enum SayDialogStyle
    {
        Type_Narrative,
        Type_NarrativeTutorial,
        Type_NarrativeBlack,
        Type_CharacterSay,
        Type_CharacterSayWithMenu,
        Type_CharacterName        
    };

    public CharacterGender characterGender = CharacterGender.Gender_None;
    public SayDialogStyle sayDialogStyle;

    [VariableProperty(typeof(StringVariable))]
    public StringVariable nameVariableRef;    

    public override void OnEnter()
    {
        if (!showAlways && executionCount >= showCount)
        {
            Continue();
            return;
        }

        executionCount++;

        if (SayInfo.instance != null)
        {
            if (characterGender != CharacterGender.Gender_None)
                SayDialog.ActiveSayDialog = SayInfo.instance.GetSayDialogueByType(sayDialogStyle, characterGender, nameVariableRef);
            else
                SayDialog.ActiveSayDialog = SayInfo.instance.GetSayDialogueByType(sayDialogStyle);
        }

        var sayDialog = SayDialog.GetSayDialog();
        if (sayDialog == null)
        {
            Continue();
            return;
        }

        var flowchart = GetFlowchart();

        sayDialog.SetActive(true);

        sayDialog.SetCharacter(character);
        sayDialog.SetCharacterImage(portrait);

        string displayText = storyText;

        var activeCustomTags = CustomTag.activeCustomTags;
        for (int i = 0; i < activeCustomTags.Count; i++)
        {
            var ct = activeCustomTags[i];
            displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
            if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
            {
                displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
            }
        }

        string subbedText = flowchart.SubstituteVariables(displayText);

        if (!sayDialog.gameObject.activeSelf)
            sayDialog.gameObject.SetActive(true);

        sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {
            Continue();
        });
    }

    public override Color GetButtonColor()
    {
        return new Color32(0, 210, 235, 255);
    }
}
