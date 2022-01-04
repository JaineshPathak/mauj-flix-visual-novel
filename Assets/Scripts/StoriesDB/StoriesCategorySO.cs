﻿#if UNITY_EDITOR
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoriesCategorySO", menuName = "Stories/Create Stories Category", order = 1)]
public class StoriesCategorySO : ScriptableObject
{
    public string categoryName;
    public int categoryIndex;
    public StoriesDBItemSO[] storiesDBItems;

    private StoriesCategory category;

    public StoriesCategory GetStoriesCategory()
    {
        category = new StoriesCategory();

        category.categoryName = this.categoryName;
        category.categoryIndex = this.categoryIndex;

        Array.Resize(ref category.storiesDBItems, this.storiesDBItems.Length);
        if (this.storiesDBItems.Length > 0)
        {
            for (int i = 0; i < this.storiesDBItems.Length; i++)
            {
                if(this.storiesDBItems[i] != null)
                {
                    StoriesDBItem item = this.storiesDBItems[i].GetStoriesDBItem();
                    category.storiesDBItems[i] = item;
                }
            }
        }

        return category;
    }
}
#endif