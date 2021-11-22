using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class SiddhantaUpdater : MonoBehaviour
{
    public Text storyText;
    public Writer myWriter;

    // Update is called once per frame
    void Update()
    {
        if (storyText != null && myWriter != null)
            storyText.text = HindiCorrector2.Correct(myWriter.currentContent);
    }
}
