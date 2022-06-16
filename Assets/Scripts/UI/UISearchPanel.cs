using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FuzzyString;
using Firebase.RemoteConfig;

//When used in LoadType = true (from Firebase Remote Config)
[Serializable]
public class SearchableItem
{
    public string storyTitleName;
    public UIStoriesItemSmall itemSmallInstance;
    public UIStoriesLoaderSmall itemLoaderInstance;

    /*public SearchableItem(string _storyTitleName, UIStoriesItemSmall _itemSmall)
    {
        storyTitleName = _storyTitleName;
        itemSmallInstance = _itemSmall;
    }*/
}

public class UISearchPanel : MonoBehaviour
{    
    [Header("Components")]
    public float searchMatchPercent = 0.75f;
    public ScrollRect searchScrollRect;
    public ContentSizeFitter searchContentFitter;
    public CharReplacerHindi searchTextReplacer;
    public GridLayoutGroup searchGridLayout;
    public Transform searchContentParent;
    public UIStoriesItemSmall storyItemSmallPrefab;
    public UIStoriesDetailsPanel detailsPanel;

    [Space(15)]    
    public UIStoriesLoaderSmall storiesLoaderPrefab;
    public UIStoriesLoaderSmall storiesLoaderShortsPrefab;

    [Header("Inputs")]
    public TMP_InputField searchInputField;

    private StoriesDB storiesDB;
    private Hashtable storiesDBHash = new Hashtable();
    private Hashtable storiesItemsHash = new Hashtable();

    private static WaitForSeconds waitDelay = new WaitForSeconds(0.3f);

    private List<FuzzyStringComparisonOptions> searchOptions = new List<FuzzyStringComparisonOptions>();
    private FuzzyStringComparisonTolerance searchTolerance = FuzzyStringComparisonTolerance.Strong;

    private FirebaseRemoteConfig remoteConfigInstance;

    private bool whatLoadType;

    //When used in LoadType = true (from Firebase Remote Config)
    private VerticalLayoutGroup verticalLayout;
    private List<SearchableItem> searchableItemsList;    

    private void Awake()
    {
        searchOptions.Add(FuzzyStringComparisonOptions.UseOverlapCoefficient);
        searchOptions.Add(FuzzyStringComparisonOptions.UseLongestCommonSubsequence);
        searchOptions.Add(FuzzyStringComparisonOptions.UseLongestCommonSubstring);

        //True = Load Search Windows in Categories (Same as Home Screen), False = Load in default Grid Layout
        whatLoadType = FirebaseRemoteConfig.DefaultInstance.GetValue("SearchLoadType").BooleanValue;
    }

    private void OnEnable()
    {
        GameController.OnStoryDBLoaded += OnStoriesDBLoaded;
    }

    private void OnDisable()
    {
        GameController.OnStoryDBLoaded -= OnStoriesDBLoaded;
    }

    private void OnDestroy()
    {
        GameController.OnStoryDBLoaded -= OnStoriesDBLoaded;
    }

    private void Start()
    {
        remoteConfigInstance = FirebaseRemoteConfig.DefaultInstance;
    }

    private void OnStoriesDBLoaded(StoriesDB _storiesDB)
    {
        storiesDB = _storiesDB;

        PopulateStoriesLists();
    }

    private void PopulateStoriesLists()
    {
        if (storiesDB.storiesCategories.Length <= 0)
            return;

        for (int i = 0; i < storiesDB.storiesCategories.Length; i++)
        {
            for (int j = 0; j < storiesDB.storiesCategories[i].storiesDBItems.Length; j++)
            {
                string hindiFix = searchTextReplacer.GetFixedText(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitle);
                if (!storiesDBHash.ContainsKey(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish + hindiFix))
                {                    
                    storiesDBHash.Add(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish + hindiFix, storiesDB.storiesCategories[i].storiesDBItems[j]);                
                    
                    //storiesDBHash.Add(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish + storiesDB.storiesCategories[i].storiesDBItems[j].storyTitle, storiesDB.storiesCategories[i].storiesDBItems[j]);                
                }
                //storiesDBList.Add(storiesDB.storiesCategories[i].storiesDBItems[j]);
                // print(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish);
            }
        }

        searchInputField.onValueChanged.AddListener(OnSearchValueEdit);

        //print(storiesDBHash.Count);

        /*if(storiesDBList.Count > 0)
        {
            for (int i = 0; i < storiesDBList.Count; i++)
            {
                for (int j = 0; j < storiesDBList.Count; j++)
                {
                    if (storiesDBList[i].storyTitleEnglish == storiesDBList[j].storyTitleEnglish)
                    {
                        //print(storiesDBList[i].storyTitleEnglish + " == " + storiesDBList[j].storyTitleEnglish);
                        storiesDBList.RemoveAt(i);
                    }
                }
            }
        }*/

        PopulateContent();
    }

    private void PopulateContent()
    {
        if (storiesDBHash.Count <= 0 || GameController.instance == null || detailsPanel == null)
            return;

        
        if(!whatLoadType)
            StartCoroutine(PopulateContentRoutineV1());
        else
            StartCoroutine(PopulateContentRoutineV2());
    }

    private IEnumerator PopulateContentRoutineV1()
    {
        /*for (int i = 0; i < storiesDBHash.Count; i++)
        {
            UIStoriesItemSmall storyItemSmallInstance = Instantiate(storyItemSmallPrefab, searchContentParent);
            storyItemSmallInstance.LoadThumbnailAsset(storiesDBHash[i], detailsPanel, GameController.instance);

            yield return waitDelay;
        }*/

        foreach(DictionaryEntry de in storiesDBHash)
        {           
            if (de.Value != null)
            {
                StoriesDBItem item = de.Value as StoriesDBItem;

                string storyTitleRaw = item.storyTitleEnglish;
                string storyTitleFresh = storyTitleRaw.Replace(" ", "");

                bool storyEnabled = remoteConfigInstance.GetValue("ST_" + storyTitleFresh + "_Status").BooleanValue;

                if (/*item.isStoryEnabled*/ storyEnabled)
                {
                    UIStoriesItemSmall storyItemSmallInstance = Instantiate(storyItemSmallPrefab, searchContentParent);
                    storyItemSmallInstance.transform.name = de.Key.ToString();
                    storyItemSmallInstance.LoadThumbnailAsset((StoriesDBItem)de.Value, detailsPanel, GameController.instance);

                    storiesItemsHash.Add(de.Key, storyItemSmallInstance);       //English Title, Item Instance                    
                }
            }

            yield return waitDelay;
        }

        searchScrollRect.normalizedPosition = new Vector2(0, 1f);
        //searchContentParent.GetComponent<ContentSizeFitter>().enabled = false;
    }

    private IEnumerator PopulateContentRoutineV2()
    {
        //searchGridLayout.enabled = false;
        searchableItemsList = new List<SearchableItem>();

        if (searchGridLayout != null)
            Destroy(searchGridLayout);

        yield return new WaitForEndOfFrame();

        verticalLayout = searchContentParent.gameObject.AddComponent<VerticalLayoutGroup>();
        verticalLayout.padding.top = 0;
        verticalLayout.padding.bottom = 0;
        verticalLayout.spacing = 55f;
        verticalLayout.childAlignment = TextAnchor.UpperLeft;
        verticalLayout.childControlWidth = false;
        verticalLayout.childControlHeight = false;
        verticalLayout.childForceExpandWidth = false;
        verticalLayout.childForceExpandHeight = false;

        yield return new WaitForEndOfFrame();

        for (int i = 1; i < storiesDB.storiesCategories.Length; i++)
        {
            bool isCategoryEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue("Category" + i + "_Status").BooleanValue;

            if(isCategoryEnabled)
            {
                UIStoriesLoaderSmall uIStoriesLoaderSmallInstance = null;

                if (storiesDB.storiesCategories[i].isForShortStories)
                    uIStoriesLoaderSmallInstance = Instantiate(storiesLoaderShortsPrefab, searchContentParent);
                else
                    uIStoriesLoaderSmallInstance = Instantiate(storiesLoaderPrefab, searchContentParent);

                uIStoriesLoaderSmallInstance.categoryIndex = i;
                uIStoriesLoaderSmallInstance.transform.name += i;
                uIStoriesLoaderSmallInstance.OnStoryDBLoaded(storiesDB, ref searchableItemsList);

                yield return waitDelay;
            }
        }

/*#if UNITY_EDITOR
        DebugItemsHashTable();
#endif*/

        yield return new WaitForEndOfFrame();

        searchScrollRect.normalizedPosition = new Vector2(0, 1f);
    }

    [ContextMenu("Show Debug Items Hash")]
    public void DebugItemsHashTable()
    {
        foreach (DictionaryEntry item in storiesItemsHash)
        {
            UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;
            Debug.Log($"{item.Key} - {item.Value}, In Parent - {itemMain.transform.parent.parent.parent.name}");
        }
    }

    public void OnSearchValueEdit(string val)
    {
        if (!whatLoadType)
            SearchWithLoadTypeV1(val);
        else
            SearchWithLoadTypeV2(val);
    }

    private void SearchWithLoadTypeV1(string val)
    {
        if (val.Length <= 0)
        {
            foreach (DictionaryEntry item in storiesItemsHash)
            {
                UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;
                if (!itemMain.gameObject.activeSelf)
                    itemMain.gameObject.SetActive(true);
            }

            if (searchGridLayout != null)
                searchGridLayout.childAlignment = TextAnchor.UpperCenter;

            searchContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else if (val.Length > 0)
        {
            #region OLDCODES
            /*foreach (DictionaryEntry item in storiesItemsHash)
                {
                    UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;

                    double matchPercent = ComputeSimilarity.CalculateSimilarity(val, item.Key.ToString());
                    if(matchPercent >= searchMatchPercent)
                    {
                        if (!itemMain.gameObject.activeSelf)
                            itemMain.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (itemMain.gameObject.activeSelf)
                            itemMain.gameObject.SetActive(false);
                    }                
                }*/

            /*Hashtable matchingStories = ComputeSimilarity.Search(val, storiesItemsHash, searchMatchPercent);
            foreach(DictionaryEntry item in storiesItemsHash)
            {
                UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;
                if (matchingStories.ContainsKey(item.Key))
                {
                    if (!itemMain.gameObject.activeSelf)
                        itemMain.gameObject.SetActive(true);
                }
                else
                {
                    if (itemMain.gameObject.activeSelf)
                        itemMain.gameObject.SetActive(false);
                }
            }*/ 
            #endregion

            searchTextReplacer.UpdateMe();

            foreach (DictionaryEntry item in storiesItemsHash)
            {
                UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;

                bool result = val.ApproximatelyEquals(item.Key.ToString(), searchTolerance, searchOptions.ToArray());
                if (result)
                {
                    if (!itemMain.gameObject.activeSelf)
                        itemMain.gameObject.SetActive(true);
                }
                else
                {
                    if (itemMain.gameObject.activeSelf)
                        itemMain.gameObject.SetActive(false);
                }
            }

            searchScrollRect.normalizedPosition = new Vector2(0, 1f);

            if (searchGridLayout != null)
                searchGridLayout.childAlignment = TextAnchor.UpperLeft;

            searchContentFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }

    private void SearchWithLoadTypeV2(string val)
    {
        if(val.Length <= 0)
        {
            foreach(SearchableItem item in searchableItemsList)
            {
                if (!item.itemLoaderInstance.gameObject.activeSelf)
                    item.itemLoaderInstance.gameObject.SetActive(true);

                if (!item.itemSmallInstance.gameObject.activeSelf)
                    item.itemSmallInstance.gameObject.SetActive(true);                
            }

            searchScrollRect.normalizedPosition = new Vector2(0, 1f);
        }
        else
        {
            searchTextReplacer.UpdateMe();

            foreach (SearchableItem item in searchableItemsList)
            {
                bool result = val.ApproximatelyEquals(item.storyTitleName, searchTolerance, searchOptions.ToArray());
                if(result)
                {
                    if (!item.itemLoaderInstance.gameObject.activeSelf)
                        item.itemLoaderInstance.gameObject.SetActive(true);

                    if (!item.itemSmallInstance.gameObject.activeSelf)
                        item.itemSmallInstance.gameObject.SetActive(true);
                }
                else
                {
                    if (item.itemSmallInstance.gameObject.activeSelf)
                        item.itemSmallInstance.gameObject.SetActive(false);

                    if (item.itemLoaderInstance.AllItemsInactive() && item.itemLoaderInstance.gameObject.activeSelf)
                        item.itemLoaderInstance.gameObject.SetActive(false);
                }
            }

            searchScrollRect.normalizedPosition = new Vector2(0, 1f);
        }
    }
}
