#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Experimental.SceneManagement;
using Unity.EditorCoroutines.Editor;
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
        MenuCommandUpdate,
        PickNameDialogue,
        EpisodeEndScreen,
        StoryEndScreen,
        StoryBranchEndScreen,
        AskYesNoPopupScreen,
        CharacterSelectionScreen,
        CharacterNameDialogueScreen
    };

    public WhatToReplace whatToReplace;
    public Flowchart episodeFlowchart;

    //Tutorial Dialogue Box
    public bool destroyOriginalTutorialDialogue;
    public SayDialog tutorialDialogueInScene;
    public SayDialog tutorialDialoguePrefab;

    //Narrative Dialogue Box
    public bool destroyOriginalNarrativeDialogue;
    public SayDialog narrativeDialogueInScene;
    public SayDialog narrativeDialoguePrefab;

    //Say Dialogue Box
    public bool destroyOriginalSayDialogue;
    public SayDialog sayDialogueInScene;
    public SayDialog sayDialoguePrefab;

    //Menu Dialogue Box
    public bool destroyOriginalMenuDialogue;
    public MenuDialog menuDialogueInScene;
    public MenuDialog menuDialoguePrefab;

    //Menu Commands Update
    public bool destroyOldMenuCommands;

    //Episode End Screen
    public bool destroyOriginalEndEpisodeScreen;
    public GameObject endEpisodeScreenInScene;
    public GameObject endEpisodeScreenPrefab;

    //Other End Screen Objects
    public GameObject storyByMaujflixPrefab;
    public GameObject storyEndScreenPrefab;

    public GameObject storyBranchEndScreenPrefab;

    //Ask Yes/No Screen
    public bool destroyOriginalAskYesPopupScreen;
    public GameObject askYesNoPopupInScene;
    public GameObject askYesNoPopupPrefab;

    //Character Selection Screen
    public bool destroyOriginalCharSelectionScreen;
    public bool copyFromOriginalCharScreen;
    public GameObject charSelectionScreenInScene;
    public GameObject charSelectionScreenPrefab;

    //Character Name Dialogue Screen
    public bool destoryOriginalCharNameScreen;    
    public GameObject charNameScreenInScene;
    public GameObject charNameScreenPrefab;

    private void OnValidate()
    {
        if (episodeFlowchart == null)
            episodeFlowchart = GetComponentInChildren<Flowchart>();
    }

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
        SayDialog prefabNarrativeSayInstance = PrefabUtility.InstantiatePrefab(narrativeDialoguePrefab, transform) as SayDialog;
        prefabNarrativeSayInstance.transform.name += "(NEW)";
        prefabNarrativeSayInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        prefabNarrativeSayInstance.gameObject.SetActive(narrativeDialogueInScene.gameObject.activeSelf);


        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == narrativeDialogueInScene && say._Character == null)
            {
                say.setSayDialog = prefabNarrativeSayInstance;
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

        SayDialog sayDialogueInstance = PrefabUtility.InstantiatePrefab(sayDialoguePrefab, transform) as SayDialog;
        sayDialogueInstance.transform.name += "(NEW)";
        sayDialogueInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        sayDialogueInstance.gameObject.SetActive(sayDialogueInScene.gameObject.activeSelf);

        PrefabUtility.RecordPrefabInstancePropertyModifications(sayDialogueInstance);

        foreach (Say say in episodeFlowchart.GetComponentsInChildren<Say>())
        {
            if (say.setSayDialog == sayDialogueInScene)
            {
                say.setSayDialog = sayDialogueInstance;

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
        //print("Original: " + sayDialogueInMenuOriginal + " (" + sayDialogueInMenuOriginal.transform.parent.parent.name + ")");

        SayDialog sayDialogueInMenuInstance = menuDialogInstance.GetComponentInChildren<SayDialog>(); /*= GameObject.Find("MFlixCharacterSayDialogWithMenu").GetComponent<SayDialog>();*/        
        //print("New: " + sayDialogueInMenuInstance + " (" + sayDialogueInMenuInstance.transform.parent.parent.name + ")");

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

        foreach(SetActive setActiveCommand in episodeFlowchart.GetComponentsInChildren<SetActive>())
        {
            if(setActiveCommand != null && setActiveCommand.TargetGameObject == menuDialogueInScene.gameObject)
            {
                setActiveCommand.TargetGameObject = menuDialogInstance.gameObject;
                PrefabUtility.RecordPrefabInstancePropertyModifications(setActiveCommand);
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

    public void UpdateMenuCommands()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null)
            return;

        /*foreach (Fungus.Block block in episodeFlowchart.GetComponentsInChildren<Fungus.Block>())
        {
            if (block != null)
            {
                
            }
        }*/

        foreach (Fungus.Menu menuCommand in episodeFlowchart.GetComponentsInChildren<Fungus.Menu>())
        {
            if (menuCommand != null && menuCommand.GetType() == typeof(Fungus.Menu))
            {
                Block block = menuCommand.ParentBlock;

                MenuMflix menuCommandv2 = Undo.AddComponent(block.gameObject, typeof(MenuMflix)) as MenuMflix;
                episodeFlowchart.AddSelectedCommand(menuCommandv2);
                menuCommandv2.ParentBlock = block;
                menuCommandv2.ItemId = episodeFlowchart.NextItemId();

                menuCommandv2.OnCommandAdded(block);

                block.CommandList.Insert(block.CommandList.Count, menuCommandv2);

                PrefabUtility.RecordPrefabInstancePropertyModifications(block);

                episodeFlowchart.ClearSelectedCommands();

                //Copy Data from old Menu Commands
                menuCommandv2.Text = menuCommand.Text;
                menuCommandv2.Description = menuCommand.Description;
                menuCommandv2.TargetBlock = menuCommand.TargetBlock;
                menuCommandv2.HideIfVisited = menuCommand.HideIfVisited;
                menuCommandv2.Interactable = menuCommand.Interactable;
                menuCommandv2.SetMenuDialog = menuCommand.SetMenuDialog;
                menuCommandv2.HideThisOption = menuCommand.HideThisOption;

                if(destroyOldMenuCommands)
                {
                    menuCommand.OnCommandRemoved(block);
                    Undo.DestroyObjectImmediate(menuCommand);

                    Undo.RecordObject(episodeFlowchart, "Delete Command");
                    episodeFlowchart.ClearSelectedCommands();
                }
            }
        }
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

    public void ReplaceCharacterSelectionScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || charSelectionScreenInScene == null || charSelectionScreenPrefab == null)
            return;

        int siblingIndexScene = charSelectionScreenInScene.transform.GetSiblingIndex();

        GameObject charSelectScreenInstance = PrefabUtility.InstantiatePrefab(charSelectionScreenPrefab, transform) as GameObject;
        charSelectScreenInstance.transform.name = charSelectionScreenInScene.transform.name + "(NEW)";
        charSelectScreenInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        charSelectScreenInstance.gameObject.SetActive(charSelectionScreenInScene.gameObject.activeSelf);

        PrefabUtility.RecordPrefabInstancePropertyModifications(charSelectScreenInstance);

        if(copyFromOriginalCharScreen)
        {
            ComponentUtility.CopyComponent(charSelectionScreenInScene.GetComponent<CharacterSelectionScreen>());

            if (charSelectScreenInstance.GetComponent<CharacterSelectionScreen>() != null)
            {
                ComponentUtility.PasteComponentValues(charSelectScreenInstance.GetComponent<CharacterSelectionScreen>());                

                //Revert a few properties which needs to be reverted. Had to use EditorCoroutine for this.
                EditorCoroutineUtility.StartCoroutineOwnerless(RevertFewPropertiesOfCharSelectScreen(charSelectScreenInstance));
            }
        }

        //"Set Active Command" in Flowchart if found any
        foreach(SetActive setActiveCommand in episodeFlowchart.GetComponentsInChildren<SetActive>())
        {
            if (setActiveCommand != null && setActiveCommand.enabled)
            {
                if (setActiveCommand.TargetGameObject == charSelectionScreenInScene)
                    setActiveCommand.TargetGameObject = charSelectScreenInstance;
            }
        }

        //"Fade Alpha Canvas Command" in Flowchart if found any
        foreach(FadeCanvasGroup fadeCanvasGroupCommand in episodeFlowchart.GetComponentsInChildren<FadeCanvasGroup>())
        {
            if (fadeCanvasGroupCommand != null && fadeCanvasGroupCommand.enabled)
            {
                if (fadeCanvasGroupCommand.TargetCanvasGroups.Contains(charSelectionScreenInScene.GetComponent<CanvasGroup>()))
                    fadeCanvasGroupCommand.UpdateItemAtIndex(charSelectScreenInstance.GetComponent<CanvasGroup>(), 0);
            }
        }        

        if (destroyOriginalCharSelectionScreen)
        {
            SceneVisibilityManager.DestroyImmediate(charSelectionScreenInScene.gameObject);
            charSelectionScreenInScene = null;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(charSelectScreenInstance);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }

    private IEnumerator RevertFewPropertiesOfCharSelectScreen(GameObject instancedObject)
    {
        yield return new EditorWaitForSeconds(0.1f);

        SerializedObject serializedCharScreen = new SerializedObject(instancedObject.GetComponent<CharacterSelectionScreen>());
        SerializedProperty contentProp = serializedCharScreen.FindProperty("characterSelectionContent");
        SerializedProperty diamondCostPanelProp = serializedCharScreen.FindProperty("diamondCostPanel");
        SerializedProperty diamondCostTextProp = serializedCharScreen.FindProperty("diamondCostText");
        SerializedProperty buttonShineAnimProp = serializedCharScreen.FindProperty("buttonShineAnim");

        PrefabUtility.RevertPropertyOverride(contentProp, InteractionMode.UserAction);
        PrefabUtility.RevertPropertyOverride(diamondCostPanelProp, InteractionMode.UserAction);
        PrefabUtility.RevertPropertyOverride(diamondCostTextProp, InteractionMode.UserAction);
        PrefabUtility.RevertPropertyOverride(buttonShineAnimProp, InteractionMode.UserAction);

        List<Image> originalImages = charSelectionScreenInScene.GetComponent<CharacterSelectionScreen>().characterSelectionContent.GetComponentsInChildren<Image>().ToList();
        List<Image> newImagesInstance = instancedObject.GetComponent<CharacterSelectionScreen>().characterSelectionContent.GetComponentsInChildren<Image>().ToList();

        for (int i = 0; i < originalImages.Count; i++)
            newImagesInstance[i].sprite = originalImages[i].sprite;

        Button charSelectButtonScene = charSelectionScreenInScene.GetComponent<CharacterSelectionScreen>().buttonShineAnim.GetComponent<Button>();
        Button charSelectButtonInstance = instancedObject.GetComponent<CharacterSelectionScreen>().buttonShineAnim.GetComponent<Button>();

        foreach (Block block in episodeFlowchart.GetComponentsInChildren<Block>())
        {
            if (block != null)
            {
                if (block._EventHandler != null)
                {
                    if (block._EventHandler.GetType() == typeof(Fungus.ButtonClicked))
                    {
                        ButtonClicked buttonClickedEH = block._EventHandler as ButtonClicked;

                        if (buttonClickedEH.TargetButton != null && buttonClickedEH.TargetButton == charSelectButtonScene)
                        {
                            buttonClickedEH.TargetButton = charSelectButtonInstance;
                            PrefabUtility.RecordPrefabInstancePropertyModifications(buttonClickedEH);
                        }
                    }
                }
            }
        }
    }

    public void ReplaceCharacterNameScreen()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError("MFlix Relacer: You need to be in Prefab Mode");
            return;
        }

        if (episodeFlowchart == null || charNameScreenInScene == null || charNameScreenPrefab == null)
            return;

        int siblingIndexScene = charNameScreenInScene.transform.GetSiblingIndex();

        GameObject charNameScreenInstance = PrefabUtility.InstantiatePrefab(charNameScreenPrefab, transform) as GameObject;
        charNameScreenInstance.transform.name = charNameScreenInScene.transform.name + "(NEW)";
        charNameScreenInstance.transform.SetSiblingIndex(siblingIndexScene + 1);
        charNameScreenInstance.gameObject.SetActive(charNameScreenInScene.gameObject.activeSelf);

        PrefabUtility.RecordPrefabInstancePropertyModifications(charNameScreenInstance);

        SayDialog charNameDialogueScene = charNameScreenInScene.GetComponent<SayDialog>();
        SayDialog charNameDialogueInstance = charNameScreenInstance.GetComponent<SayDialog>();

        if(charNameDialogueScene != null && charNameDialogueInstance != null)
        {
            foreach(Say sayCommand in episodeFlowchart.GetComponentsInChildren<Say>())
            {
                if(sayCommand != null)
                {
                    if (sayCommand.setSayDialog == charNameDialogueScene)
                        sayCommand.setSayDialog = charNameDialogueInstance;
                }
            }
        }        

        EditorCoroutineUtility.StartCoroutineOwnerless(FinishUpCharNameScreen(charNameScreenInstance));        
    }

    private IEnumerator FinishUpCharNameScreen(GameObject instancedObject)
    {
        yield return new EditorWaitForSeconds(0.1f);

        InputField charInputFieldScene = charNameScreenInScene.GetComponentInChildren<InputField>();
        InputField charInputFieldInstance = instancedObject.GetComponentInChildren<InputField>();

        Button charNameButtonScene = charNameScreenInScene.GetComponentInChildren<Button>();
        Button charNameButtonInstance = instancedObject.GetComponentInChildren<Button>();

        //GetText Command
        foreach (GetText getTextCommand in episodeFlowchart.GetComponentsInChildren<GetText>())
        {
            if (getTextCommand != null)
            {
                if (getTextCommand.TargetTextObject == charInputFieldScene.gameObject)
                {
                    getTextCommand.TargetTextObject = charInputFieldInstance.gameObject;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(getTextCommand);
                }
            }
        }

        //SetInteractable Command
        foreach (SetInteractable setInteractableCommand in episodeFlowchart.GetComponentsInChildren<SetInteractable>())
        {
            if (setInteractableCommand != null && setInteractableCommand.enabled)
            {
                if (setInteractableCommand.TargetObjects.Contains(charInputFieldScene.gameObject))
                {
                    setInteractableCommand.UpdateItemAtIndex(charInputFieldInstance.gameObject, 0);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(setInteractableCommand);
                }
            }
        }

        //"Set Active Command" in Flowchart if found any
        foreach (SetActive setActiveCommand in episodeFlowchart.GetComponentsInChildren<SetActive>())
        {
            if (setActiveCommand != null && setActiveCommand.enabled)
            {
                if (setActiveCommand.TargetGameObject == charNameScreenInScene)
                    setActiveCommand.TargetGameObject = instancedObject;
            }
        }

        //"Fade Alpha Canvas Command" in Flowchart if found any
        foreach (FadeCanvasGroup fadeCanvasGroupCommand in episodeFlowchart.GetComponentsInChildren<FadeCanvasGroup>())
        {
            if (fadeCanvasGroupCommand != null && fadeCanvasGroupCommand.enabled)
            {
                if (fadeCanvasGroupCommand.TargetCanvasGroups.Contains(charNameScreenInScene.GetComponent<CanvasGroup>()))
                    fadeCanvasGroupCommand.UpdateItemAtIndex(instancedObject.GetComponent<CanvasGroup>(), 0);
            }
        }

        //Blocks
        foreach (Block block in episodeFlowchart.GetComponentsInChildren<Block>())
        {
            if (block != null)
            {
                if (block._EventHandler != null)
                {
                    if (block._EventHandler.GetType() == typeof(Fungus.EndEdit))
                    {
                        EndEdit endEditEH = block._EventHandler as EndEdit;

                        if (endEditEH.TargetInputField != null && endEditEH.TargetInputField == charInputFieldScene)
                        {
                            endEditEH.TargetInputField = charInputFieldInstance;
                            charInputFieldInstance.transform.name = charInputFieldScene.transform.name;

                            PrefabUtility.RecordPrefabInstancePropertyModifications(endEditEH);
                        }
                    }

                    if (block._EventHandler.GetType() == typeof(Fungus.ButtonClicked))
                    {
                        ButtonClicked buttonClickedEH = block._EventHandler as ButtonClicked;

                        if (buttonClickedEH.TargetButton != null && buttonClickedEH.TargetButton == charNameButtonScene)
                        {
                            buttonClickedEH.TargetButton = charNameButtonInstance;
                            PrefabUtility.RecordPrefabInstancePropertyModifications(buttonClickedEH);
                        }
                    }
                }
            }
        }

        if (destoryOriginalCharNameScreen)
        {
            SceneVisibilityManager.DestroyImmediate(charNameScreenInScene.gameObject);
            charNameScreenInScene = null;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(instancedObject);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }
}
#endif