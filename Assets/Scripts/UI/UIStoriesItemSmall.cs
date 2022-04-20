using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Fungus;
using underDOGS.SDKEvents;
using TMPro;

public class UIStoriesItemSmall : MonoBehaviour
{
    [Header("UI")]
    public GameObject storyLoadingText;
    public Image thumbnailSmallImage;
    public Button storyButton;
    public Image comingSoonImage;
    public Image storyIsNewImage;

    [Space(15)]

    public Image storyProgressBarBg;
    public Image storyProgressBarActual;

    [Space(15)]

    //public Text storyTitleText;
    //public SiddhantaFixer storyTitleFixer;

    public TextMeshProUGUI storyTitleText;
    public CharReplacerHindi storyTitleReplacer;

    [Space(15)]

    public GameObject storyCountsParent;
    public TextMeshProUGUI storyViewsCountText;
    public TextMeshProUGUI storyLikesCountText;    

    private StoriesDBItem storyItem;
    private UIStoriesDetailsPanel storiesDetailsPanel;

    private GameController gameController;

    private Sprite thumbnailBigSprite;
    private Sprite thumbnailTitleSprite;
    private Sprite thumbnailLoadingSprite;

    private StoryData storyData;
    private Flowchart flowchartLoaded;    

    private string viewCountRef;
    private string likeCountRef;

    private void Start()
    {
        if(comingSoonImage != null)
            comingSoonImage.gameObject.SetActive(false);

        if (storyIsNewImage)
            storyIsNewImage.gameObject.SetActive(false);

        //if (storyCountsParent != null)
        //storyCountsParent.SetActive(false);
    }

    public void LoadThumbnailAsset(StoriesDBItem _storiesDBItem, UIStoriesDetailsPanel _storiesDetailsPanel, GameController _gameController)
    {        
        storyButton.interactable = false;
        comingSoonImage.gameObject.SetActive(false);
        storyIsNewImage.gameObject.SetActive(false);

        storyProgressBarBg.gameObject.SetActive(false);
        storyProgressBarActual.gameObject.SetActive(false);

        storyItem = _storiesDBItem;
        storiesDetailsPanel = _storiesDetailsPanel;

        gameController = _gameController;

        //Set the Story Title
        storyTitleText.text = storyItem.storyTitle;
        storyTitleReplacer.UpdateMe();
        //storyTitleText.text = storyItem.storyTitle;
        //storyTitleFixer.FixTexts();

        CheckForStoryCounts();

        if (SerializationManager.FileExists(DataPaths.loadProgressPath + storyItem.storyProgressFileName + DataPaths.loadProgressFileExtension))
        {
            StoryData progressStoryData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);

            if (progressStoryData != null)
            {                
                if (progressStoryData.hasStarted)
                {
                    storyProgressBarBg.gameObject.SetActive(true);
                    storyProgressBarActual.gameObject.SetActive(true);

                    progressStoryData.blocksGrandTotal = storyItem.storyTotalBlocksCount;
                    storyProgressBarActual.fillAmount = (float)progressStoryData.blocksGrandDone / (float)progressStoryData.blocksGrandTotal;
                }
                else
                {
                    storyProgressBarBg.gameObject.SetActive(false);
                    storyProgressBarActual.gameObject.SetActive(false);
                }
            }
            else
            {
                storyProgressBarBg.gameObject.SetActive(false);
                storyProgressBarActual.gameObject.SetActive(false);
            }
        }

        if (storyItem.storyFlowchartKey.Length > 0)
        {
            AsyncOperationHandle<GameObject> flowchartLoading = Addressables.LoadAssetAsync<GameObject>(storyItem.storyFlowchartKey);
            flowchartLoading.Completed += OnFlowchartLoadingDone;
        }

        if (storyItem.storyThumbnailSmallKey.Length > 0)
        {
            AsyncOperationHandle<Sprite> smallThumbnailHandle = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailSmallKey);
            smallThumbnailHandle.Completed += OnThumbnailSmallLoaded;
        }

        if (storyItem.storyTitleImageKey.Length > 0)
        {
            AsyncOperationHandle<Sprite> handleBig = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailBigKey);
            handleBig.Completed += OnThumbnailBigLoaded;
        }

        if (storyItem.storyTitleImageKey.Length > 0)
        {
            AsyncOperationHandle<Sprite> handleTitle = Addressables.LoadAssetAsync<Sprite>(storyItem.storyTitleImageKey);
            handleTitle.Completed += OnThumbnailTitleLoaded;
        }

        if (storyItem.storyThumbnailLoadingKey.Length > 0)
        {
            AsyncOperationHandle<Sprite> handleSmall = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailLoadingKey);
            handleSmall.Completed += OnThumbnailLoadingDone;
        }
    }

    private void CheckForStoryCounts()
    {
        if (storyCountsParent == null)
            return;

        if (FirebaseDBHandler.instance == null)
        {
            storyCountsParent.SetActive(false);
            return;
        }

        if(storyItem.storyEpisodesKeys.Length > 0 && !storyItem.isShortStory)
        {
            storyCountsParent.SetActive(true);

            viewCountRef = FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.viewCountKeyEnd);     //Eg: Udaan-View-Count
            likeCountRef = FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.likeCountKeyEnd);     //Eg: Udaan-Like-Count

            FirebaseDBHandler.instance.GetCountFromFirebaseDB(viewCountRef, val => storyViewsCountText.text = val);
            FirebaseDBHandler.instance.GetCountFromFirebaseDB(likeCountRef, val => storyLikesCountText.text = val);

            //storyViewsCountText.text = FirebaseDBHandler.instance.GetCountFromFirebaseDB(viewCountRef);
            //storyLikesCountText.text = FirebaseDBHandler.instance.GetCountFromFirebaseDB(likeCountRef);

            /*if (FirebaseDBHandler.instance.IsFirebaseDBInitialized)
            {
                FirebaseDBHandler.instance.GetCountFromFirebaseDB(viewCountRef, val => storyViewsCountText.text = val);
                FirebaseDBHandler.instance.GetCountFromFirebaseDB(likeCountRef, val => storyLikesCountText.text = val);
            }
            else
                FirebaseDBHandler.instance.InitFirebaseDBHandler();*/
        }
        else
        {
            storyCountsParent.SetActive(false);
        }
    }

    private void OnFlowchartLoadingDone(AsyncOperationHandle<GameObject> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;
            case AsyncOperationStatus.Succeeded:

                GameObject flowchartGameobject = obj.Result;
                flowchartLoaded = flowchartGameobject.GetComponent<Flowchart>();

                PostSetupStuffs(true);
                break;
            case AsyncOperationStatus.Failed:
                break;
        }
    }

    private void OnThumbnailSmallLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:                
                thumbnailSmallImage.sprite = obj.Result;

                PostSetupStuffs(true);
                break;

            case AsyncOperationStatus.Failed:
                break;
        }
    }

    private IEnumerator CheckAgainSmall()
    {
        yield return new WaitForSeconds(0.5f);

        PostSetupStuffs(false);
    }

    private void OnThumbnailBigLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:

                break;

            case AsyncOperationStatus.Succeeded:
                thumbnailBigSprite = obj.Result;
                break;

            case AsyncOperationStatus.Failed:

                break;
        }
    }

    private void OnThumbnailTitleLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:

                break;

            case AsyncOperationStatus.Succeeded:
                thumbnailTitleSprite = obj.Result;
                break;

            case AsyncOperationStatus.Failed:

                break;
        }
    }

    private void OnThumbnailLoadingDone(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;
            case AsyncOperationStatus.Succeeded:

                thumbnailLoadingSprite = obj.Result;

                break;
            case AsyncOperationStatus.Failed:
                break;
        }
    }

    private void PostSetupStuffs(bool checkRoutine = false)
    {
        storyLoadingText.SetActive(false);
        if (storyItem.storyEpisodesKeys.Length > 0)
        {
            storyButton.interactable = true;
            storyButton.onClick.RemoveAllListeners();
            storyButton.onClick.AddListener(OnStoryButtonClick);

            comingSoonImage.gameObject.SetActive(false);
            storyIsNewImage.gameObject.SetActive(storyItem.isNewStory);
        }
        else
        {
            /*storyButton.interactable = false;
            storyButton.onClick.RemoveListener(OnStoryButtonClick);
            storyButton.onClick.RemoveAllListeners();*/

            storyButton.interactable = true;
            storyButton.onClick.RemoveAllListeners();
            storyButton.onClick.AddListener(OnStoryButtonClickEmpty);

            comingSoonImage.gameObject.SetActive(true);
            storyIsNewImage.gameObject.SetActive(false);
        }

        if (checkRoutine)
            StartCoroutine(CheckAgainSmall());
    }

    private void OnStoryButtonClick()
    {
        if (gameController == null)
            return;

        if (storyItem == null)
            return;

        storyData = null;

        storiesDetailsPanel.PlayButtonClickSound();

        //Send an "thumbnails_clicked" Event
        if (SDKManager.instance != null)
        {
            string storyTitleProper = storyItem.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "_");

            SDKEventStringData thumbnailsClickedData;
            thumbnailsClickedData.eventParameterName = SDKEventsNames.storyParameterName;
            thumbnailsClickedData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.thumbnailsClickedEventName, thumbnailsClickedData);
        }

        if (!SerializationManager.FileExists(DataPaths.loadProgressPath + storyItem.storyProgressFileName + DataPaths.loadProgressFileExtension))
        {
            storiesDetailsPanel.SetStoryBufferingImageStatus(true);
            gameController.SaveStoryData(storyItem, flowchartLoaded, OnStoryDataLoaded);
        }
        else
        {
            storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);

            if (storyData.episodeDataList.Count != storyItem.storyEpisodesKeys.Length)
            {
#if UNITY_EDITOR
                print(storyItem.storyTitle + ": Updated Content Found! Saving Process Started...");
#endif
                IncrementViewCount();

                storyData = gameController.SaveAndLoadUpdatedStoryData(storyItem, storyData, flowchartLoaded);
                storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallImage.sprite, thumbnailBigSprite, thumbnailTitleSprite, thumbnailLoadingSprite, storyItem, storyData);
            }
            else
            {
                IncrementViewCount();

                storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallImage.sprite, thumbnailBigSprite, thumbnailTitleSprite, thumbnailLoadingSprite, storyItem, storyData);
            }
        }

        //gameController.SaveStoryData(storyItem);
        //storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);                    
    }    

    private void OnStoryDataLoaded()
    {
        storiesDetailsPanel.SetStoryBufferingImageStatus(false);

        storyData = null;

        IncrementViewCount();

        storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);
        storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallImage.sprite, thumbnailBigSprite, thumbnailTitleSprite, thumbnailLoadingSprite, storyItem, storyData);
    }

    private void OnStoryButtonClickEmpty()
    {
        storiesDetailsPanel.PlayButtonClickSound();        

        if (SDKManager.instance == null)
            return;

        //Send an "thumbnails_clicked" Event
        string storyTitleProper = storyItem.storyTitleEnglish;
        storyTitleProper = storyTitleProper.Replace(" ", "_");

        SDKEventStringData thumbnailsClickedData;
        thumbnailsClickedData.eventParameterName = SDKEventsNames.storyParameterName;
        thumbnailsClickedData.eventParameterData = storyTitleProper;

        SDKManager.instance.SendEvent(SDKEventsNames.thumbnailsClickedEventName, thumbnailsClickedData);
    }    

    private void IncrementViewCount()
    {       
        if (FirebaseDBHandler.instance == null)
            return;

        if (storyItem != null && storyItem.isShortStory)
            return;

        FirebaseDBHandler.instance.UpdateViewCount(viewCountRef);
    }
}