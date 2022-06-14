using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.RemoteConfig;

[System.Serializable]
public struct PersonalCategoryData
{
    public string categoryTitle;
    public TextMeshProUGUI countsCategoryTitleText;
    public TextMeshProUGUI countsDataText;
    public List<StoriesDBItem> storiesDBItemList;
}

public class UIPersonalProfile : MonoBehaviour
{
    [Header("Prefabs n Compotents")]
    public UIStoriesLoaderSmall sectionLoaderPrefab;
    public RectTransform sectionContentParent;
    public UIStoriesDetailsPanel detailsPanel;

    [Header("Categories")]
    //public string[] categoriesTitle = { "पसंद किये कहानिया", "चालू किये कहानिया", "खतम किये कहानिया" };
    public PersonalCategoryData[] personalCategoryDatas;    

    private bool isDBLoaded;
    private StoriesDB storiesDB;

    private Player_Data playerData;
    private EpisodesSpawner episodesSpawner;

    private List<UIStoriesLoaderSmall> sectionLoadedList = new List<UIStoriesLoaderSmall>();

    private void OnEnable()
    {
        GameController.OnStoryDBLoaded += OnStoryDBLoaded;

        UIStoriesDetailsPanel.OnStoryLiked += OnStoryLiked;
        UIStoriesDetailsPanel.OnStoryUnliked += OnStoryUnliked;
    }

    private void OnDisable()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;

        UIStoriesDetailsPanel.OnStoryLiked -= OnStoryLiked;
        UIStoriesDetailsPanel.OnStoryUnliked -= OnStoryUnliked;
    }

    private void OnDestroy()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;

        UIStoriesDetailsPanel.OnStoryLiked -= OnStoryLiked;
        UIStoriesDetailsPanel.OnStoryUnliked -= OnStoryUnliked;
    }    

    private void OnStoryDBLoaded(StoriesDB _storiesDB)
    {
        isDBLoaded = true;
        storiesDB = _storiesDB;

        PopulateSections();
    }

    private void OnStoryLiked(string _storyTitleEng)
    {
        if (!isDBLoaded)
            return;

        if (sectionLoadedList[0] == null)
            return;

        if (playerData == null)
            return;

        personalCategoryDatas[0].storiesDBItemList.Clear();
        if (playerData.StoriesLikedList.Count > 0)
        {            
            for (int i = 0; i < playerData.StoriesLikedList.Count; i++)
            {
                //if(!personalCategoryDatas[0].storiesDBItemList.Contains())
                personalCategoryDatas[0].storiesDBItemList.Add(episodesSpawner.GetStoriesDBItemFromTitle(playerData.StoriesLikedList[i]));
            }
        }

        if (playerData.StoriesLikedList.Count > 0 && !sectionLoadedList[0].gameObject.activeSelf)
            sectionLoadedList[0].gameObject.SetActive(true);

        //if (!sectionLoadedList[0].ContainsStoryItemSmall(_storyTitleEng))
        //sectionLoadedList[0].PopulateCategory(personalCategoryDatas[0].categoryTitle, personalCategoryDatas[0].storiesDBItemList.ToArray());

        if (!sectionLoadedList[0].ContainsStoryItemSmall(_storyTitleEng))
            sectionLoadedList[0].AddStoryItemSmall(episodesSpawner.GetStoriesDBItemFromTitle(_storyTitleEng));

        personalCategoryDatas[0].countsDataText.text = playerData.StoriesLikedList.Count.ToString();
    }

    private void OnStoryUnliked(string _storyTitleEng)
    {
        if (!isDBLoaded)
            return;

        if (playerData == null)
            return;

        if (sectionLoadedList[0] == null)
            return;

        personalCategoryDatas[0].storiesDBItemList.Clear();
        if (playerData.StoriesLikedList.Count > 0)
        {
            for (int i = 0; i < playerData.StoriesLikedList.Count; i++)
            {
                personalCategoryDatas[0].storiesDBItemList.Add(episodesSpawner.GetStoriesDBItemFromTitle(playerData.StoriesLikedList[i]));
            }
        }

        if (playerData.StoriesLikedList.Count > 0 && !sectionLoadedList[0].gameObject.activeSelf)
            sectionLoadedList[0].gameObject.SetActive(true);

        if (sectionLoadedList[0].ContainsStoryItemSmall(_storyTitleEng))
            sectionLoadedList[0].RemoveStoryItemSmall(_storyTitleEng);

        personalCategoryDatas[0].countsDataText.text = playerData.StoriesLikedList.Count.ToString();

        if (sectionLoadedList[0] != null && playerData.StoriesLikedList.Count <= 0)
            sectionLoadedList[0].gameObject.SetActive(false);               
    }

    private void Start()
    {
        if (detailsPanel == null)
            detailsPanel = FindObjectOfType<UIStoriesDetailsPanel>();

        if (EpisodesSpawner.instance != null && EpisodesSpawner.instance.playerData != null)
        {
            episodesSpawner = EpisodesSpawner.instance;
            playerData = episodesSpawner.playerData;

            //Bit of hard coding which is normally bad
            //personalCategoryDatas[0].storiesDBItem = playerData.StoriesStartedList;
            //personalCategoryDatas[1].storiesDBItem = playerData.StoriesCompletedList;

            //PopulateSections();
        }
    }

    private void PopulateSections()
    {
        if (personalCategoryDatas.Length <= 0)
            return;

        //Liked Stories Category
        if (playerData.StoriesLikedList.Count > 0)
        {
            for (int i = 0; i < playerData.StoriesLikedList.Count; i++)
            {
                personalCategoryDatas[0].storiesDBItemList.Add(episodesSpawner.GetStoriesDBItemFromTitle(playerData.StoriesLikedList[i]));
            }
        }

        //Started Stories Category
        if (playerData.StoriesStartedList.Count > 0)
        {
            for (int i = 0; i < playerData.StoriesStartedList.Count; i++)
            {
                personalCategoryDatas[1].storiesDBItemList.Add(episodesSpawner.GetStoriesDBItemFromTitle(playerData.StoriesStartedList[i]));
            }            
        }

        //Completed Stories Category
        if(playerData.StoriesCompletedList.Count > 0)
        {
            for (int i = 0; i < playerData.StoriesCompletedList.Count; i++)
            {
                personalCategoryDatas[2].storiesDBItemList.Add(episodesSpawner.GetStoriesDBItemFromTitle(playerData.StoriesCompletedList[i]));
            }            
        }

        personalCategoryDatas[0].countsDataText.text = playerData.StoriesLikedList.Count.ToString();
        personalCategoryDatas[1].countsDataText.text = playerData.StoriesStartedList.Count.ToString();
        personalCategoryDatas[2].countsDataText.text = playerData.StoriesCompletedList.Count.ToString();

        for (int i = 0; i < personalCategoryDatas.Length; i++)
        {
            if (personalCategoryDatas[i].storiesDBItemList != null)
            {
                UIStoriesLoaderSmall uIStoriesLoaderSmallInstance = Instantiate(sectionLoaderPrefab, sectionContentParent);

                if (i == personalCategoryDatas.Length - 1)       //All Stories Category
                {
                    if (FirebaseRemoteConfig.DefaultInstance.GetValue("Category8_Status").BooleanValue)
                        uIStoriesLoaderSmallInstance.PopulateCategory(personalCategoryDatas[i].categoryTitle, storiesDB.storiesCategories.Length - 1, storiesDB);
                    else
                        uIStoriesLoaderSmallInstance.gameObject.SetActive(false);
                }
                else
                    uIStoriesLoaderSmallInstance.PopulateCategory(personalCategoryDatas[i].categoryTitle, personalCategoryDatas[i].storiesDBItemList.ToArray());

                sectionLoadedList.Add(uIStoriesLoaderSmallInstance);

                if(personalCategoryDatas[i].countsCategoryTitleText != null)
                    personalCategoryDatas[i].countsCategoryTitleText.text = personalCategoryDatas[i].categoryTitle;                
            }
        }

        if (playerData.StoriesLikedList.Count <= 0 && sectionLoadedList[0] != null)
            sectionLoadedList[0].gameObject.SetActive(false);

        if (playerData.StoriesStartedList.Count <= 0 && sectionLoadedList[1] != null)
            sectionLoadedList[1].gameObject.SetActive(false);

        if (playerData.StoriesCompletedList.Count <= 0 && sectionLoadedList[2] != null)
            sectionLoadedList[2].gameObject.SetActive(false);

        //All Stories Category
        //if(sectionLoadedList[3] != null)
            //sectionLoadedList[3].gameObject.SetActive(FirebaseRemoteConfig.DefaultInstance.GetValue("Category8_Status").BooleanValue);
    }

    public void OpenURLLink(string _url)
    {
        if (detailsPanel)
            detailsPanel.PlayButtonClickSound();

        Application.OpenURL(_url);
    }

    public void OpenURLSocial(int index)
    {
        string url = string.Empty;
        switch(index)
        {
            case 0:         //FB
                
                url = FirebaseRemoteConfig.DefaultInstance.GetValue("Social_Link_FB").StringValue;
                Application.OpenURL(url);
                
                break;

            case 1:         //Insta

                url = FirebaseRemoteConfig.DefaultInstance.GetValue("Social_Link_Insta").StringValue;
                Application.OpenURL(url);

                break;
        }
    }

    public void OpenRateUsWindow()
    {
        if (EpisodesSpawner.instance == null)
            return;

        EpisodesSpawner.instance.OpenRateUsWindow();
    }
}
