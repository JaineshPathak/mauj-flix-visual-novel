using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(MFlixReplacer))]
public class MFlixReplacerEditor : Editor
{
    private SerializedProperty whatToReplaceSerialized;
    private SerializedProperty episodeFlowchartSerialized;

    //Tutorial Narrative Dialogue Mode
    private SerializedProperty tutorialDestroyOriginalSerialized;
    private SerializedProperty tutorialDialogueInSceneSerialized;
    private SerializedProperty tutorialDialoguePrefabSerialized;

    //Narrative Dialogue Mode
    private SerializedProperty narrativeDestroyOriginalSerialized;
    private SerializedProperty narrativeDialogueInSceneSerialized;
    private SerializedProperty narrativeDialoguePrefabSerialized;

    //Say Dialogue Mode
    private SerializedProperty sayDialogueDestroyOriginalSerialized;
    private SerializedProperty sayDialogueInSceneSerialized;
    private SerializedProperty sayDialoguePrefabSerialized;

    private MFlixReplacer flixReplacer;

    private void OnEnable()
    {
        whatToReplaceSerialized = serializedObject.FindProperty("whatToReplace");
        episodeFlowchartSerialized = serializedObject.FindProperty("episodeFlowchart");

        tutorialDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalTutorialDialogue");
        tutorialDialogueInSceneSerialized = serializedObject.FindProperty("tutorialDialogueInScene");
        tutorialDialoguePrefabSerialized = serializedObject.FindProperty("tutorialDialoguePrefab");

        narrativeDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalNarrativeDialogue");
        narrativeDialogueInSceneSerialized = serializedObject.FindProperty("narrativeDialogueInScene");
        narrativeDialoguePrefabSerialized = serializedObject.FindProperty("narrativeDialoguePrefab");

        sayDialogueDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalSayDialogue");
        sayDialogueInSceneSerialized = serializedObject.FindProperty("sayDialogueInScene");
        sayDialoguePrefabSerialized = serializedObject.FindProperty("sayDialoguePrefab");
    }

    public override void OnInspectorGUI()
    {
        flixReplacer = target as MFlixReplacer;

        serializedObject.Update();

        EditorGUILayout.Space(15f);
        DrawDividerLine(Color.grey);
        GUILayout.BeginHorizontal();
        //GUILayout.Box((Texture)AssetDatabase.LoadAssetAtPath("Assets/UI/mj_icon_small2.png", typeof(Texture)));
        //EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), (Texture)AssetDatabase.LoadAssetAtPath("Assets/UI/mj_icon_small2.png", typeof(Texture)));
        DrawTitle("Maujflix Utilities", 15);
        GUILayout.EndHorizontal();
        DrawDividerLine(Color.grey);
        EditorGUILayout.Space(10f);

        CheckForNulls();

        //DrawCenteredLabel("Episode Flowchart");
        GUIContent flowchartContent = new GUIContent("Flowchart", "Add Flowchart");
        EditorGUILayout.PropertyField(episodeFlowchartSerialized, flowchartContent, GUILayout.ExpandHeight(false));

        //DrawCenteredLabel("Replacement Options");
        GUIContent whatToReplaceContent = new GUIContent("What To Replace", "Select which mode to replace objects");
        EditorGUILayout.PropertyField(whatToReplaceSerialized, whatToReplaceContent, GUILayout.ExpandHeight(false));

        EditorGUILayout.Space(10f);

        DrawDividerLine(Color.grey);

        EditorGUILayout.Space(10f);

        switch (flixReplacer.whatToReplace)
        {
            case MFlixReplacer.WhatToReplace.TutorialNarrativeDialogue:

                GUIContent tutorialDestroyOriginalContent = new GUIContent("Destroy Original", "Destroy Original Tutorial Dialogue from Scene");
                EditorGUILayout.PropertyField(tutorialDestroyOriginalSerialized, tutorialDestroyOriginalContent, GUILayout.ExpandHeight(false));

                GUIContent tutorialDialogueSceneContent = new GUIContent("Tutorial Dialogue Scene", "Tutorial Dialogue from Scene");
                EditorGUILayout.PropertyField(tutorialDialogueInSceneSerialized, tutorialDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent tutorialDialoguePrefabContent = new GUIContent("Tutorial Dialogue Prefab", "Tutorial Dialogue Prefab");
                EditorGUILayout.PropertyField(tutorialDialoguePrefabSerialized, tutorialDialoguePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                /*if (GUILayout.Button("Replace Narrative Dialogue"))
                {
                    Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                    flixReplacer.ReplaceNarrativeDialogue();
                }*/

                if (flixReplacer.tutorialDialogueInScene != null && flixReplacer.tutorialDialoguePrefab != null && flixReplacer.episodeFlowchart != null)
                {
                    if (DrawButtonColored("Replace Tutorial Dialogue".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Tutorial Replace");
                        flixReplacer.ReplaceTutorialDialogue();
                    }
                }

                break;

            case MFlixReplacer.WhatToReplace.NarrativeDialogue:

                //EditorGUILayout.Toggle("Destroy Original", flixReplacer.destroyOriginalNarrativeDialogue);
                GUIContent narrativeDestroyOriginalContent = new GUIContent("Destroy Original", "Destroy Original Narrative Dialogue from Scene");
                EditorGUILayout.PropertyField(narrativeDestroyOriginalSerialized, narrativeDestroyOriginalContent, GUILayout.ExpandHeight(false));

                GUIContent narrativeDialogueSceneContent = new GUIContent("Narrative Dialogue Scene", "Narrative Dialogue from Scene");
                EditorGUILayout.PropertyField(narrativeDialogueInSceneSerialized, narrativeDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent narrativeDialoguePrefabContent = new GUIContent("Narrative Dialogue Prefab", "Narrative Dialogue Prefab");
                EditorGUILayout.PropertyField(narrativeDialoguePrefabSerialized, narrativeDialoguePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                /*if (GUILayout.Button("Replace Narrative Dialogue"))
                {
                    Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                    flixReplacer.ReplaceNarrativeDialogue();
                }*/

                if (flixReplacer.narrativeDialogueInScene != null && flixReplacer.narrativeDialoguePrefab != null && flixReplacer.episodeFlowchart != null)
                {
                    if (DrawButtonColored("Replace Narrative Dialogue".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Narrative Replace");
                        flixReplacer.ReplaceNarrativeDialogue();                        
                    }
                }

                break;

            case MFlixReplacer.WhatToReplace.SayDialogue:

                GUIContent sayDialogueDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Say Dialogue from Scene");
                EditorGUILayout.PropertyField(sayDialogueDestroyOriginalSerialized, sayDialogueDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialogueSceneContent = new GUIContent("Say Dialogue Scene", "Say Dialogue from Scene");
                EditorGUILayout.PropertyField(sayDialogueInSceneSerialized, sayDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialoguePrefabContent = new GUIContent("Say Dialogue Prefab", "Say Dialogue Prefab");
                EditorGUILayout.PropertyField(sayDialoguePrefabSerialized, sayDialoguePrefabContent, GUILayout.ExpandHeight(false));

                if (flixReplacer.sayDialogueInScene != null && flixReplacer.sayDialoguePrefab != null && flixReplacer.episodeFlowchart != null)
                {
                    if (DrawButtonColored("Replace Say Dialogue".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Say Replace");
                        flixReplacer.ReplaceSayDialogue();
                    }
                }

                break;

            case MFlixReplacer.WhatToReplace.EpisodeEndScreen:
                break;
            case MFlixReplacer.WhatToReplace.StoryEndScreen:
                break;
            case MFlixReplacer.WhatToReplace.StoryBranchEndScreen:
                break;
        }

        //GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void CheckForNulls()
    {
        if(flixReplacer.episodeFlowchart == null)
            EditorGUILayout.HelpBox("Flowchart field is Empty!", MessageType.Error);
    }

    private void DrawNormalLabel(string labelName)
    {
        GUILayout.Label(labelName, GUILayout.Width(150f));
    }

    private void DrawTitle(string _Title, int _titleFontSize = 12)
    {
        GUIStyle replacementOptionsStyle = new GUIStyle(GUI.skin.button);
        replacementOptionsStyle.alignment = TextAnchor.MiddleCenter;
        replacementOptionsStyle.fontSize = _titleFontSize;
        replacementOptionsStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(_Title, replacementOptionsStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(25f));
    }

    private void DrawCenteredLabel(string labelName)
    {
        GUIStyle replacementOptionsStyle = new GUIStyle(GUI.skin.label);
        replacementOptionsStyle.alignment = TextAnchor.MiddleCenter;
        //replacementOptionsStyle.fontSize = 12;
        replacementOptionsStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(labelName, replacementOptionsStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
    }

    private void DrawDividerLine(Color color)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 2;

        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }

    private bool DrawButtonColored(string buttonLabel, string buttonHexColor, Color buttonTextColor)
    {
        Color buttonColor;
        ColorUtility.TryParseHtmlString(buttonHexColor, out buttonColor);        

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 13;
        buttonStyle.normal.textColor = buttonTextColor;
        buttonStyle.hover.textColor = buttonTextColor;

        var color = GUI.color;
        GUI.backgroundColor = buttonColor;
        return GUILayout.Button(buttonLabel, buttonStyle, GUILayout.Height(30f));        
    }
}
#endif