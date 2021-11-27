using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FuzzyString;

public class UISearchPanel : MonoBehaviour
{
    [Header("Components")]
    public float searchMatchPercent = 0.75f;
    public ScrollRect searchScrollRect;
    public ContentSizeFitter searchContentFitter;
    public GridLayoutGroup searchGridLayout;
    public Transform searchContentParent;
    public UIStoriesItemSmall storyItemSmallPrefab;
    public UIStoriesDetailsPanel detailsPanel;

    [Header("Inputs")]
    public TMP_InputField searchInputField;

    private StoriesDB storiesDB;
    private Hashtable storiesDBHash = new Hashtable();
    private Hashtable storiesItemsHash = new Hashtable();    

    private static WaitForSeconds waitDelay = new WaitForSeconds(0.3f);

    private List<FuzzyStringComparisonOptions> searchOptions = new List<FuzzyStringComparisonOptions>();
    private FuzzyStringComparisonTolerance searchTolerance = FuzzyStringComparisonTolerance.Strong;

    private void Awake()
    {
        searchOptions.Add(FuzzyStringComparisonOptions.UseOverlapCoefficient);
        searchOptions.Add(FuzzyStringComparisonOptions.UseLongestCommonSubsequence);
        searchOptions.Add(FuzzyStringComparisonOptions.UseLongestCommonSubstring);
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
                if(!storiesDBHash.ContainsKey(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish + storiesDB.storiesCategories[i].storiesDBItems[j].storyTitle))
                {
                    storiesDBHash.Add(storiesDB.storiesCategories[i].storiesDBItems[j].storyTitleEnglish + storiesDB.storiesCategories[i].storiesDBItems[j].storyTitle, storiesDB.storiesCategories[i].storiesDBItems[j]);                
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

        StartCoroutine(PopulateContentRoutine());
    }

    private IEnumerator PopulateContentRoutine()
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
                UIStoriesItemSmall storyItemSmallInstance = Instantiate(storyItemSmallPrefab, searchContentParent);
                storyItemSmallInstance.transform.name = de.Key.ToString();
                storyItemSmallInstance.LoadThumbnailAsset((StoriesDBItem)de.Value, detailsPanel, GameController.instance);

                storiesItemsHash.Add(de.Key, storyItemSmallInstance);       //English Title, Item Instance
            }

            yield return waitDelay;
        }

        searchScrollRect.normalizedPosition = new Vector2(0, 1f);
        //searchContentParent.GetComponent<ContentSizeFitter>().enabled = false;
    }

    public void OnSearchValueEdit(string val)
    {
        if(val.Length <= 0)
        {
            foreach (DictionaryEntry item in storiesItemsHash)
            {
                UIStoriesItemSmall itemMain = (UIStoriesItemSmall)item.Value;
                if (!itemMain.gameObject.activeSelf)
                    itemMain.gameObject.SetActive(true);
            }

            searchGridLayout.childAlignment = TextAnchor.UpperCenter;
            searchContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else if(val.Length > 0)
        {
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
            searchGridLayout.childAlignment = TextAnchor.UpperLeft;
            searchContentFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
