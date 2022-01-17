//This will be saved in a json file and stored as Addressable Asset Bundle

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
    public bool isForShortStories;

    public string categoryName;
    public int categoryIndex;

    public StoriesDBItem[] storiesDBItems;
}