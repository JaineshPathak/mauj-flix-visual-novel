using UnityEngine;
using UnityEngine.UI;
using TMPro;
using underDOGS.SDKEvents;
using Firebase.RemoteConfig;

public class UIEpisodeItem : MonoBehaviour
{
    public string episodeString = "एपिसोड";
    public TextMeshProUGUI episodeNumberText;

    [Space(15)]

    public TextMeshProUGUI episodeDescriptionText;
    public CharReplacerHindi episodeHindiFixer;
    //public Text episodeNumberText;

    [Space(15)]

    public string episodeKey;
    public string episodeFileName;    

    [Space(15)]

    public Button playButton;
    public Button lockButton;

    [Space(15)]

    public Image episodeTicketIcon;
    public Image episodePlayIcon;
    public Image episodeLockIcon;

    [Space(15)]

    public Sprite episodePlaySprite;
    public Sprite episodePlayWhiteSprite;
    public Sprite episodeLockSprite;

    private UIStoriesDetailsPanel detailsPanel;
    private StoryData storyData;
    private StoriesDBItem storyItem;
    private EpisodeData episodeData;

    public EpisodeData EpisodesData { get { return episodeData; } }

    private int episodeNumber;

    public void Setup(int num, StoriesDBItem _storyItem, string _episodeKey, string _episodeFileName, StoryData _storyData, UIStoriesDetailsPanel _detailsPanel)
    {
        //episodeNumberText.text = episodeString + " " + num;
        episodeNumber = num;
        episodeNumberText.text =  num.ToString();

        if(FirebaseRemoteConfig.DefaultInstance != null)
        {
            if(FirebaseRemoteConfig.DefaultInstance.GetValue(DataPaths.m_ShortDescriptionStatus).BooleanValue)
            {
                if (!string.IsNullOrEmpty(_storyItem.storyEpisodesDescriptions[num - 1]))
                    episodeDescriptionText.text = episodeHindiFixer.GetFixedText(_storyItem.storyEpisodesDescriptions[num - 1]);
                else
                    episodeDescriptionText.gameObject.SetActive(false);
            }
            else
                episodeDescriptionText.gameObject.SetActive(false);
        }        

        episodeKey = _episodeKey;
        episodeFileName = _episodeFileName;

        storyData = null;
        storyData = _storyData;
        storyItem = _storyItem;

        detailsPanel = _detailsPanel;

        //episodeData = storyData.GetEpisodeDataFromKey(episodeKey);
        episodeData = storyData.GetEpisodeDataFromIndex(num - 1);
        if(episodeData != null)
        {
            if(episodeData.isUnlocked)
            {
                playButton.gameObject.SetActive(true);
                playButton.onClick.AddListener(OnEpisodePlayClick);

                lockButton.gameObject.SetActive(false);
            }
            else
            {
                playButton.gameObject.SetActive(false);

                lockButton.gameObject.SetActive(true);
                lockButton.interactable = false;
            }
        }        
    }

    public void AllowPaymentUnlockable()
    {
        if (episodeTicketIcon == null || lockButton == null)
            return;

        //episodeTicketIcon.gameObject.SetActive(true);

        lockButton.interactable = true;
        episodeLockIcon.sprite = episodePlaySprite;
        //episodeLockIcon.color = Color.yellow;
        episodeLockIcon.rectTransform.sizeDelta = episodePlayIcon.rectTransform.sizeDelta;
        lockButton.onClick.AddListener(OnLockedPaymentButton);
    }

    private void OnLockedPaymentButton()
    {
        //lockButton.interactable = false;
        if (EpisodesSpawner.instance == null)
            return;

        detailsPanel.PlayButtonClickSound();
        detailsPanel.ShowNextEpisodeAskPanel(this);
    }

    public void OnEpisodePlayClick()
    {
        if (detailsPanel == null)
            return;

        if (episodeNumber == 1 && EpisodesSpawner.instance != null)
        {
            if(!EpisodesSpawner.instance.playerData.ContainsStoryStarted(storyItem.storyTitleEnglish) && 
                !EpisodesSpawner.instance.playerData.ContainsStoryCompleted(storyItem.storyTitleEnglish))
            {
                EpisodesSpawner.instance.playerData.AddStoryStarted(storyItem.storyTitleEnglish);
                SaveLoadGame.SavePlayerData(EpisodesSpawner.instance.playerData);
            }
        }

        if (detailsPanel.episodesSpawner != null)
            detailsPanel.episodesSpawner.currentEpisodeNumber = episodeNumber;

        storyData.currentEpisodeKey = episodeKey;
        storyData.currentEpisodeIndex = storyData.GetIndexFromEpisodeData(episodeData);        

        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(episodeFileName, saveString);

        detailsPanel.PlayButtonClickSound();
        detailsPanel.LoadStoryEpisode();

        //Send a "[StoryTitle]_[episodeN]_playbtn" event
        if (SDKManager.instance != null && EpisodesSpawner.instance != null)
        {
            string storyTitleProper = EpisodesSpawner.instance.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventPlayButtonEpisode;
            eventPlayButtonEpisode.eventParameterName = storyTitleProper;
            eventPlayButtonEpisode.eventParameterData = "episode" + episodeNumber.ToString();

            SDKManager.instance.SendEvent(SDKEventsNames.episodePlayBtnEventName, eventPlayButtonEpisode);
        }
    }

    public void OnEpisodePlayClick(bool playSound = true)
    {
        if (detailsPanel == null)
            return;

        if (episodeNumber == 1 && EpisodesSpawner.instance != null)
        {
            if (!EpisodesSpawner.instance.playerData.ContainsStoryStarted(storyItem.storyTitleEnglish) &&
                !EpisodesSpawner.instance.playerData.ContainsStoryCompleted(storyItem.storyTitleEnglish))
            {
                EpisodesSpawner.instance.playerData.AddStoryStarted(storyItem.storyTitleEnglish);
                SaveLoadGame.SavePlayerData(EpisodesSpawner.instance.playerData);
            }
        }

        if (detailsPanel.episodesSpawner != null)
            detailsPanel.episodesSpawner.currentEpisodeNumber = episodeNumber;

        storyData.currentEpisodeKey = episodeKey;
        storyData.currentEpisodeIndex = storyData.GetIndexFromEpisodeData(episodeData);

        string saveString = JsonUtility.ToJson(storyData, true);
        SerializationManager.SaveAsTextFile(episodeFileName, saveString);

        if (playSound)
            detailsPanel.PlayButtonClickSound();

        detailsPanel.LoadStoryEpisode();

        //Send a "[StoryTitle]_[episodeN]_playbtn" event
        if (SDKManager.instance != null && EpisodesSpawner.instance != null)
        {
            string storyTitleProper = EpisodesSpawner.instance.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventPlayButtonEpisode;
            eventPlayButtonEpisode.eventParameterName = storyTitleProper;
            eventPlayButtonEpisode.eventParameterData = "episode" + episodeNumber.ToString();

            SDKManager.instance.SendEvent(SDKEventsNames.episodePlayBtnEventName, eventPlayButtonEpisode);
        }
    }
}
