using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Player_Data", menuName = "Scriptable Objects/Create Player Data", order = 1)]
public class Player_Data : ScriptableObject
{
    public bool hasRatedGame;
    public bool hasGivenGenderType;
    public int genderType;      //0 = Male, 1 - Female
    public bool soundStatus = true;

    [Header("Liked Stories Names")]
    [SerializeField] private List<string> likedStories = new List<string>();

    public List<string> StoriesLikedList { get { return likedStories; } }

    [Header("Started Stories")]
    [SerializeField] private List<string> storiesStartedList = new List<string>();
    //[SerializeField] private List<StoriesDBItem> storiesStartedList = new List<StoriesDBItem>();

    public List<string> StoriesStartedList { get { return storiesStartedList; } }

    [Header("Completed Stories")]
    [SerializeField] private List<string> storiesCompletedList = new List<string>();
    //[SerializeField] private List<StoriesDBItem> storiesCompletedList = new List<StoriesDBItem>();

    public List<string> StoriesCompletedList { get { return storiesCompletedList; } }

    #region Story Liked Section
    //=====================================================================================

    public bool HasStoryLiked(string storyTitle)
    {
        if (likedStories.Contains(storyTitle))
            return true;

        return false;
    }

    public void AddStoryLiked(string storyTitle)
    {
        if (likedStories.Contains(storyTitle))
            return;
            
        likedStories.Add(storyTitle);
    }

    public void RemoveStoryLiked(string storyTitle)
    {
        if (!likedStories.Contains(storyTitle))
            return;
        
        likedStories.Remove(storyTitle);
    }

    //=====================================================================================
    #endregion

    #region Started Stories Section
    //=====================================================================================

    /*public bool HasStoryStarted(StoriesDBItem storyItem)
    {
        if (storiesStartedList.Contains(storyItem))
            return true;

        return false;
    }*/

    public bool ContainsStoryStarted(string _storyTitleEng)
    {
        if (storiesStartedList.Count <= 0)
            return false;

        if (storiesStartedList.Contains(_storyTitleEng))
            return true;

        return false;
    }

    public void AddStoryStarted(string _storyTitleEng)
    {
        if (storiesStartedList.Contains(_storyTitleEng))
            return;

        storiesStartedList.Add(_storyTitleEng);
    }

    public void RemoveStoryStarted(string _storyTitleEng)
    {
        if (!storiesStartedList.Contains(_storyTitleEng))
            return;

        storiesStartedList.Remove(_storyTitleEng);
    }

    /*public void AddStoryStarted(StoriesDBItem storyItem)
    {
        if (storiesStartedList.Contains(storyItem))
            return;

        storiesStartedList.Add(storyItem);
    }

    public void RemoveStoryStarted(StoriesDBItem storyItem)
    {
        if (!storiesStartedList.Contains(storyItem))
            return;

        storiesStartedList.Remove(storyItem);
    }*/

    //=====================================================================================
    #endregion

    #region Stories Completed Section
    //=====================================================================================

    /*public bool HasStoryCompleted(StoriesDBItem storyItem)
    {
        if (storiesCompletedList.Contains(storyItem))
            return true;

        return false;
    }*/

    public bool ContainsStoryCompleted(string _storyTitleEng)
    {
        if (storiesCompletedList.Count <= 0)
            return false;

        if (storiesCompletedList.Contains(_storyTitleEng))
            return true;

        return false;
    }

    public void AddStoryCompleted(string _storyTitleEng)
    {
        if (storiesCompletedList.Contains(_storyTitleEng))
            return;

        storiesCompletedList.Add(_storyTitleEng);
    }

    public void RemoveStoryCompleted(string _storyTitleEng)
    {
        if (!storiesCompletedList.Contains(_storyTitleEng))
            return;

        storiesCompletedList.Remove(_storyTitleEng);
    }

    /*public void AddStoryCompleted(StoriesDBItem storyItem)
    {
        if (storiesCompletedList.Contains(storyItem))
            return;

        storiesCompletedList.Add(storyItem);
    }

    public void RemoveStoryCompleted(StoriesDBItem storyItem)
    {
        if (!storiesCompletedList.Contains(storyItem))
            return;

        storiesCompletedList.Remove(storyItem);
    }*/

    //=====================================================================================
    #endregion

    [ContextMenu("Reset Player Data")]
    public void ResetPlayerData()
    {        
        hasRatedGame = false;

        hasGivenGenderType = false;
        genderType = 0;

        soundStatus = true;

        likedStories.Clear();
        storiesStartedList.Clear();
        storiesCompletedList.Clear();
    }    
}
