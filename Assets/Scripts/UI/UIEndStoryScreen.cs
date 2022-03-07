using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class UIEndStoryScreen : MonoBehaviour
{
    public CanvasGroup endScreenCanvasGroup;

    [Space(15f)]

    public SayDialog sayDialog;
    public string sayText = "कहानी अनुभवने के लिए शुक्रिया!\\n हर कहानी के एक से ज्यादा एंडिंग्स हैं, तो कहानी दोबारा खेलकर सारी एंडिंग्स जरूर खेलिएगा।\\n धन्यवाद!";       

    [Space(15f)]

    public Image congralutionsImage;
    public ParticleSystem endParticleEffect;

    [Space(15)]

    public Transform endStoryPanel;
    public Button homeButton;
    public Button restartStoryButton;

    protected bool isTriggered;

    protected LTSeq endSeq;

    protected virtual void Awake()
    {
        sayText = sayText.Replace("\\n", "\n");

        endScreenCanvasGroup.interactable = false;
        endScreenCanvasGroup.blocksRaycasts = false;

        congralutionsImage.transform.localScale = Vector3.zero;
        endStoryPanel.transform.localScale = Vector3.zero;
        homeButton.transform.localScale = Vector3.zero;
        restartStoryButton.transform.localScale = Vector3.zero;
    }

    public virtual void PlayEndingStoryScreen()
    {
        if (isTriggered)
            return;

        isTriggered = true;

        if (!endScreenCanvasGroup.gameObject.activeSelf)
            endScreenCanvasGroup.gameObject.SetActive(true);

        endScreenCanvasGroup.interactable = true;
        endScreenCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        endSeq.append(LeanTween.alphaCanvas(endScreenCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congralutionsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete( () => 
        {
            if (endParticleEffect != null)
                endParticleEffect.Play();
        } ));
        endSeq.append(LeanTween.scale(endStoryPanel.gameObject, Vector3.one, 0.5f).setDelay(1f).setEase(LeanTweenType.easeOutBack).setOnComplete(OnEndStoryPanelUp));
    }

    protected virtual void OnEndStoryPanelUp()
    {        
        if (sayDialog != null)
            sayDialog.Say(sayText, true, false, false, false, false, null, OnSayTextComplete);
    }

    protected virtual void OnSayTextComplete()
    {
        LeanTween.scale(homeButton.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(restartStoryButton.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack);
    }

    public void LoadStoryEpisodesMenu()
    {
        if (EpisodesSpawner.instance == null)
            return;

        EpisodesSpawner.instance.LoadEpisodesMainMenu();
    }

    public void LoadStoryRestart()
    {
        if (EpisodesSpawner.instance == null)
            return;

        homeButton.interactable = false;
        restartStoryButton.interactable = false;

        EpisodesSpawner.instance.StartLoadingStoryScene();
    }
}
