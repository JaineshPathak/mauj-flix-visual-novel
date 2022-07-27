using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Firebase.RemoteConfig;

public class StageModifier : MonoBehaviour
{
    public Stage portraitStage;
    public List<RectTransform> phoneSprites;

    private void OnValidate()
    {
        if (portraitStage == null)
            portraitStage = GetComponent<Stage>();

        if(portraitStage != null)
        {
            if(phoneSprites != null)
                phoneSprites.Clear();

            phoneSprites = new List<RectTransform>(portraitStage.Positions);

            for (int i = 0; i < phoneSprites.Count && (phoneSprites.Count > 0); i++)
            {
                string lowerName = phoneSprites[i].transform.name.ToUpper();
                if (lowerName.IndexOf("CALL", System.StringComparison.CurrentCulture) <= 0)
                    phoneSprites.RemoveAt(i);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < phoneSprites.Count && (phoneSprites.Count > 0); i++)
        {

        }
    }
}
