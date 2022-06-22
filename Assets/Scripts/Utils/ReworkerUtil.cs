using UnityEngine;
using UnityEngine.UI;
using Fungus;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Experimental.SceneManagement;
#endif

public class ReworkerUtil : MonoBehaviour
{
    private const string PREFABSTAGE = "Reworker Utils: You need to open Prefab in Prefab Mode!";

    [Header("FlowChart/Handler")]
    public Flowchart flowchart;
    public EpisodesHandler episodesHandler;

    [Header("Character Selection Screen")]
    public CharacterSelectionScreen[] charScreens;

    [Header("Characters")]
    public Character[] characters;

    [Header("Background Sprites")]
    public Transform spritesParent;

    [Header("Position Stages")]
    public Transform positionStageParent;

    [Header("Say Dialogues")]
    public SayDialog[] sayDialogsList;

    [Header("Menu Dialogues")]
    public MenuDialog menuDialog;

    [Header("Sounds")]
    //public List<AudioClip> episodeSounds = new List<AudioClip>();
    public ReworkerSoundsSO reworkerSoundsSO;

    [Header("End Screens")]
    public UIEpisodeEndPanel endEpisodePanel;
    public UIEpisodeEndPanelMk2 endEpisodePanelMk2;
    public UIEndStoryBranchScreenMk2 endBranchScreen;
    public UIEndStoryScreenMk2 endStoryScreen;
    public GameObject storyByMaujflixScreen;

    private void OnValidate()
    {
        episodesHandler = GetComponent<EpisodesHandler>();

        flowchart = GetComponentInChildren<Flowchart>();
        charScreens = GetComponentsInChildren<CharacterSelectionScreen>();
        characters = GetComponentsInChildren<Character>();
        sayDialogsList = GetComponentsInChildren<SayDialog>(true);
        menuDialog = GetComponentInChildren<MenuDialog>();

        endEpisodePanel = GetComponentInChildren<UIEpisodeEndPanel>();
        endEpisodePanelMk2 = GetComponentInChildren<UIEpisodeEndPanelMk2>();
        endBranchScreen = GetComponentInChildren<UIEndStoryBranchScreenMk2>();
        endStoryScreen = GetComponentInChildren<UIEndStoryScreenMk2>();

        storyByMaujflixScreen = GameObject.Find("MFlixStoryByMaujFlixScreen");
    }    

    public void ReworkStuffs()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            Debug.LogError(PREFABSTAGE);
            return;
        }

        ReworkCharacterSelectionScreens();
        ReworkCharacters();
        ReworkBgSprites();
        ReworkPositionStageParent();

        ReworkSayCommands();
        ReworkMusicCommands();
        ReworkSoundCommands();

        RemoveAllDialogues();
        //ReworkEndScreenCommands();
        RemoveEndScreens();

        ReworkHandler();
    }    

    private void ReworkCharacters()
    {
        if (characters.Length <= 0)
            return;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].FillupPortraitsNamesList();
            characters[i].Portraits.Clear();

            PrefabUtility.RecordPrefabInstancePropertyModifications(characters[i]);
        }
    }

    private void ReworkCharacterSelectionScreens()
    {
        if (charScreens.Length <= 0)
            return;

        for (int i = 0; i < charScreens.Length; i++)
        {
            if(PrefabUtility.GetPrefabInstanceStatus(charScreens[i].gameObject) == PrefabInstanceStatus.Connected)
            {
                PrefabUtility.UnpackPrefabInstance(charScreens[i].gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                PrefabUtility.RecordPrefabInstancePropertyModifications(charScreens[i].gameObject);

                SceneVisibilityManager.DestroyImmediate(charScreens[i].transform.GetChild(0).gameObject);
            }
        }
    }

    private void ReworkBgSprites()
    {
        if (spritesParent == null)
            return;

        foreach (Transform t in spritesParent)
        {
            if (t.TryGetComponent(out SpriteRenderer spriteRender))
            {
                if (spriteRender != null && spriteRender.sprite != null)
                {
                    spriteRender.gameObject.AddComponent<ImageLoader>();
                    spriteRender.sprite = null;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(spriteRender);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                }
            }
        }
    }

    private void ReworkPositionStageParent()
    {
        if (positionStageParent == null)
            return;

        foreach (Transform t in positionStageParent.GetChild(0))
        {
            Image image = t.GetComponent<Image>();
            if (image != null)
            {
                if (image.sprite != null)
                {
                    image.sprite = null;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(image);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                }
            }
        }
    }

    private void ReworkSayCommands()
    {
        if (flowchart == null)
            return;

        foreach (Say sayCommand in flowchart.GetComponentsInChildren<Say>())
        {
            if ((sayCommand != null) && sayCommand.enabled && sayCommand.GetType() == typeof(Say))
            {
                Block block = sayCommand.ParentBlock;

                SayMflix sayCommandMFlix = flowchart.gameObject.AddComponent<SayMflix>();
                sayCommandMFlix.ParentBlock = block;
                sayCommandMFlix.ItemId = flowchart.NextItemId();

                sayCommandMFlix._Character = sayCommand._Character;
                sayCommandMFlix.Portrait = sayCommand.Portrait;
                sayCommandMFlix.StoryText = sayCommand.StoryText;
                sayCommandMFlix.Description = sayCommand.Description;
                sayCommandMFlix.VoiceOverClip = sayCommand.VoiceOverClip;
                sayCommandMFlix.ShowAlways = sayCommand.ShowAlways;
                sayCommandMFlix.ShowCount = sayCommand.ShowCount;
                sayCommandMFlix.ExtendPrevious = sayCommand.ExtendPrevious;
                sayCommandMFlix.FadeWhenDone = sayCommand.FadeWhenDone;
                sayCommandMFlix.WaitForClick = sayCommand.WaitForClick;
                sayCommandMFlix.WaitForVO = sayCommand.WaitForVO;
                sayCommandMFlix.StopVoiceOver = sayCommand.StopVoiceOver;

                if (sayCommand.setSayDialog != null)
                {
                    sayCommandMFlix.setSayDialog = null;
                    switch (sayCommand.setSayDialog.transform.name)
                    {
                        case "MFlixNarrativeDialog":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_Narrative;
                            break;

                        case "MFlixNarrativeTutorialDialog":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_NarrativeTutorial;
                            break;

                        case "MFlixCharacterSayDialog":
                        case "MFlixCharacterSayDialogx2":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterSay;
                            break;

                        case "MFlixCharacterSayDialogWithMenu":
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterSayWithMenu;
                            break;

                        case string s when s.Contains("Black"):
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_NarrativeBlack;
                            break;

                        case string s when s.Contains("NameDialogue"):
                            sayCommandMFlix.sayDialogStyle = SayMflix.SayDialogStyle.Type_CharacterName;
                            break;
                    }
                }

                PrefabUtility.RecordPrefabInstancePropertyModifications(sayCommandMFlix);

                //block.CommandList.Add(sayCommandMFlix);
                int index = block.CommandList.IndexOf(sayCommand);
                if (index >= 0)
                {
                    block.CommandList[index] = sayCommandMFlix;
                    block.CommandList.Remove(sayCommand);
                    DestroyImmediate(sayCommand);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(block);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
                }

                //block.CommandList.RemoveAt(sayCommand.CommandIndex);
                //DestroyImmediate(sayCommand);
            }
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }

    private void ReworkMusicCommands()
    {
        if (flowchart == null)
            return;

        int count = -1;
        foreach (CallMethodMusic musicCommand in flowchart.GetComponentsInChildren<CallMethodMusic>())
        {
            if ((musicCommand != null) && musicCommand.enabled)
            {
                Block block = musicCommand.ParentBlock;

                PlayMusicIndex playMusicIndexCmd = flowchart.gameObject.AddComponent<PlayMusicIndex>();
                playMusicIndexCmd.ParentBlock = block;
                playMusicIndexCmd.ItemId = flowchart.NextItemId();

                count++;

                playMusicIndexCmd.UnloadPreviousMusic = (count == 0);
                playMusicIndexCmd.MusicIndex = musicCommand.MusicIndex;

                PrefabUtility.RecordPrefabInstancePropertyModifications(playMusicIndexCmd);

                int index = block.CommandList.IndexOf(musicCommand);
                if (index >= 0)
                {
                    block.CommandList[index] = playMusicIndexCmd;
                    block.CommandList.Remove(musicCommand);
                    DestroyImmediate(musicCommand);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(block);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
                }
            }
        }
    }

    private void ReworkSoundCommands()
    {
        if (flowchart == null || reworkerSoundsSO == null)
            return;

        int count = -1;
        foreach (PlaySound soundCommand in flowchart.GetComponentsInChildren<PlaySound>())
        {
            if ((soundCommand != null) && soundCommand.enabled)
            {
                Block block = soundCommand.ParentBlock;

                PlaySoundIndex playSoundIndexCmd = flowchart.gameObject.AddComponent<PlaySoundIndex>();
                playSoundIndexCmd.ParentBlock = block;
                playSoundIndexCmd.ItemId = flowchart.NextItemId();

                count++;

                playSoundIndexCmd.UnloadPreviousSound = (count == 0);
                foreach(AudioClip sound in reworkerSoundsSO.reworkerSoundsList)
                {
                    if( (sound != null && soundCommand.SoundClip != null) && soundCommand.SoundClip == sound)
                    {
                        playSoundIndexCmd.SoundIndex = reworkerSoundsSO.reworkerSoundsList.IndexOf(sound);
                        break;
                    }
                }

                PrefabUtility.RecordPrefabInstancePropertyModifications(playSoundIndexCmd);

                int index = block.CommandList.IndexOf(soundCommand);
                if (index >= 0)
                {
                    block.CommandList[index] = playSoundIndexCmd;
                    block.CommandList.Remove(soundCommand);
                    DestroyImmediate(soundCommand);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(block);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
                }
            }
        }
    }

    private void RemoveAllDialogues()
    {
        if (sayDialogsList.Length <= 0)
            return;

        if (menuDialog == null)
            return;

        PrefabUtility.UnpackPrefabInstance(menuDialog.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        for (int i = 0; i < sayDialogsList.Length; i++)
        {
            SceneVisibilityManager.DestroyImmediate(sayDialogsList[i].gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        SceneVisibilityManager.DestroyImmediate(menuDialog.gameObject);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
    }

    private void ReworkEndScreenCommands()
    {
        if (flowchart == null)
            return;

        foreach (CallMethod callMethodCommand in flowchart.GetComponentsInChildren<CallMethod>())
        {
            if ((callMethodCommand != null) && callMethodCommand.enabled)
            {
                PlayEnding playEndingCmd = null;

                Block block = callMethodCommand.ParentBlock;
                if (callMethodCommand.TargetObject == endEpisodePanel.gameObject &&
                    (callMethodCommand.TargetObject != null &&
                    endEpisodePanel != null))
                {
                    playEndingCmd = flowchart.gameObject.AddComponent<PlayEnding>();
                    playEndingCmd.ParentBlock = block;
                    playEndingCmd.ItemId = flowchart.NextItemId();
                    playEndingCmd.endScreenType = EndScreenType.EpisodeEndScreen;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(playEndingCmd);
                }
                
                else if(callMethodCommand.TargetObject == endBranchScreen.gameObject &&
                    (callMethodCommand.TargetObject != null &&
                    endBranchScreen != null))
                {
                    playEndingCmd = flowchart.gameObject.AddComponent<PlayEnding>();
                    playEndingCmd.ParentBlock = block;
                    playEndingCmd.ItemId = flowchart.NextItemId();
                    playEndingCmd.endScreenType = EndScreenType.StoryEndBranchedScreen;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(playEndingCmd);
                }
                
                else if(callMethodCommand.TargetObject == endStoryScreen.gameObject &&
                    (callMethodCommand.TargetObject != null &&
                    endStoryScreen != null))
                {
                    playEndingCmd = flowchart.gameObject.AddComponent<PlayEnding>();
                    playEndingCmd.ParentBlock = block;
                    playEndingCmd.ItemId = flowchart.NextItemId();
                    playEndingCmd.endScreenType = EndScreenType.StoryEndMainScreen;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(playEndingCmd);
                }

                int index = block.CommandList.IndexOf(callMethodCommand);
                if (index >= 0)
                {
                    block.CommandList[index] = playEndingCmd;
                    block.CommandList.Remove(callMethodCommand);
                    DestroyImmediate(callMethodCommand);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(block);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
                }
            }
        }
    }

    private void RemoveEndScreens()
    {
        if (endEpisodePanel)
        {
            SceneVisibilityManager.DestroyImmediate(endEpisodePanel.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        if (endEpisodePanelMk2)
        {
            SceneVisibilityManager.DestroyImmediate(endEpisodePanelMk2.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        if (endBranchScreen)
        {
            SceneVisibilityManager.DestroyImmediate(endBranchScreen.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        if (endStoryScreen)
        {
            SceneVisibilityManager.DestroyImmediate(endStoryScreen.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        if (storyByMaujflixScreen)
        {
            SceneVisibilityManager.DestroyImmediate(storyByMaujflixScreen);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }

        if(menuDialog)
        {
            SceneVisibilityManager.DestroyImmediate(menuDialog.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }
    }

    private void ReworkHandler()
    {
        if (episodesHandler == null)
            return;

        episodesHandler.endPanel = null;
        Array.Resize(ref episodesHandler.musicsList, 0);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ReworkerUtil)), DisallowMultipleComponent, CanEditMultipleObjects]
public class ReworkerUtilEditor : Editor
{
    private ReworkerUtil reworker;

    public override void OnInspectorGUI()
    {
        reworker = target as ReworkerUtil;

        DrawDefaultInspector();

        EditorGUILayout.Space(15);

        DrawDividerLine(Color.grey);

        var color = GUI.color;
        if(DrawButtonColored("DO THE JOB!", "#FF0000", Color.white))        
            reworker.ReworkStuffs();

        //EditorGUILayout.Space(10);

        //if (DrawButtonColored("FILL UP COMPONENTS", "#0055ff", Color.white))
            //reworker.FillupComponentsData();
        GUI.color = color;

        DrawDividerLine(Color.grey);
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

    private void DrawTitle(string _Title, int _titleFontSize = 12)
    {
        GUIStyle replacementOptionsStyle = new GUIStyle(GUI.skin.button);
        replacementOptionsStyle.alignment = TextAnchor.MiddleCenter;
        replacementOptionsStyle.fontSize = _titleFontSize;
        replacementOptionsStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(_Title, replacementOptionsStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(25f));
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