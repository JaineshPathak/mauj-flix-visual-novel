using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Fungus;
using underDOGS.SDKEvents;

public class UIStoriesItemBig : MonoBehaviour
{
    [Header("UI")]
    public GameObject loadingText;
    public Image thumbnailBigImage;
    public Image thumbnailTitleImage;
    public Image comingSoonImage;
    public Button playNowButton;

    private GameController gameController;

    private StoriesDBItem storyItem;
    private UIStoriesDetailsPanel storiesDetailsPanel;

    private Sprite thumbnailSmallSprite;
    private Sprite thumbnailLoadingSprite;

    private StoryData storyData;
    private Flowchart flowchartLoaded;

    private string viewCountRef;

    private void Start()
    {
        if (GameController.instance != null)
            gameController = GameController.instance;

        comingSoonImage.gameObject.SetActive(false);

        playNowButton.interactable = false;
        playNowButton.gameObject.SetActive(false);
    }

    public void LoadThumbnailAssets(StoriesDBItem _storiesDBItem, UIStoriesDetailsPanel _storiesDetailsPanel)
    {        
        comingSoonImage.gameObject.SetActive(false);

        storyItem = _storiesDBItem;
        storiesDetailsPanel = _storiesDetailsPanel;

        if (storyItem.storyFlowchartKey.Length > 0)
        {
            AsyncOperationHandle<GameObject> flowchartLoading = Addressables.LoadAssetAsync<GameObject>(storyItem.storyFlowchartKey);
            flowchartLoading.Completed += OnFlowchartLoadingDone;
        }        

        if (ThumbnailsBucket.instance != null)
        {
            thumbnailSmallSprite = ThumbnailsBucket.instance.GetThumbnailSprite(storyItem.storyThumbnailSmallName, ThumbnailType.Small);
            thumbnailBigImage.sprite = ThumbnailsBucket.instance.GetThumbnailSprite(storyItem.storyThumbnailBigName, ThumbnailType.Big);
            thumbnailLoadingSprite = ThumbnailsBucket.instance.GetThumbnailSprite(storyItem.storyThumbnailLoadingName, ThumbnailType.Loading);
            thumbnailTitleImage.sprite = ThumbnailsBucket.instance.GetThumbnailSprite(storyItem.storyThumbnailTitleName, ThumbnailType.Title);
        }

        if (FirebaseDBHandler.instance != null)
            viewCountRef = FirebaseDBHandler.instance.GetReferenceFromStoryTitle(storyItem.storyTitleEnglish, FirebaseDBHandler.viewCountKeyEnd);     //Eg: Udaan-View-Count

        /*AsyncOperationHandle<Sprite> smallThumbnailHandle = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailSmallKey);
        smallThumbnailHandle.Completed += OnThumbnailSmallLoaded;

        AsyncOperationHandle<Sprite> handleBig = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailBigKey);
        handleBig.Completed += OnThumbnailBigLoaded;

        AsyncOperationHandle<Sprite> handleTitle = Addressables.LoadAssetAsync<Sprite>(storyItem.storyTitleImageKey);
        handleTitle.Completed += OnThumbnailTitleLoaded;

        AsyncOperationHandle<Sprite> handleLoading = Addressables.LoadAssetAsync<Sprite>(storyItem.storyThumbnailLoadingKey);
        handleLoading.Completed += OnThumbnailLoadingDone;*/
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

    private void OnThumbnailBigLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:
                thumbnailBigImage.sprite = obj.Result;

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
                thumbnailSmallSprite = obj.Result;                
                break;

            case AsyncOperationStatus.Failed:
                break;
        }
    }

    private IEnumerator CheckAgainBig()
    {
        yield return new WaitForSeconds(0.5f);

        PostSetupStuffs(false);
    }

    private void OnThumbnailTitleLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:               
                thumbnailTitleImage.sprite = obj.Result;               
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

    /*private void OnStoryPlayButtonClicked()
    {
        if (gameController == null)
            return;

        if (storyItem == null)
            return;

        StoryData storyData = null;

        if (!SerializationManager.FileExists(DataPaths.loadProgressPath + storyItem.storyProgressFileName + DataPaths.loadProgressFileExtension))
            gameController.SaveStoryData(storyItem, flowchartLoaded, OnStoryDataLoaded);
        else
            storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);

        if (storyData != null)
            storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyDescription, thumbnailBigImage.sprite, thumbnailTitleImage.sprite, thumbnailLoadingSprite, storyItem, storyData);
    }*/

    private void PostSetupStuffs(bool checkRoutine = false)
    {
        loadingText.SetActive(false);
        if (storyItem.storyEpisodesKeys.Length > 0)
        {
            playNowButton.gameObject.SetActive(true);
            playNowButton.onClick.RemoveAllListeners();
            playNowButton.onClick.AddListener(OnStoryPlayButtonClicked);
            playNowButton.interactable = true;

            comingSoonImage.gameObject.SetActive(false);
        }
        else
        {
            playNowButton.onClick.RemoveListener(OnStoryPlayButtonClicked);
            playNowButton.onClick.RemoveAllListeners();
            playNowButton.interactable = false;

            playNowButton.gameObject.SetActive(false);

            comingSoonImage.gameObject.SetActive(true);
        }

        if(checkRoutine)
            StartCoroutine(CheckAgainBig());
    }

    private void OnStoryPlayButtonClicked()
    {
        if (gameController == null && GameController.instance != null)
            gameController = GameController.instance;        

        if (storyItem == null)
            return;

        storyData = null;

        storiesDetailsPanel.PlayButtonClickSound();

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
                storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallSprite, thumbnailBigImage.sprite, thumbnailTitleImage.sprite, thumbnailLoadingSprite, storyItem, storyData);
            }
            else
            {
                IncrementViewCount();

                storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallSprite, thumbnailBigImage.sprite, thumbnailTitleImage.sprite, thumbnailLoadingSprite, storyItem, storyData);
            }

            //Send "topbanner_[StoryTitle]_[episodeN]_playbtn_clicked" (GameAnalytics ONLY!)
            if (SDKManager.instance != null)
            {
                string storyTitleProper = storyItem.storyTitleEnglish;
                storyTitleProper = storyTitleProper.Replace(" ", "");

                SDKEventStringData eventPlayBtn;
                eventPlayBtn.eventParameterName = SDKEventsNames.storyParameterName;
                eventPlayBtn.eventParameterData = storyTitleProper;

                SDKManager.instance.SendEvent(SDKEventsNames.topBannerEventNameStart, eventPlayBtn);
            }
        }

        //gameController.SaveStoryData(storyItem);
        //storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);                    
    }

    private void OnStoryDataLoaded()
    {
        storiesDetailsPanel.SetStoryBufferingImageStatus(false);

        IncrementViewCount();

        storyData = null;

        storyData = gameController.GetStoryDataFromFilename(storyItem.storyProgressFileName);
        storiesDetailsPanel.MoveHidePanel(storyItem.storyTitle, storyItem.storyTitleEnglish, storyItem.storyDescription, thumbnailSmallSprite, thumbnailBigImage.sprite, thumbnailTitleImage.sprite, thumbnailLoadingSprite, storyItem, storyData);

        //Send "topbanner_[StoryTitle]_[episodeN]_playbtn_clicked" (GameAnalytics ONLY!)
        if (SDKManager.instance != null)
        {
            string storyTitleProper = storyItem.storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData eventPlayBtn;
            eventPlayBtn.eventParameterName = SDKEventsNames.storyParameterName;
            eventPlayBtn.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.topBannerEventNameStart, eventPlayBtn);
        }
    }

    private void IncrementViewCount()
    {
        if (FirebaseDBHandler.instance == null)
            return;

        FirebaseDBHandler.instance.UpdateViewCount(viewCountRef);
    }
}