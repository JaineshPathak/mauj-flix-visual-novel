//This will be saved in a json file and stored as Addressable Asset Bundle

public enum CategoryType
{
    Type_Normal,
    Type_Trending,
    Type_Shorts
};

[System.Serializable]
public class StoriesDB
{
    public StoriesCategory[] storiesCategories;    

    /*public StoriesDBItem GetStoryDBItemAtIndex(int index)
    {
        if (storiesDBItems.Length <= 0)
            return null;

        return storiesDBItems[index];
    }*/
}

[System.Serializable]
public class StoriesCategory
{
    public bool isEnabled = true;
    public CategoryType categoryType;
    //public bool isForShortStories;

    public string categoryName;
    public int categoryIndex;

    public StoriesDBItem[] storiesDBItems;
}