using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MflixUIDailyRewards : MonoBehaviour
{
    [Header("Main Canvas / Prefabs")]
    public CanvasGroup dailyCanvasMain;
    public MflixDailyRewardItemSmall dailyRewardSmallPrefab;
    public MflixDailyRewardItemBig dailyRewardBigPrefab;

    [Header("Debug")]
    public bool isDebug;
    public GameObject panelDebug;
    public Button buttonAdvanceDay;
    public Button buttonAdvanceHour;
    public Button buttonReset;
    public Button buttonReloadScene;

    [Header("Rewards Panel")]
    public Transform rewardPanelRow1;
    public Transform rewardPanelRow2;
    public Transform rewardPanelRow3;

    [Header("Buttons")]
    public Button buttonClaim;
    public Button buttonClaimAd;

    [Header("Gifts Panel")]
    public Image giftProgressBar;
    public MflixUIGiftItem[] giftItems;

    [Space(15)]

    public CanvasGroup giftPanelMain;
    public Text giftDayText;

    public Image giftLockedImage;
    public Image giftUnlockedImage;

    public Transform giftDiamondPanel;
    public TextMeshProUGUI giftDiamondAmountText;

    public Transform giftTicketPanel;
    public TextMeshProUGUI giftTicketAmountText;

    public Button giftCollectButton;

    private MflixDailyRewards dailyRewards;

    private bool readyToClaim;
    private List<MflixDailyRewardItemSmall> dailyRewardsUIItems = new List<MflixDailyRewardItemSmall>();

    private EpisodesSpawner episodesSpawner;

    private int giftDay;
    private float giftProgress;

    private int giftDiamondAmount;
    private int giftTicketAmount;

    private bool isGiftCheckupDone;
    private bool isGiftRewardAvailable = false;
    private bool isDoubleRewardTriggered;

    private void OnValidate()
    {
        if (giftItems.Length <= 0)
            giftItems = GetComponentsInChildren<MflixUIGiftItem>();
    }

    private void Awake()
    {
        dailyRewards = GetComponent<MflixDailyRewards>();

        if (dailyCanvasMain)
        {
            dailyCanvasMain.alpha = 0;
            dailyCanvasMain.interactable = false;
            dailyCanvasMain.blocksRaycasts = false;
        }

        if(giftPanelMain)
        {
            giftPanelMain.alpha = 0;
            giftPanelMain.interactable = false;
            giftPanelMain.blocksRaycasts = false;
        }

        if (giftDiamondPanel)
            giftDiamondPanel.transform.localScale = Vector3.zero;

        if (giftTicketPanel)
            giftTicketPanel.transform.localScale = Vector3.zero;

        if(giftCollectButton)
        {
            giftCollectButton.transform.localScale = Vector3.zero;
            giftCollectButton.onClick.AddListener(OnGiftCollectButton);
        }    
    }

    private void OnEnable()
    {
        AdsManager.OnIronSrcRewardVideoComplete += OnRewardAdComplete;

        if (dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize += OnClaimPrize;
            //dailyRewards.onMFlixMissedPrize += OnMissedPrize;
            dailyRewards.onInitialize += OnInitialize;
        }
    }

    private void OnDisable()
    {
        AdsManager.OnIronSrcRewardVideoComplete -= OnRewardAdComplete;

        if (dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize -= OnClaimPrize;
            //dailyRewards.onMFlixMissedPrize -= OnMissedPrize;
            dailyRewards.onInitialize -= OnInitialize;
        }
    }

    private void OnDestroy()
    {
        AdsManager.OnIronSrcRewardVideoComplete -= OnRewardAdComplete;

        if (dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize -= OnClaimPrize;
            //dailyRewards.onMFlixMissedPrize -= OnMissedPrize;
            dailyRewards.onInitialize -= OnInitialize;
        }
    }

    private void OnRewardAdComplete(string placementName)
    {
        switch (placementName)
        {
            case AdsNames.rewardDoubleDaily:
                OnAdDoubleDailyRewardComplete();
                break;
        }
    }

    private void OnAdDoubleDailyRewardComplete()
    {
        isDoubleRewardTriggered = true;

        dailyRewards.ClaimPrize();
        readyToClaim = false;

        buttonClaim.interactable = false;
        LeanTween.scale(buttonClaim.gameObject, Vector3.zero, 0.5f).setEaseInBack();

        buttonClaimAd.interactable = false;
        LeanTween.scale(buttonClaimAd.gameObject, Vector3.zero, 0.5f).setEaseInBack();
    }

    private void Start()
    {
        if (EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        InitializeDailyRewardsUI();

        if (buttonClaim)
        {
            buttonClaim.onClick.AddListener(() =>
            {
                dailyRewards.ClaimPrize();
                readyToClaim = false;
                
                buttonClaim.interactable = false;
                LeanTween.scale(buttonClaim.gameObject, Vector3.zero, 0.5f).setEaseInBack();

                buttonClaimAd.interactable = false;
                LeanTween.scale(buttonClaimAd.gameObject, Vector3.zero, 0.5f).setEaseInBack();
                //UpdateUI();
            });
        }

        if(buttonClaimAd)
        {
            buttonClaimAd.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                isDoubleRewardTriggered = true;

                dailyRewards.ClaimPrize();
                readyToClaim = false;

                buttonClaim.interactable = false;
                LeanTween.scale(buttonClaim.gameObject, Vector3.zero, 0.5f).setEaseInBack();

                buttonClaimAd.interactable = false;
                LeanTween.scale(buttonClaimAd.gameObject, Vector3.zero, 0.5f).setEaseInBack();
#elif UNITY_ANDROID || UNITY_IOS
                if (AdsManager.instance == null)
                    return;

                AdsManager.instance.ShowRewardAd(AdsNames.rewardDoubleDaily);
#endif                
            });
        }

        if (panelDebug)
        {
            panelDebug.SetActive(isDebug);

            if (buttonAdvanceDay)
            {
                buttonAdvanceDay.onClick.AddListener(() =>
                {
                    dailyRewards.debugTime = dailyRewards.debugTime.Add(new TimeSpan(1, 0, 0, 0));
                    SetupGiftRewardsPanel();
                    UpdateUI();
                });
            }

            // Simulates the next hour
            if (buttonAdvanceHour)
            {
                buttonAdvanceHour.onClick.AddListener(() =>
                {
                    dailyRewards.debugTime = dailyRewards.debugTime.Add(new TimeSpan(1, 0, 0));
                    UpdateUI();
                });
            }

            if (buttonReset)
            {
                // Resets Daily Rewards from Player Preferences
                buttonReset.onClick.AddListener(() =>
                {
                    dailyRewards.Reset();
                    dailyRewards.debugTime = new TimeSpan();
                    dailyRewards.lastRewardTime = System.DateTime.MinValue;
                    readyToClaim = false;
                });
            }

            // Reloads the same scene
            if (buttonReloadScene)
            {
                buttonReloadScene.onClick.AddListener(() =>
                {
                    //Application.LoadLevel(Application.loadedLevel);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            }
        }

        UpdateUI();
    }

    private void InitializeDailyRewardsUI()
    {
        if (dailyRewards.rewards.Count <= 0)
            return;

        for (int i = 0; i < dailyRewards.rewards.Count; i++)
        {
            int day = i + 1;
            var reward = dailyRewards.GetReward(day);

            if(reward.isBigReward)
            {
                MflixDailyRewardItemBig rewardItemBigInstance = Instantiate(dailyRewardBigPrefab, rewardPanelRow3);
                if(rewardItemBigInstance)
                {
                    rewardItemBigInstance.day = day;
                    rewardItemBigInstance.reward = reward;
                    rewardItemBigInstance.Initialize();
                }

                dailyRewardsUIItems.Add(rewardItemBigInstance);
            }
            else
            {
                MflixDailyRewardItemSmall rewardItemSmallInstance = Instantiate(dailyRewardSmallPrefab);
                if (rewardItemSmallInstance)
                {
                    if (i <= 2)
                    {
                        rewardItemSmallInstance.transform.SetParent(rewardPanelRow1);
                        rewardItemSmallInstance.day = day;
                        rewardItemSmallInstance.reward = reward;
                        rewardItemSmallInstance.Initialize();
                    }
                    else if (i > 2 && i <= 6)
                    {
                        rewardItemSmallInstance.transform.SetParent(rewardPanelRow2);
                        rewardItemSmallInstance.day = day;
                        rewardItemSmallInstance.reward = reward;
                        rewardItemSmallInstance.Initialize();
                    }

                    dailyRewardsUIItems.Add(rewardItemSmallInstance);
                }
            }
        }
    }

    public void UpdateUI()
    {
        dailyRewards.CheckRewards();

        bool isRewardAvailableNow = false;

        var lastReward = dailyRewards.lastReward;
        var availableReward = dailyRewards.availableReward;

        foreach (var dailyRewardUI in dailyRewardsUIItems)
        {
            var day = dailyRewardUI.day;

            if (day == availableReward)
            {
                dailyRewardUI.state = MflixDailyRewardItemSmall.DailyRewardState.UNCLAIMED_AVAILABLE;

                isRewardAvailableNow = true;
            }
            else if (day <= lastReward)
            {
                dailyRewardUI.state = MflixDailyRewardItemSmall.DailyRewardState.CLAIMED;
            }
            else
            {
                dailyRewardUI.state = MflixDailyRewardItemSmall.DailyRewardState.UNCLAIMED_UNAVAILABLE;
            }

            dailyRewardUI.Refresh();
        }

        //buttonClaim.gameObject.SetActive(isRewardAvailableNow);
        if(isRewardAvailableNow)
        {
            buttonClaim.interactable = true;
            LeanTween.scale(buttonClaim.gameObject, Vector3.one, 0.5f).setEaseOutBack();

            buttonClaimAd.interactable = true;
            LeanTween.scale(buttonClaimAd.gameObject, Vector3.one, 0.5f).setEaseOutBack();
        }
        else
        {
            buttonClaim.interactable = false;
            LeanTween.scale(buttonClaim.gameObject, Vector3.zero, 0.5f).setEaseInBack();

            buttonClaimAd.interactable = false;
            LeanTween.scale(buttonClaimAd.gameObject, Vector3.zero, 0.5f).setEaseInBack();
        }

        readyToClaim = isRewardAvailableNow;

        //Gifts Section
        giftDay = PlayerPrefs.GetInt(dailyRewards.GetLastRewardGiftDayKey());
        giftProgress = PlayerPrefs.GetFloat(dailyRewards.GetLastRewardGiftProgressKey());

        giftProgressBar.fillAmount = giftProgress;

        for (int i = 0; i < giftItems.Length && (giftItems.Length > 0); i++)        
            giftItems[i].IsDone = (giftDay > giftItems[i].day) ? true : false;

        //if (isRewardAvailableNow && isDebug)
            //SetupGiftRewards();

        /*print(isRewardAvailableNow);
        if(isRewardAvailableNow && !isGiftCheckupDone)
        {
            isGiftCheckupDone = true;

            giftDay++;
            if (giftDay >= 31)
                giftDay = 1;

            giftProgress = giftDay / 30f;

            //giftProgressBar.fillAmount = giftProgress;
            LeanTween.value(giftProgressBar.fillAmount, giftProgress, 1f).setEaseLinear();

            PlayerPrefs.SetInt(dailyRewards.GetLastRewardGiftDayKey(), giftDay);
            PlayerPrefs.SetFloat(dailyRewards.GetLastRewardGiftProgressKey(), giftProgress);

            for (int i = 0; i < giftItems.Length && (giftItems.Length > 0); i++)
            {
                giftItems[i].IsDone = (giftDay > giftItems[i].day) ? true : false;

                if (giftDay == giftItems[i].day)
                {
                    isGiftRewardAvailable = true;

                    if(giftItems[i].giftParticlesVfx)
                        giftItems[i].giftParticlesVfx.Play(true);

                    break;
                }
            }
        }*/
    }

    private void OnClaimPrize(int day)
    {
        if (episodesSpawner == null && EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        /*panelReward.SetActive(true);

        var reward = dailyRewards.GetReward(day);
        var unit = reward.unit;
        var rewardQt = reward.reward;
        imageReward.sprite = reward.sprite;
        if (rewardQt > 0)
        {
            textReward.text = string.Format("You got {0} {1}!", reward.reward, unit);
        }
        else
        {
            textReward.text = string.Format("You got {0}!", unit);
        }*/

        MflixReward reward = dailyRewards.GetReward(day);
        if(isDoubleRewardTriggered)
        {
            reward.rewardDiamondAmount *= 2;
            reward.rewardTicketAmount *= 2;

            isDoubleRewardTriggered = false;
        }

        MflixDailyRewardItemSmall rewardItem = dailyRewardsUIItems[day - 1];

        MflixRewardGift giftReward = null;
        MflixUIGiftItem giftItem = null;
        
        if (isGiftRewardAvailable)
        {
            giftReward = dailyRewards.GetRewardGiftFromDay(giftDay);
            giftItem = GetGiftItem(giftDay);
        }

        episodesSpawner.topPanel.ShowTopPanel(0.3f);

        if (reward.hasDiamondReward && !reward.hasTicketReward)          //Only Diamonds
        {
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.diamondsPanelIcon, reward.rewardDiamondAmount >= 10 ? 10 : reward.rewardDiamondAmount, reward.rewardDiamondAmount, () =>
            {
                if (isGiftRewardAvailable && giftReward != null && giftItem != null)
                {
                    giftItem.IsDone = true;
                    ShowGiftRewardPanel(giftReward);
                }
                else
                    OnPriceClaimedDone();
            }, 200f);
        }
        else if(!reward.hasDiamondReward && reward.hasTicketReward)     //Only tickets
        {
            episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.ticketsPanelIcon, reward.rewardTicketAmount >= 15f ? 10 : reward.rewardTicketAmount, reward.rewardTicketAmount, () =>
            {
                if (isGiftRewardAvailable && giftReward != null && giftItem != null)
                {
                    giftItem.IsDone = true;
                    ShowGiftRewardPanel(giftReward);
                }
                else
                    OnPriceClaimedDone();
            }, 200f);
        }
        else if(reward.hasDiamondReward && reward.hasTicketReward)      //Both
        {
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.diamondsPanelIcon, reward.rewardDiamondAmount >= 10 ? 10 : reward.rewardDiamondAmount, reward.rewardDiamondAmount, () =>
            {
                episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.ticketsPanelIcon, reward.rewardTicketAmount >= 15f ? 10 : reward.rewardTicketAmount, reward.rewardTicketAmount, () =>
                {
                    if (isGiftRewardAvailable && giftReward != null && giftItem != null)
                    {
                        giftItem.IsDone = true;
                        ShowGiftRewardPanel(giftReward);
                    }
                    else
                        OnPriceClaimedDone();
                }, 200f);
            }, 200f);
        }
    }

    private void OnMissedPrize()
    {
        if (dailyRewards == null)
            return;

        giftDay = 1;
        giftProgress = giftDay / 30f;
        giftProgressBar.fillAmount = 0;

        dailyRewards.Reset();
    }

    private void OnPriceClaimedDone()
    {
        UpdateUI();

        if(isDebug)
            episodesSpawner.topPanel.HideTopPanel(0.3f, 1f);
        else if (!isDebug)
        {
            dailyCanvasMain.interactable = false;
            dailyCanvasMain.blocksRaycasts = false;
            LeanTween.alphaCanvas(dailyCanvasMain, 0, 0.3f).setDelay(1f);
        }
    }

    private void CheckTimeDifference()
    {
        if (!readyToClaim)
        {
            TimeSpan difference = dailyRewards.GetTimeDifference();

            // If the counter below 0 it means there is a new reward to claim
            if (difference.TotalSeconds <= 0)
            {
                readyToClaim = true;
                UpdateUI();                
                return;
            }            
        }
    }

    private void OnInitialize(bool error, string errorMessage)
    {
        if (!error)
        {
            var showWhenNotAvailable = dailyRewards.keepOpen;
            var isRewardAvailable = dailyRewards.availableReward > 0;

            UpdateUI();
            //canvas.gameObject.SetActive(showWhenNotAvailable || (!showWhenNotAvailable && isRewardAvailable));
            if(showWhenNotAvailable || (!showWhenNotAvailable && isRewardAvailable))
            {
                dailyCanvasMain.interactable = true;
                dailyCanvasMain.blocksRaycasts = true;
                LeanTween.alphaCanvas(dailyCanvasMain, 1f, 0.3f).setEaseInOutSine();

                //GiftProgress++;

                if (EpisodesSpawner.instance != null)
                {
                    episodesSpawner = EpisodesSpawner.instance;
                    episodesSpawner.topPanel.HideTopPanel(0.3f);
                }

                if(isRewardAvailable)
                    SetupGiftRewardsPanel();
            }

            CheckTimeDifference();

            StartCoroutine(TickTime());
        }
    }

    private void SetupGiftRewardsPanel()
    {
        giftDay++;
        if (giftDay >= 31)
            giftDay = 1;

        giftProgress = giftDay / 30f;
        giftProgressBar.fillAmount = giftProgress;
        //LeanTween.value(giftProgressBar.fillAmount, giftProgress, 1f).setEaseLinear().setOnUpdate( (float val) => giftProgressBar.fillAmount = val );

        PlayerPrefs.SetInt(dailyRewards.GetLastRewardGiftDayKey(), giftDay);
        PlayerPrefs.SetFloat(dailyRewards.GetLastRewardGiftProgressKey(), giftProgress);

        for (int i = 0; i < giftItems.Length && (giftItems.Length > 0); i++)
        {
            giftItems[i].IsDone = (giftDay > giftItems[i].day) ? true : false;

            if (giftDay == giftItems[i].day)
            {
                isGiftRewardAvailable = true;

                if (giftItems[i].giftParticlesVfx)
                    giftItems[i].giftParticlesVfx.Play(true);

                break;
            }
        }
    }

    private IEnumerator TickTime()
    {
        for (; ; )
        {
            dailyRewards.TickTime();
            // Updates the time due
            CheckTimeDifference();
            yield return null;
        }
    }

    private MflixUIGiftItem GetGiftItem(int day)
    {
        for (int i = 0; i < giftItems.Length && (giftItems.Length > 0); i++)
        {
            if (giftItems[i].day == day)
                return giftItems[i];
        }

        return null;
    }

    private void ShowGiftRewardPanel(MflixRewardGift giftReward)
    {
        giftCollectButton.interactable = true;

        giftDayText.text = string.Format("दिन {0}", MflixDailyRewards.GetHindiNumber(giftReward.dayToReward.ToString()));
        giftDayText.GetComponent<SiddhantaFixer>().FixTexts();

        giftDiamondAmount = giftReward.giftDiamondAmount;
        giftTicketAmount = giftReward.giftTicketAmount;

        giftLockedImage.sprite = giftReward.giftClosedSprite;
        giftLockedImage.color = Color.white;

        giftUnlockedImage.sprite = giftReward.giftOpenedSprite;
        giftUnlockedImage.color = new Color(1f, 1f, 1f, 0);

        giftDiamondPanel.localScale = Vector3.zero;
        giftTicketPanel.localScale = Vector3.zero;

        giftDiamondAmountText.text = "0";
        giftTicketAmountText.text = "0";

        LTSeq giftPanelSeq = LeanTween.sequence();

        giftPanelSeq.append(1f);
        giftPanelSeq.append(LeanTween.alphaCanvas(giftPanelMain, 1f, 0.5f).setOnStart( () => 
        {
            giftPanelMain.interactable = true;
            giftPanelMain.blocksRaycasts = true;
        }).setEaseInOutSine());
        giftPanelSeq.append(0.5f);
        giftPanelSeq.append(LeanTween.scale(giftDiamondPanel.gameObject, Vector3.one, 0.5f).setEaseOutBack());
        giftPanelSeq.append(LeanTween.value(0, giftReward.giftDiamondAmount, 0.5f).setEaseLinear().setOnUpdate( (float f) => giftDiamondAmountText.text = "+" + Mathf.RoundToInt(f).ToString()));
        giftPanelSeq.append(0.5f);
        giftPanelSeq.append(LeanTween.scale(giftTicketPanel.gameObject, Vector3.one, 0.5f).setEaseOutBack());
        giftPanelSeq.append(LeanTween.value(0, giftReward.giftTicketAmount, 0.5f).setEaseLinear().setOnUpdate( (float f) => giftTicketAmountText.text = "+" + Mathf.RoundToInt(f).ToString()));
        giftPanelSeq.append(0.5f);
        giftPanelSeq.append(LeanTween.scale(giftCollectButton.gameObject, Vector3.one, 0.5f).setEaseOutBack());
    }

    private void OnGiftCollectButton()
    {
        giftCollectButton.interactable = false;

        LTSeq rewardSeq = LeanTween.sequence();

        rewardSeq.append(LeanTween.scale(giftCollectButton.gameObject, Vector3.zero, 0.4f).setEaseInBack());        
        rewardSeq.append(LeanTween.alpha(giftLockedImage.rectTransform, 0, 1f).setEaseInOutSine().setOnStart( () => 
        {
            LeanTween.alpha(giftUnlockedImage.rectTransform, 1f, 1f).setEaseInOutSine();
        }));
        rewardSeq.append(0.5f);
        rewardSeq.append(() =>
        {
            episodesSpawner.topPanel.ShowTopPanel();
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(giftDiamondPanel, episodesSpawner.topPanel.diamondsPanelIcon, giftDiamondAmount >= 30f ? 10 : giftDiamondAmount, giftDiamondAmount, () =>
            {
                episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(giftTicketPanel, episodesSpawner.topPanel.ticketsPanelIcon, giftTicketAmount >= 15f ? 10 : giftTicketAmount, giftTicketAmount, () =>
                {
                    giftPanelMain.interactable = false;
                    giftPanelMain.blocksRaycasts = false;
                    LeanTween.alphaCanvas(giftPanelMain, 0, 0.5f).setEaseInOutSine().setDelay(1f);

                    OnPriceClaimedDone();
                });
            });
        });        
    }
}
