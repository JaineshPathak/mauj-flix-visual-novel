using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using underDOGS.SDKEvents;

public class UIEpisodeEndPanel : MonoBehaviour
{
    public CanvasGroup endScreenCanvasGroup;

    [Space(15f)]

    public SayDialog sayDialog;
    public string sayText = "इस कहानी का ये एपिसोड यही समाप्त होता हैं। आगे की कहानी जानने के लिए खेलिए अगला एपिसोड।";

    [Space(15f)]

    public Image congralutionsImage;

    [Space(15f)]

    public Image nextEpisodeBarTimer;
    public Image nextEpisodePanel;

    [Space(15f)]

    public ParticleSystem endParticleEffect;
    public Button playButton;
    public Button rateUsButton;
    public Button nextEpisodeButton;
    public Button playAgainEpisodeButton;

    private LTSeq endSeq;
    private LTSeq timerSeq;
    private int timerId;
    private bool isTriggered;

    [HideInInspector] public EpisodesHandler episodesHandler;

    public static event Action OnNextEpisodePanelOpened;

    private void Awake()
    {
        endScreenCanvasGroup.interactable = false;
        endScreenCanvasGroup.blocksRaycasts = false;

        congralutionsImage.transform.localScale = Vector3.zero;
        nextEpisodePanel.transform.localScale = Vector3.zero;

        playButton.transform.localScale = Vector3.zero;

        if (nextEpisodeButton == null)
        {
            //nextEpisodeButton = endScreenCanvasGroup.transform.Find("NextEpisodeButton").gameObject.GetComponent<Button>();
            nextEpisodeButton = GameObject.Find("NextEpisodeButton").GetComponent<Button>();
        }

        if(nextEpisodeButton != null)
        {
            nextEpisodeButton.onClick.RemoveAllListeners();
            nextEpisodeButton.onClick.AddListener(OnNextEpisodeButton);
            nextEpisodeButton.transform.localScale = Vector3.zero;
        }

        if (playAgainEpisodeButton == null)
            playAgainEpisodeButton = GameObject.Find("PlayEpisodeAgainButton").GetComponent<Button>();

        if(playAgainEpisodeButton != null)
        {
            playAgainEpisodeButton.onClick.RemoveAllListeners();
            playAgainEpisodeButton.onClick.AddListener(OnPlayAgainEpisodeButton);
            playAgainEpisodeButton.transform.localScale = Vector3.zero;
        }

        rateUsButton.transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        EpisodesSpawner.OnRateUsWindowOpened += OnRateUsWindowOpened;
        EpisodesSpawner.OnRateUsWindowClosed += OnRateUsWindowClosed;
    }

    private void OnDisable()
    {
        EpisodesSpawner.OnRateUsWindowOpened -= OnRateUsWindowOpened;
        EpisodesSpawner.OnRateUsWindowClosed -= OnRateUsWindowClosed;
    }

    private void OnDestroy()
    {
        EpisodesSpawner.OnRateUsWindowOpened -= OnRateUsWindowOpened;
        EpisodesSpawner.OnRateUsWindowClosed -= OnRateUsWindowClosed;
    }

    private void OnRateUsWindowOpened()
    {
        if (LeanTween.isTweening(timerId))
            LeanTween.pause(timerId);
    }

    private void OnRateUsWindowClosed()
    {        
        LeanTween.resume(timerId);

        if(EpisodesSpawner.instance != null)
        {
            if (EpisodesSpawner.instance.playerData.hasRatedGame)
                rateUsButton.gameObject.SetActive(false);
        }
    }

    public void PlayEndingScreen()
    {
        if (isTriggered)
            return;

        isTriggered = true;

        //Next Episode Button, too lazy to again assign in Editor in all episode prefabs
        if (nextEpisodeButton == null)
        {
            //nextEpisodeButton = endScreenCanvasGroup.transform.Find("NextEpisodeButton").GetComponent<Button>();
            nextEpisodeButton = GameObject.Find("NextEpisodeButton").GetComponent<Button>();
        }

        if (nextEpisodeButton != null)
        {
            nextEpisodeButton.onClick.RemoveAllListeners();
            nextEpisodeButton.onClick.AddListener(OnNextEpisodeButton);
        }

        //Play Again Button, too lazy to again assign in Editor in all episode prefabs
        if (playAgainEpisodeButton == null)
            playAgainEpisodeButton = GameObject.Find("PlayEpisodeAgainButton").GetComponent<Button>();

        if (playAgainEpisodeButton != null)
        {
            playAgainEpisodeButton.onClick.RemoveAllListeners();
            playAgainEpisodeButton.onClick.AddListener(OnPlayAgainEpisodeButton);
            playAgainEpisodeButton.transform.localScale = Vector3.zero;
        }

        if (!endScreenCanvasGroup.gameObject.activeSelf)
            endScreenCanvasGroup.gameObject.SetActive(true);

        //endScreenCanvasGroup.alpha = 0;
        endScreenCanvasGroup.interactable = true;
        endScreenCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        endSeq.append(LeanTween.alphaCanvas(endScreenCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congralutionsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(OnCongratsComplete));        
        /*endSeq.append(() => 
        {
            LeanTween.scale(playButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(2f);
        });*/
    }

    private void OnCongratsComplete()
    {
        if(endParticleEffect != null)        
            endParticleEffect.Play();        

        if (sayDialog != null)
            sayDialog.Say(sayText, true, false, false, false, false, null, OnSayTextComplete);
    }

    private void OnSayTextComplete()
    {
        LeanTween.scale(nextEpisodePanel.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(ShowNextEpisodeButton);
    }

    private void ShowNextEpisodeButton()
    {
        if (nextEpisodeBarTimer.gameObject.activeSelf)
        {
            /*timerSeq = LeanTween.sequence();
            timerSeq.append(LeanTween.value(0, 1f, 8f).setEase(LeanTweenType.linear).setOnUpdate((float f) =>
            {
                nextEpisodeBarTimer.fillAmount = f;
            }).setOnComplete(OnNextEpisodeTimerUp));*/            

            /*timerId = LeanTween.value(0, 1f, 8f).setEase(LeanTweenType.linear).setOnUpdate((float f) =>
            {
                nextEpisodeBarTimer.fillAmount = f;
            }).setOnComplete(OnNextEpisodeTimerUp).id;*/

            StartCoroutine(OpenedActionEventDelay());
        }

        playButton.interactable = true;
        LeanTween.scale(playButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(1f);

        nextEpisodeButton.interactable = true;
        LeanTween.scale(nextEpisodeButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(1f);

        playAgainEpisodeButton.interactable = true;
        LeanTween.scale(playAgainEpisodeButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(1f);

        if (EpisodesSpawner.instance != null)
        {
            if(EpisodesSpawner.instance.playerData != null)
            {
                if (!EpisodesSpawner.instance.playerData.hasRatedGame)
                    LeanTween.scale(rateUsButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(1f);
                else
                    rateUsButton.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator OpenedActionEventDelay()
    {
        yield return new WaitForSeconds(1f);

        OnNextEpisodePanelOpened?.Invoke();
    }

    private void OnNextEpisodeTimerUp()
    {
        if (EpisodesSpawner.instance == null)
            return;

        playButton.interactable = false;
        nextEpisodeButton.interactable = false;
        playAgainEpisodeButton.interactable = false;

        EpisodesSpawner.instance.StartLoadingStoryScene();
    }

    public void OnNextEpisodeButton()
    {
        if (EpisodesSpawner.instance == null)
            return;

        if (LeanTween.isTweening(timerId))
            LeanTween.cancel(timerId);

        playButton.interactable = false;
        nextEpisodeButton.interactable = false;
        playAgainEpisodeButton.interactable = false;

        EpisodesSpawner.instance.StartLoadingStoryScene();
    }

    public void OnPlayAgainEpisodeButton()
    {
        playButton.interactable = false;
        nextEpisodeButton.interactable = false;
        playAgainEpisodeButton.interactable = false;

        episodesHandler.PlayEpisodeAgain();
    }

    public void LoadStoryEpisodesMenu()
    {
        if (EpisodesSpawner.instance == null)
            return;

        if (LeanTween.isTweening(timerId))        
            LeanTween.cancel(timerId);

        //Send a Eg: "Udaan_episode1_completed_homebtn_clicked" event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = EpisodesSpawner.instance.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");
            
            SDKEventStringData episodeHomeClickedData;
            episodeHomeClickedData.eventParameterName = storyTitleProper;
            episodeHomeClickedData.eventParameterData = "episode" + EpisodesSpawner.instance.currentEpisodeNumber;

            SDKManager.instance.SendEvent(SDKEventsNames.episodeCompleteHomeEventName, episodeHomeClickedData);
        }

        EpisodesSpawner.instance.LoadEpisodesMainMenu();
    }
}
