using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStoriesLoaderSmall : MonoBehaviour
{
    public bool loadOnEvent = false;

    [Space(15)]

    public int categoryIndex;
    public UIStoriesItemSmall storiesItemSmallPrefab;

    [Space(15)]

    public RectTransform categoryLayoutGroup;

    public Text categoryTitleText;
    public SiddhantaFixer categoryTitleFontFixer;
    public Text categoryCountText;

    [Space(15)]

    public Transform scrollContent;

    private UIStoriesDetailsPanel storiesDetailsPanel;

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
        if (categoryIndex >= storyDB.storiesCategories.Length)
            return;

        //categoryTitleText.text = storyDB.storiesCategories[categoryIndex].categoryName + " (" + storyDB.storiesCategories[categoryIndex].storiesDBItems.Length + ")";
        if (categoryTitleText != null && categoryTitleFontFixer != null)
        {
            categoryTitleText.text = storyDB.storiesCategories[categoryIndex].categoryName + " ";
            categoryTitleFontFixer.FixTexts();
        }

        if(categoryCountText != null)
            categoryCountText.text = "(" + storyDB.storiesCategories[categoryIndex].storiesDBItems.Length + ")";

        if(categoryLayoutGroup)
            LayoutRebuilder.ForceRebuildLayoutImmediate(categoryLayoutGroup);

        for (int i = 0; i < storyDB.storiesCategories[categoryIndex].storiesDBItems.Length; i++)
        {
            UIStoriesItemSmall storyItemSmallInstance = Instantiate(storiesItemSmallPrefab, scrollContent);
            storyItemSmallInstance.LoadThumbnailAsset(storyDB.storiesCategories[categoryIndex].storiesDBItems[i], storiesDetailsPanel, GameController.instance);
        }
    }
}