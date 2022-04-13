[System.Serializable]
public class StoriesDBItem
{
    public bool isNewStory;
    public bool isStoryEnabled = true;
    public bool isShortStory;

    public string storyTitle;
    public string storyTitleEnglish;

    public string storyDescription;

    public int storyTotalBlocksCount;

    public string storyTitleImageKey;
    public string storyThumbnailSmallKey;
    public string storyThumbnailBigKey;
    public string storyThumbnailLoadingKey;
    
    public string storyFlowchartKey;
    public string storyProgressFileName;
    
    public string[] storyEpisodesKeys;

    public string[] storyBranchEpisodesKeys;
}