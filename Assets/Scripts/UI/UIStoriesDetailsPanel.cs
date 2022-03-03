using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using underDOGS.SDKEvents;

public class UIStoriesDetailsPanel : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource uISoundSource;
    public AudioClip buttonClickSound;

    [Header("Tweens")]
    public LeanTweenType easeType = LeanTweenType.easeInOutSine;

    [Header("Story Section")]
    public ScrollRect mainScrollRect;
    public RectTransform buttonsVerticalGroup;
    public ContentSizeFitter buttonsGroupSizeFitter;
    public Text storyTitleText;
    public Text storyDescriptionText;
    public Image storyThumbnailBigImage;
    public Image storyThumbnailTitleImage;
    [HideInInspector] public Sprite storyThumbnailLoadingImage;

    [Space(15)]

    public UIShareButton shareButton;

    [Header("Episodes Section")]
    public UIEpisodeItem episodeItemPrefab;
    public Transform episodeContainer;
    public RectTransform storyEpisodesContainer;
    public Transform moreLikeThisSection;

    [Header("Like Story Section")]
    public GameObject likeButtonParent;
    public Image likeOutlineImage;
    public Image likeImage;
    public TextMeshProUGUI likesCountText;

    [Header("Views Section")]
    public GameObject viewsCountParent;
    public TextMeshProUGUI viewsCountText;

    [Header("Story Loading Screen")]
    public CanvasGroup loadingBufferScreen;
    public EpisodesSpawner episodesSpawner;
    public Button startNowButton;

    private bool isShown;
    private StoryData storyData;
    private StoriesDBItem storyItem;

    private LTSeq moveSeq;

    private RectTransform episodesContainerRect;

    public static event Action<string> OnStoryLiked;
    public static event Action<string> OnStoryUnliked;

    private void Start()
    {
        if (episodesSpawner == null)
        {
            episodesSpawner = EpisodesSpawner.instance;
            episodesSpawner.topPanel.ShowTopPanel();
        }

        episodesContainerRect = episodeContainer.GetComponent<RectTransform>();

        if (episodesSpawner.immediateOpenDetailsPanel && episodesSpawner.storiesDBItem != null && episodesSpawner.storyData != null)
        {
            storyTitleText.text = episodesSpawner.storyTitle;
            if (storyTitleText.GetComponent<SiddhantaFixer>() != null)
                storyTitleText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = false;
            buttonsGroupSizeFitter.SetLayoutVertical();

            storyDescriptionText.text = episodesSpawner.storyDescription;
            if (storyDescriptionText.GetComponent<SiddhantaFixer>() != null)
                storyDescriptionText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = true;

            if (shareButton && episodesSpawner)
                shareButton.Setup(episodesSpawner.storyTitle, episodesSpawner.storyTitleEnglish, episodesSpawner.storyThumbnailSmallSprite);

            storyThumbnailBigImage.sprite = episodesSpawner.storyThumbnailBigSprite;
            storyThumbnailTitleImage.sprite = episodesSpawner.storyloadingTitleImage.sprite;

            storyItem = episodesSpawner.storiesDBItem;
            storyData = episodesSpawner.storyData;

            startNowButton.onClick.RemoveAllListeners();
            startNowButton.onClick.AddListener(LoadStoryEpisodeStartNow);

            if (episodesSpawner.playerData.HasStoryLiked(storyItem.storyTitleEnglish))
            {
                likeImage.gameObject.SetActive(true);
                likeOutlineImage.gameObject.SetActive(false);
            }
            else
            {
                likeImage.gameObject.SetActive(false);
                likeOutlineImage.gameObject.SetActive(true);
            }

            if (FirebaseDBHandler.instance != null)
            {
                if (!likeButtonParent.activeSelf)
                    likeButtonParent.SetActive(true);

                if (!viewsCountParent.activeSelf)
                    viewsCountParent.SetActive(true);

                FirebaseDBHandler.instance.GetCountFromFirebaseDB(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd), val => likesCountText.text = val);
                FirebaseDBHandler.instance.GetCountFromFirebaseDB(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.viewCountKeyEnd), val => viewsCountText.text = val);
            }
            else
            {
                if (likeButtonParent.activeSelf)
                    likeButtonParent.SetActive(false);

                if (viewsCountParent.activeSelf)
                    viewsCountParent.SetActive(false);
            }            

            ShowPanel();
        }
        else if (episodesSpawner.blackScreenCanvasGroup.alpha > 0)
            episodesSpawner.FadeOutBlackScreen();        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isShown)
            {
                MoveHidePanel();
            }
            else
            {
                Application.Quit();
            }
        }
    }

    private void ShowPanel()
    {
        isShown = !isShown;
        MovePanel(isShown, 0.001f);

        episodesSpawner.FadeOutBlackScreen();
    }

    public void MoveHidePanel()
    {
        isShown = !isShown;
        MovePanel(isShown);
    }

    public void MoveHidePanel(string _storyTitle, string _storyTitleEng, string _storyDescription, Sprite _thumbnailSmallSprite, Sprite _thumbnailBigSprite, Sprite _thumbnailTitleSprite, Sprite _thumbnailLoadingSprite, StoriesDBItem _storiesDBItem, StoryData _storyData)
    {
        isShown = !isShown;

        if(storyItem == null)
            storyItem = _storiesDBItem;

        if (storyData == null)
            storyData = _storyData;

        //If it's a short story (only 1 episode) then load immediately
        if(storyItem.isShortStory)
        {
            isShown = false;

            episodesSpawner.storyTitle = _storyTitle;
            episodesSpawner.storyTitleEnglish = _storyTitleEng;
            episodesSpawner.storyDescription = _storyDescription;
            episodesSpawner.storyloadingThumbnailImage.sprite = _thumbnailLoadingSprite;
            episodesSpawner.storyThumbnailSmallSprite = _thumbnailSmallSprite;
            episodesSpawner.storyThumbnailBigSprite = _thumbnailBigSprite;
            episodesSpawner.storyloadingTitleImage.sprite = _thumbnailTitleSprite;
            episodesSpawner.storyData = _storyData;
            episodesSpawner.storiesDBItem = _storiesDBItem;

            //Send a simple analytics event that user has opened/viewed a story
            if (SDKManager.instance != null)
            {
                string storyTitleProper = _storyTitleEng;
                storyTitleProper = storyTitleProper.Replace(" ", "_");

                SDKEventStringData eventStoryViewedData;
                eventStoryViewedData.eventParameterName = SDKEventsNames.storyParameterName;
                eventStoryViewedData.eventParameterData = storyTitleProper;

                SDKManager.instance.SendEvent(SDKEventsNames.storyViewedEventName, eventStoryViewedData);                
            }

            LoadStoryEpisode();

            return;
        }

        if (isShown)
        {
            if(shareButton)
                shareButton.Setup(_storyTitle, _storyTitleEng, _thumbnailSmallSprite);

            episodesSpawner.storyTitle = _storyTitle;
            episodesSpawner.storyTitleEnglish = _storyTitleEng;
            episodesSpawner.storyDescription = _storyDescription;
            episodesSpawner.storyloadingThumbnailImage.sprite = _thumbnailLoadingSprite;
            episodesSpawner.storyThumbnailSmallSprite = _thumbnailSmallSprite;
            episodesSpawner.storyThumbnailBigSprite = _thumbnailBigSprite;
            episodesSpawner.storyloadingTitleImage.sprite = _thumbnailTitleSprite;
            episodesSpawner.storyData = _storyData;
            episodesSpawner.storiesDBItem = _storiesDBItem;

            storyTitleText.text = _storyTitle;
            if (storyTitleText.GetComponent<SiddhantaFixer>() != null)
                storyTitleText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = false;
            buttonsGroupSizeFitter.SetLayoutVertical();
            storyDescriptionText.text = _storyDescription;

            if (storyDescriptionText.GetComponent<SiddhantaFixer>() != null)
                storyDescriptionText.GetComponent<SiddhantaFixer>().FixTexts();

            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsVerticalGroup);
            buttonsGroupSizeFitter.enabled = true;

            storyThumbnailBigImage.sprite = _thumbnailBigSprite;
            storyThumbnailTitleImage.sprite = _thumbnailTitleSprite;
            storyThumbnailLoadingImage = _thumbnailLoadingSprite;

            if(episodesSpawner.playerData.HasStoryLiked(_storyTitleEng))
            {
                likeImage.gameObject.SetActive(true);
                likeOutlineImage.gameObject.SetActive(false);
            }
            else
            {
                likeImage.gameObject.SetActive(false);
                likeOutlineImage.gameObject.SetActive(true);
            }

            if (FirebaseDBHandler.instance != null)
            {
                if (!likeButtonParent.activeSelf)
                    likeButtonParent.SetActive(true);

                if (!viewsCountParent.activeSelf)
                    viewsCountParent.SetActive(true);

                FirebaseDBHandler.instance.GetCountFromFirebaseDB(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(_storyTitleEng, FirebaseDBHandler.likeCountKeyEnd), val => likesCountText.text = val);
                FirebaseDBHandler.instance.GetCountFromFirebaseDB(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(_storyTitleEng, FirebaseDBHandler.viewCountKeyEnd), val => viewsCountText.text = val);
            }
            else
            {
                if (likeButtonParent.activeSelf)
                    likeButtonParent.SetActive(false);

                if (viewsCountParent.activeSelf)
                    viewsCountParent.SetActive(false);
            }

            startNowButton.onClick.AddListener(LoadStoryEpisodeStartNow);
        }
        else
            startNowButton.onClick.RemoveListener(LoadStoryEpisodeStartNow);

        MovePanel(isShown);

        //Send a simple analytics event that user has opened/viewed a story
        if (SDKManager.instance != null)
        {
            //SDKManager.instance.SendEvent(SDKEventsNames.storyViewedEventName + _storyTitle);

            string storyTitleProper = _storyTitleEng;
            storyTitleProper = storyTitleProper.Replace(" ", "_");

            SDKEventStringData eventStoryViewedData;
            eventStoryViewedData.eventParameterName = SDKEventsNames.storyParameterName;
            eventStoryViewedData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.storyViewedEventName, eventStoryViewedData);

            /*SDKEventData startLevelData;
            startLevelData.level = 100f;
            startLevelData.status = "test";
            SDKManager.instance.SendEvent(startLevelData);*/
        }
    }

    private void MovePanel(bool status, float speed = 0.5f)
    {
        moveSeq = LeanTween.sequence();

        if (status)
        {
            moveSeq.append(() =>
            {
                LeanTween.moveLocalX(gameObject, 0, speed).setEase(easeType).setOnStart(OnPanelShown);
            });
        }
        else
        {
            moveSeq.append(() =>
            {
                LeanTween.moveLocalX(gameObject, 1500f, speed).setEase(easeType).setOnComplete(OnPanelHidden);
            });
        }
    }

    private void OnPanelShown()
    {
        PopulateEpisodesContainer();
    }

    private void PopulateEpisodesContainer()
    {
        if (episodeContainer == null)
            return;

        if (episodeItemPrefab == null)
            return;

        if (storyItem == null || storyData == null)
            return;

        if (storyItem.storyEpisodesKeys.Length > 0)
        {
            for (int i = 0; i < storyItem.storyEpisodesKeys.Length; i++)
            {
                UIEpisodeItem episodeItemInstance = Instantiate(episodeItemPrefab, episodeContainer);
                episodeItemInstance.Setup(i + 1, storyItem, storyItem.storyEpisodesKeys[i], storyItem.storyProgressFileName, storyData, this);
            }

            float totalEpisodesItems = episodeContainer.childCount - 1;            

            float newYPos = (totalEpisodesItems * 100f) + 1000f;
            newYPos *= -1f;

            episodesContainerRect.anchoredPosition = new Vector2(episodesContainerRect.anchoredPosition.x, newYPos);

            float newHeight = totalEpisodesItems * 2f;
            newHeight += 3f;
            newHeight *= 100f;
            newHeight += 1000f;
            storyEpisodesContainer.sizeDelta = new Vector2(storyEpisodesContainer.sizeDelta.x, newHeight);

            moreLikeThisSection.SetAsLastSibling();            
        }

        mainScrollRect.normalizedPosition = new Vector2(0, 1f);
    }

    private void OnPanelHidden()
    {
        ClearEpisodesContainer();
    }

    private void ClearEpisodesContainer()
    {
        if (episodeContainer == null)
            return;

        if (episodeContainer.childCount > 0)
        {
            foreach (Transform episodeItem in episodeContainer)
            {
                if (episodeItem != null && episodeItem != moreLikeThisSection)
                    Destroy(episodeItem.gameObject);
            }
        }

        if (storyItem != null)
            storyItem = null;

        storyData = null;
    }

    //Called from Episode Start Now button (Big Purple color button)
    //Loads the currently saved item
    public void LoadStoryEpisodeStartNow()
    {
        if (episodesSpawner == null)
            return;

        if (storyItem == null)
            return;

        episodesSpawner.storyDataKey = storyItem.storyProgressFileName;
        episodesSpawner.StartLoadingStoryScene();
        //episodesSpawner.storyloadingThumbnailImage.sprite = storyThumbnailLoadingImage.sprite;
        //episodesSpawner.storyloadingTitleImage.sprite = storyThumbnailTitleImage.sprite;

        /*LeanTween.alphaCanvas(episodesSpawner.percentCanvasGroup, 1f, 1f).setOnComplete(() =>
        {
            episodesSpawner.StartLoadingStoryScene();
        });*/
    }

    //Called from Episode Play Button (Triangle Button pointing right side)
    public void LoadStoryEpisode()
    {
        if (episodesSpawner == null)
            return;

        //For Testing Events ONLY!
        //--------------------------------------------------------------------------------
        //Send "episode_completed" Event
        /*if (SDKManager.instance != null)
        {
            SDKEventEpisodeData eventEpisodeData;
            eventEpisodeData.storyTitle = storyItem.storyTitle;
            eventEpisodeData.episodeNum = 1;

            SDKManager.instance.SendEvent(eventEpisodeData);
        }

        //Send "story_completed" Event
        if (SDKManager.instance != null)
        {
            SDKEventStringData eventStoryCompleteData;
            eventStoryCompleteData.eventParameterName = SDKEventsNames.storyParameterName;
            eventStoryCompleteData.eventParameterData = storyItem.storyTitle;

            SDKManager.instance.SendEvent(SDKEventsNames.storyCompleteEventName, eventStoryCompleteData);
        }*/
        //--------------------------------------------------------------------------------

        //Send "number_of_users_per_story" Event
        if (SDKManager.instance != null)
        {
            SDKEventStringData eventStoryCompleteData;
            eventStoryCompleteData.eventParameterName = SDKEventsNames.storyParameterName;
            eventStoryCompleteData.eventParameterData = storyItem.storyTitle;

            SDKManager.instance.SendEvent(SDKEventsNames.numUsersPerStoryEventName, eventStoryCompleteData);
        }

        episodesSpawner.storyDataKey = storyItem.storyProgressFileName;
        episodesSpawner.StartLoadingStoryScene();

        /*LeanTween.alphaCanvas(episodesSpawner.percentCanvasGroup, 1f, 1f).setOnComplete( () => 
        {            
        });*/
    }

    public void PlayButtonClickSound()
    {
        if (uISoundSource == null)
            return;

        uISoundSource.PlayOneShot(buttonClickSound);
    }

    public void SetStoryBufferingImageStatus(bool status)
    {
        if (loadingBufferScreen == null)
            return;

        switch(status)
        {
            case true:

                LeanTween.alphaCanvas(loadingBufferScreen, 1f, 0.1f);
                loadingBufferScreen.interactable = true;
                loadingBufferScreen.blocksRaycasts = true;

                break;

            case false:

                LeanTween.alphaCanvas(loadingBufferScreen, 0, 0.1f);
                loadingBufferScreen.interactable = false;
                loadingBufferScreen.blocksRaycasts = false;

                break;
        }
    }

    public void OnLikeStoryClicked()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance == null)
            return;

        if (episodesSpawner == null && EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        if (FirebaseDBHandler.instance == null)
            return;

        PlayButtonClickSound();

        //Already liked, dislike it. Decrement Likes counter
        if(episodesSpawner.playerData.HasStoryLiked(storyItem.storyTitleEnglish))
        {
            FirebaseDBHandler.instance.LikesCountDecrement(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd));

            likeImage.gameObject.SetActive(false);
            likeOutlineImage.gameObject.SetActive(true);

            episodesSpawner.playerData.RemoveStoryLiked(storyItem.storyTitleEnglish);
            SaveLoadGame.SavePlayerData(episodesSpawner.playerData);

            OnStoryUnliked?.Invoke(storyItem.storyTitleEnglish);
        }
        else    //Like the story, Increment Likes counter.
        {
            FirebaseDBHandler.instance.LikesCountIncrement(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd));

            likeImage.gameObject.SetActive(true);
            likeImage.transform.localScale = Vector3.zero;
            LeanTween.scale(likeImage.gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);

            likeOutlineImage.gameObject.SetActive(false);

            episodesSpawner.playerData.AddStoryLiked(storyItem.storyTitleEnglish);
            SaveLoadGame.SavePlayerData(episodesSpawner.playerData);

            OnStoryLiked?.Invoke(storyItem.storyTitleEnglish);
        }
    }
}
