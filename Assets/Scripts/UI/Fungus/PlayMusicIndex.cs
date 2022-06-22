using UnityEngine;
using Fungus;

[CommandInfo("Audio",
                 "Play Music (Maujflix)",
                 "Loads and plays music at given index from Sounds Bucket. (Modified for Maujflix)")]
public class PlayMusicIndex : Command
{
    [SerializeField] protected bool unloadPreviousMusic;
    [SerializeField] protected int musicIndex;

    public bool UnloadPreviousMusic
    {
        get { return unloadPreviousMusic; }
        set { unloadPreviousMusic = value; }
    }

    public int MusicIndex
    {
        get { return musicIndex; }
        set { musicIndex = value; }
    }

    public override void OnEnter()
    {
        if(SoundsBucket.instance == null)
        {
            Continue();
            return;
        }

        if(FungusManager.Instance == null)
        {
            Continue();
            return;
        }

        SoundsBucket.instance.GetMusicAtIndex(musicIndex, (AudioClip musicClip) => 
        {
            //FungusManager.Instance.MusicManager.PlayMusic(musicClip, true, 1f, 0);
            FindObjectOfType<EpisodesHandler>().PlayMusicAtIndex(musicClip, musicIndex);
        }, unloadPreviousMusic);

        Continue();
    }

    public override Color GetButtonColor()
    {
        return Color.magenta;
    }
}
