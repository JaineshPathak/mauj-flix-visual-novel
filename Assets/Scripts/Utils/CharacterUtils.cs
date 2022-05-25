#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus;

[RequireComponent(typeof(Fungus.Character))]
public class CharacterUtils : MonoBehaviour
{
    public Flowchart episodeFlowchart;
    public Character character;

    [Space(10f)]

    public int usageCount;
    public bool isChecked;

    private void OnValidate()
    {
        if (episodeFlowchart == null)
            episodeFlowchart = transform.root.GetComponentInChildren<Flowchart>();

        if (character == null)
            character = GetComponent<Character>();
    }

    public void CheckCharacterUsage()
    {
        if (character == null || episodeFlowchart == null)
            return;

        isChecked = true;
        usageCount = 0;
        foreach(Portrait portraitCmd in episodeFlowchart.GetComponentsInChildren<Portrait>())
        {
            if (portraitCmd != null && portraitCmd.enabled && portraitCmd._Character != null)
                if (portraitCmd._Character == character)
                    usageCount++;
        }
    }
}

[CustomEditor(typeof(CharacterUtils)), DisallowMultipleComponent]
public class CharacterUtilsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CharacterUtils characterUtils = target as CharacterUtils;

        EditorGUILayout.Space(10f);

        if(characterUtils.episodeFlowchart != null && characterUtils.character != null)
        {
            if (characterUtils.isChecked)
            {
                if (characterUtils.usageCount <= 0)
                    EditorGUILayout.HelpBox("This character is not used in flowchart. Maybe it's safe to Delete?", MessageType.Info);
                else
                    EditorGUILayout.HelpBox("This character is being used in flowchart. Don't Delete it!", MessageType.Warning);
            }

            if (GUILayout.Button("Check Character Usage"))
                characterUtils.CheckCharacterUsage();
        }
    }
}
#endif