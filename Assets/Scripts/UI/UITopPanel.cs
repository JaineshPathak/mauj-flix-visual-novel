using System;
using UnityEngine;
using TMPro;

public class UITopPanel : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI diamondsText;
    public TextMeshProUGUI ticketsText;

    [Header("Panels")]
    public RectTransform topPanel;
    public float yPosOn = -100f;
    public float yPosOff = 100f;

    [Space(15)]

    public Transform diamondsPanel;
    public Transform diamondsPanelIcon;
    public ParticleSystem diamondPickEffectVFX;

    [Space(10)]

    public Transform ticketsPanel;
    public Transform ticketsPanelIcon;
    public ParticleSystem ticketPickEffectVFX;

    [Space(15)]

    public Transform devModeIcon;

    private int showSeqID;
    private int diamondSeqID;
    private int ticketSeqID;

    #region
    /*private void OnEnable()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded += OnFirestoreLoaded;
    }

    private void OnDisable()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded -= OnFirestoreLoaded;
    }

    private void OnDestroy()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded -= OnFirestoreLoaded;
    }

    private void OnFirestoreLoaded(FirebaseFirestoreHandler fireStoreHandler)
    {
        if (fireStoreHandler == null)
            return;

        if (fireStoreHandler.mainMenuDiamondsText == null)
            fireStoreHandler.mainMenuDiamondsText = diamondsText;
    }*/
    #endregion

    private void Start()
    {
        if (FirebaseFirestoreOffline.instance && diamondsText && ticketsText)
        {
            FirebaseFirestoreOffline.instance.RegisterDiamondAmountText(diamondsText);
            FirebaseFirestoreOffline.instance.RegisterTicketAmountText(ticketsText);
        }

        if (GameController.instance != null && devModeIcon)
            devModeIcon.gameObject.SetActive(GameController.instance.DevMode);
    }

    //----------------------------------------------------------------------------------------------------

    //Google Sign In

    public void TryGoogleSignIn()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignIn();
    }

    public void TryGoogleSignOut()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignOut();
    }

    //----------------------------------------------------------------------------------------------------

    //Tweens

    public void ShowTopPanel(float speed = 0.3f, float delay = 0)
    {
        if (LeanTween.isTweening(showSeqID))
            LeanTween.cancel(showSeqID);

        if (topPanel.anchoredPosition.y == yPosOn)
            return;

        showSeqID = LeanTween.moveY(topPanel, yPosOn, speed).setDelay(delay).setEase(LeanTweenType.easeInOutSine).id;
    }

    public void HideTopPanel(float speed = 0.3f, float delay = 0)
    {
        if (LeanTween.isTweening(showSeqID))
            LeanTween.cancel(showSeqID);

        if (topPanel.anchoredPosition.y == yPosOff)
            return;

        showSeqID = LeanTween.moveY(topPanel, yPosOff, speed).setDelay(delay).setEase(LeanTweenType.easeInOutSine).id;
    }

    public void HideTopPanel(Action callback, float speed = 0.3f, float delay = 0)
    {
        if (LeanTween.isTweening(showSeqID))
            LeanTween.cancel(showSeqID);

        if (topPanel.anchoredPosition.y == yPosOff)
            return;

        showSeqID = LeanTween.moveY(topPanel, yPosOff, speed).setDelay(delay).setEase(LeanTweenType.easeInOutSine).setOnComplete( () => callback?.Invoke() ).id;
    }

    public void PlayDiamondPanelCollectAnim(float speed = 1f, bool playVFXEffect = false)
    {
        if (diamondsPanel == null)
            return;

        if (LeanTween.isTweening(diamondSeqID))
            LeanTween.cancel(diamondSeqID);

        diamondsPanel.localScale = Vector3.one;
        diamondSeqID = LeanTween.scale(diamondsPanel.gameObject, new Vector3(1.2f, 1.2f, 1.2f), speed).setEasePunch().id;

        if(playVFXEffect && diamondPickEffectVFX != null)
        {
            if (diamondPickEffectVFX.isPlaying)
                diamondPickEffectVFX.Stop();

            diamondPickEffectVFX.Play();
        }
    }

    public void PlayDiamondIconCollectAnim(float speed = 1f, float scale = 1.5f, bool playVFXEffect = false)
    {
        if (diamondsPanelIcon == null)
            return;

        if (LeanTween.isTweening(diamondSeqID))
            LeanTween.cancel(diamondSeqID);

        diamondsPanelIcon.localScale = Vector3.one;
        diamondSeqID = LeanTween.scale(diamondsPanelIcon.gameObject, new Vector3(scale, scale, scale), speed).setEasePunch().id;

        if (playVFXEffect && diamondPickEffectVFX != null)
        {
            if (diamondPickEffectVFX.isPlaying)
                diamondPickEffectVFX.Stop();

            diamondPickEffectVFX.Play();
        }
    }

    public void PlayTicketPanelCollectAnim(float speed = 1f)
    {
        if (ticketsPanel == null)
            return;

        if (LeanTween.isTweening(ticketSeqID))
            LeanTween.cancel(ticketSeqID);

        ticketsPanel.localScale = Vector3.one;
        ticketSeqID = LeanTween.scale(ticketsPanel.gameObject, new Vector3(1.2f, 1.2f, 1.2f), speed).setEasePunch().id;
    }

    public void PlayTicketIconCollectAnim(float speed = 1f, float scale = 1.5f, bool playVFXEffect = false)
    {
        if (ticketsPanelIcon == null)
            return;

        if (LeanTween.isTweening(ticketSeqID))
            LeanTween.cancel(ticketSeqID);

        ticketsPanelIcon.localScale = Vector3.one;
        ticketSeqID = LeanTween.scale(ticketsPanelIcon.gameObject, new Vector3(scale, scale, scale), speed).setEasePunch().id;

        if (playVFXEffect && ticketPickEffectVFX != null)
        {
            if (ticketPickEffectVFX.isPlaying)
                ticketPickEffectVFX.Stop();

            ticketPickEffectVFX.Play();
        }
    }

    //----------------------------------------------------------------------------------------------------
}
