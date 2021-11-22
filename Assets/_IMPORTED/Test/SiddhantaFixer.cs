using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiddhantaFixer : MonoBehaviour
{
    public Text normalText;
    public bool fixAtStart = true;
    public bool updateAllTime;

    private void Start()
    {
        if(fixAtStart)
            FixTexts();
    }

    private void OnValidate()
    {
        if (normalText == null)
            normalText = GetComponent<Text>();
    }

    [ContextMenu("Fix Texts")]
    public void FixTexts()
    {
        if (normalText == null)
            return;

        if(normalText.text.Length > 0)
            normalText.text = HindiCorrector2.Correct(normalText.text);
    }

    private void Update()
    {
        if (!updateAllTime)
            return;

        FixTexts();
    }
}
