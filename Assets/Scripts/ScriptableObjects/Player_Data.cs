using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "Player_Data", menuName = "Scriptable Objects/Create Player Data", order = 1)]
public class Player_Data : ScriptableObject
{
    public bool hasRatedGame;
    public bool hasGivenGenderType;
    public int genderType;      //0 = Male, 1 - Female

    [Space(15)]

    [SerializeField] private List<string> likedStories = new List<string>();

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

    public bool HasStoryLiked(string storyTitle)
    {
        if (likedStories.Contains(storyTitle))
            return true;

        return false;
    }

    [ContextMenu("Reset Player Data")]
    public void ResetPlayerData()
    {        
        hasRatedGame = false;

        hasGivenGenderType = false;
        genderType = 0;

        likedStories.Clear();
    }    
}
