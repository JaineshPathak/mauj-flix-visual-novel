using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class UIAskQuitPopup : MonoBehaviour
{
    public bool isOn;
    public CanvasGroup popupScreenCanvas;

    [Space(15)]

    public SayDialog sayDialog;
    public string sayText = "कहानी बंद करना चाहते हैं?";

    [Space(15)]
    public Text storyText;

    private EpisodesSpawner episodesSpawner;
    private bool isYesClicked;
    private bool isInTransition;

    private void Start()
    {
        isOn = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isInTransition)
        {
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        if (episodesSpawner == null)
            episodesSpawner = EpisodesSpawner.instance;

        isOn = !isOn;

        if (isOn)
        {
            storyText.text = "";
            popupScreenCanvas.interactable = true;
            popupScreenCanvas.blocksRaycasts = true;
            LeanTween.alphaCanvas(popupScreenCanvas, 1f, 0.5f).setOnStart(() => 
            { 
                isInTransition = true;

                if (sayDialog != null)
                    sayDialog.Say(sayText, true, false, false, false, false, null, null);

            } ).setOnComplete(() => isInTransition = false );
        }
        else
        {
            LeanTween.alphaCanvas(popupScreenCanvas, 0, 0.5f).setOnStart(() => isInTransition = true).setOnComplete( () => 
            {
                isInTransition = false;

                popupScreenCanvas.interactable = false;
                popupScreenCanvas.blocksRaycasts = false;
            } );
        }
    }

    public void OkButtonClicked()
    {
        if (isYesClicked)
            return;

        isYesClicked = true;

        if(episodesSpawner != null)
        {
            if(episodesSpawner.storiesDBItem.isShortStory)
                episodesSpawner.LoadEpisodesMainMenu(false);
            else
                episodesSpawner.LoadEpisodesMainMenu();
        }
    }

    public void NoButtonClicked()
    {
        isYesClicked = false;
        ShowPopup();
    }
}
