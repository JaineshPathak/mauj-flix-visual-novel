using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;
using underDOGS.SDKEvents;
using System.Linq;
using Firebase.RemoteConfig;
using NestedScroll.Core;
using NestedScroll.ScrollElement;

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

    public RectTransform episodesCommentsGroup;
    public ContentSizeFitter episodesCommentsSizeFitter;
    public float episodesNewHeightFactor = 2f;

    public RectTransform storyContentGroup;
    public ContentSizeFitter storyContentSizeFitter;

    [Space(15)]

    //public Text storyTitleText;
    public TextMeshProUGUI storyTitleText;
    //public Text storyDescriptionText;
    public TextMeshProUGUI storyDescriptionText;
    public Image storyThumbnailBigImageBg;
    public Image storyThumbnailBigImage;
    public Image storyThumbnailTitleImage;
    [HideInInspector] public Sprite storyThumbnailLoadingImage;

    //[Space(10)]
    //public Image blurMaskLayer;

    [Header("Share")]
    public UIShareButton shareButton;
    public Image storyThumbnailBigShareImage;
    public Image storyThumbnailTitleShareImage;

    [Header("Episodes Section")]
    public UIEpisodeItem episodeItemPrefab;
    public Transform episodeContainer;
    public RectTransform storyEpisodesContainer;
    
    [Space(10)]

    public ScrollRect moreLikeScrollMain;
    public Transform moreLikeThisSection;

    [Header("Like Story Section")]
    public GameObject likeButtonParent;
    public Image likedImageBg;
    public Image likedImage;
    public TextMeshProUGUI likesCountText;

    [Header("Views Section")]
    public GameObject viewsCountParent;
    public TextMeshProUGUI viewsCountText;

    [Header("Story Loading Screen")]
    public CanvasGroup loadingBufferScreen;
    public EpisodesSpawner episodesSpawner;
    public Button startNowButton;

    [Header("Ask Next Episode Panel")]
    public CanvasGroup nextEpAskPanelCanvasGroup;
    public Transform nextEpAskPanel;
    public Transform nextEpTicketPanel;
    public Button nextEpTicketButton;
    public Button nextEpAdButton;
    public Button nextEpCloseButton;

    [Header("Bottom Panel")]
    public UIBottomPanel bottomPanel;

    [Header("Section Button")]
    public NestedScrollView sectionButtonScroller;
    public SnapScrolling sectionSnapScroller;
    public Button episodesSectionBtn;
    public Button commentsSectionBtn;

    [Space(15)]

    public Scrollbar sectionScrollbar;
    public TextMeshProUGUI episodesSectionText;
    public TextMeshProUGUI commentsSectionText;

    [Space(15)]

    public Color sectionTextOnColor;
    public Color sectionTextOffColor = Color.white;

    [Header("Comments Section")]
    public UICommentsSection commentsSection;

    //-----------------------------------------------------------------------------------------------------------

    private bool isShown;
    public bool IsShown { get { return isShown; } }

    private StoryData storyData;
    private StoriesDBItem storyItem;

    private List<UIEpisodeItem> episodeItemsList = new List<UIEpisodeItem>();
    private UIEpisodeItem selectedEpisodeItem;

    private LTSeq moveSeq;
    private LTSeq sectionSeq;
    private int sectionSeqId;

    private RectTransform episodesContainerRect;
    private RectTransform commentsSectionRect;

    public static event Action<string> OnStoryLiked;
    public static event Action<string> OnStoryUnliked;


    private void Awake()
    {
        if(nextEpAskPanelCanvasGroup)
        {
            nextEpAskPanelCanvasGroup.alpha = 0;
            nextEpAskPanelCanvasGroup.interactable = false;
            nextEpAskPanelCanvasGroup.blocksRaycasts = false;
        }

        if (nextEpAskPanel)
            nextEpAskPanel.localScale = Vector3.zero;

        if (nextEpTicketButton)
            nextEpTicketButton.onClick.AddListener(OnNextEpisodeTicketButton);

        if (nextEpAdButton)
            nextEpAdButton.onClick.AddListener(OnNextEpisodeAdButton);

        if (nextEpCloseButton)
            nextEpCloseButton.onClick.AddListener(OnNextEpisodeCloseButton);

        if (episodesSectionBtn)
            episodesSectionBtn.onClick.AddListener(OnEpisodesSectionClicked);

        if (commentsSectionBtn)
            commentsSectionBtn.onClick.AddListener(OnCommentsSectionClicked);

        if(sectionButtonScroller)
            sectionButtonScroller.normalizedPosition = new Vector2(0, 1f);

        if (commentsSection)
            commentsSectionRect = commentsSection.transform as RectTransform;
    }

    private void OnEpisodesSectionClicked()
    {
        if (sectionButtonScroller == null)
            return;

        MoveToSection(1f, 0, 0.2f);
        //sectionButtonScroller.horizontalNormalizedPosition = 0;
    }

    private void OnCommentsSectionClicked()
    {
        if (sectionButtonScroller == null)
            return;

        MoveToSection(0, 1f, 0.2f);
        //sectionButtonScroller.horizontalNormalizedPosition = 1f;
    }

    //From = 0, To = 1, Go to 'Comments' section
    //From = 1, To = 0, Go to 'Episodes' section
    private void MoveToSection(float from, float to, float time = 0.3f)
    {
        if (sectionButtonScroller.horizontalNormalizedPosition == to)
            return;

        if (LeanTween.isTweening(sectionSeqId))
            LeanTween.cancel(sectionSeqId);

        sectionSeq = LeanTween.sequence();
        sectionSeqId = sectionSeq.id;

        sectionSeq.append(() => sectionSnapScroller.allowUpdate = false);
        sectionSeq.append(LeanTween.value(from, to, time).setOnUpdate((float val) => sectionButtonScroller.horizontalNormalizedPosition = val).setEaseLinear());
        sectionSeq.append(() => { sectionSnapScroller.allowUpdate = true; sectionSnapScroller.SelectedPanID = (int)to; });
    }

    private void LerpSectionTextColor()
    {
        if (episodesSectionText == null || commentsSectionText == null || sectionScrollbar == null)
            return;

        /*if (sectionScrollbar.value <= 0.2f)
            GetEpisodesSectionHeight();
        else if (sectionScrollbar.value >= 0.6f && commentsSection.transform.childCount > 7)
            GetCommentsSectionHeight();*/

        episodesSectionText.color = Color.Lerp(sectionTextOnColor, sectionTextOffColor, sectionScrollbar.value);
        commentsSectionText.color = Color.Lerp(sectionTextOnColor, sectionTextOffColor, 1f - sectionScrollbar.value);
    }

    private void Update()
    {
        LerpSectionTextColor();
    }

    private void OnEnable()
    {
        AdsManager.OnIronSrcRewardVideoComplete += OnRewardAdComplete;
    }

    private void OnDisable()
    {
        AdsManager.OnIronSrcRewardVideoComplete -= OnRewardAdComplete;
    }

    private void OnDestroy()
    {
        AdsManager.OnIronSrcRewardVideoComplete -= OnRewardAdComplete;
    }

    private void OnRewardAdComplete(string placementName)
    {
        switch (placementName)
        {
            case AdsNames.rewardFreeNextEpisode:
                OnNextEpisodeAdButtonComplete();
                break;
        }
    }

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
            if (storyTitleText.GetComponent<CharReplacerHindi>() != null)
                storyTitleText.text = storyTitleText.GetComponent<CharReplacerHindi>().GetFixedText(episodesSpawner.storyTitle);

            //if (storyTitleText.GetComponent<SiddhantaFixer>() != null)
            //storyTitleText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = false;
            buttonsGroupSizeFitter.SetLayoutVertical();

            storyDescriptionText.text = episodesSpawner.storyDescription;
            if (storyDescriptionText.GetComponent<CharReplacerHindi>() != null)
                storyDescriptionText.GetComponent<CharReplacerHindi>().UpdateMe();
            //if (storyDescriptionText.GetComponent<SiddhantaFixer>() != null)
            //storyDescriptionText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = true;

            if (shareButton && episodesSpawner)
            {
                shareButton.Setup(episodesSpawner.storyTitle, episodesSpawner.storyTitleEnglish, episodesSpawner.storyThumbnailSmallSprite);
                //shareButton.Setup(episodesSpawner.storyTitle, episodesSpawner.storyTitleEnglish, ThumbnailsBucket.instance.GetThumbnailSprite(episodesSpawner.storiesDBItem.storyThumbnailSmallName, ThumbnailType.Small));

                //Sprite thumbnailSpriteShare = Sprite.Create(episodesSpawner.storyThumbnailSmallSprite.texture, new Rect(0, 0, episodesSpawner.storyThumbnailSmallSprite.texture.width, episodesSpawner.storyThumbnailSmallSprite.texture.height), new Vector2(0.5f, 0.5f), 100f);
                //shareButton.Setup(episodesSpawner.storyTitle, episodesSpawner.storyTitleEnglish, thumbnailSpriteShare);
            }

            storyThumbnailBigImageBg.sprite = episodesSpawner.storyThumbnailBigSprite;
            storyThumbnailBigImage.sprite = episodesSpawner.storyThumbnailBigSprite;
            storyThumbnailTitleImage.sprite = episodesSpawner.storyloadingTitleImage.sprite;

            storyThumbnailBigShareImage.sprite = episodesSpawner.storyThumbnailBigSprite;
            storyThumbnailTitleShareImage.sprite = episodesSpawner.storyloadingTitleImage.sprite;

            storyItem = episodesSpawner.storiesDBItem;
            storyData = episodesSpawner.storyData;

            if(startNowButton)
            {
                startNowButton.onClick.RemoveAllListeners();
                startNowButton.onClick.AddListener(LoadStoryEpisodeStartNow);
            }

            if (episodesSpawner.playerData.HasStoryLiked(storyItem.storyTitleEnglish))
            {
                //likedImage.gameObject.SetActive(true);
                //likedImageBg.gameObject.SetActive(false);

                likedImage.transform.localScale = Vector3.one;
            }
            else
            {
                //likedImage.gameObject.SetActive(false);
                //likedImageBg.gameObject.SetActive(true);

                likedImage.transform.localScale = Vector3.zero;
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

        //Incase if Blur Shader causes lags
        /*if(blurMaskLayer && FirebaseRemoteConfig.DefaultInstance != null)
        {
            if(!FirebaseRemoteConfig.DefaultInstance.GetValue("Enable_Thumbnail_Blur").BooleanValue)
            {
                blurMaskLayer.color = new Color(1f, 1f, 1f, 0);
                
                if(blurMaskLayer.material != null)
                    blurMaskLayer.material = null;
            }
        }*/
    }

    /*private void Update()
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
    }*/

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

        episodesSpawner.topPanel.ShowTopPanel();

        episodesSpawner.storyDataKey = storyItem.storyProgressFileName;        
        //If it's a short story (only 1 episode) then load immediately
        if (storyItem.isShortStory)
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
            if (shareButton)
            {
                shareButton.Setup(_storyTitle, _storyTitleEng, _thumbnailSmallSprite);
                
                //Sprite thumbnailSpriteShare = Sprite.Create(_thumbnailSmallSprite.texture, new Rect(0, 0, _thumbnailSmallSprite.texture.width, _thumbnailSmallSprite.texture.height), new Vector2(0.5f, 0.5f), 100f);
                //shareButton.Setup(_storyTitle, _storyTitleEng, thumbnailSpriteShare);
                
                //print("NEW SHARE NAME: " + thumbnailSpriteShare.texture.name);
            }

            episodesSpawner.storyTitle = _storyTitle;
            episodesSpawner.storyTitleEnglish = _storyTitleEng;
            episodesSpawner.storyDescription = _storyDescription;
            episodesSpawner.storyloadingThumbnailImage.sprite = _thumbnailLoadingSprite;
            episodesSpawner.storyThumbnailSmallSprite = _thumbnailSmallSprite;
            episodesSpawner.storyThumbnailBigSprite = _thumbnailBigSprite;
            episodesSpawner.storyloadingTitleImage.sprite = _thumbnailTitleSprite;
            episodesSpawner.storyData = _storyData;
            episodesSpawner.storiesDBItem = _storiesDBItem;

            //storyTitleText.text = _storyTitle;
            storyTitleText.text = _storyTitle;
            if (storyTitleText.GetComponent<CharReplacerHindi>() != null)
                storyTitleText.text = storyTitleText.GetComponent<CharReplacerHindi>().GetFixedText(_storyTitle);
            //if (storyTitleText.GetComponent<CharReplacerHindi>() != null)
            //storyTitleText.GetComponent<CharReplacerHindi>().UpdateMe();
            //if (storyTitleText.GetComponent<SiddhantaFixer>() != null)
            //storyTitleText.GetComponent<SiddhantaFixer>().FixTexts();

            buttonsGroupSizeFitter.enabled = false;
            buttonsGroupSizeFitter.SetLayoutVertical();

            storyDescriptionText.text = _storyDescription;
            if (storyDescriptionText.GetComponent<CharReplacerHindi>() != null)
                storyDescriptionText.GetComponent<CharReplacerHindi>().UpdateMe();
            //if (storyDescriptionText.GetComponent<SiddhantaFixer>() != null)
            //storyDescriptionText.GetComponent<SiddhantaFixer>().FixTexts();

            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsVerticalGroup);
            buttonsGroupSizeFitter.enabled = true;

            storyThumbnailBigImageBg.sprite = _thumbnailBigSprite;
            storyThumbnailBigImage.sprite = _thumbnailBigSprite;
            storyThumbnailTitleImage.sprite = _thumbnailTitleSprite;
            storyThumbnailLoadingImage = _thumbnailLoadingSprite;

            storyThumbnailBigShareImage.sprite = _thumbnailBigSprite;
            storyThumbnailTitleShareImage.sprite = _thumbnailTitleSprite;

            /*print("NEW SHARE 1: " + storyThumbnailBigImage.mainTexture.name);
            print("NEW SHARE 2: " + storyThumbnailBigImage.sprite.name);
            print("NEW SHARE 3: " + storyThumbnailBigImage.sprite.rect.width + ", " + storyThumbnailBigImage.sprite.rect.height);
            print("NEW SHARE 4: " + storyThumbnailBigImage.sprite.texture.width + ", " + storyThumbnailBigImage.sprite.texture.height);

            Texture2D texBigFromSprite = TextureFromSpriteAtlas(storyThumbnailBigImage.sprite);
            texBigFromSprite.name = "TEST(CLONE)";
            print($"TEXTURE SHARE 1: {texBigFromSprite.name}, {texBigFromSprite.width}, {texBigFromSprite.height}");

            Sprite thumbnailSpriteShare = Sprite.Create(texBigFromSprite, new Rect(0, 0, texBigFromSprite.width, texBigFromSprite.height), new Vector2(0.5f, 0.5f), 100f);
            thumbnailSpriteShare.name = "YO(CLONE)";
            storyThumbnailBigImage.sprite = thumbnailSpriteShare;*/

            if (episodesSpawner.playerData.HasStoryLiked(_storyTitleEng))
            {
                //likedImage.gameObject.SetActive(true);
                //likedImageBg.gameObject.SetActive(false);

                likedImage.transform.localScale = Vector3.one;
            }
            else
            {
                //likedImage.gameObject.SetActive(false);
                //likedImageBg.gameObject.SetActive(true);

                likedImage.transform.localScale = Vector3.zero;
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

            if(startNowButton)
                startNowButton.onClick.AddListener(LoadStoryEpisodeStartNow);
        }
        else
        {
            if(startNowButton)
                startNowButton.onClick.RemoveListener(LoadStoryEpisodeStartNow);
        }

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
            if (bottomPanel.LastButtonIndex == 0)
                episodesSpawner.topPanel.ShowTopPanel();
            else
                episodesSpawner.topPanel.HideTopPanel();

            moreLikeScrollMain.normalizedPosition = new Vector2(0, 1f);
            MoveToSection(1f, 0, 0.01f);

            moveSeq.append(() =>
            {
                LeanTween.moveLocalX(gameObject, 1500f, speed).setEase(easeType).setOnComplete(OnPanelHidden);
            });
        }
    }

    private void OnPanelShown()
    {
        PopulateEpisodesContainer();

        if(commentsSection)
            commentsSection.PopulateSection();
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

                episodeItemsList.Add(episodeItemInstance);
            }

            if(episodeItemsList.Count > 0)
            {
                for (int i = 0; i < episodeItemsList.Count; i++)
                {
                    if(episodeItemsList[i] != null)
                    {
                        if (i + 1 < episodeItemsList.Count - 1)
                        {
                            if(episodeItemsList[i].EpisodesData.isUnlocked &&
                                episodeItemsList[i].EpisodesData.isFinished &&
                                !episodeItemsList[i + 1].EpisodesData.isUnlocked)
                            {
                                episodeItemsList[i + 1].AllowPaymentUnlockable();
                                break;
                            }
                        }
                    }
                }
            }

            GetEpisodesSectionHeight();

            moreLikeThisSection.SetAsLastSibling();

            episodesCommentsSizeFitter.enabled = false;
            episodesCommentsSizeFitter.SetLayoutVertical();
            LayoutRebuilder.ForceRebuildLayoutImmediate(episodesCommentsGroup);
            episodesCommentsSizeFitter.enabled = true;

            storyContentSizeFitter.enabled = false;
            storyContentSizeFitter.SetLayoutVertical();
            LayoutRebuilder.ForceRebuildLayoutImmediate(storyContentGroup);
            storyContentSizeFitter.enabled = true;
        }

        mainScrollRect.normalizedPosition = new Vector2(0, 1f);
    }

    private void GetEpisodesSectionHeight()
    {
        if (episodeContainer == null || storyEpisodesContainer == null)
            return;

        float totalEpisodesItems = episodeContainer.childCount;
        //float newYPos = ( (totalEpisodesItems * 100f) + /*1000f*/ 750f) * -1f;
        //newYPos *= -1f;
        //episodesContainerRect.anchoredPosition = new Vector2(episodesContainerRect.anchoredPosition.x, newYPos);

        float newHeight = totalEpisodesItems * 2f;
        newHeight += 3f;
        newHeight *= 100f;
        //newHeight += 750f;
        storyEpisodesContainer.sizeDelta = new Vector2(storyEpisodesContainer.sizeDelta.x, newHeight);
    }

    private void GetCommentsSectionHeight()
    {
        if (storyEpisodesContainer == null || commentsSection == null)
            return;

        storyEpisodesContainer.sizeDelta = new Vector2(storyEpisodesContainer.sizeDelta.x, commentsSectionRect.sizeDelta.y);
    }

    [ContextMenu("OnCommentAdded")]
    internal void OnCommentAdded()
    {
        if (commentsSection == null || (commentsSection != null && commentsSection.transform.childCount <= 7))
            return;

        GetCommentsSectionHeight();
    }    

    private void OnPanelHidden()
    {        
        ClearEpisodesContainer();
        
        if(commentsSection)
            commentsSection.EmptySection();
    }

    private void ClearEpisodesContainer()
    {
        if (episodeContainer == null)
            return;

        episodeItemsList.Clear();
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

            string storyTitleMain = storyItem.storyTitleEnglish;
            storyTitleMain = storyTitleMain.Replace(":", "");
            storyTitleMain = storyTitleMain.Replace(".", "");
            storyTitleMain = storyTitleMain.Replace(" ", "");

            eventStoryCompleteData.eventParameterData = storyTitleMain;

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

            //likedImage.gameObject.SetActive(false);
            //likedImageBg.gameObject.SetActive(true);
            LeanTween.scale(likedImage.gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack);

            episodesSpawner.playerData.RemoveStoryLiked(storyItem.storyTitleEnglish);
            SaveLoadGame.SavePlayerData(episodesSpawner.playerData);

            OnStoryUnliked?.Invoke(storyItem.storyTitleEnglish);
        }
        else    //Like the story, Increment Likes counter.
        {
            FirebaseDBHandler.instance.LikesCountIncrement(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd));

            //likedImage.gameObject.SetActive(true);
            likedImage.transform.localScale = Vector3.zero;
            LeanTween.scale(likedImage.gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);

            //likedImageBg.gameObject.SetActive(false);

            episodesSpawner.playerData.AddStoryLiked(storyItem.storyTitleEnglish);
            SaveLoadGame.SavePlayerData(episodesSpawner.playerData);

            OnStoryLiked?.Invoke(storyItem.storyTitleEnglish);
        }
    }

    //===============================================================================================================

    public void ShowNextEpisodeAskPanel()
    {
        if (nextEpAskPanelCanvasGroup == null || nextEpAskPanel == null)
            return;

        nextEpAskPanelCanvasGroup.interactable = true;
        nextEpAskPanelCanvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(nextEpAskPanelCanvasGroup, 1f, 0.5f).setEaseInOutSine();
        LeanTween.scale(nextEpAskPanel.gameObject, Vector3.one, 0.5f).setEaseOutBack();
    }

    public void ShowNextEpisodeAskPanel(UIEpisodeItem _episodeItem)
    {
        if (nextEpAskPanelCanvasGroup == null || nextEpAskPanel == null)
            return;

        nextEpAskPanelCanvasGroup.interactable = true;
        nextEpAskPanelCanvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(nextEpAskPanelCanvasGroup, 1f, 0.4f).setEaseInOutSine();
        LeanTween.scale(nextEpAskPanel.gameObject, Vector3.one, 0.4f).setEaseOutBack();

        selectedEpisodeItem = _episodeItem;
    }

    public void HideNextEpisodeAskPanel()
    {
        if (nextEpAskPanelCanvasGroup == null || nextEpAskPanel == null)
            return;

        nextEpAskPanelCanvasGroup.interactable = false;
        nextEpAskPanelCanvasGroup.blocksRaycasts = false;

        LeanTween.alphaCanvas(nextEpAskPanelCanvasGroup, 0, 0.4f).setEaseInOutSine();
        LeanTween.scale(nextEpAskPanel.gameObject, Vector3.zero, 0.4f).setEaseInBack();

        selectedEpisodeItem = null;
    }

    private void OnNextEpisodeTicketButton()
    {
        if (nextEpAskPanelCanvasGroup == null)
            return;

        if (FirebaseFirestoreHandler.instance == null)
            return;

        if (FirebaseFirestoreHandler.instance.GetUserTicketsAmountInt() < 1)
            return;

        nextEpAskPanelCanvasGroup.interactable = false;
        nextEpAskPanelCanvasGroup.blocksRaycasts = false;

        episodesSpawner.diamondsPool.PlayTicketsAnimationDebit(nextEpTicketPanel, episodesSpawner.topPanel.ticketsPanelIcon, 1, 1, () =>
        {
            if(selectedEpisodeItem != null)
            {
                selectedEpisodeItem.EpisodesData.isUnlocked = true;
                selectedEpisodeItem.OnEpisodePlayClick(false);
            }
        }, 150f, Color.red);
    }

    private void OnNextEpisodeAdButton()
    {
#if UNITY_EDITOR
        nextEpAskPanelCanvasGroup.interactable = false;
        nextEpAskPanelCanvasGroup.blocksRaycasts = false;

        if (selectedEpisodeItem != null)
        {
            selectedEpisodeItem.EpisodesData.isUnlocked = true;
            selectedEpisodeItem.OnEpisodePlayClick(false);
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (AdsManager.instance == null)
            return;

        AdsManager.instance.ShowRewardAd(AdsNames.rewardFreeNextEpisode);
#endif
    }

    private void OnNextEpisodeAdButtonComplete()
    {
        nextEpAskPanelCanvasGroup.interactable = false;
        nextEpAskPanelCanvasGroup.blocksRaycasts = false;

        if (selectedEpisodeItem != null)
        {
            selectedEpisodeItem.EpisodesData.isUnlocked = true;
            selectedEpisodeItem.OnEpisodePlayClick(false);
        }
    }

    private void OnNextEpisodeCloseButton()
    {
        if (nextEpAskPanelCanvasGroup == null)
            return;

        nextEpAskPanelCanvasGroup.interactable = false;
        nextEpAskPanelCanvasGroup.blocksRaycasts = false;

        HideNextEpisodeAskPanel();
    }

    //===============================================================================================================

    /*public static Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        try
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] colors = newText.GetPixels();
                Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                             (int)System.Math.Ceiling(sprite.textureRect.y),
                                                             (int)System.Math.Ceiling(sprite.textureRect.width),
                                                             (int)System.Math.Ceiling(sprite.textureRect.height));
                Debug.Log(colors.Length + "_" + newColors.Length);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }
        catch
        {
            return sprite.texture;
        }
    }

    public static Texture2D TextureFromSpriteAtlas(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            int texWid = (int)sprite.rect.width;
            int texHei = (int)sprite.rect.height;
            
            Texture2D newTex = new Texture2D(texWid, texHei);
            Color[] defaultPixels = Enumerable.Repeat<Color>(new Color(1, 1, 1, 1), texWid * texHei).ToArray();
            Color[] pixels = sprite.texture.GetPixels(Mathf.CeilToInt(sprite.textureRect.x)
            , Mathf.CeilToInt(sprite.textureRect.y)
            , Mathf.CeilToInt(sprite.textureRect.width)
            , Mathf.CeilToInt(sprite.textureRect.height));

            newTex.name = "TEST(Clone)";
            //newTex.SetPixels(defaultPixels);
            newTex.SetPixels(Mathf.CeilToInt(sprite.textureRectOffset.x), Mathf.CeilToInt(sprite.textureRectOffset.y), Mathf.CeilToInt(sprite.textureRect.width), Mathf.CeilToInt(sprite.textureRect.height), pixels);
            newTex.Apply();
            
            return newTex;
        }
        else
        {
            return sprite.texture;
        }
    }*/
}
