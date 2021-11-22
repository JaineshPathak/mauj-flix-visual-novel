using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISectionLoader : MonoBehaviour
{
    [Header("Prefabs")]
    public UIStoriesLoaderSmall storiesLoaderPrefab;
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

        //Starting from 1. 0th is reserved for top big thumbnails ones
        for (int i = 1; i < storiesDB.storiesCategories.Length; i++)
        {
            UIStoriesLoaderSmall uIStoriesLoaderSmallInstance = Instantiate(storiesLoaderPrefab, sectionLoaderContent);
            uIStoriesLoaderSmallInstance.categoryIndex = i;
            uIStoriesLoaderSmallInstance.OnStoryDBLoaded(storiesDB);
        }
    }
}
