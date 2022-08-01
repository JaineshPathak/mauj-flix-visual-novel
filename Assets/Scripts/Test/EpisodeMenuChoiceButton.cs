using UnityEngine;
using Fungus;
using TMPro;

public class EpisodeMenuChoiceButton : MonoBehaviour
{
    [HideInInspector] public EpisodesTesting episodesTesting;
    [HideInInspector] public AdminTestTools adminTesting;

    public Block myTargetBlock;
    public TextMeshProUGUI choicebuttonText;
    public CharReplacerHindi replacerHindi;

    public void OnMenuChoiceClicked()
    {
        if (myTargetBlock == null)
            return;

        if(episodesTesting != null)
            episodesTesting.ExecuteMenuChoice(myTargetBlock);

        if(adminTesting != null)
            adminTesting.ExecuteMenuChoice(myTargetBlock);
    }
}
