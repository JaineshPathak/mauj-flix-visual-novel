using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NestedScroll.ScrollElement;
using Firebase.RemoteConfig;

public class UIStoriesLoaderBig : MonoBehaviour
{
    public int categoryIndex;
    public UIStoriesItemBig storiesItemBigPrefab;

    [Space(15)]

    public bool autoScrolling = true;
    public float autoScrollInterval = 3f;
    public SnapScrolling snapScroller;

    [Space(15)]

    public Transform scrollContent;

    private UIStoriesDetailsPanel storiesDetailsPanel;    

    private void Awake()
    {
        storiesDetailsPanel = FindObjectOfType<UIStoriesDetailsPanel>();        
    }

    private void OnEnable()
    {
        GameController.OnStoryDBLoaded += OnStoryDBLoaded;
    }

    private void OnDisable()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;
    }

    private void OnDestroy()
    {
        GameController.OnStoryDBLoaded -= OnStoryDBLoaded;
    }

    private void OnStoryDBLoaded(StoriesDB storyDB)
    {
        if (categoryIndex >= storyDB.storiesCategories.Length)
            return;

        for (int i = 0; i < storyDB.storiesCategories[categoryIndex].storiesDBItems.Length; i++)
        {
            string storyTitleRaw = storyDB.storiesCategories[categoryIndex].storiesDBItems[i].storyTitleEnglish;
            string storyTitleFresh = storyTitleRaw.Replace(" ", "");

            bool storyEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue("ST_" + storyTitleFresh + "_Status").BooleanValue;

            if (storyEnabled)
            {
                UIStoriesItemBig storyItemBigInstance = Instantiate(storiesItemBigPrefab, scrollContent);
                storyItemBigInstance.LoadThumbnailAssets(storyDB.storiesCategories[categoryIndex].storiesDBItems[i], storiesDetailsPanel);
            }
        }

        snapScroller.UpdateScroll();
        snapScroller.autoScrolling = autoScrolling;
        snapScroller.autoScrollingInterval = autoScrollInterval;
    }
}