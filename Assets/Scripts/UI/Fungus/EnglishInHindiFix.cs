using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/*
 * IMPORTANT NOTICE:
 * ONLY ADD THIS WHEN YOU ARE USING CLUMSYDEV HINDI FONT ASSET
 * OTHERWISE IMMEDIATELY REMOVE THIS COMPONENT FROM THE PREFAB!
 * 
 * WHAT THIS SCRIPT DOES?
 * The problem is with ClumsyDev Hindi font asset is that it shows the Hindi font correctly.
 * But English letters like Player Names are not shown properly. Instead english letters are shown in form of Hindi letters. 
 * Because 90% the Player Name will be in english
 * A way to fix this is to load a different font via HTML <font></font> tag inside Say commands
 * 
 * THIS COMPONENT IS USED FOR UPDATING THE SAY COMMAND OF FUNGUS ASSET
 * AND IT WILL ONLY WORK WITH FUNGUS VARIABLE WHOSE DATA TYPE IS STRING
 * 
 */

public class EnglishInHindiFix : MonoBehaviour
{
    public Flowchart flowchart;

    [Space(15)]

    public VariableReference[] variableReferences;
    public List<string> variableNamesProper = new List<string>();    

    public static string replaceTextStartPre = "\"" + "LiberationSans SDF" + "\"";

    [Space(15)]

    public string replaceTextStart = "<font=" + replaceTextStartPre + "><size=50>";
    public string replaceTextEnd = "</size></font>";

    private void OnValidate()
    {
        if (flowchart == null)
            flowchart = GetComponentInChildren<Flowchart>();

        variableNamesProper.Clear();

        if (variableReferences.Length > 0)
        {
            for (int i = 0; i < variableReferences.Length; i++)
            {
                if(variableReferences[i].variable != null)
                    variableNamesProper.Add("{$" + variableReferences[i].variable.Key + "}");
            }
        }
    }

    private void Start()
    {
        UpdateSayCommands();
    }

    [ContextMenu("Update Say Commands")]
    public void UpdateSayCommands()
    {
        if(flowchart != null && variableNamesProper.Count > 0)
        {
            for (int i = 0; i < variableNamesProper.Count; i++)
            {
                foreach (Say sayCommand in flowchart.GetComponentsInChildren<Say>())
                {
                    if(sayCommand.enabled)
                    {
                        string storyTextTemp = sayCommand.GetStandardText();                        
                        if (storyTextTemp.Contains(variableNamesProper[i]))
                        {
                            storyTextTemp = storyTextTemp.Replace(variableNamesProper[i], replaceTextStart + variableNamesProper[i] + replaceTextEnd);
                            //print(storyTextTemp);
                            sayCommand.SetStandardText(storyTextTemp);
                        }
                    }
                }
            }
        }
    }
}
