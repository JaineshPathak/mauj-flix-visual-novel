using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Gifts Panel")]
    public Image giftProgressBar;

    private MflixDailyRewards dailyRewards;

    private bool readyToClaim;
    private List<MflixDailyRewardItemSmall> dailyRewardsUIItems = new List<MflixDailyRewardItemSmall>();

    private EpisodesSpawner episodesSpawner;

    private float giftProgress;

    private float GiftProgress 
    {
        get { return giftProgress; }
        set
        {
            if (giftProgressBar)
            {
                //value = Mathf.Clamp(value, 0, 1f);
                giftProgressBar.fillAmount = value / 30f;
            }
        } 
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
    }

    private void OnEnable()
    {
        if(dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize += OnClaimPrize;
            dailyRewards.onInitialize += OnInitialize;
        }
    }

    private void OnDisable()
    {
        if (dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize -= OnClaimPrize;
            dailyRewards.onInitialize -= OnInitialize;
        }
    }

    private void OnDestroy()
    {
        if (dailyRewards)
        {
            dailyRewards.onMFlixClaimPrize -= OnClaimPrize;
            dailyRewards.onInitialize -= OnInitialize;
        }
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
                //UpdateUI();
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
        }
        else
        {
            buttonClaim.interactable = false;
            LeanTween.scale(buttonClaim.gameObject, Vector3.zero, 0.5f).setEaseInBack();
        }

        readyToClaim = isRewardAvailableNow;
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
        MflixDailyRewardItemSmall rewardItem = dailyRewardsUIItems[day - 1];

        episodesSpawner.topPanel.ShowTopPanel(0.3f);

        if (reward.hasDiamondReward && !reward.hasTicketReward)          //Only Diamonds
        {
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.diamondsPanelIcon, reward.rewardDiamondAmount, reward.rewardDiamondAmount, () =>
            {
                OnPriceClaimedDone();
            }, 200f);
        }
        else if(!reward.hasDiamondReward && reward.hasTicketReward)     //Only tickets
        {
            episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.ticketsPanelIcon, reward.rewardTicketAmount, () =>
            {
                OnPriceClaimedDone();
            }, 200f);
        }
        else if(reward.hasDiamondReward && reward.hasTicketReward)      //Both
        {
            episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.diamondsPanelIcon, reward.rewardDiamondAmount, reward.rewardDiamondAmount, () =>
            {
                episodesSpawner.diamondsPool.PlayTicketsAnimationDeposit(rewardItem.transform, episodesSpawner.topPanel.ticketsPanelIcon, reward.rewardTicketAmount, () =>
                {
                    OnPriceClaimedDone();
                }, 200f);
            }, 200f);
        }
    }

    private void OnPriceClaimedDone()
    {
        UpdateUI();
        episodesSpawner.topPanel.HideTopPanel(0.3f, 1f);
        //dailyCanvasMain.interactable = false;
        //dailyCanvasMain.blocksRaycasts = false;
        //LeanTween.alphaCanvas(dailyCanvasMain, 0, 0.3f).setDelay(1f);
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
            }

            CheckTimeDifference();

            StartCoroutine(TickTime());
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
}
