#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReworkerSounds", menuName = "Stories/Create Reworker Sounds", order = 1)]
public class ReworkerSoundsSO : ScriptableObject
{
    public List<AudioClip> reworkerSoundsList = new List<AudioClip>();
}
#endif