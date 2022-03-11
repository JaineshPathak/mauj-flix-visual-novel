using UnityEngine;
using UnityEngine.UI;

public class MflixUIDailyRewardIconSmallGroupItem : MonoBehaviour
{
    public Image rewardIcon;
    public Text rewardName;

    private Sprite availableSprite;
    private Sprite unavailableSprite;

    public void Initialize(Sprite _availableSprite, Sprite _unavailableSprite, string _rewardName, bool scalingRequired)
    {
        availableSprite = _availableSprite;
        unavailableSprite = _unavailableSprite;

        rewardName.text = _rewardName;

        if (scalingRequired && rewardIcon)
            rewardIcon.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void Refresh(MflixDailyRewardItemSmall.DailyRewardState state)
    {
        switch (state)
        {
            case MflixDailyRewardItemSmall.DailyRewardState.UNCLAIMED_AVAILABLE:

                if(availableSprite)
                    rewardIcon.sprite = availableSprite;

                break;

            case MflixDailyRewardItemSmall.DailyRewardState.UNCLAIMED_UNAVAILABLE:

                if(unavailableSprite)
                    rewardIcon.sprite = unavailableSprite;

                break;

            case MflixDailyRewardItemSmall.DailyRewardState.CLAIMED:

                if(availableSprite)
                    rewardIcon.sprite = availableSprite;

                break;
        }
    }
}
