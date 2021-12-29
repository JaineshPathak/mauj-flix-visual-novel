using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif

#if UNITY_EDITOR
public class MFlixReplacer : MonoBehaviour
{
    public enum WhatToReplace
    {
        TutorialNarrativeDialogue,
        NarrativeDialogue,
        SayDialogue,
        EpisodeEndScreen,
        StoryEndScreen,
        StoryBranchEndScreen
    };

    public WhatToReplace whatToReplace;
    public Flowchart episodeFlowchart;

    public bool destroyOriginalTutorialDialogue;
    public SayDialog tutorialDialogueInScene;
    public SayDialog tutorialDialoguePrefab;

    public bool destroyOriginalNarrativeDialogue;
    public SayDialog narrativeDialogueInScene;
    public SayDialog narrativeDialoguePrefab;

    public bool destroyOriginalSayDialogue;
    public SayDialog sayDialogueInScene;
    public SayDialog sayDialoguePrefab;

    public void ReplaceTutorialDialogue()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || tutorialDialogueInScene == null || tutorialDialoguePrefab == null)
            return;

        int siblingIndexScene = tutorialDialogueInScene.transform.GetSiblingIndex();

        //SayDialog prefabNarrativeSay = Instantiate(narrativeDialoguePrefab, transform) as SayDialog;
        SayDialog tutorialSayInstance = PrefabUtility.InstantiatePrefab(tutorialDialoguePrefab, transform) as SayDialog;
        tutorialSayInstance.transform.name += "(NEW)";
        tutorialSayInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        tutorialSayInstance.gameObject.SetActive(tutorialDialogueInScene.gameObject.activeSelf);


        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == tutorialDialogueInScene && say._Character == null)
            {
                say.setSayDialog = tutorialSayInstance;
                PrefabUtility.RecordPrefabInstancePropertyModifications(say);
            }
        }

        if (destroyOriginalNarrativeDialogue)
        {
            //PrefabUtility.RevertAddedGameObject(narrativeDialogueInScene.gameObject, InteractionMode.UserAction);
            //PrefabUtility.RevertPrefabInstance(narrativeDialogueInScene.gameObject, InteractionMode.UserAction);            
            SceneVisibilityManager.DestroyImmediate(tutorialDialogueInScene.gameObject);
            tutorialDialogueInScene = null;
            //DestroyImmediate(narrativeDialogueInScene);
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(episodeFlowchart);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }

    public void ReplaceNarrativeDialogue()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || narrativeDialogueInScene == null || narrativeDialoguePrefab == null)
            return;

        int siblingIndexScene = narrativeDialogueInScene.transform.GetSiblingIndex();

        //SayDialog prefabNarrativeSay = Instantiate(narrativeDialoguePrefab, transform) as SayDialog;
        SayDialog prefabNarrativeSay = PrefabUtility.InstantiatePrefab(narrativeDialoguePrefab, transform) as SayDialog;
        prefabNarrativeSay.transform.name += "(NEW)";
        prefabNarrativeSay.transform.SetSiblingIndex(siblingIndexScene + 1);
        prefabNarrativeSay.gameObject.SetActive(narrativeDialogueInScene.gameObject.activeSelf);


        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == narrativeDialogueInScene && say._Character == null)
            {
                say.setSayDialog = prefabNarrativeSay;
                PrefabUtility.RecordPrefabInstancePropertyModifications(say);
            }
        }

        if (destroyOriginalNarrativeDialogue)
        {
            //PrefabUtility.RevertAddedGameObject(narrativeDialogueInScene.gameObject, InteractionMode.UserAction);
            //PrefabUtility.RevertPrefabInstance(narrativeDialogueInScene.gameObject, InteractionMode.UserAction);            
            SceneVisibilityManager.DestroyImmediate(narrativeDialogueInScene.gameObject);
            narrativeDialogueInScene = null;
            //DestroyImmediate(narrativeDialogueInScene);
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(episodeFlowchart);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }
    
    public void ReplaceSayDialogue()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || sayDialogueInScene == null || sayDialoguePrefab == null)
            return;

        int siblingIndexScene = sayDialogueInScene.transform.GetSiblingIndex();

        SayDialog prefabSayInstance = PrefabUtility.InstantiatePrefab(sayDialoguePrefab, transform) as SayDialog;
        prefabSayInstance.transform.name += "(NEW)";
        prefabSayInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        prefabSayInstance.gameObject.SetActive(sayDialogueInScene.gameObject.activeSelf);

        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == sayDialogueInScene)
            {
                say.setSayDialog = prefabSayInstance;
                PrefabUtility.RecordPrefabInstancePropertyModifications(say);
            }
        }

        if (destroyOriginalNarrativeDialogue)
        {
            SceneVisibilityManager.DestroyImmediate(sayDialogueInScene.gameObject);
            sayDialogueInScene = null;           
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(episodeFlowchart);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }
}
#endif