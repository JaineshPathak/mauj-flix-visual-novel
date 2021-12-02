using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using underDOGS.SDKEvents;

public class UIEpisodeItem : MonoBehaviour
{
    public string episodeString = "एपिसोड";
    public Text episodeNumberText;

    [Space(15)]

    public string episodeKey;
    public string episodeFileName;    

    [Space(15)]

    public Button playButton;
    public Button lockButton;

    private UIStoriesDetailsPanel detailsPanel;
    private StoryData storyData;
    private StoriesDBItem storyItem;
    private EpisodeData episodeData;

    private int episodeNumber;

    public void Setup(int num, StoriesDBItem _storyItem, string _episodeKey, string _episodeFileName, StoryData _storyData, UIStoriesDetailsPanel _detailsPanel)
    {
        //episodeNumberText.text = episodeString + " " + num;
        episodeNumber = num;
        episodeNumberText.text =  num.ToString();

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

    private void OnEpisodePlayClick()
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
}
