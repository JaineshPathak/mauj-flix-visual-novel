using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MflixUIGiftItem : MonoBehaviour
{
    public int day;
    public Image isDoneIcon;
    public TextMeshProUGUI dayText;
    public CharReplacerHindi dayTextFixer;
    //public Text dayText;
    public ParticleSystem giftParticlesVfx;

    public bool IsDone
    {
        set
        {
            if(value == true)
            {
                if(isDoneIcon)
                    LeanTween.scale(isDoneIcon.gameObject, Vector3.one, 0.5f).setEaseOutBack();

                if (giftParticlesVfx && giftParticlesVfx.isPlaying)
                    giftParticlesVfx.Stop(true);
            }
            else if(value == false)
            {
                if(isDoneIcon)
                    LeanTween.scale(isDoneIcon.gameObject, Vector3.zero, 0.5f).setEaseOutBack();

                if (giftParticlesVfx && giftParticlesVfx.isPlaying)
                    giftParticlesVfx.Stop(true);
            }
        }
    }

    private void Awake()
    {
        if (isDoneIcon)
        {
            isDoneIcon.gameObject.SetActive(true);
            isDoneIcon.transform.localScale = Vector3.zero;
        }
    }

    private void Start()
    {
        if (dayText)
        {
            dayText.text = MflixDailyRewards.GetHindiNumber(day.ToString());

            if (dayTextFixer)
                dayTextFixer.UpdateMe();
        }
    }
}
