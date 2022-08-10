using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class UITopMenuButton : MonoBehaviourSingleton<UITopMenuButton>
{
    [Header("Timers")]
    [SerializeField] private float currentShowDelay;
    [SerializeField] private float totalShowDelay = 4f;

    [Header("Buttons")]
    [SerializeField] private Button changeClothesButton;
    [SerializeField] private Button soundsOnOffButton;

    [Header("Sounds")]
    [SerializeField] private Image soundImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    public bool ChangeClothesButtonStatus
    {        
        set { changeClothesButton.gameObject.SetActive(value); }
    }

    //private LTSeq showUpSeq;
    //private int showUpSeqId = -1;

    private RectTransform changeClothesRect;
    private RectTransform soundsOnOffRect;

    private CameraManager cameraManager;
    private Writer currentWriter;
    private bool writerBusy;

    private bool isBusyBackground;
    private bool buttonsAreShown;
    private bool buttonsWereShown;

    private EpisodesHandler episodesHandler;

    private void OnEnable()
    {
        WriterSignals.OnWriterState += OnWriterState;

        /*CameraManager.OnCameraPanStarted += OnCameraPanStarted;
        CameraManager.OnCameraPanCompleted += OnCameraPanCompleted;

        CameraManager.OnCameraFadeToStarted += OnCameraFadeToStarted;
        CameraManager.OnCameraFadeToCompleted += OnCameraFadeToCompleted;*/
    }

    private void OnDisable()
    {
        WriterSignals.OnWriterState -= OnWriterState;

        /*CameraManager.OnCameraPanStarted -= OnCameraPanStarted;
        CameraManager.OnCameraPanCompleted -= OnCameraPanCompleted;

        CameraManager.OnCameraFadeToStarted -= OnCameraFadeToStarted;
        CameraManager.OnCameraFadeToCompleted -= OnCameraFadeToCompleted;*/
    }

    private void OnDestroy()
    {
        WriterSignals.OnWriterState -= OnWriterState;

        /*CameraManager.OnCameraPanStarted -= OnCameraPanStarted;
        CameraManager.OnCameraPanCompleted -= OnCameraPanCompleted;

        CameraManager.OnCameraFadeToStarted -= OnCameraFadeToStarted;
        CameraManager.OnCameraFadeToCompleted -= OnCameraFadeToCompleted;*/
    }    

    //private void OnCameraPanStarted() => ResetShowUpSeq();

    //private void OnCameraPanCompleted() => FireShowUpSeq();

    //private void OnCameraFadeToStarted() => ResetShowUpSeq();

    //private void OnCameraFadeToCompleted() => FireShowUpSeq();

    private void Awake()
    {
        if (FungusManager.Instance != null)
            cameraManager = FungusManager.Instance.CameraManager;

        if (changeClothesButton)
        {
            changeClothesButton.onClick.AddListener(OnClothesChangeButton);
            changeClothesRect = changeClothesButton.GetComponent<RectTransform>();
            changeClothesRect.anchoredPosition = new Vector2(-140f, changeClothesRect.anchoredPosition.y);
        }

        if (soundsOnOffButton)
        {
            soundsOnOffButton.onClick.AddListener(OnSoundsOnOffButton);
            soundsOnOffRect = soundsOnOffButton.GetComponent<RectTransform>();
            soundsOnOffRect.anchoredPosition = new Vector2(140f, changeClothesRect.anchoredPosition.y);
        }

        buttonsWereShown = true;

        if( (EpisodesSpawner.instance != null) && EpisodesSpawner.instance.playerData != null && soundImage != null)
        {
            soundImage.sprite = EpisodesSpawner.instance.playerData.soundStatus ? soundOnSprite : soundOffSprite;
            AudioListener.volume = EpisodesSpawner.instance.playerData.soundStatus ? 1f : 0;
        }
    }

    private void OnWriterState(Writer writer, WriterState writerState)
    {
        currentWriter = writer;        
    }

    private void Update()
    {
        if (!cameraManager)
            return;

        if(currentWriter != null)
            writerBusy = currentWriter.IsWriting && currentWriter.IsWaitingForInput;

        isBusyBackground = !writerBusy || 
                            cameraManager.CameraIsMoving ||
                            cameraManager.CameraIsFading ||
                            cameraManager.CameraIsRotating ||
                            cameraManager.CameraIsZooming;

        if (!isBusyBackground)
        {
            currentShowDelay += Time.deltaTime;
            if (currentShowDelay >= totalShowDelay && (!buttonsAreShown && buttonsWereShown))
            {
                buttonsAreShown = true;
                buttonsWereShown = false;

                //Show them
                ShowTopButtons();

                currentShowDelay = 0;
            }
        }
        else
        {
            //Hide them
            if (buttonsAreShown && !buttonsWereShown)
            {
                buttonsAreShown = false;
                buttonsWereShown = true;

                HideTopButtons();
            }

            currentShowDelay = 0;
        }
    }

    private void HideTopButtons()
    {
        //if (LeanTween.isTweening(showUpSeqId) && showUpSeqId != -1)
        //LeanTween.cancel(showUpSeqId);

        //changeClothesRect.anchoredPosition = new Vector2(-140f, changeClothesRect.anchoredPosition.y);
        //soundsOnOffRect.anchoredPosition = new Vector2(140f, changeClothesRect.anchoredPosition.y);

#if UNITY_EDITOR
        Debug.Log("Top Buttons: HIDE!");
#endif

        //Move Offscreen
        LeanTween.moveX(changeClothesRect, -140f, 0.4f).setEaseOutQuint();
        LeanTween.moveX(soundsOnOffRect, 140f, 0.4f).setEaseOutQuint();
    }

    private void ShowTopButtons()
    {
#if UNITY_EDITOR
        Debug.Log("Top Buttons: SHOWN!");
#endif

        /*showUpSeq = LeanTween.sequence();
        showUpSeqId = showUpSeq.id;
        showUpSeq.append(() =>
        {
            //Move offscreen
            changeClothesRect.anchoredPosition = new Vector2(-140f, changeClothesRect.anchoredPosition.y);
            soundsOnOffRect.anchoredPosition = new Vector2(140f, changeClothesRect.anchoredPosition.y);
        });
        showUpSeq.append(5f);
        showUpSeq.append(() =>
        {
            //Move inscreen
            LeanTween.moveX(changeClothesRect, 140f, 0.4f).setEaseOutQuint();
            LeanTween.moveX(soundsOnOffRect, -140f, 0.4f).setEaseOutQuint();
        });*/

        LeanTween.moveX(changeClothesRect, 140f, 0.4f).setEaseOutQuint();
        LeanTween.moveX(soundsOnOffRect, -140f, 0.4f).setEaseOutQuint();
    }

    private void OnClothesChangeButton()
    {
        if (episodesHandler == null)
            episodesHandler = FindObjectOfType<EpisodesHandler>();

        if (UICharacterSelection.instance == null)
            return;

        if (EpisodesSpawner.instance == null)
            return;
        else if (EpisodesSpawner.instance != null && EpisodesSpawner.instance.storiesDBItem != null)
            return;
    }

    private void OnSoundsOnOffButton()
    {
        if (EpisodesSpawner.instance == null || EpisodesSpawner.instance.playerData == null)
            return;

        if (soundImage == null)
            return;

        EpisodesSpawner.instance.playerData.soundStatus = !EpisodesSpawner.instance.playerData.soundStatus;
        SaveLoadGame.SavePlayerData(EpisodesSpawner.instance.playerData);

        soundImage.sprite = EpisodesSpawner.instance.playerData.soundStatus ? soundOnSprite : soundOffSprite;
        AudioListener.volume = EpisodesSpawner.instance.playerData.soundStatus ? 1f : 0;
    }
}
