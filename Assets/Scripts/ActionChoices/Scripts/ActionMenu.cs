using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/*[System.Serializable]
public class ActionItemChoice
{
    public string choiceText;
    public Block targetBlock;
}*/

[System.Serializable]
public class ActionItem
{
    public string itemName;
    public Sprite itemSprite;
    public ItemAnchorPresets itemAnchorPoint = ItemAnchorPresets.MiddleCenter;    
    //public List<ActionItemChoice> actionItemChoices = new List<ActionItemChoice>();
}

[CommandInfo("Narrative",
                 "Action Menu (Maujflix)",
                 "Displays a single or more items to collect (Designed for Maujflix/StoryPix)")]
public class ActionMenu : Command
{    
    public string centerActionText = "Center Text";

    public List<ActionItem> actionItemsList = new List<ActionItem>();

    //public ActionItem currentActionItemSelected;

    public override void OnEnter()
    {
        if(actionItemsList.Count <= 0)
        {
            Continue();
            return;
        }

        if(ActionMenuUI.instance == null)
        {
            Continue();
            return;
        }

        ActionMenuUI.instance.SetupActionItems(centerActionText, this, actionItemsList.ToArray(), () => 
        {
            Continue();
        });
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 239, 0, 255);
    }

    public override string GetSummary()
    {
        string result = centerActionText;

        //if (actionItemsList.Length > 0 && actionItemsList != null)
            //result = centerActionText + " | Total Actions - " + actionItemsList.Length;

        return result;
    }
}