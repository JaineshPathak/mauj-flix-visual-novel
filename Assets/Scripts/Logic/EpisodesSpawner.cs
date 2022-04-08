using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fungus;
using TMPro;

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

    [Header("Player Data")]
    public Player_Data playerData;

    [Header("Rate Us Window")]
    public CanvasGroup rateUsCanvasGroup;

    [Header("Like Story Window")]
    public CanvasGroup likeStoryCanvasGroup;

    [Header("Diamonds Panel")]
    public CanvasGroup diamondsPanelCanvasGroup;
    public TextMeshProUGUI diamondText;

    [Header("Background Progress")]
    public CanvasGroup blackScreenCanvasGroup;
    public TextMeshProUGUI percentDownloadedText;

    [Header("Top Panel")]
    public UITopPanel topPanel;
    public UIDiamondsPool diamondsPool;

    [Header("Ask To Quit Panel")]
    public bool isAskPopupOn;
    public bool isInTransition;
    public CanvasGroup askToQuitCanvasGroup;
    public string askToQuitDialogue = "आप गेम बंद करना चाहते है?";
    public SayDialog askToQuitSayDialogue;
    public Text askToQuitText;
    public Button askToQuitYesButton;
    public Button askToQuitNoButton;

    [Header("First Time Reward Panel")]
    public CanvasGroup firstTimeRewardCanvasGroup;
    public Transform firstTimeDiamondPanel;
    public Transform firstTimeTicketPanel;
    public Button firstTimeRewardCollectButton;

    private EpisodesHandler episodesHandler;

    [HideInInspector] public string storyTitle;    
    [HideInInspector] public string storyTitleEnglish;
    [HideInInspector] public string storyDescription;
    [HideInInspector] public StoriesDBItem storiesDBItem;
    [HideInInspector] public StoryData storyData;
    [HideInInspector] public int currentEpisodeNumber;
    [HideInInspector] public Sprite storyThumbnailSmallSprite;
    [HideInInspector] public Sprite storyThumbnailBigSprite;

    public static event Action OnRateUsWindowOpened;
    public static event Action OnRateUsWindowClosed;

    private StoriesDB storiesDB;    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        if(firstTimeRewardCanvasGroup)
        {
            firstTimeRewardCanvasGroup.alpha = 0;
            firstTimeRewardCanvasGroup.interactable = false;
            firstTimeRewardCanvasGroup.blocksRaycasts = false;
        }

        if(firstTimeRewardCollectButton)        
            firstTimeRewardCollectButton.onClick.AddListener(OnFirstTimeRewardCollectButton);        
    }

    private void OnEnable()
    {
        GameController.OnStoryDBLoaded += OnStoryDBLoaded;

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;

        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnDestroy()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;

        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {
        if(FirebaseFirestoreOffline.instance != null && diamondText != null)        
            FirebaseFirestoreOffline.instance.RegisterDiamondAmountText(diamondText);
    }

    private void OnStoryDBLoaded(StoriesDB _storiesDB)
    {
        storiesDB = _storiesDB;
    }

    public StoriesDBItem GetStoriesDBItemFromTitle(string _storyTitleEng)
    {
        StoriesDBItem storiesDBItem = null;

        if (storiesDB == null)
            return null;

        for (int i = 0; i < storiesDB.storiesCategories[storiesDB.storiesCategories.Length - 1].storiesDBItems.Length; i++)
        {
            if (storiesDB.storiesCategories[storiesDB.storiesCategories.Length - 1].storiesDBItems[i].storyTitleEnglish.Equals(_storyTitleEng))
            {
                storiesDBItem = storiesDB.storiesCategories[storiesDB.storiesCategories.Length - 1].storiesDBItems[i];
                return storiesDBItem;
            }
        }

        return storiesDBItem;
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
        isAskPopupOn = false;
        if(askToQuitYesButton)        
            askToQuitYesButton.onClick.AddListener(OnAskYesButton);

        if (askToQuitNoButton)
            askToQuitNoButton.onClick.AddListener(OnAskNoButton);
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

        if (topPanel)
            topPanel.HideTopPanel();

        storyPercentBar.fillAmount = 0;
        percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

        LeanTween.alphaCanvas(percentCanvasGroup, 1f, 1f).setOnComplete( () => 
        {
            StartCoroutine(StartLoadingStorySceneAsync());
        });        
    }

    private IEnumerator StartLoadingStorySceneAsync()
    {
        storyPercentBar.fillAmount = 0;
        percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(3);
        sceneOperation.completed += OnStorySceneLoadedComplete;

        while(!sceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneOperation.progress / 0.9f);

            storyPercentBar.fillAmount = progress;
            percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

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

        if (storyData != null)
        {
            DownloadEpisodeTask();
            //StartCoroutine(DownloadEpisodeRoutine());
        }
    }

    public StoryData GetStoryData()
    {
        string filePath = DataPaths.loadProgressPath + storyDataKey + DataPaths.loadProgressFileExtension;
        if (SerializationManager.FileExists(filePath))
        {
            string saveString = SerializationManager.LoadFromTextFile(filePath);
            if (saveString != null)
                storyData = JsonUtility.FromJson<StoryData>(saveString);
        }

        return storyData;
    }

    private async void DownloadEpisodeTask()
    {
        var totalDownloadSizeKb = BToKb(await Addressables.GetDownloadSizeAsync(storyData.currentEpisodeKey).Task);

        var downloadedKb = 0f;
        var keyDownloadOperation = Addressables.LoadAssetAsync<GameObject>(storyData.currentEpisodeKey);
        keyDownloadOperation.Completed += EpisodeDownloadComplete;

        if (totalDownloadSizeKb > 0)
        {
            while (!keyDownloadOperation.IsDone)
            {
                await System.Threading.Tasks.Task.Yield();

                var acquiredKb = downloadedKb + (keyDownloadOperation.PercentComplete * totalDownloadSizeKb);
                var totalProgressPercentage = (acquiredKb / totalDownloadSizeKb);

                storyPercentBar.fillAmount = totalProgressPercentage;
                percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

                //Debug.Log("Download progress: " + (totalProgressPercentage * 100).ToString("0.00") + "% - " + acquiredKb + "kb /" + totalDownloadSizeKb + "kb");
            }
        }
        else
        {
            storyPercentBar.fillAmount = 1f;
            percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";
        }
    }

    private static float BToKb(long bytes)
    {
        return bytes / 1000f;
    }

    private IEnumerator DownloadEpisodeRoutine()
    {
        var isDone = false;

        storyPercentBar.fillAmount = 0;
        percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

        AsyncOperationHandle<GameObject> downloadEpisode = Addressables.LoadAssetAsync<GameObject>(storyData.currentEpisodeKey);
        downloadEpisode.Completed += EpisodeDownloadComplete;

        while(!isDone)
        {            
            downloadProgressCurrent = downloadEpisode.PercentComplete;
            storyPercentBar.fillAmount = downloadProgressCurrent;
            percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";

            yield return null;
        }

        yield return new WaitUntil(() => isDone);

        //Download Complete
        storyPercentBar.fillAmount = 1f;
        percentDownloadedText.text = (storyPercentBar.fillAmount * 100f).ToString("0") + "%";
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
        LeanTween.cancelAll();

        immediateOpenDetailsPanel = true;

        if (FungusManager.Instance != null)
            FungusManager.Instance.MusicManager.StopMusic();

        blackScreenCanvasGroup.interactable = true;
        blackScreenCanvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(blackScreenCanvasGroup, 1f, 1f).setOnComplete( () => 
        {
            SceneManager.LoadScene(2);
        });
    }

    public void LoadEpisodesMainMenu(bool _immediateOpenDetailsPanel)
    {
        LeanTween.cancelAll();

        immediateOpenDetailsPanel = _immediateOpenDetailsPanel;

        if (FungusManager.Instance != null)
            FungusManager.Instance.MusicManager.StopMusic();

        blackScreenCanvasGroup.interactable = true;
        blackScreenCanvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(blackScreenCanvasGroup, 1f, 1f).setOnComplete(() =>
        {
            SceneManager.LoadScene(2);
        });
    }

    //=========================================================================================================

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

    //=========================================================================================================

    public void OpenLikeStoryWindow()
    {
        if (!likeStoryCanvasGroup.gameObject.activeSelf)
            likeStoryCanvasGroup.gameObject.SetActive(true);

        likeStoryCanvasGroup.alpha = 0;
        likeStoryCanvasGroup.interactable = true;
        likeStoryCanvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(likeStoryCanvasGroup, 1f, 0.2f);
    }

    public void CloseLikeStoryWindow()
    {
        likeStoryCanvasGroup.alpha = 0;
        likeStoryCanvasGroup.interactable = false;
        likeStoryCanvasGroup.blocksRaycasts = false;
        likeStoryCanvasGroup.gameObject.SetActive(false);
    }

    public void OnLikeStoryClicked()
    {
        CloseLikeStoryWindow();

        if (!playerData.HasStoryLiked(storyTitleEnglish))
        {
            if(FirebaseDBHandler.instance != null)
                FirebaseDBHandler.instance.LikesCountIncrement(FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd));

            playerData.AddStoryLiked(storyTitleEnglish);
            SaveLoadGame.SavePlayerData(playerData);
        }
    }

    //=========================================================================================================

    public void ShowHideDiamondPanel(bool showPanel, float speed = 0.5f, float delay = 0)
    {
        if (diamondsPanelCanvasGroup == null)
            return;

        if(showPanel)        
            LeanTween.alphaCanvas(diamondsPanelCanvasGroup, 1f, speed).setDelay(delay);
        else
            LeanTween.alphaCanvas(diamondsPanelCanvasGroup, 0, speed).setDelay(delay);
    }

    //=========================================================================================================    

    public void ShowAskToQuitPanel()
    {
        isAskPopupOn = !isAskPopupOn;

        if (isAskPopupOn)
        {
            askToQuitText.text = "";
            
            askToQuitCanvasGroup.interactable = true;
            askToQuitCanvasGroup.blocksRaycasts = true;

            askToQuitYesButton.interactable = true;
            askToQuitNoButton.interactable = true;

            LeanTween.alphaCanvas(askToQuitCanvasGroup, 1f, 0.5f).setOnStart(() =>
            {
                isInTransition = true;

                if (askToQuitSayDialogue != null)
                    askToQuitSayDialogue.Say(askToQuitDialogue, true, false, false, false, false, null, null);

            }).setOnComplete(() => isInTransition = false);
        }
        else
        {
            LeanTween.alphaCanvas(askToQuitCanvasGroup, 0, 0.5f).setOnStart(() => isInTransition = true).setOnComplete(() =>
            {
                isInTransition = false;

                askToQuitCanvasGroup.interactable = false;
                askToQuitCanvasGroup.blocksRaycasts = false;
            });
        }
    }

    private void OnAskYesButton()
    {
        askToQuitYesButton.interactable = false;
        askToQuitNoButton.interactable = false;

        Application.Quit();
    }

    private void OnAskNoButton()
    {
        askToQuitYesButton.interactable = false;
        askToQuitNoButton.interactable = false;

        ShowAskToQuitPanel();
    }

    //=========================================================================================================

    public void ShowFirstTimeReward()
    {
        topPanel.ShowTopPanel();

        firstTimeRewardCanvasGroup.interactable = true;
        firstTimeRewardCanvasGroup.blocksRaycasts = true;

        firstTimeRewardCollectButton.interactable = true;

        LeanTween.alphaCanvas(firstTimeRewardCanvasGroup, 1f, 0.5f).setEaseInOutSine();
    }
    
    private void OnFirstTimeRewardCollectButton()
    {
        topPanel.ShowTopPanel();

        firstTimeRewardCollectButton.interactable = false;
        LeanTween.scale(firstTimeRewardCollectButton.gameObject, Vector3.zero, 0.4f).setEaseInBack();

        diamondsPool.PlayDiamondsAnimationDeposit(firstTimeDiamondPanel, topPanel.diamondsPanelIcon, 10, 50, () =>
        {
            diamondsPool.PlayTicketsAnimationDeposit(firstTimeTicketPanel, topPanel.ticketsPanelIcon, 15, 15, () =>
            {
                StartCoroutine(OnFirstRewardCompleteRoutine());
            });
        });
    }

    private IEnumerator OnFirstRewardCompleteRoutine()
    {
        yield return new WaitForSeconds(1f);

        firstTimeRewardCanvasGroup.interactable = false;
        firstTimeRewardCanvasGroup.blocksRaycasts = false;

        LeanTween.alphaCanvas(firstTimeRewardCanvasGroup, 0, 0.5f).setEaseInOutSine();
    }
}