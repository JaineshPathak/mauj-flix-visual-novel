using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using NiobiumStudios;

//Modified version of Daily Rewards by Niobium Studios for Maujflix game
public class MflixDailyRewards : DailyRewardsCore<MflixDailyRewards>
{
    public List<MflixReward> rewards;
    public List<MflixRewardGift> rewardsGift;
    public DateTime lastRewardTime;
    public int availableReward;
    public int lastReward;
    public bool keepOpen = true;
    public bool resetPrize = true;

    public delegate void OnMFlixClaimPrize(int day);
    public OnMFlixClaimPrize onMFlixClaimPrize;

    public delegate void OnMFlixMissedPrize();
    public OnMFlixMissedPrize onMFlixMissedPrize;

    private const string LAST_REWARD_TIME = "LastRewardTime";
    private const string LAST_REWARD = "LastReward";
    private const string LAST_GIFT_REWARD_DAY = "LastGiftRewardDay";
    private const string LAST_GIFT_REWARD_PROGRESS = "LastGiftRewardProgress";
    private const string DEBUG_TIME = "DebugTime";
    private const string FMT = "O";

    public TimeSpan debugTime;         // For debug purposes only

    private void Start()
    {
        StartCoroutine(InitializeTimer());
    }

    private IEnumerator InitializeTimer()
    {
        yield return StartCoroutine(base.InitializeDate());

        if (base.isErrorConnect)
        {
            if (onInitialize != null)
                onInitialize(true, base.errorMessage);
        }
        else
        {
            LoadDebugTime();
            // We don't count seconds on Daily Rewards
            //now = now.AddSeconds(-now.Second);
            CheckRewards();

            if (onInitialize != null)
                onInitialize();
        }
    }

    protected override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
        CheckRewards();
    }

    public TimeSpan GetTimeDifference()
    {
        TimeSpan difference = (lastRewardTime - now);
        difference = difference.Subtract(debugTime);
        return difference.Add(new TimeSpan(0, 24, 0, 0));
    }

    private void LoadDebugTime()
    {
        int debugHours = PlayerPrefs.GetInt(GetDebugTimeKey(), 0);
        debugTime = new TimeSpan(debugHours, 0, 0);
    }

    public void CheckRewards()
    {
        string lastClaimedTimeStr = PlayerPrefs.GetString(GetLastRewardTimeKey());
        lastReward = PlayerPrefs.GetInt(GetLastRewardKey());

        // It is not the first time the user claimed.
        // We need to know if he can claim another reward or not
        if (!string.IsNullOrEmpty(lastClaimedTimeStr))
        {
            lastRewardTime = DateTime.ParseExact(lastClaimedTimeStr, FMT, CultureInfo.InvariantCulture);

            // if Debug time was added, we use it to check the difference
            DateTime advancedTime = now.AddHours(debugTime.TotalHours);

            TimeSpan diff = advancedTime - lastRewardTime;
            //Debug.Log("ID "+instanceId+" Last claim was " + (long)diff.TotalHours + " hours ago.");

            int days = (int)(Math.Abs(diff.TotalHours) / 24);
            if (days == 0)
            {
                // No claim for you. Try tomorrow
                availableReward = 0;
                return;
            }

            // The player can only claim if he logs between the following day and the next.
            if (days >= 1 && days < 2)
            {
                // If reached the last reward, resets to the first restarting the cicle
                if (lastReward == rewards.Count)
                {
                    availableReward = 1;
                    lastReward = 0;
                    return;
                }

                availableReward = lastReward + 1;

                //Debug.Log("ID " + instanceId + " Player can claim prize " + availableReward);
                return;
            }

            if (days >= 2 && resetPrize)
            {
                // The player loses the following day reward and resets the prize
                if (onMFlixMissedPrize != null)
                    onMFlixMissedPrize();

                availableReward = 1;
                lastReward = 0;                
                //Debug.Log("ID " + instanceId + " Prize reset ");
            }
        }
        else
        {
            // Is this the first time? Shows only the first reward
            availableReward = 1;
        }
    }

    public void ClaimPrize()
    {
        if (availableReward > 0)
        {
            // Delegate
            if (onMFlixClaimPrize != null)
                onMFlixClaimPrize(availableReward);

            Debug.Log("ID " + instanceId + " Reward [" + rewards[availableReward - 1] + "] Claimed!");
            PlayerPrefs.SetInt(GetLastRewardKey(), availableReward);

            // Remove seconds
            //var timerNoSeconds = now.AddSeconds(-now.Second);
            // If debug time was added then we store it
            //timerNoSeconds = timerNoSeconds.AddHours(debugTime.TotalHours);

            string lastClaimedStr = now.AddHours(debugTime.TotalHours).ToString(FMT);
            PlayerPrefs.SetString(GetLastRewardTimeKey(), lastClaimedStr);
            PlayerPrefs.SetInt(GetDebugTimeKey(), (int)debugTime.TotalHours);
        }
        else if (availableReward == 0)
        {
            Debug.LogError("Error! The player is trying to claim the same reward twice.");
        }

        CheckRewards();
    }

    private string GetLastRewardKey()
    {
        if (instanceId == 0)
            return LAST_REWARD;

        return string.Format("{0}_{1}", LAST_REWARD, instanceId);
    }

    //Returns the lastRewardTime playerPrefs key depending on instanceId
    private string GetLastRewardTimeKey()
    {
        if (instanceId == 0)
            return LAST_REWARD_TIME;

        return string.Format("{0}_{1}", LAST_REWARD_TIME, instanceId);
    }

    public string GetLastRewardGiftDayKey()
    {
        if (instanceId == 0)
            return LAST_GIFT_REWARD_DAY;

        return string.Format("{0}_{1}", LAST_GIFT_REWARD_DAY, instanceId);
    }

    public string GetLastRewardGiftProgressKey()
    {
        if (instanceId == 0)
            return LAST_GIFT_REWARD_PROGRESS;

        return string.Format("{0}_{1}", LAST_GIFT_REWARD_PROGRESS, instanceId);
    }

    //Returns the advanced debug time playerPrefs key depending on instanceId
    private string GetDebugTimeKey()
    {
        if (instanceId == 0)
            return DEBUG_TIME;

        return string.Format("{0}_{1}", DEBUG_TIME, instanceId);
    }    

    // Returns the daily Reward of the day
    public MflixReward GetReward(int day)
    {
        return rewards[day - 1];
    }

    public MflixRewardGift GetRewardGift(int index)
    {
        return rewardsGift[index - 1];
    }

    public MflixRewardGift GetRewardGiftFromDay(int day)
    {
        for (int i = 0; i < rewardsGift.Count; i++)
        {
            if (rewardsGift[i].dayToReward == day)
                return rewardsGift[i];
        }

        return null;
    }

    // Resets the Daily Reward for testing purposes
    public void Reset()
    {
        PlayerPrefs.DeleteKey(GetLastRewardKey());
        PlayerPrefs.DeleteKey(GetLastRewardTimeKey());
        PlayerPrefs.DeleteKey(GetLastRewardGiftDayKey());
        PlayerPrefs.DeleteKey(GetLastRewardGiftProgressKey());
        PlayerPrefs.DeleteKey(GetDebugTimeKey());
    }

    public static string GetHindiNumber(string englishNum)
    {
        string hindiNum = string.Empty;

        if (englishNum.Length <= 0)
            return hindiNum;

        char[] englishNumChar = englishNum.ToCharArray();
        List<char> hindiCharList = new List<char>();

        for (int i = 0; i < englishNumChar.Length; i++)
        {
            switch (englishNumChar[i])
            {
                case '1':
                    hindiCharList.Add('१');
                    break;

                case '2':
                    hindiCharList.Add('२');
                    break;

                case '3':
                    hindiCharList.Add('३');
                    break;

                case '4':
                    hindiCharList.Add('४');
                    break;

                case '5':
                    hindiCharList.Add('५');
                    break;

                case '6':
                    hindiCharList.Add('६');
                    break;

                case '7':
                    hindiCharList.Add('७');
                    break;

                case '8':
                    hindiCharList.Add('८');
                    break;

                case '9':
                    hindiCharList.Add('९');
                    break;

                case '0':
                    hindiCharList.Add('०');
                    break;
            }
        }

        hindiNum = new string(hindiCharList.ToArray());

        return hindiNum;
    }
}
