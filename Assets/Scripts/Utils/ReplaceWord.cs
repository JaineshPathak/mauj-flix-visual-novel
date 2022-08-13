#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Fungus;

[DisallowMultipleComponent]
public class ReplaceWord : MonoBehaviour
{
    public string originalWord;
    public string newWord;

    [Space(15)]

    public Flowchart episodeFlowchart;

    private void OnValidate()
    {
        if (episodeFlowchart == null)
            episodeFlowchart = GetComponentInChildren<Flowchart>();
    }

    public void StartReplacingWord()
    {
        if (episodeFlowchart == null)
            return;

        foreach(Say sayCmd in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if(sayCmd != null && sayCmd.enabled)
            {
                string descOld = sayCmd.StoryText;
                string descNew = descOld;
                if(descOld.Contains(originalWord))
                {
                    descNew = descNew.Replace(originalWord, newWord);
                    sayCmd.StoryText = descNew;
                    Debug.Log($"Found and Replaced Old Word '{originalWord}' with New Word '{newWord}'");
                }
            }
        }
    }
}

[CustomEditor(typeof(ReplaceWord))]
public class ReplaceWordEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ReplaceWord replaceWord = target as ReplaceWord;

        EditorGUILayout.HelpBox("This will replace a word in all Say Commands in Flowchart. \nProcess with CAUTION!", MessageType.Warning);

        DrawDefaultInspector();

        EditorGUILayout.Space(10f);

        if(replaceWord.episodeFlowchart != null)
        {
            if (GUILayout.Button("Replace Word"))
            {
                replaceWord.StartReplacingWord();
            }
        }        
    }
}
#endif