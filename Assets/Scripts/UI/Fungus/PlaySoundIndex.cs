using UnityEngine;
using Fungus;

[CommandInfo("Audio",
                 "Play Sound (Maujflix)",
                 "Loads and plays sound at given index from Sounds Bucket. (Modified for Maujflix)")]
public class PlaySoundIndex : Command
{
    [SerializeField] protected bool unloadPreviousSound;
    [SerializeField] protected int soundIndex;

    public bool UnloadPreviousSound
    {
        get { return unloadPreviousSound; }
        set { unloadPreviousSound = value; }
    }

    public int SoundIndex
    {
        get { return soundIndex; }
        set { soundIndex = value; }
    }

    public override void OnEnter()
    {
        if (SoundsBucket.instance == null)
        {
            Continue();
            return;
        }

        if (FungusManager.Instance == null)
        {
            Continue();
            return;
        }

        SoundsBucket.instance.GetSoundAtIndex(soundIndex, (AudioClip soundClip) =>
        {
            FungusManager.Instance.MusicManager.PlaySound(soundClip, 1f);            
        }, unloadPreviousSound);

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 150, 0, 255);
    }
}
