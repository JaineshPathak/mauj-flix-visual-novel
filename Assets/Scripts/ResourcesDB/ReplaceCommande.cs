using UnityEngine;
using Fungus;

public class ReplaceCommande : MonoBehaviour
{
    public Flowchart episodeFlowchart;    

    [ContextMenu("Replace Say Dialogues")]
    public void ReplaceSayDialogues()
    {
        if (episodeFlowchart == null)
            return;

        foreach(Say sayCommand in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if( (sayCommand != null) && sayCommand.enabled && sayCommand.GetType() == typeof(Say))
            {
                Block block = sayCommand.ParentBlock;

                SayMflix sayCommandMFlix = episodeFlowchart.gameObject.AddComponent<SayMflix>();
                sayCommandMFlix.ParentBlock = block;
                sayCommandMFlix.ItemId = episodeFlowchart.NextItemId();

                sayCommandMFlix._Character = sayCommand._Character;
                sayCommandMFlix.Portrait = sayCommand.Portrait;
                sayCommandMFlix.StoryText = sayCommand.StoryText;
                sayCommandMFlix.Description = sayCommand.Description;
                sayCommandMFlix.VoiceOverClip = sayCommand.VoiceOverClip;
                sayCommandMFlix.ShowAlways = sayCommand.ShowAlways;
                sayCommandMFlix.ShowCount = sayCommand.ShowCount;
                sayCommandMFlix.ExtendPrevious = sayCommand.ExtendPrevious;
                sayCommandMFlix.FadeWhenDone = sayCommand.FadeWhenDone;
                sayCommandMFlix.WaitForClick = sayCommand.WaitForClick;
                sayCommandMFlix.WaitForVO = sayCommand.WaitForVO;
                sayCommandMFlix.StopVoiceOver = sayCommand.StopVoiceOver;

                if (sayCommand.setSayDialog != null)
                {
                    sayCommandMFlix.setSayDialog = null;
                    switch(sayCommand.setSayDialog.transform.name)
                    {
                        case "MFlixNarrativeDialog":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_Narrative;
                            break;

                        case "MFlixNarrativeTutorialDialog":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_NarrativeTutorial;
                            break;

                        case "MFlixCharacterSayDialog":
                        case "MFlixCharacterSayDialogx2":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterSay;
                            break;

                        case "MFlixCharacterSayDialogWithMenu":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterSayWithMenu;
                            break;

                        case string s when s.Contains("Black"):
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_NarrativeBlack;
                            break;

                        case string s when s.Contains("PickNameDialogue"):
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterName;
                            break;
                    }
                }

                //block.CommandList.Add(sayCommandMFlix);
                int index = block.CommandList.IndexOf(sayCommand);
                if (index >= 0)
                {
                    block.CommandList[index] = sayCommandMFlix;
                    block.CommandList.Remove(sayCommand);
                    DestroyImmediate(sayCommand);
                }

                //block.CommandList.RemoveAt(sayCommand.CommandIndex);
                //DestroyImmediate(sayCommand);
            }
        }        
    }
}
