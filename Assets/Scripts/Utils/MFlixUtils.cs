using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif

#if UNITY_EDITOR
public class MFlixUtils : MonoBehaviour
{
    public enum WhatToReplace
    {
        TutorialNarrativeDialogue,
        NarrativeDialogue,
        SayDialogue,
        MenuDialogue,
        PickNameDialogue,
        EpisodeEndScreen,
        StoryEndScreen,
        StoryBranchEndScreen,
        AskYesNoPopupScreen
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

    public bool destroyOriginalMenuDialogue;
    public MenuDialog menuDialogueInScene;
    public MenuDialog menuDialoguePrefab;

    public bool destroyOriginalEndEpisodeScreen;
    public GameObject endEpisodeScreenInScene;
    public GameObject endEpisodeScreenPrefab;

    public GameObject storyByMaujflixPrefab;
    public GameObject storyEndScreenPrefab;

    public GameObject storyBranchEndScreenPrefab;

    public bool destroyOriginalAskYesPopupScreen;
    public GameObject askYesNoPopupInScene;
    public GameObject askYesNoPopupPrefab;

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

        if (destroyOriginalTutorialDialogue)
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

        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabSayInstance);

        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == sayDialogueInScene)
            {
                say.setSayDialog = prefabSayInstance;

                if (say._Character != null && say.Portrait != null)
                    say.Portrait = null;

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

    public void ReplaceMenuDialogue()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || menuDialogueInScene == null || menuDialoguePrefab == null)
            return;

        int siblingIndexScene = menuDialogueInScene.transform.GetSiblingIndex();

        MenuDialog menuDialogInstance = PrefabUtility.InstantiatePrefab(menuDialoguePrefab, transform) as MenuDialog;
        menuDialogInstance.transform.name += "(NEW)";
        menuDialogInstance.transform.SetSiblingIndex(siblingIndexScene + 1);

        PrefabUtility.RecordPrefabInstancePropertyModifications(menuDialogInstance);

        foreach(Fungus.Menu menuItem in episodeFlowchart.GetComponentsInChildren<Fungus.Menu>())
        {
            if(menuItem.SetMenuDialog == menuDialogueInScene || menuItem.SetMenuDialog == null)
            {
                menuItem.SetMenuDialog = menuDialogInstance;

                PrefabUtility.RecordPrefabInstancePropertyModifications(menuItem);
            }
        }

        SayDialog sayDialogueInMenuOriginal = menuDialogueInScene.GetComponentInChildren<SayDialog>(); /*= GameObject.Find("MFlixCharacterSayDialogWithMenu").GetComponent<SayDialog>();*/        
        print("Original: " + sayDialogueInMenuOriginal + " (" + sayDialogueInMenuOriginal.transform.parent.parent.name + ")");

        SayDialog sayDialogueInMenuInstance = menuDialogInstance.GetComponentInChildren<SayDialog>(); /*= GameObject.Find("MFlixCharacterSayDialogWithMenu").GetComponent<SayDialog>();*/        
        print("New: " + sayDialogueInMenuInstance + " (" + sayDialogueInMenuInstance.transform.parent.parent.name + ")");

        if(sayDialogueInMenuOriginal != null && sayDialogueInMenuInstance != null)
        {
            foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
            {
                if(say.setSayDialog == sayDialogueInMenuOriginal)
                {
                    say.setSayDialog = sayDialogueInMenuInstance;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(say);
                }
            }
        }        

        if(destroyOriginalMenuDialogue)
        {
            SceneVisibilityManager.DestroyImmediate(menuDialogueInScene.gameObject);
            menuDialogueInScene = null;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(episodeFlowchart);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }
    
    public void ReplaceEndEpisodeScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || endEpisodeScreenInScene == null || endEpisodeScreenPrefab == null)
            return;

        int siblingIndexScene = endEpisodeScreenInScene.transform.GetSiblingIndex();

        GameObject endEpisodeInstance = PrefabUtility.InstantiatePrefab(endEpisodeScreenPrefab, transform) as GameObject;
        endEpisodeInstance.transform.name += "(NEW)";
        endEpisodeInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        endEpisodeInstance.gameObject.SetActive(endEpisodeScreenInScene.gameObject.activeSelf);

        PrefabUtility.RecordPrefabInstancePropertyModifications(endEpisodeInstance);

        if(destroyOriginalEndEpisodeScreen)
        {
            SceneVisibilityManager.DestroyImmediate(endEpisodeScreenInScene.gameObject);
            endEpisodeScreenInScene = null;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(endEpisodeInstance);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }

    public void AddStoryEndScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (storyEndScreenPrefab == null || storyByMaujflixPrefab == null)
            return;

        GameObject storyByMaujflixInstance = PrefabUtility.InstantiatePrefab(storyByMaujflixPrefab, transform) as GameObject;
        storyByMaujflixInstance.transform.name += "(NEW)";
        storyByMaujflixInstance.gameObject.SetActive(true);

        GameObject storyEndScreenInstance = PrefabUtility.InstantiatePrefab(storyEndScreenPrefab, transform) as GameObject;
        storyEndScreenInstance.transform.name += "(NEW)";
        storyEndScreenInstance.gameObject.SetActive(true);

        PrefabUtility.RecordPrefabInstancePropertyModifications(storyByMaujflixInstance);
        PrefabUtility.RecordPrefabInstancePropertyModifications(storyEndScreenInstance);
    }

    public void AddStoryBranchEndScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (storyBranchEndScreenPrefab == null || storyByMaujflixPrefab == null)
            return;

        GameObject storyByMaujflixInstance = PrefabUtility.InstantiatePrefab(storyByMaujflixPrefab, transform) as GameObject;
        storyByMaujflixInstance.transform.name += "(NEW)";
        storyByMaujflixInstance.gameObject.SetActive(true);

        GameObject storyBranchEndScreenInstance = PrefabUtility.InstantiatePrefab(storyBranchEndScreenPrefab, transform) as GameObject;
        storyBranchEndScreenInstance.transform.name += "(NEW)";
        storyBranchEndScreenInstance.gameObject.SetActive(true);

        PrefabUtility.RecordPrefabInstancePropertyModifications(storyByMaujflixInstance);
        PrefabUtility.RecordPrefabInstancePropertyModifications(storyBranchEndScreenInstance);
    }

    public void ReplaceYesNoPopupScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || askYesNoPopupInScene == null || askYesNoPopupPrefab == null)
            return;

        int siblingIndexScene = askYesNoPopupInScene.transform.GetSiblingIndex();

        GameObject askNoPopupInstance = PrefabUtility.InstantiatePrefab(askYesNoPopupPrefab, transform) as GameObject;
        askNoPopupInstance.transform.name += "(NEW)";
        askNoPopupInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        askNoPopupInstance.gameObject.SetActive(askYesNoPopupInScene.gameObject.activeSelf);

        PrefabUtility.RecordPrefabInstancePropertyModifications(askNoPopupInstance);

        if (destroyOriginalAskYesPopupScreen)
        {
            SceneVisibilityManager.DestroyImmediate(askYesNoPopupInScene.gameObject);
            askYesNoPopupInScene = null;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(askNoPopupInstance);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }
}
#endif