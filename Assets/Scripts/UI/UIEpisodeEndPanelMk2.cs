using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using underDOGS.SDKEvents;

public class UIEpisodeEndPanelMk2 : MonoBehaviour
{
    public static UIEpisodeEndPanelMk2 instance;

    [Header("Main Canvas")]    
    public CanvasGroup endScreenCanvasGroup;
    public Image congratulationsImage;
    public ParticleSystem congratsParticles;

    [Header("Part 1")]
    public RectTransform middlePanelPart1;
    public CanvasGroup storyTextPart1;
    
    public Transform collectDiamondsPanel;
    public TextMeshProUGUI collectDiamondsText;

    public Button collectDiamondsButton;

    [Header("Part 2")]
    public Image nextEpisodeImage;
    public RectTransform middlePanelPart2;
    public RectTransform nextEpisodeButtonGroups;
    public CanvasGroup storyTextPart2;
    
    [Space(10)]

    public Button nextEpisodeButton;
    public Button nextEpisodeAdButton;
    public Transform nextEpisodeTicketIcon;
    public RectTransform nextEpisodeText;

    [Space(10)]

    public Button noThanksButton;

    [Header("VFX")]
    public ParticleSystem vfxDiamondsParticles;
    public AnimationCurve outBackMore;

    [Header("No Tickets Panel")]
    public CanvasGroup noTicketsPanel;
    public Button noTicketsOkButton;

    [HideInInspector] public EpisodesHandler episodesHandler;
    [HideInInspector] public Sprite nextEpisodeImageSprite;
    private EpisodesSpawner episodesSpawner;

    private CanvasGroup noThanksButtonCanvasGrp;

    private bool isTriggered;
    private bool debitTicketsNextEpisode;
    private LTSeq endSeq;

    public static event Action OnNextEpisodePanelOpened;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (endScreenCanvasGroup)
        {
            endScreenCanvasGroup.alpha = 0;
            endScreenCanvasGroup.interactable = false;
            endScreenCanvasGroup.blocksRaycasts = false;
        }

        if(noTicketsPanel)
        {
            noTicketsPanel.alpha = 0;
            noTicketsPanel.interactable = false;
            noTicketsPanel.blocksRaycasts = false;
        }

        if (noTicketsOkButton)
            noTicketsOkButton.onClick.AddListener(OnNoTicketsOkButton);

        if(congratulationsImage)
            congratulationsImage.transform.localScale = Vector3.zero;

        if (middlePanelPart1)
            middlePanelPart1.anchoredPosition = new Vector2(-1000f, middlePanelPart1.anchoredPosition.y);

        if (middlePanelPart2)
            middlePanelPart2.anchoredPosition = new Vector2(-1000f, middlePanelPart2.anchoredPosition.y);

        if(nextEpisodeButtonGroups)
            nextEpisodeButtonGroups.anchoredPosition = new Vector2(-1000f, nextEpisodeButtonGroups.anchoredPosition.y);

        if (storyTextPart1)
            storyTextPart1.alpha = 0;

        if (storyTextPart2)
            storyTextPart2.alpha = 0;

        if (collectDiamondsPanel)
            collectDiamondsPanel.localScale = Vector3.zero;

        if (collectDiamondsText)
            collectDiamondsText.text = "0";

        if (collectDiamondsButton)
        {
            collectDiamondsButton.transform.localScale = Vector3.zero;
            collectDiamondsButton.onClick.AddListener(OnEndDiamondCollectButton);
        }

        if(nextEpisodeButton)
        {
            nextEpisodeButton.transform.localScale = Vector3.zero;
            nextEpisodeButton.onClick.AddListener(OnNextEpisodeButton);
        }

        if(nextEpisodeAdButton)
        {
            nextEpisodeAdButton.transform.localScale = Vector3.zero;
            nextEpisodeAdButton.onClick.AddListener(OnNextEpisodeAdButton);
        }

        if (noThanksButton)
        {
            noThanksButton.onClick.AddListener(OnNoThanksButton);

            noThanksButtonCanvasGrp = noThanksButton.GetComponent<CanvasGroup>();
            noThanksButtonCanvasGrp.alpha = 0;
            noThanksButtonCanvasGrp.interactable = false;
            noThanksButtonCanvasGrp.blocksRaycasts = false;
        }
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
        if (EpisodesSpawner.instance != null)
        {
            episodesSpawner = EpisodesSpawner.instance;
            //episodesSpawner.topPanel.HideTopPanel();
        }
    }

    public void PlayEndingScreen()
    {
        if (isTriggered)
            return;

        isTriggered = true;

        if (nextEpisodeImage && nextEpisodeImageSprite)
            nextEpisodeImage.sprite = nextEpisodeImageSprite;

        if (!endScreenCanvasGroup.gameObject.activeSelf)
            endScreenCanvasGroup.gameObject.SetActive(true);

        endScreenCanvasGroup.interactable = true;
        endScreenCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        endSeq.append(LeanTween.alphaCanvas(endScreenCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congratulationsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete( () => 
        {
            if (congratsParticles != null)
                congratsParticles.Play(true);
        }));
        endSeq.append(0.5f);
        endSeq.append(LeanTween.moveLocalX(middlePanelPart1.gameObject, 0, 0.7f).setEase(outBackMore).setOnStart(() =>
        {
            if (vfxDiamondsParticles != null)
                vfxDiamondsParticles.Play(true);
        }));
        endSeq.append(LeanTween.alphaCanvas(storyTextPart1, 1f, 0.5f));
        endSeq.append(LeanTween.scale(collectDiamondsPanel.gameObject, Vector3.one, 0.5f).setEase(outBackMore));
        endSeq.append(LeanTween.value(0, 5f, 0.8f).setOnUpdate((float val) => collectDiamondsText.text = "+" + Mathf.RoundToInt(val).ToString()).setEase(LeanTweenType.linear));
        endSeq.append(LeanTween.scale(collectDiamondsButton.gameObject, Vector3.one, 0.4f).setEase(outBackMore));
        endSeq.append(() =>
        {
            if (episodesSpawner == null && EpisodesSpawner.instance)
                episodesSpawner = EpisodesSpawner.instance;

            episodesSpawner.topPanel.ShowTopPanel();
        });
    }

    private void OnEndDiamondCollectButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        collectDiamondsButton.interactable = false;
        LeanTween.scale(collectDiamondsButton.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInBack);

        episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(collectDiamondsPanel, episodesSpawner.topPanel.diamondsPanelIcon, 5, 5, OnEndDiamondCollected);
    }

    private void OnEndDiamondCollected()
    {
        if (episodesHandler == null)
            episodesHandler = FindObjectOfType<EpisodesHandler>();

        LTSeq seqPart2 = LeanTween.sequence();

        seqPart2.append(0.5f);
        seqPart2.append(LeanTween.moveLocalX(middlePanelPart1.gameObject, 1000f, 0.7f).setOnStart(() =>
        {
            LeanTween.moveLocalX(middlePanelPart2.gameObject, 0, 0.7f).setEase(LeanTweenType.easeInOutBack);
            LeanTween.moveLocalX(nextEpisodeButtonGroups.gameObject, 0, 0.7f).setEase(LeanTweenType.easeInOutBack);

            if (episodesSpawner)
                episodesSpawner.topPanel.HideTopPanel();
        }).setEase(LeanTweenType.easeInOutBack));
        //seqPart2.insert(LeanTween.moveLocalX(middlePanelPartTwo.gameObject, 0, 0.7f).setEase(LeanTweenType.easeInOutBack));
        seqPart2.append(LeanTween.alphaCanvas(storyTextPart2, 1f, 0.5f));
        seqPart2.append(LeanTween.scale(nextEpisodeButton.gameObject, Vector3.one, 0.4f).setOnStart( () => 
        {
            //If the next episode is already unlocked i.e If User is playing the already unlocked episode again, then don't debit the tickets
            //If not unlocked then do debit the tickets and unlock the episode
            if(episodesHandler != null && episodesHandler.NextEpisodeData != null)
                debitTicketsNextEpisode = episodesHandler.NextEpisodeData.isUnlocked ? false : true;

            if (nextEpisodeTicketIcon && nextEpisodeText)
            {
                nextEpisodeTicketIcon.gameObject.SetActive(debitTicketsNextEpisode);
                nextEpisodeText.anchoredPosition = new Vector2(0, nextEpisodeText.anchoredPosition.y);
            }

            if (nextEpisodeAdButton)
            {
                if (!debitTicketsNextEpisode)
                    nextEpisodeAdButton.gameObject.SetActive(false);
                else
                    LeanTween.scale(nextEpisodeAdButton.gameObject, Vector3.one, 0.4f);
            }

        }).setEase(outBackMore));
        seqPart2.append(3f);
        seqPart2.append(LeanTween.alphaCanvas(noThanksButtonCanvasGrp, 1f, 1f).setOnStart(() =>
        {
            OnNextEpisodePanelOpened?.Invoke();

            noThanksButton.interactable = true;
            noThanksButtonCanvasGrp.interactable = true;
            noThanksButtonCanvasGrp.blocksRaycasts = true;
        }).setEase(LeanTweenType.easeInOutSine).setOnComplete( () => 
        {
            if (episodesSpawner)
                episodesSpawner.topPanel.ShowTopPanel();
        }));
    }

    private void OnNextEpisodeButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        noThanksButton.interactable = false;
        nextEpisodeButton.interactable = false;
        nextEpisodeAdButton.interactable = false;

        if (debitTicketsNextEpisode)
        {
            if ( (FirebaseFirestoreHandler.instance != null) && FirebaseFirestoreOffline.instance.GetTicketsAmountInt() < 1)
            {
                ShowNoTicketsPopup();
                return;
            }

            episodesSpawner.diamondsPool.PlayTicketsAnimationDebit(nextEpisodeButton.transform, episodesSpawner.topPanel.ticketsPanelIcon, 1, 1, () =>
            {
                StartCoroutine(OnNextEpisodeButtonRoutine());
            }, 200f, Color.red);
        }
        else
        {
            episodesSpawner.topPanel.HideTopPanel(0.3f, 0.7f);
            episodesSpawner.StartLoadingStoryScene();
        }
    }

    private IEnumerator OnNextEpisodeButtonRoutine()
    {
        noThanksButton.interactable = false;
        nextEpisodeButton.interactable = false;
        nextEpisodeAdButton.interactable = false;

        episodesSpawner.topPanel.HideTopPanel(0.3f, 0.7f);

        yield return new WaitForSeconds(0.5f);

        //After Tickets Payment, unlock and save the Next Episode Data and Start Loading next episode
        if(episodesHandler != null && episodesHandler.NextEpisodeData != null)
        {
            episodesHandler.NextEpisodeData.isUnlocked = true;
            episodesHandler.SaveStoryData();
            
            episodesSpawner.StartLoadingStoryScene();
        }
    }

    private void OnNextEpisodeAdButton()
    {
#if UNITY_EDITOR
        OnNextEpisodeAdButtonComplete();
#elif UNITY_ANDROID || UNITY_IOS
        if (AdsManager.instance == null)
            return;

        AdsManager.instance.ShowRewardAd(AdsNames.rewardFreeNextEpisode);
#endif
    }

    private void OnNextEpisodeAdButtonComplete()
    {
        StartCoroutine(OnNextEpisodeButtonRoutine());
    }

    private void OnNoThanksButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        noThanksButton.interactable = false;
        nextEpisodeButton.interactable = false;
        nextEpisodeAdButton.interactable = false;

        episodesSpawner.LoadEpisodesMainMenu(false);
    }


    //-------------------------------------------------------------------------------------------------------

    private void ShowNoTicketsPopup()
    {
        if (noTicketsPanel == null)
            return;

        noTicketsPanel.interactable = true;
        noTicketsPanel.blocksRaycasts = true;
        LeanTween.alphaCanvas(noTicketsPanel, 1f, 0.2f).setEaseInOutSine();
    }

    private void HideNoTicketsPopup()
    {
        if (noTicketsPanel == null)
            return;

        noTicketsPanel.interactable = false;
        noTicketsPanel.blocksRaycasts = false;
        LeanTween.alphaCanvas(noTicketsPanel, 0, 0.2f).setEaseInOutSine();

        noThanksButton.interactable = true;
        nextEpisodeButton.interactable = true;
        nextEpisodeAdButton.interactable = true;
    }

    private void OnNoTicketsOkButton()
    {
        HideNoTicketsPopup();
    }

    //-------------------------------------------------------------------------------------------------------
}