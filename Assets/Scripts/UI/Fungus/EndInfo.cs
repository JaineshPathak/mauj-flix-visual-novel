using UnityEngine;

public enum EndScreenType
{
    EpisodeEndScreen,
    StoryEndBranchedScreen,
    StoryEndMainScreen
};

public class EndInfo : MonoBehaviour
{
    public static EndInfo instance;

    public UIEpisodeEndPanelMk2 endScreenNormal;
    public UIEndStoryBranchScreen endScreenStoryBranched;
    public UIEndStoryScreen endScreenStoryFull;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayEndingScreen(EndScreenType endScreenType, Sprite _nextEpSprite = null)
    {
        switch (endScreenType)
        {
            case EndScreenType.EpisodeEndScreen:
                if(_nextEpSprite)
                    endScreenNormal.nextEpisodeImageSprite = _nextEpSprite;

                endScreenNormal.PlayEndingScreen();
                break;
            case EndScreenType.StoryEndBranchedScreen:
                endScreenStoryBranched.PlayEndingStoryBranchScreen();
                break;
            case EndScreenType.StoryEndMainScreen:
                endScreenStoryFull.PlayEndingStoryScreen();
                break;
        }
    }
}
