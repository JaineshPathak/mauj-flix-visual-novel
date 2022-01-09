using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using TMPro;

public class EpisodeMenuChoiceButton : MonoBehaviour
{
    [HideInInspector] public EpisodesTesting episodesTesting;

    public Block myTargetBlock;
    public TextMeshProUGUI choicebuttonText;
    public CharReplacerHindi replacerHindi;

    public void OnMenuChoiceClicked()
    {
        if (episodesTesting == null || myTargetBlock == null)
            return;

        episodesTesting.ExecuteMenuChoice(myTargetBlock);        
    }
}
