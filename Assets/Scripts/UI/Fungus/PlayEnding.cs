using System;
using Fungus;
using UnityEngine;
using UnityEngine.U2D;

[CommandInfo("UI",
                 "Play Ending Screen",
                 "Plays an Ending Screen based on type selected. (Made for Maujflix)")]
public class PlayEnding : Command
{
    public EndScreenType endScreenType;
    public SpriteAtlas nextEpisodesSpriteAtlas;    

    public int currentTextureIndexToLoad;
    public string currentTextureNameToLoad;

    public override void OnEnter()
    {
        if(EndInfo.instance == null)
        {
            Continue();
            return;
        }

        if (endScreenType == EndScreenType.EpisodeEndScreen)
        {
            if(AtlasDB.instance != null)
                if(AtlasDB.instance.nextEpsAtlas != null)
                    EndInfo.instance.PlayEndingScreen(endScreenType, AtlasDB.instance.nextEpsAtlas.GetSprite(currentTextureNameToLoad));
        }
        else
            EndInfo.instance.PlayEndingScreen(endScreenType);
    }

    public Sprite[] GetSpritesList()
    {
        if (nextEpisodesSpriteAtlas == null)
            return null;

        Sprite[] spritesList = null;

        Array.Resize(ref spritesList, nextEpisodesSpriteAtlas.spriteCount);
        nextEpisodesSpriteAtlas.GetSprites(spritesList);

        return spritesList;
    }

    public override string GetSummary()
    {
        return "Type: " + endScreenType.ToString();
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 184, 0, 255);
    }
}
