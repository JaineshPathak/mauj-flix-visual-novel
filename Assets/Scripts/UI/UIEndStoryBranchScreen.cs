using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using underDOGS.SDKEvents;

public class UIEndStoryBranchScreen : MonoBehaviour
{
    public CanvasGroup endScreenBranchCanvasGroup;

    [Space(15f)]

    public SayDialog sayDialog;
    public string sayText = "कहानी अनुभवने के लिए शुक्रिया।\\nहर कहानी के एक से ज्यादा एंडिंग्स हैं, तो कहानी दोबारा खेलकर सारी एंडिंग्स जरूर खेलिएगा।\\nधन्यवाद।";

    public SayDialog secondSayDialog;
    public string secondText = "दूसरी एंडिंग खेलना चाहेंगे?";

    [Space(15f)]

    public Image congralutionsImage;
    public ParticleSystem endParticleEffect;

    [Space(15)]

    public Transform endStoryPanel;
    public Button yesButton;
    public Button noButton;

    private bool isTriggered;

    private LTSeq endSeq;

    private void Awake()
    {
        sayText = sayText.Replace("\\n", "\n");

        endScreenBranchCanvasGroup.interactable = false;
        endScreenBranchCanvasGroup.blocksRaycasts = false;

        congralutionsImage.transform.localScale = Vector3.zero;
        endStoryPanel.transform.localScale = Vector3.zero;
        yesButton.transform.localScale = Vector3.zero;
        noButton.transform.localScale = Vector3.zero;
    }

    public void PlayEndingStoryBranchScreen()
    {
        if (isTriggered)
            return;

        isTriggered = true;

        if (!endScreenBranchCanvasGroup.gameObject.activeSelf)
            endScreenBranchCanvasGroup.gameObject.SetActive(true);

        endScreenBranchCanvasGroup.interactable = true;
        endScreenBranchCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        endSeq.append(LeanTween.alphaCanvas(endScreenBranchCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congralutionsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            if (endParticleEffect != null)
                endParticleEffect.Play();
        }));
        endSeq.append(LeanTween.scale(endStoryPanel.gameObject, Vector3.one, 0.5f).setDelay(1f).setEase(LeanTweenType.easeOutBack).setOnComplete(OnEndStoryPanelUp));
    }

    private void OnEndStoryPanelUp()
    {
        if (sayDialog != null)
            sayDialog.Say(sayText, true, false, false, false, false, null, OnSayTextComplete);
    }

    private void OnSayTextComplete()
    {
        if (secondSayDialog != null)
        {
            if (secondSayDialog.GetComponent<CanvasGroup>() != null)
                secondSayDialog.GetComponent<CanvasGroup>().alpha = 1f;

            secondSayDialog.Say(secondText, true, false, false, false, false, null, null);
        }

        LeanTween.scale(yesButton.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(noButton.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack);
    }

    public void OnYesClicked()
    {
        if (EpisodesSpawner.instance == null)
            return;

        //Send a "[StoryTitle]_episodeN_branchending_yesbtn_clicked" event
        if(SDKManager.instance != null)
        {
            string storyTitleProper = EpisodesSpawner.instance.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData yesDataEvent;
            yesDataEvent.eventParameterName = storyTitleProper;
            yesDataEvent.eventParameterData = "episode" + EpisodesSpawner.instance.currentEpisodeNumber;

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventNameYes, yesDataEvent);
        }

        yesButton.interactable = false;
        noButton.interactable = false;

        EpisodesSpawner.instance.StartLoadingStoryScene();
    }

    public void OnNoClicked()
    {
        if (EpisodesSpawner.instance == null)
            return;

        //Send a "[StoryTitle]_episodeN_branchending_nobtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = EpisodesSpawner.instance.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData noDataEvent;
            noDataEvent.eventParameterName = storyTitleProper;
            noDataEvent.eventParameterData = "episode" + EpisodesSpawner.instance.currentEpisodeNumber;

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventNameNo, noDataEvent);
        }

        EpisodesSpawner.instance.LoadEpisodesMainMenu(false);
    }    
}
