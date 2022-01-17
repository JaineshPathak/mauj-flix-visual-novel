using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISectionLoader : MonoBehaviour
{
    [Header("Prefabs")]
    public UIStoriesLoaderSmall storiesLoaderPrefab;
    public UIStoriesLoaderSmall storiesLoaderShortsPrefab;

    [Space(15)]

    public Transform sectionLoaderContent;

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

    private void OnStoryDBLoaded(StoriesDB storiesDB)
    {
        if (storiesDB == null)
            return;

        //Starting from index 1. 0th index is reserved for top big thumbnails ones
        for (int i = 1; i < storiesDB.storiesCategories.Length; i++)
        {
            if (storiesDB.storiesCategories[i].isEnabled)       //Useful if you want to disable unwanted categories
            {
                UIStoriesLoaderSmall uIStoriesLoaderSmallInstance;

                if (storiesDB.storiesCategories[i].isForShortStories)
                    uIStoriesLoaderSmallInstance = Instantiate(storiesLoaderShortsPrefab, sectionLoaderContent);
                else
                    uIStoriesLoaderSmallInstance = Instantiate(storiesLoaderPrefab, sectionLoaderContent);

                uIStoriesLoaderSmallInstance.categoryIndex = i;
                uIStoriesLoaderSmallInstance.OnStoryDBLoaded(storiesDB);
            }
        }
    }
}