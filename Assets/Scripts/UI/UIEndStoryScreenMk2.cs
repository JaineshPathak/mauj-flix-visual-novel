using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;

public class UIEndStoryScreenMk2 : UIEndStoryScreen
{
    private EpisodesSpawner episodesSpawner;

    [Header("End Story Screen Mk2")]
    public CanvasGroup storyByMaujflixCanvas;
    public string endStoryDiamondsText = "कहानी की एक एंडींग खेलने के लिए आपने जीते हैं कुछ डायमंड!";
    public string endStoryMultipleEndingsText = "हर कहानी के एक से ज्यादा एंडिंग्स हैं, तो कहानी दोबारा खेलकर सारी एंडिंग्स जरूर खेलिएगा।";
    public string endStoryMultipleEndingsAsk = "क्या आप दूसरी एंडींग खेलना चाहेंगे?";

    [Header("Part 1")]
    public RectTransform middlePanelPartOne;

    [Space(15)]

    public Transform collectDiamondsPanel;
    public TextMeshProUGUI collectDiamondsText;

    [Space(10)]

    public Transform collectTicketsPanel;
    public TextMeshProUGUI collectTicketsText;
    
    public Button collectDiamondsTicketsButton;

    [Header("VFX")]
    public ParticleSystem vfxDiamondsParticles;
    public AnimationCurve outBackMore;

    [Header("Part 2")]
    public RectTransform middlePanelPartTwo;

    [Space(10)]

    public SayDialog topTextDialog;
    public SayDialog topTextAskDialog;

    [Space(10)]

    public Button playEndingButton;
    public Transform playEndingDiamondIcon;
    public Button playEndingButtonAd;
    public Button noThanksButton;

    [Header("No Diamonds Panel")]
    public CanvasGroup noDiamondsPanel;
    public Button noDiamondsOkButton;

    private CanvasGroup noThanksButtonCanvasGrp;
    private EpisodesHandler episodesHandler;

    protected override void Awake()
    {
        ResetStuffs();

        base.Awake();
    }

    public void ResetStuffs()
    {
        if (noDiamondsPanel)
        {
            noDiamondsPanel.alpha = 0;
            noDiamondsPanel.interactable = false;
            noDiamondsPanel.blocksRaycasts = false;
        }

        if (noDiamondsOkButton)
        {
            noDiamondsOkButton.interactable = true;
            noDiamondsOkButton.onClick.RemoveAllListeners();
            noDiamondsOkButton.onClick.AddListener(OnNoDiamondsOkButton);
        }

        if (middlePanelPartOne)
            middlePanelPartOne.anchoredPosition = new Vector2(-1000f, middlePanelPartOne.anchoredPosition.y);

        if (middlePanelPartTwo)
            middlePanelPartTwo.anchoredPosition = new Vector2(-1000f, middlePanelPartTwo.anchoredPosition.y);

        if (collectDiamondsPanel)
            collectDiamondsPanel.localScale = Vector3.zero;

        if (collectTicketsPanel)
            collectTicketsPanel.localScale = Vector3.zero;

        if (collectDiamondsTicketsButton)
        {
            collectDiamondsTicketsButton.interactable = true;
            collectDiamondsTicketsButton.onClick.RemoveAllListeners();
            collectDiamondsTicketsButton.onClick.AddListener(OnEndDiamondTicketsCollectButton);
            
            collectDiamondsTicketsButton.transform.localScale = Vector3.zero;
        }

        if (collectDiamondsText)
            collectDiamondsText.text = "0";

        if (collectTicketsText)
            collectTicketsText.text = "0";

        if (playEndingButton)
        {
            playEndingButton.interactable = true;
            playEndingButton.onClick.RemoveAllListeners();
            playEndingButton.onClick.AddListener(OnPlayEndingButton);
            
            playEndingButton.transform.localScale = Vector3.zero;
        }

        if (playEndingButtonAd)
        {
            playEndingButtonAd.interactable = true;
            playEndingButtonAd.onClick.RemoveAllListeners();
            playEndingButtonAd.onClick.AddListener(OnPlayEndingButtonAd);
            
            playEndingButtonAd.transform.localScale = Vector3.zero;
        }

        if (noThanksButton)
        {
            noThanksButton.interactable = true;
            noThanksButton.onClick.RemoveAllListeners();
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
            case AdsNames.rewardFreeStoryEnd:
                OnAdFreeStoryEnd();
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

        episodesHandler = FindObjectOfType<EpisodesHandler>();
    }

    [ContextMenu("Play Story End Screen")]
    public override void PlayEndingStoryScreen()
    {
        ResetStuffs();

        if (!endScreenCanvasGroup.gameObject.activeSelf)
            endScreenCanvasGroup.gameObject.SetActive(true);

        endScreenCanvasGroup.interactable = true;
        endScreenCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        if ((episodesSpawner != null && episodesSpawner.storiesDBItem != null) && episodesSpawner.storiesDBItem.isReworked)
        {
            endSeq.append(LeanTween.alphaCanvas(storyByMaujflixCanvas, 1f, 1f));
            endSeq.append(3f);
            endSeq.append(LeanTween.alphaCanvas(storyByMaujflixCanvas, 0, 1f));
            endSeq.append(0.5f);
        }
        endSeq.append(LeanTween.alphaCanvas(endScreenCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congralutionsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            if (endParticleEffect != null)
                endParticleEffect.Play();
        }));
        endSeq.append(0.5f);
        endSeq.append(LeanTween.moveLocalX(middlePanelPartOne.gameObject, 0, 0.7f).setEase(outBackMore).setOnStart(() =>
        {
            //if(sayDialog != null)
            //sayDialog.StoryText = "";

            if (sayDialog != null)
                LeanTween.alphaText(sayDialog.StoryTextRectTrans, 0, 0);

            if (vfxDiamondsParticles != null)
                vfxDiamondsParticles.Play(true);
        }).setOnComplete(OnMiddlePanelComplete));
    }

    private void OnMiddlePanelComplete()
    {
        if (sayDialog == null)
            return;

        //sayDialog.Say(endBranchDiamondsText, true, false, false, false, false, null, OnEndDiamondsTextComplete);              
        LeanTween.alphaText(sayDialog.StoryTextRectTrans, 1f, 1f).setOnComplete(OnEndDiamondsTextComplete);
    }

    private void OnEndDiamondsTextComplete()
    {
        LTSeq endDiamondsPanelSeq = LeanTween.sequence();

        endDiamondsPanelSeq.append(0.3f);
        endDiamondsPanelSeq.append(LeanTween.scale(collectDiamondsPanel.gameObject, Vector3.one, 0.5f).setEase(outBackMore));
        endDiamondsPanelSeq.append(LeanTween.value(0, 25f, 0.8f).setOnUpdate((float val) => collectDiamondsText.text = "+" + Mathf.RoundToInt(val).ToString()).setEase(LeanTweenType.linear));
        endDiamondsPanelSeq.append(LeanTween.scale(collectTicketsPanel.gameObject, Vector3.one, 0.5f).setEase(outBackMore));
        endDiamondsPanelSeq.append(LeanTween.value(0, 10f, 0.8f).setOnUpdate((float val) => collectTicketsText.text = "+" + Mathf.RoundToInt(val).ToString()).setEase(LeanTweenType.linear));        
        endDiamondsPanelSeq.append(LeanTween.scale(collectDiamondsTicketsButton.gameObject, Vector3.one, 0.4f).setEase(outBackMore));
        endDiamondsPanelSeq.append(() =>
        {
            if (episodesSpawner == null && EpisodesSpawner.instance)
                episodesSpawner = EpisodesSpawner.instance;

            episodesSpawner.topPanel.ShowTopPanel();
        });
    }

    private void OnEndDiamondTicketsCollectButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        collectDiamondsTicketsButton.interactable = false;
        LeanTween.scale(collectDiamondsTicketsButton.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInBack);

        episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(collectDiamondsPanel, episodesSpawner.topPanel.diamondsPanelIcon, 10, 25, OnEndStoryDiamondsCollected);
    }

    private void OnEndStoryDiamondsCollected()
    {
        episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(collectTicketsPanel, episodesSpawner.topPanel.ticketsPanelIcon, 10, 10, OnEndStoryDiamondsTicketsCollected);
    }

    private void OnEndStoryDiamondsTicketsCollected()
    {
        LTSeq seqPart2 = LeanTween.sequence();

        seqPart2.append(0.5f);
        seqPart2.append(LeanTween.moveLocalX(middlePanelPartOne.gameObject, 1000f, 0.7f).setOnStart(() =>
        {
            LeanTween.moveLocalX(middlePanelPartTwo.gameObject, 0, 0.7f).setEase(LeanTweenType.easeInOutBack);

            if (episodesSpawner)
                episodesSpawner.topPanel.HideTopPanel();

            if (topTextDialog != null && topTextAskDialog != null)
            {
                //topTextDialog.StoryText = "";
                //topTextAskDialog.StoryText = "";

                LeanTween.alphaText(topTextDialog.StoryTextRectTrans, 0, 0);
                LeanTween.alphaText(topTextAskDialog.StoryTextRectTrans, 0, 0);
            }
        }).setEase(LeanTweenType.easeInOutBack));
        //seqPart2.insert(LeanTween.moveLocalX(middlePanelPartTwo.gameObject, 0, 0.7f).setEase(LeanTweenType.easeInOutBack));
        seqPart2.append(() =>
        {
            if (topTextDialog)
            {
                //topTextDialog.Say(endBranchMultipleEndingsText, true, false, false, false, false, null, OnTopTextComplete);
                LeanTween.alphaText(topTextDialog.StoryTextRectTrans, 1f, 1f).setOnComplete(OnTopTextComplete);
            }
        });
    }

    private void OnTopTextComplete()
    {
        if (topTextAskDialog == null)
            return;

        if (topTextAskDialog)
        {
            //topTextAskDialog.Say(endBranchMultipleEndingsAsk, true, false, false, false, false, null, OnTopTextAskComplete);
            LeanTween.alphaText(topTextAskDialog.StoryTextRectTrans, 1f, 1f).setOnComplete(OnTopTextAskComplete);
        }
    }

    private void OnTopTextAskComplete()
    {
        if (playEndingButton == null || playEndingButtonAd == null)
            return;

        LTSeq lastSeq = LeanTween.sequence();

        lastSeq.append(LeanTween.scale(playEndingButton.gameObject, Vector3.one, 0.5f).setEase(outBackMore));
        lastSeq.insert(LeanTween.scale(playEndingButtonAd.gameObject, Vector3.one, 0.5f).setEase(outBackMore)/*.setOnComplete(LoopPlayEndingAdButton)*/);
        lastSeq.append(3f);
        lastSeq.append(LeanTween.alphaCanvas(noThanksButtonCanvasGrp, 1f, 1f).setOnStart(() =>
        {
            noThanksButtonCanvasGrp.interactable = true;
            noThanksButtonCanvasGrp.blocksRaycasts = true;
        }).setEase(LeanTweenType.easeInOutSine));
    }

    private void OnPlayEndingButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        if (FirebaseFirestoreOffline.instance == null)
            return;

        if(FirebaseFirestoreOffline.instance.GetDiamondsAmountInt() < 5)
        {
            ShowNoDiamondsPopup();
            return;
        }

        //if (FirebaseFirestoreOffline.instance == null)
        //return;

        //FirebaseFirestoreOffline.instance.DebitDiamondsAmount(5f);
        //episodesSpawner.topPanel.PlayDiamondPanelCollectAnim();

        //EpisodesSpawner.instance.StartLoadingStoryScene();

        StartCoroutine(OnPlayEndingButtonRoutine());
    }

    private IEnumerator OnPlayEndingButtonRoutine()
    {
        noThanksButton.interactable = false;
        playEndingButton.interactable = false;
        playEndingButtonAd.interactable = false;

        episodesSpawner.topPanel.ShowTopPanel();

        yield return new WaitForSeconds(0.35f);

        episodesSpawner.diamondsPool.PlayDiamondsAnimationDebit(playEndingDiamondIcon, episodesSpawner.topPanel.diamondsPanelIcon, 5, 5, OnStoryEndDiamondDebitComplete);
    }

    private void OnStoryEndDiamondDebitComplete()
    {
        StartCoroutine(OnStoryEndDiamondDebitCompleteRoutine());
    }

    private IEnumerator OnStoryEndDiamondDebitCompleteRoutine()
    {
        episodesSpawner.topPanel.HideTopPanel(0.3f, 0.7f);

        yield return new WaitForSeconds(0.5f);

        endScreenCanvasGroup.interactable = false;
        endScreenCanvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(endScreenCanvasGroup, 0, 1f).setEaseInOutSine();

        if (episodesHandler == null)
            episodesHandler = FindObjectOfType<EpisodesHandler>();

        //episodesSpawner.StartLoadingStoryScene();
        if (episodesHandler != null)
            episodesHandler.PlayLatestBranchEpisode();
    }

    private void OnPlayEndingButtonAd()
    {
        //TODO: Add Iron Source Integration
#if UNITY_EDITOR
        OnAdFreeStoryEnd();
#elif UNITY_ANDROID || UNITY_IOS
        if (AdsManager.instance == null)
            return;

        AdsManager.instance.ShowRewardAd(AdsNames.rewardFreeStoryEnd);
#endif
    }

    private void OnAdFreeStoryEnd()
    {
        noThanksButton.interactable = false;
        playEndingButton.interactable = false;
        playEndingButtonAd.interactable = false;

        StartCoroutine(OnStoryEndDiamondDebitCompleteRoutine());
    }

    private void OnNoThanksButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        noThanksButton.interactable = false;
        playEndingButton.interactable = false;
        playEndingButtonAd.interactable = false;

        episodesSpawner.LoadEpisodesMainMenu(false);
    }

    //-------------------------------------------------------------------------------------------------------

    private void ShowNoDiamondsPopup()
    {
        if (noDiamondsPanel == null)
            return;

        noDiamondsPanel.interactable = true;
        noDiamondsPanel.blocksRaycasts = true;
        LeanTween.alphaCanvas(noDiamondsPanel, 1f, 0.2f).setEaseInOutSine();
    }

    private void HideNoDiamondsPopup()
    {
        if (noDiamondsPanel == null)
            return;

        noDiamondsPanel.interactable = false;
        noDiamondsPanel.blocksRaycasts = false;
        LeanTween.alphaCanvas(noDiamondsPanel, 0, 0.2f).setEaseInOutSine();
    }

    private void OnNoDiamondsOkButton()
    {
        HideNoDiamondsPopup();
    }

    //-------------------------------------------------------------------------------------------------------
}
