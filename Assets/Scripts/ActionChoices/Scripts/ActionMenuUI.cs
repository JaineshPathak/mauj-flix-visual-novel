using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviourSingleton<ActionMenuUI>
{
    public CanvasGroup canvasGroup;
    public Button[] cachedChoiceButtons;

    private void OnValidate()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetupActionItems(string centerTitleText, ActionItem[] actionItemsList)
    {
        if (actionItemsList.Length <= 0)
            return;
    }
}
