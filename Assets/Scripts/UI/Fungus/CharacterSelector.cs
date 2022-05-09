using UnityEngine;
using Fungus;

[CommandInfo("UI",
                 "Select Character",
                 "Selects a Characters. (Made for Maujflix)")]
public class CharacterSelector : Command
{
    [SerializeField] protected string topText = "अपना रूप चुने";    
    [SerializeField] protected CharacterSelectionScreen selectionScreen;

    public CharacterSelectionScreen SelectionScreen 
    {
        get { return selectionScreen; }        
    }    

    public override void OnEnter()
    {
        if(selectionScreen == null)
        {
            Continue();
            return;
        }

        selectionScreen.ShowSelectionScreen(topText);
    }

    public override string GetSummary()
    {
        return (selectionScreen != null) ? "Screen: " + selectionScreen.name : "Screen: None";
    }

    public override Color GetButtonColor()
    {
        return new Color32(42, 204, 131, 255);
    }
}
