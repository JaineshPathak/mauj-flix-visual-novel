using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Fungus;
//using TMPro;

public class EpisodesSpawner : MonoBehaviour
{
    public static EpisodesSpawner instance;

    [Header("Episodes Reference")]
    public bool testMode;
    public bool saveOrLoadData;
    public bool immediateOpenDetailsPanel = false;
    public string storyDataKey;
    //public string currentEpisodeReferenceKey;
    //public string nextEpisodeReferenceKey;

    [Header("UI")]
    public float downloadProgressCurrent;
    public CanvasGroup percentCanvasGroup;
    public Image storyloadingThumbnailImage;
    public Image storyloadingTitleImage;
    public Image storyPercentBar;

    [Space(15f)]

    public Player_Data playerData;
    public CanvasGroup rateUsCanvasGroup;

    [Space(15f)]

    public CanvasGroup blackScreenCanvasGroup;
    //public TextMeshProUGUI percentDownloadedText;

    private EpisodesHandler episodesHandler;

    [HideInInspector] public string storyTitle;    
    [HideInInspector] public string storyTitleEnglish;
    [HideInInspector] public string storyDescription;
    [HideInInspector] public StoriesDBItem storiesDBItem;
    [HideInInspector] public StoryData storyData;
    [HideInInspector] public int currentEpisodeNumber;
    [HideInInspector] public Sprite storyThumbnailBigSprite;

    public static event Action OnRateUsWindowOpened;
    public static event Action OnRateUsWindowClosed;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FadeOutBlackScreen();

        if (testMode)
            StartLoadingEpisode();

        if(playerData)
        {
            SaveLoadGame.SetPlayerDataInstance(playerData);

            if (SaveLoadGame.CheckForFile())
                playerData = SaveLoadGame.LoadPlayerData();     //Load
            else
                SaveLoadGame.SavePlayerData(playerData);
        }

        rateUsCanvasGroup.alpha = 0;
        rateUsCanvasGroup.interactable = false;
        rateUsCanvasGroup.blocksRaycasts = false;

        //OpenRateUsWindow();
    }

    public void FadeOutBlackScreen()
    {
        blackScreenCanvasGroup.alpha = 1f;
        blackScreenCanvasGroup.interactable = true;
        blackScreenCanvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(blackScreenCanvasGroup, 0, 1f).setOnComplete(() =>
        {
            blackScreenCanvasGroup.interactable = false;
            blackScreenCanvasGroup.blocksRaycasts = false;
        });
    }

    /*private void Awake()
    {
        AsyncOperationHandle<GameObject> handle = assetReference.InstantiateAsync().Completed += EpisodeLoadingDone;
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetReference);
        handle.Completed += EpisodeLoadingDone;
    }*/

    public void StartLoadingStoryScene()
    {
        percentCanvasGroup.interactable = true;
        percentCanvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(percentCanvasGroup, 1f, 1f).setOnComplete( () => 
        {
            StartCoroutine(StartLoadingStorySceneAsync());
        });        
    }

    private IEnumerator StartLoadingStorySceneAsync()
    {
        storyPercentBar.fillAmount = 0;

        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(2);
        sceneOperation.completed += OnStorySceneLoadedComplete;

        while(!sceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneOperation.progress / 0.9f);
            storyPercentBar.fillAmount = progress;

            yield return null;
        }
    }

    private void OnStorySceneLoadedComplete(AsyncOperation obj)
    {
        StartLoadingEpisode();
    }

    private void StartLoadingEpisode()
    {
        string filePath = DataPaths.loadProgressPath + storyDataKey + DataPaths.loadProgressFileExtension;
        if (SerializationManager.FileExists(filePath))
        {
            string saveString = SerializationManager.LoadFromTextFile(filePath);
            if(saveString != null)            
                storyData = JsonUtility.FromJson<StoryData>(saveString);
        }

        if(storyData != null)
            StartCoroutine(DownloadEpisodeRoutine());
    }

    private IEnumerator DownloadEpisodeRoutine()
    {
        var isDone = false;

        storyPercentBar.fillAmount = 0;

        AsyncOperationHandle<GameObject> downloadEpisode = Addressables.LoadAssetAsync<GameObject>(storyData.currentEpisodeKey);
        downloadEpisode.Completed += EpisodeDownloadComplete;

        while(!isDone)
        {            
            downloadProgressCurrent = downloadEpisode.PercentComplete;
            storyPercentBar.fillAmount = downloadProgressCurrent;

            yield return null;
        }

        yield return new WaitUntil(() => isDone);

        //Download Complete
        //percentDownloadedText.text = "100%";
        storyPercentBar.fillAmount = 1f;
    }

    /*private void Update()
    {
        //percentDownloadedText.text = (int)(downloadProgressCurrent * 100f) + "%";
        if(storyPercentBar != null)
            storyPercentBar.fillAmount = downloadProgressCurrent;
    }*/

    private void EpisodeDownloadComplete(AsyncOperationHandle<GameObject> obj)
    {        
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:

                percentCanvasGroup.interactable = false;
                percentCanvasGroup.blocksRaycasts = false;
                LeanTween.alphaCanvas(percentCanvasGroup, 0, 1f);

                GameObject episodeDownloaded = Instantiate(obj.Result);
                episodesHandler = episodeDownloaded.GetComponent<EpisodesHandler>();
                episodesHandler.Init(this);
                break;

            case AsyncOperationStatus.Failed:
                break;
        }
    }

    public void LoadEpisodesMainMenu()
    {
        immediateOpenDetailsPanel = true;

        if (FungusManager.Instance != null)
            FungusManager.Instance.MusicManager.StopMusic();

        blackScreenCanvasGroup.interactable = true;
        blackScreenCanvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(blackScreenCanvasGroup, 1f, 1f).setOnComplete( () => 
        {
            SceneManager.LoadScene(1);
        });
    }

    public void LoadEpisodesMainMenu(bool _immediateOpenDetailsPanel)
    {
        immediateOpenDetailsPanel = _immediateOpenDetailsPanel;

        if (FungusManager.Instance != null)
            FungusManager.Instance.MusicManager.StopMusic();

        blackScreenCanvasGroup.interactable = true;
        blackScreenCanvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(blackScreenCanvasGroup, 1f, 1f).setOnComplete(() =>
        {
            SceneManager.LoadScene(1);
        });
    }

    public void OpenRateUsWindow()
    {
        if (!rateUsCanvasGroup.gameObject.activeSelf)
            rateUsCanvasGroup.gameObject.SetActive(true);

        rateUsCanvasGroup.alpha = 0;
        rateUsCanvasGroup.interactable = true;
        rateUsCanvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(rateUsCanvasGroup, 1f, 0.2f);

        OnRateUsWindowOpened?.Invoke();
    }

    public void CloseRateUsWindow()
    {
        rateUsCanvasGroup.alpha = 0;
        rateUsCanvasGroup.interactable = false;
        rateUsCanvasGroup.blocksRaycasts = false;
        rateUsCanvasGroup.gameObject.SetActive(false);

        OnRateUsWindowClosed?.Invoke();
    }

    public void OnRateUsClicked()
    {
        rateUsCanvasGroup.alpha = 0;
        rateUsCanvasGroup.interactable = false;
        rateUsCanvasGroup.blocksRaycasts = false;
        rateUsCanvasGroup.gameObject.SetActive(false);

        if (playerData)
        {
            playerData.hasRatedGame = true;
            SaveLoadGame.SavePlayerData(playerData);
        }

        OnRateUsWindowClosed?.Invoke();

#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.culttales.maujflix");
#endif
    }
}