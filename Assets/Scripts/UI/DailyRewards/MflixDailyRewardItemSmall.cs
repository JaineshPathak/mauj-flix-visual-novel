using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Modified version of DailyRewardUI from Niobium Studios
public class MflixDailyRewardItemSmall : MonoBehaviour
{
    public enum DailyRewardState
    {
        UNCLAIMED_AVAILABLE,
        UNCLAIMED_UNAVAILABLE,
        CLAIMED
    }

    [Header("Texts")]
    //public Text textDay;
    public TextMeshProUGUI textDay;

    [Header("UI")]
    public Image backgroundBg;
    public Sprite backgroundBgAvailable;
    public Sprite backgroundBgUnavailable;
    public Image backgroundClaimedBg;

    [Space(15)]
    public HorizontalLayoutGroup iconsLayoutGroup;

    [Header("Prefab")]
    public Transform groupItemSmallParent;
    public MflixUIDailyRewardIconSmallGroupItem groupItemSmallPrefab;
    public List<MflixUIDailyRewardIconSmallGroupItem> groupItemsList = new List<MflixUIDailyRewardIconSmallGroupItem>();

    [Header("Internal")]
    public int day;
    public DailyRewardState state;

    [HideInInspector]
    public MflixReward reward;

    private void OnValidate()
    {        
        if (!Application.isPlaying)
        {
            if (groupItemSmallParent)
            {
                groupItemsList.Clear();
                foreach (Transform groupItem in groupItemSmallParent)                
                    groupItemsList.Add(groupItem.GetComponent<MflixUIDailyRewardIconSmallGroupItem>());                
            }

            Refresh();
        }
    }

    public void Initialize()
    {        
        if (textDay)
        {
            textDay.text = string.Format("दिन {0}", MflixDailyRewards.GetHindiNumber( (day).ToString()));
            if(textDay.GetComponent<CharReplacerHindi>() != null)
                textDay.GetComponent<CharReplacerHindi>().UpdateMe();
            //textDay.GetComponent<SiddhantaFixer>().FixTexts();
        }

        if(reward.hasDiamondReward)
        {
            MflixUIDailyRewardIconSmallGroupItem groupItemDiamond = Instantiate(groupItemSmallPrefab, groupItemSmallParent);
            groupItemDiamond.Initialize(reward.rewardDiamondSpriteNormal, reward.rewardDiamondSpriteLocked, MflixDailyRewards.GetHindiNumber(reward.rewardDiamondAmount.ToString()) + " " + reward.rewardDiamondName, reward.hasDiamondReward && reward.hasTicketReward && !reward.isBigReward);

            groupItemsList.Add(groupItemDiamond);
        }

        if(reward.hasTicketReward)
        {
            MflixUIDailyRewardIconSmallGroupItem groupItemTicket = Instantiate(groupItemSmallPrefab, groupItemSmallParent);
            groupItemTicket.Initialize(reward.rewardTicketSpriteNormal, reward.rewardTicketSpriteLocked, MflixDailyRewards.GetHindiNumber(reward.rewardTicketAmount.ToString()) + " " + reward.rewardTicketName, reward.hasDiamondReward && reward.hasTicketReward && !reward.isBigReward);

            groupItemsList.Add(groupItemTicket);
        }

        if (reward.hasDiamondReward && reward.hasTicketReward && !reward.isBigReward)
            iconsLayoutGroup.spacing = -20f;

        LayoutRebuilder.ForceRebuildLayoutImmediate(iconsLayoutGroup.GetComponent<RectTransform>());
    }

    public void Refresh()
    {
        for (int i = 0; i < groupItemsList.Count && (groupItemsList.Count > 0); i++)
        {
            if (groupItemsList[i] != null)
                groupItemsList[i].Refresh(state);
        }

        switch (state)
        {
            case DailyRewardState.UNCLAIMED_AVAILABLE:

                if(backgroundBgAvailable)
                    backgroundBg.sprite = backgroundBgAvailable;

                if(backgroundClaimedBg)
                    backgroundClaimedBg.gameObject.SetActive(false);                

                break;

            case DailyRewardState.UNCLAIMED_UNAVAILABLE:

                if(backgroundBgUnavailable)
                    backgroundBg.sprite = backgroundBgUnavailable;

                if (backgroundClaimedBg)
                    backgroundClaimedBg.gameObject.SetActive(false);

                break;

            case DailyRewardState.CLAIMED:

                if(backgroundBgAvailable)
                    backgroundBg.sprite = backgroundBgAvailable;

                if (backgroundClaimedBg)
                    backgroundClaimedBg.gameObject.SetActive(true);

                break;
        }
    }    
}