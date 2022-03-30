using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
                 "Menu (Maujflix)",
                 "Displays a button in a multiple choice menu (modified for Maujflix)")]
public class MenuMflix : Menu
{
    [Tooltip("Whether this menu choice option requires diamonds")]
    [SerializeField] protected bool hasDiamondCost = false;

    [Tooltip("Actual cost of Diamonds")]
    [SerializeField] protected float diamondCost = 0;    

    public override void OnEnter()
    {
        if (setMenuDialog != null)
        {
            // Override the active menu dialog
            MenuDialog.ActiveMenuDialog = setMenuDialog;
        }

        bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

        /*var menuDialog = MenuDialog.GetMenuDialog();              
        if (menuDialog != null)
        {
            menuDialog.SetActive(true);

            var flowchart = GetFlowchart();
            string displayText = flowchart.SubstituteVariables(text);

            if (menuDialog.GetType() == typeof(MenuDialogMflix))
            {
                MenuDialogMflix menuDialogMflix = menuDialog as MenuDialogMflix;
                menuDialogMflix.AddOptionWithCost(displayText, interactable, hideOption, targetBlock, hasDiamondCost, diamondCost);
            }
            else
                menuDialog.AddOption(displayText, interactable, hideOption, targetBlock);
        }*/

        MenuDialogMflix menuDialogMflix = FindObjectOfType<MenuDialogMflix>();
        if(menuDialogMflix != null)
        {
            Debug.Log("MenuDialogMflix : [0] - FOUND!");

            menuDialogMflix.SetActive(true);

            var flowchart = GetFlowchart();
            string displayText = flowchart.SubstituteVariables(text);

            menuDialogMflix.AddOptionWithCost(displayText, interactable, hideOption, targetBlock, hasDiamondCost, diamondCost);
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (targetBlock == null)
        {
            return "Error: No target block selected";
        }

        if (text == "")
        {
            return "Error: No button text selected";
        }

        return "[Cost: " + diamondCost + "] " + text + " : " + targetBlock.BlockName;
    }

    public override Color GetButtonColor()
    {
        return new Color32(184, 60, 235, 255);
    }
}
