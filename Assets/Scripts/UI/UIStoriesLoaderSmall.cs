using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.RemoteConfig;

public class UIStoriesLoaderSmall : MonoBehaviour
{
    public bool loadOnEvent = false;

    [Space(15)]

    public int categoryIndex;
    public UIStoriesItemSmall storiesItemSmallPrefab;
    public UIStoriesItemSmall storiesItemShortsPrefab;

    [Space(15)]

    public RectTransform categoryLayoutGroup;

    /*public Text categoryTitleText;
    public SiddhantaFixer categoryTitleFontFixer;
    public Text categoryCountText;*/

    public TextMeshProUGUI categoryTitleText;
    public CharReplacerHindi categoryTitleFontFixer;
    public TextMeshProUGUI categoryCountText;

    [Space(15)]

    public Transform scrollContent;

    private UIStoriesDetailsPanel storiesDetailsPanel;
    private List<UIStoriesItemSmall> storiesItemSmallList = new List<UIStoriesItemSmall>();

    private void Awake()
    {
        storiesDetailsPanel = FindObjectOfType<UIStoriesDetailsPanel>();
    }

    private void OnEnable()
    {
        if(loadOnEvent)
            GameController.OnStoryDBLoaded += OnStoryDBLoaded;
    }

    private void OnDisable()
    {
        if (loadOnEvent)
            GameController.OnStoryDBLoaded -= OnStoryDBLoaded;
    }

    private void OnDestroy()
    {
        if (loadOnEvent)
            GameController.OnStoryDBLoaded -= OnStoryDBLoaded;
    }

    public void OnStoryDBLoaded(StoriesDB storyDB)
    {
        if (GameController.instance == null)
            return;

        if (categoryIndex >= storyDB.storiesCategories.Length)
            return;

        //categoryTitleText.text = storyDB.storiesCategories[categoryIndex].categoryName + " (" + storyDB.storiesCategories[categoryIndex].storiesDBItems.Length + ")";
        if (categoryTitleText != null && categoryTitleFontFixer != null)
        {
            categoryTitleText.text = storyDB.storiesCategories[categoryIndex].categoryName + " ";
            categoryTitleFontFixer.UpdateMe();
            
            //categoryTitleFontFixer.FixTexts();
        }

        if(categoryLayoutGroup)
            LayoutRebuilder.ForceRebuildLayoutImmediate(categoryLayoutGroup);

        for (int i = 0; i < storyDB.storiesCategories[categoryIndex].storiesDBItems.Length; i++)
        {
            string storyTitleRaw = storyDB.storiesCategories[categoryIndex].storiesDBItems[i].storyTitleEnglish;
            string storyTitleFresh = storyTitleRaw.Replace(" ", "");

            bool storyEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue("ST_" + storyTitleFresh + "_Status").BooleanValue;

            if (/*storyDB.storiesCategories[categoryIndex].storiesDBItems[i].isStoryEnabled*/storyEnabled)
            {
                UIStoriesItemSmall storyItemSmallInstance = null;

                if (storyDB.storiesCategories[categoryIndex].storiesDBItems[i].isShortStory)
                {
                    if(ThumbnailItemsPool.instance != null)
                    {
                        GameObject itemInstanceGO = ThumbnailItemsPool.instance.GetThumbnailItem(2);
                        if(itemInstanceGO)
                            storyItemSmallInstance = itemInstanceGO.GetComponent<UIStoriesItemSmall>();

                        if(storyItemSmallInstance)
                        {
                            storyItemSmallInstance.gameObject.SetActive(true);
                            storyItemSmallInstance.transform.parent = scrollContent;
                        }
                        else
                            storyItemSmallInstance = Instantiate(storiesItemShortsPrefab, scrollContent);
                    }
                    else
                        storyItemSmallInstance = Instantiate(storiesItemShortsPrefab, scrollContent);
                }
                else
                {
                    if (ThumbnailItemsPool.instance != null)
                    {
                        GameObject itemInstanceGO = ThumbnailItemsPool.instance.GetThumbnailItem(1);
                        if (itemInstanceGO)
                            storyItemSmallInstance = itemInstanceGO.GetComponent<UIStoriesItemSmall>();

                        if (storyItemSmallInstance)
                        {
                            storyItemSmallInstance.gameObject.SetActive(true);
                            storyItemSmallInstance.transform.parent = scrollContent;
                        }
                        else
                            storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);
                    }
                    else
                        storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);                    
                }

                storyItemSmallInstance.transform.name = storyDB.storiesCategories[categoryIndex].storiesDBItems[i].storyTitleEnglish;
                storyItemSmallInstance.LoadThumbnailAsset(storyDB.storiesCategories[categoryIndex].storiesDBItems[i], storiesDetailsPanel, GameController.instance);

                storiesItemSmallList.Add(storyItemSmallInstance);
            }            
        }

        if (categoryCountText != null)
            categoryCountText.text = "(" + storiesItemSmallList.Count + ")";

        //Shuffle List if the Category is "New Stories"
        if(categoryIndex == 1)
        {
            storiesItemSmallList = OtherUtils.Shuffle<UIStoriesItemSmall>(storiesItemSmallList);
            for (int i = 0; i < storiesItemSmallList.Count; i++)            
                storiesItemSmallList[i].transform.SetSiblingIndex(i);            
        }
    }

    //Called from UIPersonalProfile.cs
    public void PopulateCategory(string _categoryTitle, StoriesDBItem[] _storiesDBItems)
    {
        if (GameController.instance == null)
            return;

        if (categoryTitleText != null && categoryTitleFontFixer != null)
        {
            categoryTitleText.text = _categoryTitle + " ";
            categoryTitleFontFixer.UpdateMe();
            //categoryTitleFontFixer.FixTexts();
        }        

        for (int i = 0; i < _storiesDBItems.Length; i++)
        {
            string storyTitleRaw = _storiesDBItems[i].storyTitleEnglish;
            string storyTitleFresh = storyTitleRaw.Replace(" ", "");

            bool storyEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue("ST_" + storyTitleFresh + "_Status").BooleanValue;

            if (storyEnabled)
            {
                UIStoriesItemSmall storyItemSmallInstance = null;
                if (ThumbnailItemsPool.instance != null)
                {
                    GameObject itemInstanceGO = ThumbnailItemsPool.instance.GetThumbnailItem(1);
                    if (itemInstanceGO)
                        storyItemSmallInstance = itemInstanceGO.GetComponent<UIStoriesItemSmall>();

                    if (storyItemSmallInstance != null)
                    {
                        storyItemSmallInstance.gameObject.SetActive(true);
                        storyItemSmallInstance.transform.parent = scrollContent;
                    }
                    else
                        storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);
                }
                else
                    storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);

                storyItemSmallInstance.transform.name = _storiesDBItems[i].storyTitleEnglish;
                storyItemSmallInstance.LoadThumbnailAsset(_storiesDBItems[i], storiesDetailsPanel, GameController.instance);

                storiesItemSmallList.Add(storyItemSmallInstance);
            }
        }

        if (categoryCountText != null)
            categoryCountText.text = "(" + storiesItemSmallList.Count + ")";
    }

    //Called from UIPersonalProfile.cs
    public void PopulateCategory(string _categoryTitle, int _databaseIndex, StoriesDB _storyDB)
    {
        if (GameController.instance == null)
            return;

        if (categoryTitleText != null && categoryTitleFontFixer != null)
        {
            categoryTitleText.text = _categoryTitle + " ";
            categoryTitleFontFixer.UpdateMe();
            //categoryTitleFontFixer.FixTexts();
        }

        for (int i = 0; i < _storyDB.storiesCategories[_databaseIndex].storiesDBItems.Length; i++)
        {
            string storyTitleRaw = _storyDB.storiesCategories[_databaseIndex].storiesDBItems[i].storyTitleEnglish;
            string storyTitleFresh = storyTitleRaw.Replace(" ", "");

            bool storyEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue("ST_" + storyTitleFresh + "_Status").BooleanValue;

            if (storyEnabled)
            {
                UIStoriesItemSmall storyItemSmallInstance = null;
                if (ThumbnailItemsPool.instance != null)
                {
                    GameObject itemInstanceGO = ThumbnailItemsPool.instance.GetThumbnailItem(1);
                    if (itemInstanceGO)
                        storyItemSmallInstance = itemInstanceGO.GetComponent<UIStoriesItemSmall>();

                    if (storyItemSmallInstance != null)
                    {
                        storyItemSmallInstance.gameObject.SetActive(true);
                        storyItemSmallInstance.transform.parent = scrollContent;
                    }
                    else
                        storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);
                }
                else
                    storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);

                storyItemSmallInstance.transform.name = _storyDB.storiesCategories[_databaseIndex].storiesDBItems[i].storyTitleEnglish;
                storyItemSmallInstance.LoadThumbnailAsset(_storyDB.storiesCategories[_databaseIndex].storiesDBItems[i], storiesDetailsPanel, GameController.instance);

                storiesItemSmallList.Add(storyItemSmallInstance);
            }
        }

        if (categoryCountText != null)
            categoryCountText.text = "(" + storiesItemSmallList.Count + ")";
    }

    public void AddStoryItemSmall(StoriesDBItem _storiesDBItem)
    {
        if (ContainsStoryItemSmall(_storiesDBItem.storyTitleEnglish))
            return;

        UIStoriesItemSmall storyItemSmallInstance = null;
        if (ThumbnailItemsPool.instance != null)
        {
            GameObject itemInstanceGO = ThumbnailItemsPool.instance.GetThumbnailItem(1);
            if (itemInstanceGO)
                storyItemSmallInstance = itemInstanceGO.GetComponent<UIStoriesItemSmall>();

            if (storyItemSmallInstance != null)
            {
                storyItemSmallInstance.gameObject.SetActive(true);
                storyItemSmallInstance.transform.parent = scrollContent;
            }
            else
                storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);
        }
        else
            storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);

        storyItemSmallInstance.transform.name = _storiesDBItem.storyTitleEnglish;
        storyItemSmallInstance.LoadThumbnailAsset(_storiesDBItem, storiesDetailsPanel, GameController.instance);

        storiesItemSmallList.Add(storyItemSmallInstance);

        if (categoryCountText != null)
            categoryCountText.text = "(" + storiesItemSmallList.Count + ")";
    }

    public bool ContainsStoryItemSmall(string _storyTitleEng)
    {
        if (storiesItemSmallList.Count <= 0)
            return false;

        for (int i = 0; i < storiesItemSmallList.Count; i++)
        {
            if (storiesItemSmallList[i].transform.name.Equals(_storyTitleEng))
                return true;
        }

        return false;
    }

    public void RemoveStoryItemSmall(string _storyTitleEng)
    {
        if (storiesItemSmallList.Count <= 0)
            return;

        for (int i = 0; i < storiesItemSmallList.Count; i++)
        {
            if (storiesItemSmallList[i].transform.name.Equals(_storyTitleEng))
            {
                if (storiesItemSmallList[i].isFromPool && ThumbnailItemsPool.instance != null)                                    
                    ThumbnailItemsPool.instance.ResetThumbnailItem(1, storiesItemSmallList[i].gameObject);                
                else
                    Destroy(storiesItemSmallList[i].gameObject);

                storiesItemSmallList.Remove(storiesItemSmallList[i]);
                break;
            }
        }

        if (categoryCountText != null)
            categoryCountText.text = "(" + storiesItemSmallList.Count + ")";
    }
}