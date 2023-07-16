using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(MFlixUtils)), CanEditMultipleObjects]
public class MFlixUtilsEditor : Editor
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

    //Narrative Black Dialogue Mode
    private SerializedProperty narrativeDestroyBlackOriginalSerialized;
    private SerializedProperty narrativeBlackDialogueInSceneSerialized;
    private SerializedProperty narrativeBlackDialoguePrefabSerialized;

    //Say Dialogue Mode
    private SerializedProperty sayDialogueDestroyOriginalSerialized;
    private SerializedProperty sayDialogueInSceneSerialized;
    private SerializedProperty sayDialoguePrefabSerialized;

    //Say Dialogue Replace In Scene
    private SerializedProperty sayDialogueDestroyOldSerialized;
    private SerializedProperty sayDialogueSceneOldSerialized;
    private SerializedProperty sayDialogueSceneNewSerialized;

    //Menu Dialog Mode
    private SerializedProperty menuDialogueDestroyOriginalSerialized;
    private SerializedProperty menuDialogueInSceneSerialized;
    private SerializedProperty menuDialoguePrefabSerialized;

    //Menu Commands Update
    private SerializedProperty destroyOldMenuCommandsSerialized;

    //End Episode Screen Mode
    private SerializedProperty episodeEndDestroyOriginalSerialized;
    private SerializedProperty episodeEndInSceneSerialized;
    private SerializedProperty episodeEndPrefabSerialized;

    //Story End Screen
    private SerializedProperty storyByMaujflixPrefabSerialized;
    private SerializedProperty storyEndScreenPrefabSerialized;

    //Branch End Screen
    private SerializedProperty storyBranchEndScreenPrefabSerialized;

    //Ask No Yes Popup
    private SerializedProperty askYesNoPopupDestroyOriginalSerialized;
    private SerializedProperty askYesNoPopupInSceneSerialized;
    private SerializedProperty askYesNoPopupPrefabSerialized;

    //Character Selection Screen
    private SerializedProperty destroyOriginalCharSelectionScreenSerialized;
    private SerializedProperty copyFromOriginalCharScreenSerialized;
    private SerializedProperty charSelectionScreenInSceneSerialized;
    private SerializedProperty charSelectionScreenPrefabSerialized;

    //Character Name Screen
    private SerializedProperty destoryOriginalCharNameScreenSerialized;
    private SerializedProperty charNameScreenInSceneSerialized;
    private SerializedProperty charNameScreenPrefabSerialized;

    //Play Sound
    private SerializedProperty originalSoundSerialized;
    private SerializedProperty newSoundSerialized;

    //Call Mode
    private SerializedProperty callModeNewSerialized;

    //Say Commands Say Dialogue
    private SerializedProperty sayDialogueReplaceSerialized;

    //Say Commands MFlix Update
    private SerializedProperty newDialogStyleSerialized;

    private MFlixUtils flixReplacer;

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

        narrativeDestroyBlackOriginalSerialized = serializedObject.FindProperty("destroyOriginalNarrativeBlackDialogue");
        narrativeBlackDialogueInSceneSerialized = serializedObject.FindProperty("narrativeBlackDialogueInScene");
        narrativeBlackDialoguePrefabSerialized = serializedObject.FindProperty("narrativeBlackDialoguePrefab");

        sayDialogueDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalSayDialogue");
        sayDialogueInSceneSerialized = serializedObject.FindProperty("sayDialogueInScene");
        sayDialoguePrefabSerialized = serializedObject.FindProperty("sayDialoguePrefab");

        sayDialogueDestroyOldSerialized = serializedObject.FindProperty("destroyOldSayDialogue");
        sayDialogueSceneOldSerialized = serializedObject.FindProperty("sayDialogueInSceneOld");
        sayDialogueSceneNewSerialized = serializedObject.FindProperty("sayDialogueInSceneNew");

        menuDialogueDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalMenuDialogue");
        menuDialogueInSceneSerialized = serializedObject.FindProperty("menuDialogueInScene");
        menuDialoguePrefabSerialized = serializedObject.FindProperty("menuDialoguePrefab");

        destroyOldMenuCommandsSerialized = serializedObject.FindProperty("destroyOldMenuCommands");

        episodeEndDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalEndEpisodeScreen");
        episodeEndInSceneSerialized = serializedObject.FindProperty("endEpisodeScreenInScene");
        episodeEndPrefabSerialized = serializedObject.FindProperty("endEpisodeScreenPrefab");

        storyByMaujflixPrefabSerialized = serializedObject.FindProperty("storyByMaujflixPrefab");

        storyEndScreenPrefabSerialized = serializedObject.FindProperty("storyEndScreenPrefab");

        storyBranchEndScreenPrefabSerialized = serializedObject.FindProperty("storyBranchEndScreenPrefab");

        askYesNoPopupDestroyOriginalSerialized = serializedObject.FindProperty("destroyOriginalAskYesPopupScreen");
        askYesNoPopupInSceneSerialized = serializedObject.FindProperty("askYesNoPopupInScene");
        askYesNoPopupPrefabSerialized = serializedObject.FindProperty("askYesNoPopupPrefab");

        destroyOriginalCharSelectionScreenSerialized = serializedObject.FindProperty("destroyOriginalCharSelectionScreen");
        copyFromOriginalCharScreenSerialized = serializedObject.FindProperty("copyFromOriginalCharScreen");
        charSelectionScreenInSceneSerialized = serializedObject.FindProperty("charSelectionScreenInScene");
        charSelectionScreenPrefabSerialized = serializedObject.FindProperty("charSelectionScreenPrefab");

        destoryOriginalCharNameScreenSerialized = serializedObject.FindProperty("destoryOriginalCharNameScreen");
        charNameScreenInSceneSerialized = serializedObject.FindProperty("charNameScreenInScene");
        charNameScreenPrefabSerialized = serializedObject.FindProperty("charNameScreenPrefab");

        originalSoundSerialized = serializedObject.FindProperty("originalSound");
        newSoundSerialized = serializedObject.FindProperty("newSound");

        callModeNewSerialized = serializedObject.FindProperty("callModeNew");

        sayDialogueReplaceSerialized = serializedObject.FindProperty("sayDialogueReplace");

        newDialogStyleSerialized = serializedObject.FindProperty("newDialogStyle");
    }

    public override void OnInspectorGUI()
    {
        flixReplacer = target as MFlixUtils;

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
            case MFlixUtils.WhatToReplace.TutorialNarrativeDialogue:

                GUIContent tutorialDestroyOriginalContent = new GUIContent("Destroy Original", "Destroy Original Tutorial Dialogue from Prefab");
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

            case MFlixUtils.WhatToReplace.NarrativeDialogue:

                //EditorGUILayout.Toggle("Destroy Original", flixReplacer.destroyOriginalNarrativeDialogue);
                GUIContent narrativeDestroyOriginalContent = new GUIContent("Destroy Original", "Destroy Original Narrative Dialogue from Prefab");
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

            case MFlixUtils.WhatToReplace.NarrativeBlackDialogue:

                //EditorGUILayout.Toggle("Destroy Original", flixReplacer.destroyOriginalNarrativeDialogue);
                GUIContent narrativeBlackDestroyOriginalContent = new GUIContent("Destroy Original", "Destroy Original Narrative Black Dialogue from Episode Prefab");
                EditorGUILayout.PropertyField(narrativeDestroyBlackOriginalSerialized, narrativeBlackDestroyOriginalContent, GUILayout.ExpandHeight(false));

                GUIContent narrativeBlackDialogueSceneContent = new GUIContent("Narrative Black Dialogue Scene", "Narrative Black Dialogue from Scene");
                EditorGUILayout.PropertyField(narrativeBlackDialogueInSceneSerialized, narrativeBlackDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent narrativeBlackDialoguePrefabContent = new GUIContent("Narrative Black Dialogue Prefab", "Narrative Black Dialogue Prefab");
                EditorGUILayout.PropertyField(narrativeBlackDialoguePrefabSerialized, narrativeBlackDialoguePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                /*if (GUILayout.Button("Replace Narrative Dialogue"))
                {
                    Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                    flixReplacer.ReplaceNarrativeDialogue();
                }*/

                if (flixReplacer.narrativeBlackDialogueInScene != null && flixReplacer.narrativeBlackDialoguePrefab != null && flixReplacer.episodeFlowchart != null)
                {
                    if (DrawButtonColored("Replace Narrative Black Dialogue".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Narrative Black Replace");
                        flixReplacer.ReplaceNarrativeBlackDialogue();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.SayDialogue:

                GUIContent sayDialogueDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Say Dialogue from Prefab");
                EditorGUILayout.PropertyField(sayDialogueDestroyOriginalSerialized, sayDialogueDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialogueSceneContent = new GUIContent("Say Dialogue Scene", "Say Dialogue from Scene");
                EditorGUILayout.PropertyField(sayDialogueInSceneSerialized, sayDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialoguePrefabContent = new GUIContent("Say Dialogue Prefab", "Say Dialogue Prefab");
                EditorGUILayout.PropertyField(sayDialoguePrefabSerialized, sayDialoguePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

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

            case MFlixUtils.WhatToReplace.SayDialogueReplace:

                GUIContent sayDialogueDestroyOldContent = new GUIContent("Destroy Old", "Destroy Old Say Dialogue from Prefab");
                EditorGUILayout.PropertyField(sayDialogueDestroyOldSerialized, sayDialogueDestroyOldContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialogueSceneOldContent = new GUIContent("Say Dialogue Old", "Say Dialogue Old from Scene");
                EditorGUILayout.PropertyField(sayDialogueSceneOldSerialized, sayDialogueSceneOldContent, GUILayout.ExpandHeight(false));

                GUIContent sayDialogueNewContent = new GUIContent("Say Dialogue New", "Say Dialogue New from Scene");
                EditorGUILayout.PropertyField(sayDialogueSceneNewSerialized, sayDialogueNewContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.sayDialogueInSceneOld != null && flixReplacer.sayDialogueInSceneNew != null)
                {
                    if (DrawButtonColored("Replace Say Dialogue Here".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Say Replace Here");
                        flixReplacer.ReplaceSayDialogueScene();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.MenuDialogue:

                GUIContent menuDialogueDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Menu Dialogue from Prefab");
                EditorGUILayout.PropertyField(menuDialogueDestroyOriginalSerialized, menuDialogueDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent menuDialogueSceneContent = new GUIContent("Menu Dialogue Scene", "Menu Dialogue from Scene");
                EditorGUILayout.PropertyField(menuDialogueInSceneSerialized, menuDialogueSceneContent, GUILayout.ExpandHeight(false));

                GUIContent menuDialoguePrefabContent = new GUIContent("Menu Dialogue Prefab", "Menu Dialogue Prefab");
                EditorGUILayout.PropertyField(menuDialoguePrefabSerialized, menuDialoguePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.menuDialogueInScene != null && flixReplacer.menuDialoguePrefab != null && flixReplacer.episodeFlowchart != null)
                {
                    if (DrawButtonColored("Replace Menu Dialogue".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Menu Replace");
                        flixReplacer.ReplaceMenuDialogue();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.MenuCommandUpdate:
                
                if(flixReplacer.episodeFlowchart != null)
                {
                    GUIContent menuCommandsContent = new GUIContent("Destroy Old Menu Commands", "Destroy Old Menu Commands from Flowchart");
                    EditorGUILayout.PropertyField(destroyOldMenuCommandsSerialized, menuCommandsContent, GUILayout.ExpandHeight(false));

                    EditorGUILayout.Space(10f);

                    if (DrawButtonColored("Update Menu Commands".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Menu Update");
                        flixReplacer.UpdateMenuCommands();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.EpisodeEndScreen:

                GUIContent endEpisodeDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original End Episode Screen from Prefab");
                EditorGUILayout.PropertyField(episodeEndDestroyOriginalSerialized, endEpisodeDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent endEpisodeSceneContent = new GUIContent("End Episode Screen Scene", "End Episode Screen from Scene");
                EditorGUILayout.PropertyField(episodeEndInSceneSerialized, endEpisodeSceneContent, GUILayout.ExpandHeight(false));

                GUIContent endEpisodePrefabContent = new GUIContent("End Episode Screen Prefab", "End Episode Screen Prefab");
                EditorGUILayout.PropertyField(episodeEndPrefabSerialized, endEpisodePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.endEpisodeScreenInScene != null && flixReplacer.endEpisodeScreenPrefab != null)
                {
                    if (DrawButtonColored("Replace End Episode Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer End Episode Screen");
                        flixReplacer.ReplaceEndEpisodeScreen();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.StoryEndScreen:

                GUIContent storyByMaujflixContent = new GUIContent("Story By Maujflix Prefab", "Story By Maujflix Prefab");
                EditorGUILayout.PropertyField(storyByMaujflixPrefabSerialized, storyByMaujflixContent, GUILayout.ExpandHeight(false));

                GUIContent endStoryScreenContent = new GUIContent("Story End Screen Prefab", "Story End Screen Prefab");
                EditorGUILayout.PropertyField(storyEndScreenPrefabSerialized, endStoryScreenContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.storyByMaujflixPrefab != null && flixReplacer.storyEndScreenPrefab != null)
                {
                    if (DrawButtonColored("Add Story End Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Story End Screen");
                        flixReplacer.AddStoryEndScreen();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.StoryBranchEndScreen:

                GUIContent storyByMaujflixContent2 = new GUIContent("Story By Maujflix Prefab", "Story By Maujflix Prefab");
                EditorGUILayout.PropertyField(storyByMaujflixPrefabSerialized, storyByMaujflixContent2, GUILayout.ExpandHeight(false));

                GUIContent branchEndStoryScreenContent = new GUIContent("Story Branch End Screen Prefab", "Story End Screen Prefab");
                EditorGUILayout.PropertyField(storyBranchEndScreenPrefabSerialized, branchEndStoryScreenContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.storyByMaujflixPrefab != null && flixReplacer.storyBranchEndScreenPrefab != null)
                {
                    if (DrawButtonColored("Add Story Branch End Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Story Branch End Screen");
                        flixReplacer.AddStoryBranchEndScreen();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.AskYesNoPopupScreen:

                GUIContent askNoPopupDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Ask/No Popup from Prefab");
                EditorGUILayout.PropertyField(askYesNoPopupDestroyOriginalSerialized, askNoPopupDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent askNoPopupSceneContent = new GUIContent("Ask/No Popup In Scene", "Ask/No Popup from Scene");
                EditorGUILayout.PropertyField(askYesNoPopupInSceneSerialized, askNoPopupSceneContent, GUILayout.ExpandHeight(false));

                GUIContent askNoPopupPrefabContent = new GUIContent("Ask/No Popup Prefab", "Ask/No Popup Prefab");
                EditorGUILayout.PropertyField(askYesNoPopupPrefabSerialized, askNoPopupPrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.askYesNoPopupInScene != null && flixReplacer.askYesNoPopupPrefab != null)
                {
                    if (DrawButtonColored("Replace Ask/No Popup Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Ask/No Popup Screen");
                        flixReplacer.ReplaceYesNoPopupScreen();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.CharacterSelectionScreen:

                GUIContent charSelectDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Character Selection Screen from Prefab");
                EditorGUILayout.PropertyField(destroyOriginalCharSelectionScreenSerialized, charSelectDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent charSelectCopyContent = new GUIContent("Copy Original Component", "Copy Original Component and Paste it to new ones");
                EditorGUILayout.PropertyField(copyFromOriginalCharScreenSerialized, charSelectCopyContent, GUILayout.ExpandHeight(false));

                GUIContent charSelectSceneContent = new GUIContent("Char Selection In Scene", "Character Selection Screen from Scene");
                EditorGUILayout.PropertyField(charSelectionScreenInSceneSerialized, charSelectSceneContent, GUILayout.ExpandHeight(false));

                GUIContent charSelectPrefabContent = new GUIContent("Char Selection Prefab", "Character Selection Screen Prefab");
                EditorGUILayout.PropertyField(charSelectionScreenPrefabSerialized, charSelectPrefabContent, GUILayout.ExpandHeight(false));                

                EditorGUILayout.Space(10f);

                if (flixReplacer.charSelectionScreenInScene != null && flixReplacer.charSelectionScreenPrefab != null)
                {
                    if (DrawButtonColored("Replace Char Selection Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Replace Character Screen");
                        flixReplacer.ReplaceCharacterSelectionScreen();                        
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.CharacterNameDialogueScreen:

                GUIContent charNameDestroySceneContent = new GUIContent("Destroy Original", "Destroy Original Character Name Screen from Prefab");
                EditorGUILayout.PropertyField(destoryOriginalCharNameScreenSerialized, charNameDestroySceneContent, GUILayout.ExpandHeight(false));

                GUIContent charNameSceneContent = new GUIContent("Char Name In Scene", "Character Name Screen from Scene");
                EditorGUILayout.PropertyField(charNameScreenInSceneSerialized, charNameSceneContent, GUILayout.ExpandHeight(false));

                GUIContent charNamePrefabContent = new GUIContent("Char Name Prefab", "Character Name Screen Prefab");
                EditorGUILayout.PropertyField(charNameScreenPrefabSerialized, charNamePrefabContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.charNameScreenInScene != null && flixReplacer.charNameScreenPrefab != null)
                {
                    if (DrawButtonColored("Replace Char Name Screen".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Replace Character Name");
                        flixReplacer.ReplaceCharacterNameScreen();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.ReplacePlaySound:

                EditorGUILayout.HelpBox("Total PlaySound Commands Found: " + flixReplacer.GetPlaySoundCount(), MessageType.Info);
                List<AudioClip> soundsCommandList = flixReplacer.GetPlaySoundDetails();
                if(soundsCommandList.Count > 0)
                {
                    for (int i = 0; i < soundsCommandList.Count; i++)
                    {
                        EditorGUILayout.HelpBox("Sound Clips Used: " + soundsCommandList[i].name, MessageType.Info);
                    }
                }

                EditorGUILayout.Space(10f);

                GUIContent originalSoundContent = new GUIContent("Original Sound", "Original Sound");
                EditorGUILayout.PropertyField(originalSoundSerialized, originalSoundContent, GUILayout.ExpandHeight(false));

                GUIContent newSoundContent = new GUIContent("New Sound", "New Sound which will replace original sound");
                EditorGUILayout.PropertyField(newSoundSerialized, newSoundContent, GUILayout.ExpandHeight(false));

                EditorGUILayout.Space(10f);

                if (flixReplacer.originalSound != null && flixReplacer.newSound != null)
                {
                    if (DrawButtonColored("Replace Sound".ToUpper(), "#3c8b50", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Replace Sound");
                        flixReplacer.ReplaceSoundClip();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.ReplaceSayCommands:

                if (flixReplacer.episodeFlowchart != null)
                {
                    EditorGUILayout.HelpBox("This will update the default Say Commands to Maujflix modified ones.\n NOTE: Be careful in this step and check properly!", MessageType.Warning);

                    EditorGUILayout.Space(10f);

                    if (DrawButtonColored("Update Say Commands".ToUpper(), "#FF0000", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Say Update");
                        flixReplacer.ReplaceSayCommands();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.UpdateCallCommands:

                if (flixReplacer.episodeFlowchart != null)
                {
                    EditorGUILayout.HelpBox("This will set all the 'Call' commands CallMode to desired CallMode given below.", MessageType.Warning);

                    EditorGUILayout.Space(10f);

                    GUIContent callModeContent = new GUIContent("Call Mode (New)", "New Call Mode to all 'Call' Commands");
                    EditorGUILayout.PropertyField(callModeNewSerialized, callModeContent, GUILayout.ExpandHeight(false));

                    EditorGUILayout.Space(10f);

                    if (DrawButtonColored("Update Call Commands".ToUpper(), "#FF0000", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Call Update");
                        flixReplacer.UpdateCallCommands();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.SayCommandDialogueReplace:

                if (flixReplacer.episodeFlowchart != null)
                {
                    GUIContent sayDialogueReplaceContent = new GUIContent("Say Dialogue In Scene", "Character Say Dialogue From Scene");
                    EditorGUILayout.PropertyField(sayDialogueReplaceSerialized, sayDialogueReplaceContent, GUILayout.ExpandHeight(false));

                    EditorGUILayout.Space(10f);

                    if (DrawButtonColored("Update Say Commands".ToUpper(), "#FF0000", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer Say Replace Update");
                        flixReplacer.ReplaceSayCommandsSayDialogue();
                    }
                }

                break;

            case MFlixUtils.WhatToReplace.UpdateSayCommandMflixCharacter:

                if (flixReplacer.episodeFlowchart != null)
                {
                    EditorGUILayout.HelpBox("This will set all the 'SayMFlix' commands SayMode to desired SayMode given below.", MessageType.Warning);

                    EditorGUILayout.Space(10f);

                    GUIContent sayModeContent = new GUIContent("Dialogue Mode (New) Character", "New Say Mode to all 'SayMFlix' (Cyan) Commands");
                    EditorGUILayout.PropertyField(newDialogStyleSerialized, sayModeContent, GUILayout.ExpandHeight(false));

                    EditorGUILayout.Space(10f);

                    if (DrawButtonColored("Update SayMFlix Commands".ToUpper(), "#FF0000", Color.white))
                    {
                        //Undo.RecordObject(target, "FlixReplacer Narrative Replace");
                        Undo.RegisterCompleteObjectUndo(target, "FlixReplacer SayMflix Update");
                        flixReplacer.UpdateSayMflixCommand();
                    }
                }

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
        buttonStyle.focused.textColor = Color.gray;

        var color = GUI.color;
        GUI.backgroundColor = buttonColor;
        return GUILayout.Button(buttonLabel, buttonStyle, GUILayout.Height(30f));        
    }
}
#endif