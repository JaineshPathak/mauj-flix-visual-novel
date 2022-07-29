using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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
    public Block itemTargetBlock;
    //public List<ActionItemChoice> actionItemChoices = new List<ActionItemChoice>();
}

[CommandInfo("Narrative",
                 "Action Menu (Maujflix)",
                 "Displays a single or more items to collect (Designed for Maujflix/StoryPix)")]
public class ActionMenu : Command
{    
    public string centerActionText = "Center Text";

    public List<ActionItem> actionItemsList = new List<ActionItem>();

    [HideInInspector] public ActionItem currentActionItemSelected;

    public void ActionMenuInit(string newCenterText, List<ActionItem> newActionsItems)
    {
        centerActionText = newCenterText;
        //actionItemsList.AddRange(newActionsItems);

        foreach (ActionItem a in newActionsItems)
            actionItemsList.Add(a);
    }

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
            if (currentActionItemSelected != null && currentActionItemSelected.itemTargetBlock != null)
            {
#if UNITY_EDITOR
                Debug.Log($"Moving Branch to -> {currentActionItemSelected.itemTargetBlock.BlockName}");
#endif
                var block = currentActionItemSelected.itemTargetBlock;
                EventSystem.current.SetSelectedGameObject(null);
                StopAllCoroutines();

                // Stop timeout
                if (block != null)
                {
                    var flowchart = block.GetFlowchart();
                    flowchart.StartCoroutine(CallBlock(block));
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"Moving Branch to -> Continue()");
#endif

                Continue();
            }
        });
    }

    protected IEnumerator CallBlock(Block block)
    {
        yield return new WaitForEndOfFrame();
        block.StartExecution();
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

    public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
    {
        /*if (targetBlock != null)
        {
            connectedBlocks.Add(targetBlock);
        }*/

        for (int i = 0; i < actionItemsList.Count && (actionItemsList.Count > 0); i++)
        {
            if(actionItemsList[i].itemTargetBlock != null)
                connectedBlocks.Add(actionItemsList[i].itemTargetBlock);
        }
    }
}