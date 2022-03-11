using UnityEngine;

[System.Serializable]
public class MflixReward
{
    public bool isBigReward;

    public bool hasDiamondReward;
    public string rewardDiamondName = "डायमंड";
    public int rewardDiamondAmount;
    public Sprite rewardDiamondSpriteNormal;
    public Sprite rewardDiamondSpriteLocked;

    public bool hasTicketReward;
    public string rewardTicketName = "तिकीट";
    public int rewardTicketAmount;
    public Sprite rewardTicketSpriteNormal;
    public Sprite rewardTicketSpriteLocked;
}
