using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using underDOGS.SDKEvents;

public class SDKEventsSender : MonoBehaviour
{
    public TMP_Dropdown currentStoryDropdown;
    public TMP_Dropdown currentEpisodeDropdown;

    [Space(15)]

    public string currentStoryName;
    public string currentStoryEpisodeNum;

    [Space(15)]

    public int cooldownTimer = 10;
    public Transform cooldownPanel;
    public TextMeshProUGUI cooldownText;

    [Space(15)]

    public Button thumbnailClickedButtonMale;
    public Button thumbnailClickedButtonFemale;

    [Space(15)]

    public Button topBannerClickedButton;

    [Space(15)]

    public Button shareButtonClickedButton;

    [Space(15)]

    public Button episodePlayButton;
    public Button episodeCompletedButton;
    public Button episodeCompletedHomeButton;

    [Space(15)]

    public Button storyCompletedButton;

    [Space(15)]

    public Button branchEndingButton;
    public Button branchEndingYesButton;
    public Button branchEndingNoButton;

    private void Start()
    {
        if (currentStoryDropdown)
        {
            currentStoryDropdown.onValueChanged.AddListener(OnCurrentStoryVal);
            OnCurrentStoryVal(0);
        }

        if (currentEpisodeDropdown)
        {
            currentEpisodeDropdown.onValueChanged.AddListener(OnCurrentEpisodeVal);
            OnCurrentEpisodeVal(0);
        }

        thumbnailClickedButtonMale.onClick.AddListener(OnThumbnailClickedMale);
        thumbnailClickedButtonFemale.onClick.AddListener(OnThumbnailClickedFemale);

        topBannerClickedButton.onClick.AddListener(OnTopBannerPlayButtonClicked);

        shareButtonClickedButton.onClick.AddListener(OnShareButtonClicked);

        episodePlayButton.onClick.AddListener(OnEpisodePlayButtonClicked);
        episodeCompletedButton.onClick.AddListener(OnEpisodeCompleted);
        episodeCompletedHomeButton.onClick.AddListener(OnEpisodeHomeButtonClicked);

        storyCompletedButton.onClick.AddListener(OnStoryCompleted);

        branchEndingButton.onClick.AddListener(OnBranchEndingCompleted);
        branchEndingYesButton.onClick.AddListener(OnBranchEndingYesClicked);
        branchEndingNoButton.onClick.AddListener(OnBranchEndingNoClicked);
    }

    private void OnCurrentStoryVal(int index)
    {
        if (currentStoryDropdown == null)
            return;

        currentStoryName = currentStoryDropdown.options[index].text;
        //currentStoryName = currentStoryName.Replace(":", "");
        //currentStoryName = currentStoryName.Replace(" ", "");
    }

    private void OnCurrentEpisodeVal(int index)
    {
        if (currentEpisodeDropdown == null)
            return;

        currentStoryEpisodeNum = currentEpisodeDropdown.options[index].text;
    }

    //-==================================================

    public void OnThumbnailClickedMale()
    {
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "_");

            SDKEventStringData thumbnailsClickedData;
            thumbnailsClickedData.eventParameterName = SDKEventsNames.storyParameterName;
            thumbnailsClickedData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent("Male_" + SDKEventsNames.thumbnailsClickedEventName, thumbnailsClickedData);
        }

        StartCoroutine(CooldownTimer());
    }

    public void OnThumbnailClickedFemale()
    {
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "_");

            SDKEventStringData thumbnailsClickedData;
            thumbnailsClickedData.eventParameterName = SDKEventsNames.storyParameterName;
            thumbnailsClickedData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent("Female_" + SDKEventsNames.thumbnailsClickedEventName, thumbnailsClickedData);
        }

        StartCoroutine(CooldownTimer());
    }

    //-==================================================

    public void OnTopBannerPlayButtonClicked()
    {
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventPlayBtn;
            eventPlayBtn.eventParameterName = SDKEventsNames.storyParameterName;
            eventPlayBtn.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.topBannerEventNameStart, eventPlayBtn);
        }

        StartCoroutine(CooldownTimer());
    }

    //-===================================================

    public void OnShareButtonClicked()
    {
        //Send a "[StoryTitle]_sharebtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData shareBtnData;
            shareBtnData.eventParameterName = SDKEventsNames.storyParameterName;
            shareBtnData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.shareButtonEventName, shareBtnData);
        }

        StartCoroutine(CooldownTimer());
    }

    //-===================================================

    public void OnEpisodePlayButtonClicked()
    {
        //Send a "[StoryTitle]_[episodeN]_playbtn" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventPlayButtonEpisode;
            eventPlayButtonEpisode.eventParameterName = storyTitleProper;
            eventPlayButtonEpisode.eventParameterData = "episode" + currentStoryEpisodeNum;

            SDKManager.instance.SendEvent(SDKEventsNames.episodePlayBtnEventName, eventPlayButtonEpisode);
        }

        StartCoroutine(CooldownTimer());
    }

    public void OnEpisodeCompleted()
    {
        //Send "episode_completed" Event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventEpisodeData eventEpisodeData;
            eventEpisodeData.storyTitle = storyTitleProper;
            eventEpisodeData.episodeNum = int.Parse(currentStoryEpisodeNum);

            SDKManager.instance.SendEvent(eventEpisodeData);
        }

        StartCoroutine(CooldownTimer());
    }

    public void OnEpisodeHomeButtonClicked()
    {
        //Send a Eg: "Udaan_episode1_completed_homebtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData episodeHomeClickedData;
            episodeHomeClickedData.eventParameterName = storyTitleProper;
            episodeHomeClickedData.eventParameterData = "episode" + currentStoryEpisodeNum;

            SDKManager.instance.SendEvent(SDKEventsNames.episodeCompleteHomeEventName, episodeHomeClickedData);
        }

        StartCoroutine(CooldownTimer());
    }

    //-===================================================

    public void OnStoryCompleted()
    {
        if (SDKManager.instance != null)
        {
            //Send "story_completed" Event
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventStoryCompleteData;
            eventStoryCompleteData.eventParameterName = SDKEventsNames.storyParameterName;
            eventStoryCompleteData.eventParameterData = storyTitleProper;
            SDKManager.instance.SendEvent(SDKEventsNames.storyCompleteEventName, eventStoryCompleteData);
        }

        StartCoroutine(CooldownTimer());
    }

    //-===================================================

    public void OnBranchEndingCompleted()
    {
        //Send a "[StoryTitle]_episodeN_branchending_completed" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData branchEventData;
            branchEventData.eventParameterName = storyTitleProper;
            branchEventData.eventParameterData = "episode" + currentStoryEpisodeNum;

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventName, branchEventData);
        }

        StartCoroutine(CooldownTimer());
    }

    public void OnBranchEndingYesClicked()
    {
        //Send a "[StoryTitle]_episodeN_branchending_yesbtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData yesDataEvent;
            yesDataEvent.eventParameterName = storyTitleProper;
            yesDataEvent.eventParameterData = "episode" + currentStoryEpisodeNum;

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventNameYes, yesDataEvent);
        }

        StartCoroutine(CooldownTimer());
    }

    public void OnBranchEndingNoClicked()
    {
        //Send a "[StoryTitle]_episodeN_branchending_nobtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = currentStoryName;
            storyTitleProper = storyTitleProper.Replace(":", "");
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData noDataEvent;
            noDataEvent.eventParameterName = storyTitleProper;
            noDataEvent.eventParameterData = "episode" + currentStoryEpisodeNum;

            SDKManager.instance.SendEvent(SDKEventsNames.branchEndEventNameNo, noDataEvent);
        }

        StartCoroutine(CooldownTimer());
    }

    //-===================================================

    private IEnumerator CooldownTimer()
    {
        cooldownPanel.gameObject.SetActive(true);

        cooldownText.text = cooldownTimer.ToString();

        for (int i = cooldownTimer; i > 0; i--)
        {
            cooldownText.text = i.ToString();

            yield return new WaitForSeconds(1f);
        }

        cooldownPanel.gameObject.SetActive(false);
    }

    //-===================================================
}
