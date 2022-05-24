using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Automaton : MonoBehaviour
{
    int blockNumber = 1;

    int menuBlockCount = 0;

    int diamondButtonIndex = -1;

    int diamondButtonCost = 0;

    int previousNarrationID = -1;

    string previousCharacter = "";

    string previousExpression = "";

    string cameraMove = "";

    string previousFromPos = "";

    string previousToPos = "";

    string previousHair = "";

    string previousDress = "";

    string previousBg = "";

    string previousCommand = "";

    string menuCharacter = "";

    string menuBG = "";

    string menuFromPos = "";

    string menuToPos = ""; 

    bool previousFacingL = false;

    bool previousWasMenu = false;

    bool menuDialogue = false;

    // bool lastDialogue = false;
    
    bool currentRight = true;

    bool previousWasScreenChararcter = false;
    
    float fadeDuration = 1.5f;

    public GameObject episodeInProgress;

    public TextAsset EpisodeFile;

    public AudioClip[] audioClips;

    Flowchart flowchart;

    Block currentBlock;

    UIEpisodeEndPanel uiEndPannel;

    GameObject MFlixMenuDialog;

    GameObject FemaleNameInputField;

    GameObject MaleNameInputField;

    Animator menuAnimator;

    CanvasGroup maleCharacterSelectionScreen;

    CanvasGroup femaleCharacterSelectionScreen;

    Character[] characters;

    SayDialog[] sayDialogues;

    View[] views;

    List<Block> MenuBlocksList = new List<Block>();

    List<string> MenuBlockTexting = new List<string>();

    Vector2 newBlockPosition = new Vector2(0, 0);
    
    const string SAVE = "SaveEpisodeData";

    const string SAVE_EPISODE = "SaveEpisodeDataFinish";

    const string END_PANNEL = "PlayEndingScreen";

    const string PLAY_MUSIC = "PlayMusicAtIndex";

    const string ALLOW_MUSIC_FALSE = "SetAllowMusicFalse";

    const string ALLOW_MUSIC_TRUE = "SetAllowMusicTrue";

    const string NARRATIVE_DIALOGUE = "MFlixNarrativeDialog";

    const string BLACK_SCREEN_NARRATION = "MFlixBlackScreenNarrativeDialog";

    const string NARRATIVE_TUTORIAL_DIALOGUE = "MFlixNarrativeTutorialDialog";

    const string NARRATIVE_SCREEN_DIALOGUE = "MFlixNarrativeScreenDialog";

    const string CHARACTER_SAY_DIALOGUE = "MFlixCharacterSayDialog";

    const string CHARACTER_SAY_DIALOGUE_X2 = "MFlixCharacterSayDialogx2";

    const string CHARACTER_SAY_DIALOGUE_WITH_MENU = "MFlixCharacterSayDialogWithMenu";

    const string PICK_FEMALE_NAME = "MFlixPickFemaleNameDialog";

    const string PICK_MALE_NAME = "MFlixPickMaleNameDialog";

    const string TUTORIAL_STARTS = "----------------------Tutorial Zone Starts------------";

    const string TUTORIAL_ENDS = "----------------------Tutorial Zone Ends------------";

    const string EPISODE_STARTS = "-------------------------Episode Starts------------------------";

    const string FEMALE_CHARACTER_SELECTION = "----------------Show the Female Character Selection Screen--------------";

    const string MALE_CHARACTER_SELECTION = "----------------Show the Male Character Selection Screen--------------";

    const string LEFT = "Left";

    const string OFFSCREEN_LEFT = "Offscreen Left";

    const string RIGHT = "Right";

    const string OFFSCREEN_RIGHT = "Offscreen Right";

    const string MIDDLE = "Middle";

    const string MIDDLE_SMALL = "Middle_Small";

    const string LEFT_SMALL = "Left_Small";

    const string OFFSCREEN_LEFT_SMALL = "Offscreen Left_Small";

    const string RIGHT_SMALL = "Right_Small";

    const string OFFSCREEN_RIGHT_SMALL = "Offscreen Right_Small";

    const string LEFT_CALL = "Left_Call";

    const string LEFT_CALL_OFFSCREEN = "Left_Call_Offscreen";

    const string RIGHT_CALL = "Right_Call";

    const string RIGHT_CALL_OFFSCREEN = "Right_Call_Offscreen";

    const string UP_LEFT_CALL = "Up_Left_Call";

    const string UP_LEFT_CALL_OFFSCREEN = "Up_Left_Call_Offscreen";

    const string UP_RIGHT_CALL = "Up_Right_Call";

    const string UP_RIGHT_CALL_OFFSCREEN = "Up_Right_Call_Offscreen";

    public enum EventHandlerType
    {
        Type_None,
        Type_EndEdit,
        Type_ButtonClicked,
        Type_MessageReceived
    };

    void CreateEpisode()
    {
        blockNumber = 1;
        menuBlockCount = 0;
        diamondButtonIndex = -1;
        diamondButtonCost = 0;
        previousCharacter = "";
        previousExpression = "";
        cameraMove = "";
        previousFromPos = "";
        previousDress = "";
        previousFacingL = false;
        previousToPos = "";
        previousHair = "";
        previousBg = "";
        previousCommand = "";
        previousNarrationID = -1;
        previousWasScreenChararcter = false;
        previousWasMenu = false;
        menuToPos = "";
        menuFromPos = "";
        menuBG = "";
        menuCharacter = "";
        MenuBlockTexting.Clear();
        MenuBlocksList.Clear();

        GetDependencies();
        currentBlock = CreateNewBlock("Game Start");

        string rawCSV = EpisodeFile.text;

        string[] csvLines;

        string[] csvRow;

        List<string[]> listRow = new List<string[]>();
        
        csvLines = rawCSV.Split('\n');

        foreach (string l in csvLines)
        {
            csvRow = l.Trim().Split(';');
            listRow.Add(csvRow);
        }

        foreach (string [] r in listRow)
        {
            if (r[0] == "MBE" || previousWasScreenChararcter)
            {
                // Skip Last Convo
            }
            else
            {
                if ((previousCommand == "D" || previousCommand == "MBE") && r[0] != "D")
                {
                    LastConversationDialogue();
                }
            }

            switch (r[0])
            {
                case "ML":
                    CreateMusicLoop(int.Parse((r[1])));
                    break;
                case "BG":
                    if (r[2] == "")
                        BGPans(r[1]);
                    else
                        BGPans(r[1], int.Parse(r[2]));
                    break;
                case "D":
                    if (r[8] == "" || int.Parse((r[8])) == 0) // Check If Menu
                        menuDialogue = false;
                    else
                    {
                        menuDialogue = true;
                        diamondButtonIndex = int.Parse(r[10]);
                        diamondButtonCost = int.Parse(r[11]);

                        for (int i = 12; i < r.Length; i++)
                        {
                            if (r[i] != "")
                                MenuBlockTexting.Add(r[i]);
                            else
                                break;
                        }
                    }

                    /*if (int.Parse((r[10])) == 0) // Check Last Dialogue in Conversation
                        lastDialogue = false;
                    else
                        lastDialogue = true;*/

                    if (int.Parse(r[5]) == 0) // Check Dialogue Size
                        CreateCharacterDialogue(r[1], r[2], r[3], r[4], MenuBlockTexting, CHARACTER_SAY_DIALOGUE, r[6], r[7], menuDialogue, r[9]/*, lastDialogue*/); // r[5], r[8] and r[10] are dialogue size, menu and last dialogue
                    else
                        CreateCharacterDialogue(r[1], r[2], r[3], r[4], MenuBlockTexting, CHARACTER_SAY_DIALOGUE_X2, r[6], r[7], menuDialogue, r[9]/*, lastDialogue*/); // r[5], r[8] and r[10] are dialogue size, menu and last dialogue
                    break;
                case "MBE":
                    menuBlockCount += 1;
                    
                    if (menuBlockCount <= MenuBlocksList.Count - 2)
                    {
                        CreateNewCallCommand(flowchart, MenuBlocksList[MenuBlocksList.Count - 1]);
                        currentBlock = MenuBlocksList[menuBlockCount];
                        previousBg = menuBG;
                        previousCharacter = menuCharacter;
                        previousFromPos = menuToPos;
                        previousWasMenu = true;
                    }
                    else
                    {
                        CreateNewCallCommand(flowchart, MenuBlocksList[MenuBlocksList.Count - 1]);
                        currentBlock = MenuBlocksList[MenuBlocksList.Count - 1];
                        menuBlockCount = 0;
                        diamondButtonIndex = -1;
                        diamondButtonCost = 0;
                        MenuBlockTexting.Clear();
                        MenuBlocksList.Clear();
                        menuBG = "";
                        menuCharacter = "";
                        menuToPos = "";
                        previousWasMenu = false;
                    }
                    break;
                case "BE":
                    menuBlockCount += 1;
                    CreateEnd();

                    if (menuBlockCount <= MenuBlocksList.Count - 2)
                    {
                        currentBlock = MenuBlocksList[menuBlockCount];
                        previousBg = menuBG;
                        previousCharacter = menuCharacter;
                        previousFromPos = menuToPos;
                        previousWasMenu = true;
                    }
                    else
                    {
                        currentBlock = MenuBlocksList[MenuBlocksList.Count - 1];
                        menuBlockCount = 0;
                        diamondButtonIndex = -1;
                        diamondButtonCost = 0;
                        MenuBlockTexting.Clear();
                        MenuBlocksList.Clear();
                        menuBG = "";
                        menuCharacter = "";
                        menuToPos = "";
                        previousWasMenu = false;
                    }
                    break;
                case "N":
                    if (int.Parse(r[1]) == 1 && previousNarrationID != 1)
                    {
                        CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");
                        previousBg = "BlackBg-View";
                    }
                    
                    CreateNarration(int.Parse(r[1]), r[2], r[3]);
                    break;
                case "S":
                    CreateSave();
                    break;
                case "END":
                    CreateEnd();
                    break;
                case "FO":
                    if (r[2] == "")
                        CreateFadeOut(r[1]);
                    else
                        CreateFadeOut(r[1], int.Parse((r[2])));
                    break;
                case "PN":
                    if (int.Parse(r[3]) == 0)
                        PickCharacterName(r[1], r[2]);
                    else if (int.Parse(r[3]) == 1)
                        PickCharacterName(r[1], r[2], false);
                    break;
                case "PC":
                    if (int.Parse(r[2]) == 0)
                        PickCharacterSelection(r[1]);
                    else
                        PickCharacterSelection(r[1], false);
                    break;
                case "SFX":
                    foreach(AudioClip ac in audioClips)
                    {
                        if (ac.name == r[1])
                            CreateSFX(ac);
                    }
                    break;
                case "CS":
                    if (int.Parse(r[4]) == 0)
                        CreateNewCameraShakeCommand(float.Parse(r[1]), float.Parse(r[2]), float.Parse(r[3]));
                    else
                        CreateNewCameraShakeCommand(float.Parse(r[1]), float.Parse(r[2]), float.Parse(r[3]), true);
                    break;
                case "IMGNZ":
                    CreateZoomImage(r[1], r[2], r[3]);
                    break;
                case "IMGP":
                    if (r[4] != "")
                        CreateFromToImage(r[1], r[2], r[3], int.Parse(r[4]));
                    else
                        CreateFromToImage(r[1], r[2], r[3]);
                    break;
                case "IMG3":
                    if (r[7] != "")
                        CreateScreenWithThreeImages(int.Parse(r[1]), r[2], r[3], r[4], r[5], r[6], int.Parse(r[7]));
                    else
                        CreateScreenWithThreeImages(int.Parse(r[1]), r[2], r[3], r[4], r[5], r[6]);
                    break;
            }

            previousCommand = r[0];

            if (r[0] == "N")
                previousNarrationID = int.Parse(r[1]);

            if (r[0] == "D")
                previousWasScreenChararcter = r[2].Contains("ScreenCharacter");
        }

        /*GetDependencies(); 
        currentBlock = CreateNewBlock();

        BGPans("CollegeHall");
        CreateCharacterDialogue("CollegeHall", "Mohit", "Happy", "Hello I made It Mohit C01", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Mohit C02", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Mohit C03", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Rohan", "Confuse", "Hello I made It Rohan C04", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Rohan C05", MenuBlockTexting, "", "D1", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Happy", "Hello I made It Mohit C11", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Mohit C12", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Mohit C13", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("Cafe", "Rohan", "Confuse", "Hello I made It Mohit C14", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("Cafe", "Mohit", "Confuse", "Hello I made It Mohit C15", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("Cafe", "Rohan", "Confuse", "Hello I made It Mohit C16", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("Cafe", "Mohit", "Confuse", "Hello I made It Mohit C17", MenuBlockTexting, "", "D1", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Confuse", "Hello I made It Mohit C18", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("Cafe", "Mohit", "Confuse", "Hello I made It Mohit C19", MenuBlockTexting, "", "D1", false, "R", false);
        CreateCharacterDialogue("Cafe", "Mohit", "Confuse", "Hello I made It Mohit C20", MenuBlockTexting, "", "D1", false, "L", false);
        CreateCharacterDialogue("Cafe", "Mohit", "Happy", "Hello I made It Mohit C21", MenuBlockTexting, "", "D1", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Happy", "Hello I made It Mohit C22", MenuBlockTexting, "", "D1", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Phone", "Hello I made It Mohit C23", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Rohan", "Phone", "Hello I made It Mohit C24", MenuBlockTexting, "", "", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Rohan", "Phone", "Hello I made It Mohit C25", MenuBlockTexting, "", "", false, "L", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Phone", "Hello I made It Mohit C26", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Phone", "Hello I made It Mohit C27", MenuBlockTexting, "", "", true, "R", false);
        CreateCharacterDialogue("CollegeHall", "Mohit", "Phone", "Hello I made It Mohit C28", MenuBlockTexting, "", "", false, "R", false);
        CreateCharacterDialogue("CollegeHall", "Rohan", "Happy", "Hello I made It Mohit C29", MenuBlockTexting, "", "", true, "R", false);
        CreateCharacterDialogue("CollegeHall", "Rohan", "Happy", "Hello I made It Mohit C29", MenuBlockTexting, "", "", false, "R", true);

        Debug.Log(previousCharacter);
        Debug.Log(previousExpression);
        Debug.Log(previousFromPos);
        Debug.Log(cameraMove);
        Debug.Log(previousWasMenu);*/
    }

    public void GetDependencies()
    {
        blockNumber = 1;
        flowchart = episodeInProgress.GetComponentInChildren<Flowchart>();
        uiEndPannel = episodeInProgress.GetComponentInChildren<UIEpisodeEndPanel>();
        MFlixMenuDialog = episodeInProgress.GetComponentInChildren<MenuDialogMflix>().gameObject;
        menuAnimator = MFlixMenuDialog.GetComponentInChildren<Animator>();
        //MFlixMenuDialog = episodeInProgress.GetComponentInChildren<MenuDialog>().gameObject;
        characters = episodeInProgress.GetComponentsInChildren<Character>();
        sayDialogues = episodeInProgress.GetComponentsInChildren<SayDialog>(true);
        views = episodeInProgress.GetComponentsInChildren<View>();

        Transform[] children = episodeInProgress.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == "EnterFemaleNameInputField")
                FemaleNameInputField = child.gameObject;
            else if (child.name == "EnterMaleNameInputField")
                MaleNameInputField = child.gameObject;
            else if (child.name == "MaleCharacterSelectionScreen")
                maleCharacterSelectionScreen = child.gameObject.GetComponent<CanvasGroup>();
            else if (child.name == "FemaleCharacterSelectionScreen")
                femaleCharacterSelectionScreen = child.gameObject.GetComponent<CanvasGroup>();
        }
    }

    public void BGPans(string BGName, int blockType = 0)
    {
        // blockType 1 is for Panning
        // blockType 2 is for No Panning

        string left = BGName + "View-Left";
        string right = BGName + "View-Right";
        string front = BGName + "View-Front";

        if (episodeInProgress.name.Contains("Ep1") && currentBlock.ItemId == 0 && blockType == 0)
        {
            // First block for Episode 1

            CreateNewCallMethodCommand(episodeInProgress, ALLOW_MUSIC_FALSE);
            CreateNewSetLanguageCommand();
            CreateNewFadeToViewCommand(1.5f, false, front);
            CreateNewWaitCommand(1);
            CreateNewCommentCommand(TUTORIAL_STARTS);
            CreateNewSayCommand("कहानियों से भरी इस रोमांचक दुनिया में आपका स्वागत हैं।", NARRATIVE_TUTORIAL_DIALOGUE, false);
            CreateNewWaitCommand(1);
            CreateNewWaitForClickCommand();
            CreateNewSayCommand("सभी कहानियाँ आपको अपने हिसाब से विकल्प चुनने का मौका देंगी।", NARRATIVE_TUTORIAL_DIALOGUE);
            CreateNewSayCommand("यही चुने हुए विकल्प कहानी का अंत तय करेंगे, हर कहानी आपको एक से ज्यादा अंत का अनुभव देगी।", NARRATIVE_TUTORIAL_DIALOGUE);
            CreateNewSayCommand("हर कहानी के सारे अंत दिलचस्प हैं, कहानी दोबारा खेल कर इन्हे ज़रूर देखिएगा।", NARRATIVE_TUTORIAL_DIALOGUE);
            CreateNewSayCommand("कहानी शुरू करते हैं।", NARRATIVE_TUTORIAL_DIALOGUE);
            CreateNewCommentCommand(TUTORIAL_ENDS);
            CreateNewCommentCommand(EPISODE_STARTS);
            CreateNewCallMethodCommand(episodeInProgress, ALLOW_MUSIC_TRUE);
            CreateNewCallMusicMethodCommand(episodeInProgress, PLAY_MUSIC, 0);
            CreateNewMoveToViewCommand(0, front);
            CreateNewMoveToViewCommand(4f, left);
            CreateNewMoveToViewCommand(3.5f, right);
            CreateNewMoveToViewCommand(2f, front);
            cameraMove = "F";
            previousBg = BGName;
        }
        else
        {

            if (blockType == 1)
            {
                // New block in episode with Panning

                CreateNewMoveToViewCommand(0, front);
                CreateNewMoveToViewCommand(4f, left);
                CreateNewMoveToViewCommand(3.5f, right);
                CreateNewMoveToViewCommand(2f, front);
                previousBg = BGName;
            }
            else if (blockType == 2)
            {
                // New block in episode without Panning

                CreateNewMoveToViewCommand(0, front);
                cameraMove = "F";
                previousBg = BGName;
            }
            else if (currentBlock.ItemId == 0)
            {
                // First block for Episode 2 - n

                CreateNewSetLanguageCommand();
                CreateNewFadeToViewCommand(1.5f, false, front);
                CreateNewCallMethodCommand(episodeInProgress, SAVE);
                CreateNewWaitCommand(1.0f);
                CreateNewMoveToViewCommand(4f, left);
                CreateNewMoveToViewCommand(3.5f, right);
                CreateNewMoveToViewCommand(2f, front);
                cameraMove = "F";
                previousBg = BGName;
            }
            else
            {
                Debug.Log("Invalid Input for Block Type use 1 for Panning and 2 for No Panning");
                return;
            }
        }
    }

    public void CreateCharacterDialogue(string BGName, string newCharacter, string newExpression, string newDialogue, List<string> menuBlockText, string dialogueSize = CHARACTER_SAY_DIALOGUE_X2, string dress = "", string hair = "", bool menu = false, string moveTo = "L"/*, bool lastConversationDialogue = false*/)
    {
        if (newCharacter == "XXXM")
            newCharacter = "Character_Male-LookType-0";
        else if (newCharacter == "XXXF")
            newCharacter = "Character_Female-LookType-0";
        else if (newCharacter == "YYYM")
            newCharacter = "Character_Male";
        else if (newCharacter == "YYYF")
            newCharacter = "Character_Female";

        bool phone = newExpression == "Phone";
        bool screenCharacter = newCharacter.Contains("ScreenCharacter");
        string fromPos = "";
        string toPos = "";
        bool facingL = false;

        if (previousCharacter == newCharacter && cameraMove != moveTo && previousBg == BGName && !phone)
            moveTo = cameraMove;

        if (menu)
        {
            if (phone)
            {
                if (moveTo == "L")
                {
                    fromPos = LEFT_CALL_OFFSCREEN; // UP_LEFT_CALL_OFFSCREEN
                    toPos = UP_LEFT_CALL;
                    facingL = false;
                    previousFacingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = RIGHT_CALL_OFFSCREEN; // UP_RIGHT_CALL_OFFSCREEN
                    toPos = UP_RIGHT_CALL;
                    facingL = true;
                    previousFacingL = true;
                }
            }
            else
            {
                if (moveTo == "L")
                {
                    fromPos = OFFSCREEN_LEFT;
                    toPos = LEFT;
                    facingL = false;
                    previousFacingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = OFFSCREEN_RIGHT;
                    toPos = RIGHT;
                    facingL = true;
                    previousFacingL = true;
                }
            }
        }
        else
        {
            if (phone)
            {
                if (moveTo == "L")
                {
                    fromPos = LEFT_CALL_OFFSCREEN;
                    toPos = LEFT_CALL;
                    facingL = false;
                    previousFacingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = RIGHT_CALL_OFFSCREEN;
                    toPos = RIGHT_CALL;
                    facingL = true;
                    previousFacingL = true;
                }
            }
            else
            {
                if (moveTo == "L")
                {
                    fromPos = OFFSCREEN_LEFT;
                    toPos = LEFT;
                    facingL = false;
                    previousFacingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = OFFSCREEN_RIGHT;
                    toPos = RIGHT;
                    facingL = true;
                    previousFacingL = true;
                }
            }
        }

        if (phone)
            cameraMove = moveTo;

        if (screenCharacter)
            cameraMove = "F";

        if (newCharacter == previousCharacter && cameraMove == moveTo && previousBg == BGName)
        {
            if (newExpression != previousExpression && !phone && !screenCharacter)
            {
                previousExpression = newExpression;
                CreateNewPortraitCommand(true, newCharacter, toPos, facingL, false, false, 0f, "", dress, hair, newExpression);
                previousCharacter = newCharacter;
            }

            if (previousWasMenu && phone)
            {
                if (moveTo == "L")
                {
                    fromPos = UP_LEFT_CALL;
                    toPos = LEFT_CALL;
                    previousFromPos = LEFT_CALL_OFFSCREEN;
                    menuToPos = previousFromPos;
                    facingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = UP_RIGHT_CALL;
                    toPos = RIGHT_CALL;
                    previousFromPos = RIGHT_CALL_OFFSCREEN;
                    menuToPos = previousFromPos;
                    facingL = true;
                }

                CreateNewPortraitCommand(true, newCharacter, toPos, facingL, true, false, 0, fromPos, dress, hair, newExpression);
                previousCharacter = newCharacter;
                previousExpression = newExpression;
            }
            
            if (menu && phone)
            {
                if (moveTo == "L")
                {
                    fromPos = LEFT_CALL;
                    toPos = UP_LEFT_CALL;
                    previousFromPos = LEFT_CALL_OFFSCREEN; // UP_LEFT_CALL_OFFSCREEN
                    menuToPos = previousFromPos;
                    menuFromPos = previousFromPos;
                    facingL = false;
                }
                else if (moveTo == "R")
                {
                    fromPos = RIGHT_CALL;
                    toPos = UP_RIGHT_CALL;
                    previousFromPos = RIGHT_CALL_OFFSCREEN; // UP_RIGHT_CALL_OFFSCREEN
                    menuToPos = previousFromPos;
                    menuFromPos = previousFromPos;
                    facingL = true;
                }

                CreateNewPortraitCommand(true, newCharacter, toPos, facingL, true, false, 0, fromPos, dress, hair, newExpression);
                previousCharacter = newCharacter;
                previousExpression = newExpression;
                CreateNewSetActiveCommand(MFlixMenuDialog);
            }

            if (phone && previousExpression != "Phone")
            {
                moveTo = "L";
                CreatePortraitAndSay(BGName, newCharacter, newExpression, newDialogue, menuBlockText, dialogueSize, dress, hair, menu, moveTo, phone, fromPos, toPos, facingL);
            }
            else if (menu)
            {
                if (!phone)
                    CreateNewSetActiveCommand(MFlixMenuDialog);
                previousWasMenu = true;
                menuBG = BGName;
                menuCharacter = newCharacter;
                menuToPos = previousFromPos;
                CreateNewSayCommand(newDialogue, CHARACTER_SAY_DIALOGUE_WITH_MENU, false, false, newCharacter);
                CreateNewSetAnimTriggerCommand(menuAnimator);

                /*foreach (string m in menuBlockText)
                {
                    CreateNewMenuCommand(m);
                }*/

                for (int i = 0; i < menuBlockText.Count; i++)
                {
                    if (diamondButtonIndex == i)
                        CreateNewMenuCommand(menuBlockText[i], diamondButtonCost);
                    else
                        CreateNewMenuCommand(menuBlockText[i]);
                }

                Block menuEndBlock = CreateNewBlock("Menu End");
                MenuBlocksList.Add(menuEndBlock);

                currentBlock = MenuBlocksList[menuBlockCount];
            }
            else
            {
                previousWasMenu = false;
                CreateNewSayCommand(newDialogue, dialogueSize, true, true, newCharacter);

                if (menu && phone)
                    CreateNewSetAnimTriggerCommand(menuAnimator);
            }
        }
        else
        {
            /*if (screenCharacter)
            {
                if (previousBg != BGName)
                {
                    CreateNewFadeToViewCommand(2f, true, BGName);
                    previousBg = BGName;
                }

                cameraMove = "F";
                CreateNewSayCommand(newDialogue, dialogueSize, true, true, newCharacter);
                previousFromPos = fromPos;
                previousCharacter = newCharacter;
                previousExpression = newExpression;
                previousFacingL = facingL;
                previousToPos = toPos;
                previousHair = hair;
                previousDress = dress;
                previousWasMenu = false;
            }
            else
            {*/
                if (phone)
                {
                    moveTo = "L";
                    cameraMove = "L";
                    menuToPos = menuFromPos;
                }

                if (screenCharacter)
                {
                    moveTo = "F";
                    cameraMove = "F";
                }

                CreatePortraitAndSay(BGName, newCharacter, newExpression, newDialogue, menuBlockText, dialogueSize, dress, hair, menu, moveTo, phone, fromPos, toPos, facingL);
            //}
        }
    }

    public void LastConversationDialogue()
    {
        if (previousCharacter != "")
            CreateNewPortraitCommand(false, previousCharacter, previousFromPos);
        CreateSave();
        previousCharacter = "";
        previousExpression = "";
        cameraMove = "";
        previousFromPos = "";
        previousToPos = "";
        previousHair = "";
        previousDress = "";
        previousFacingL = false;
        currentRight = true;
        previousBg = "";
        fadeDuration = 1.5f;
    }

    private void CreatePortraitAndSay(string BGName, string newCharacter, string newExpression, string newDialogue, List<string> menuBlockText, string dialogueSize, string dress, string hair, bool menu, string moveTo, bool phone, string fromPos, string toPos, bool facingL)
    {
        string previousCameraMove = cameraMove;
        bool screenCharacter = newCharacter.Contains("ScreenCharacter");

        if (previousCharacter != "" && !previousCharacter.Contains("ScreenCharacter"))
        {
            if (cameraMove == moveTo && !phone && previousBg == BGName)
            {
                if (currentRight)
                {
                    currentRight = false;
                    CreateNewMoveToViewCommand(1, BGName + "View-FurtherRight", false);
                }
                else if (!currentRight)
                {
                    currentRight = true;
                    CreateNewMoveToViewCommand(1, BGName + "View-Right", false);
                }
                CreateNewPortraitCommand(true, previousCharacter, previousFromPos, previousFacingL, true, true, 0.5f, previousToPos, previousDress, previousHair, previousExpression);
            }
            else
                CreateNewPortraitCommand(false, previousCharacter, previousFromPos);

            if (previousBg != BGName && !screenCharacter)
                CreateNewWaitCommand(0.75f);

            CreateSave();
            previousCharacter = newCharacter;
        }

        if (screenCharacter)
        {
            if (previousBg != BGName)
            {
                CreateNewFadeToViewCommand(2f, true, BGName);
                previousBg = BGName;
            }

            fadeDuration = 1.5f;
            cameraMove = "F";
            CreateNewSayCommand(newDialogue, dialogueSize, true, true, newCharacter);
            previousFromPos = fromPos;
            previousCharacter = newCharacter;
            previousExpression = newExpression;
            previousFacingL = facingL;
            previousToPos = toPos;
            previousHair = hair;
            previousDress = dress;
            previousWasMenu = false;
        }
        else
        {

            if (previousBg != BGName)
            {
                cameraMove = moveTo;

                if (moveTo == "L")
                {
                    CreateNewFadeToViewCommand(fadeDuration, true, BGName + "View-Left");
                    previousBg = BGName;
                }
                else if (moveTo == "R")
                {
                    CreateNewFadeToViewCommand(fadeDuration, true, BGName + "View-Right");
                    previousBg = BGName;
                }

                fadeDuration = 0;
            }
            else
            {
                previousBg = BGName;
                fadeDuration = 1.5f;

                if (cameraMove != moveTo)
                {
                    cameraMove = moveTo;

                    if (moveTo == "R" && !phone)
                    {
                        currentRight = true;
                        CreateNewMoveToViewCommand(1, BGName + "View-Right");
                    }
                    else if (moveTo == "L")
                        CreateNewMoveToViewCommand(1, BGName + "View-Left");
                }
                else
                {
                    if (phone)
                        CreateNewWaitCommand(0.75f);
                }
            }

            if (menu)
            {
                CreateNewPortraitCommand(true, newCharacter, toPos, facingL, true, true, 0.5f, fromPos, dress, hair, newExpression);
                previousFromPos = fromPos;
                previousCharacter = newCharacter;
                previousExpression = newExpression;
                previousWasMenu = true;
                menuBG = BGName;
                menuCharacter = newCharacter;
                menuToPos = previousFromPos;
                menuFromPos = previousFromPos;
                previousFacingL = facingL;
                previousToPos = toPos;
                previousHair = hair;
                previousDress = dress;
                CreateNewSetActiveCommand(MFlixMenuDialog);

                if (previousCameraMove == moveTo && !phone)
                {
                    CreateNewWaitCommand(0.8f);
                }

                CreateNewSayCommand(newDialogue, CHARACTER_SAY_DIALOGUE_WITH_MENU, false, false, newCharacter);
                CreateNewSetAnimTriggerCommand(menuAnimator);

                /*foreach (string m in menuBlockText)
                {
                    CreateNewMenuCommand(m);
                }*/

                for (int i = 0; i < menuBlockText.Count; i++)
                {
                    if (diamondButtonIndex == i)
                        CreateNewMenuCommand(menuBlockText[i], diamondButtonCost);
                    else
                        CreateNewMenuCommand(menuBlockText[i]);
                }

                Block menuEndBlock = CreateNewBlock("Menu End");
                MenuBlocksList.Add(menuEndBlock);

                currentBlock = MenuBlocksList[menuBlockCount];
            }
            else
            {
                CreateNewPortraitCommand(true, newCharacter, toPos, facingL, true, true, 0.5f, fromPos, dress, hair, newExpression);

                if (previousCameraMove == moveTo && !phone)
                {
                    CreateNewWaitCommand(0.8f);
                }

                CreateNewSayCommand(newDialogue, dialogueSize, true, true, newCharacter);
                previousFromPos = fromPos;
                previousCharacter = newCharacter;
                previousExpression = newExpression;
                previousFacingL = facingL;
                previousToPos = toPos;
                previousHair = hair;
                previousDress = dress;
                previousWasMenu = false;
            }
        }
    }

    public void CreateNarration(int narrationType, string narrationDialogue, string BGName = "")
    {
        // 0 = Narration Normal 
        // 1 = Narration Black Screen 
        // 2 = Narration With Screen
        // 3 = Narration Tutorial 

        switch (narrationType)
        {
            case 0:
                if (BGName != "")
                {
                    if (previousWasScreenChararcter || previousBg == "BlackBg-View")
                    {
                        CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
                        cameraMove = "F";
                        previousBg = BGName;
                    }
                    else
                    {
                        CreateNewMoveToViewCommand(1, BGName + "View-Front");
                        cameraMove = "F";
                        previousBg = BGName;
                    }
                }

                CreateNewSayCommand(narrationDialogue, NARRATIVE_DIALOGUE);
                break;
            case 1:
                CreateNewSayCommand(narrationDialogue, BLACK_SCREEN_NARRATION);
                break;
            case 2:
                CreateNewSayCommand(narrationDialogue, NARRATIVE_SCREEN_DIALOGUE);
                break;
            case 3:
                CreateNewSayCommand(narrationDialogue, NARRATIVE_TUTORIAL_DIALOGUE);
                break;
            default:
                Debug.Log("Invalid Narration Type use 0 for Normal Narration, use 1 for Black Screen Narration, use 2 for Narration with Screen and use 3 for Tutorial Narration");
                break;
        }
    }

    public void CreateMusicLoop(int musicIndex, float newDelay = 0.5f)
    {
        CreateNewCallMusicMethodCommand(episodeInProgress, PLAY_MUSIC, musicIndex, newDelay);
    }

    public void CreateSave()
    {
        CreateNewCallMethodCommand(episodeInProgress, SAVE);
    }

    public void CreateSFX(AudioClip newAudio)
    {
        CreateNewSetAudioVolumeCommand(0.1f, 0.5f);
        CreateNewPlaySoundCommand(newAudio);
        CreateNewWaitCommand(1.5f);
        CreateNewSetAudioVolumeCommand(1f, 0.5f);
    }
    
    public void CreateScreenWithThreeImages(int screenType, string img1, string img2, string img3, string BGName = "", string newClip = "", int newDuration = 10)
    {
        if (screenType == 0)
        {
            CreateNewIFCommand();
            CreateZoomImage(img1, BGName, newClip);
            CreateNewELSEIFCommand();
            CreateZoomImage(img2, BGName, newClip);
            CreateNewELSEIFCommand();
            CreateZoomImage(img3, BGName, newClip);
            CreateNewENDCommand();
        }
        else if (screenType == 1)
        {
            CreateNewIFCommand();
            CreateFromToImage(img1, BGName, newClip, newDuration);
            CreateNewELSEIFCommand();
            CreateFromToImage(img2, BGName, newClip, newDuration);
            CreateNewELSEIFCommand();
            CreateFromToImage(img3, BGName, newClip, newDuration);
            CreateNewENDCommand();
        }
        else
            Debug.Log("Invalid Screen Type: Use 0 for Normal Zoom with 3 Images & 1 for Pan with 3 Images");
    }

    public void CreateZoomImage(string viewName, string BGName = "", string newClip = "")
    {
        if (newClip == "")
        {
            CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");
            CreateNewFadeToViewCommand(2, false, viewName);
            CreateNewZoomCommand();
            CreateNewWaitForClickCommand();
            CreateNewCallMethodCommand(episodeInProgress, SAVE);

            if (BGName != "")
            {
                CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
                cameraMove = "F";
                previousBg = BGName;
            }
            else
            {
                previousBg = viewName;
                cameraMove = "F";
            }
        }
        else
        {
            CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");
            CreateNewSetAudioVolumeCommand(0.1f, 0.5f);

            foreach (AudioClip ac in audioClips)
            {
                if (ac.name == newClip)
                    CreateNewPlaySoundCommand(ac);
            }

            CreateNewFadeToViewCommand(2, false, viewName);
            CreateNewZoomCommand();
            CreateNewWaitForClickCommand();
            CreateNewSetAudioVolumeCommand(1f, 0.5f);
            CreateNewCallMethodCommand(episodeInProgress, SAVE);

            if (BGName != "")
            {
                CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
                cameraMove = "F";
                previousBg = BGName;
            }
            else
            {
                previousBg = viewName;
                cameraMove = "F";
            }
        }
    }

    public void CreateFromToImage(string fromImg, string BGName = "", string newClip = "", int newDuration = 10)
    {
        string toImg = "";
        if (fromImg.Contains("TOP"))
            toImg = fromImg.Replace("TOP", "BOTTOM");
        else if (fromImg.Contains("BOTTOM"))
            toImg = fromImg.Replace("BOTTOM", "TOP");
        else if (fromImg.Contains("MID"))
            toImg = fromImg.Replace("MID", "FULL");
        else if (fromImg.Contains("FULL"))
            toImg = fromImg.Replace("FULL", "MID");
        else if (fromImg.Contains("RIGHT"))
            toImg = fromImg.Replace("RIGHT", "LEFT");
        else if (fromImg.Contains("LEFT"))
            toImg = fromImg.Replace("LEFT", "RIGHT");
        else
            Debug.Log("Improper View Names for Panning Screens: Use Capital Letters to Indicate Form and To Views");

        if (newClip == "")
        {
            CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");
            CreateNewFadeToViewCommand(2, false, fromImg);
            CreateNewMoveToViewCommand(newDuration, toImg);
            CreateNewWaitForClickCommand();
            CreateNewCallMethodCommand(episodeInProgress, SAVE);

            if (BGName != "")
            {
                CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
                cameraMove = "F";
                previousBg = BGName;
            }
            else
            {
                previousBg = fromImg;
                cameraMove = "F";
            }
        }
        else
        {
            CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");
            CreateNewSetAudioVolumeCommand(0.1f, 0.5f);

            foreach (AudioClip ac in audioClips)
            {
                if (ac.name == newClip)
                    CreateNewPlaySoundCommand(ac);
            }

            CreateNewFadeToViewCommand(2, false, fromImg);
            CreateNewMoveToViewCommand(newDuration, toImg);
            CreateNewWaitForClickCommand();
            CreateNewSetAudioVolumeCommand(1f, 0.5f);

            if (BGName != "")
            {
                CreateNewCallMethodCommand(episodeInProgress, SAVE);
                CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
                cameraMove = "F";
                previousBg = BGName;
            }
            else
            {
                previousBg = fromImg;
                cameraMove = "F";
            }
        }
    }

    public void CreateEnd()
    {
        CreateNewCallMethodCommand(episodeInProgress, SAVE_EPISODE);
        CreateNewFadeToViewCommand(6f, true, "BlackBg-View");
        CreateNewCallMethodCommand(uiEndPannel.gameObject, END_PANNEL);
    }

    public void CreateFadeOut(string nextBG, int musicIndex = -1)
    {
        CreateNewFadeToViewCommand(4.5f, true, "BlackBg-View");

        if (musicIndex >= 0)
        {
            CreateMusicLoop(musicIndex, 0.25f);
        }

        CreateNewFadeToViewCommand(1.5f, false, nextBG + "View-Front");
        Block newBlock = CreateNewBlock(nextBG);
        CreateNewCallCommand(flowchart, newBlock);
        currentBlock = newBlock;
    }

    public void PickCharacterName(string BGName, string pickNameDialogue, bool male = true)
    {
        if (male)
        {
            CreateNewMoveToViewCommand(1, BGName + "View-Front");
            cameraMove = "F";
            previousBg = BGName;
            CreateNewSayCommand(pickNameDialogue, PICK_MALE_NAME);
            currentBlock = CreateNewBlock("Get Male Name", EventHandlerType.Type_EndEdit);
            CreateNewGetTextCommand(MaleNameInputField);
            currentBlock = CreateNewBlock("Submit Male Name", EventHandlerType.Type_ButtonClicked);
            CreateNewSetInteractableCommand(MaleNameInputField, false);
            SayDialog s = GetSayDialogue(PICK_MALE_NAME);
            CreateNewFadeCanvasGroupCommand(s.gameObject.GetComponent<CanvasGroup>(), false, 0);
            CreateNewSetActiveCommand(s.gameObject, false);
            //CreateNewWaitCommand(1.5f);
            CreateSave();
        }
        else
        {
            CreateNewMoveToViewCommand(1, BGName + "View-Front");
            cameraMove = "F";
            previousBg = BGName;
            CreateNewSayCommand(pickNameDialogue, PICK_FEMALE_NAME);
            currentBlock = CreateNewBlock("Get Female Name", EventHandlerType.Type_EndEdit);
            CreateNewGetTextCommand(FemaleNameInputField);
            currentBlock = CreateNewBlock("Submit Female Name", EventHandlerType.Type_ButtonClicked);
            CreateNewSetInteractableCommand(FemaleNameInputField, false);
            SayDialog s = GetSayDialogue(PICK_FEMALE_NAME);
            CreateNewFadeCanvasGroupCommand(s.gameObject.GetComponent<CanvasGroup>(), false, 0);
            CreateNewSetActiveCommand(s.gameObject, false);
            //CreateNewWaitCommand(1.5f);
            CreateSave();
        }
    }

    public void PickCharacterSelection(string BGName, bool male = true)
    {
        if (male)
        {
            CreateSave();
            CreateNewCommentCommand(MALE_CHARACTER_SELECTION);
            CreateNewFadeToViewCommand(3.2f, true, "BlackBg-View");
            CreateNewSetActiveCommand(maleCharacterSelectionScreen.gameObject, true);
            CreateNewFadeCanvasGroupCommand(maleCharacterSelectionScreen, true, 1);
            currentBlock = CreateNewBlock("Submit Male Character");
            CreateNewFadeCanvasGroupCommand(maleCharacterSelectionScreen, false, 0);
            CreateNewSetActiveCommand(maleCharacterSelectionScreen.gameObject, false);
            CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
            cameraMove = "F";
            previousBg = BGName;
        }
        else
        {
            CreateSave();
            CreateNewCommentCommand(FEMALE_CHARACTER_SELECTION);
            CreateNewFadeToViewCommand(3.2f, true, "BlackBg-View");
            CreateNewSetActiveCommand(femaleCharacterSelectionScreen.gameObject, true);
            CreateNewFadeCanvasGroupCommand(femaleCharacterSelectionScreen, true, 1);
            currentBlock = CreateNewBlock("Submit Female Character");
            CreateNewFadeCanvasGroupCommand(femaleCharacterSelectionScreen, false, 0);
            CreateNewSetActiveCommand(femaleCharacterSelectionScreen.gameObject, false);
            CreateNewFadeToViewCommand(2, true, BGName + "View-Front");
            cameraMove = "F";
            previousBg = BGName;
        }
    }

    public Block CreateNewBlock(string blockBGName = "Game Start")
    {
        Block newBlock = flowchart.CreateBlock(newBlockPosition);
        newBlock.BlockName = blockNumber.ToString("D2") + " - " + blockBGName;
        newBlockPosition += new Vector2(0, 85);
        blockNumber++;
        return newBlock;
    }

    public Block CreateNewBlock(string blockBGName = "Game Start", EventHandlerType handlerType = EventHandlerType.Type_None)
    {
        Block newBlock = flowchart.CreateBlock(newBlockPosition);

        switch (handlerType)
        {
            case EventHandlerType.Type_None:
                break;
            case EventHandlerType.Type_EndEdit:

                EndEdit endEditEH = flowchart.gameObject.AddComponent<EndEdit>();
                endEditEH.ParentBlock = newBlock;
                newBlock._EventHandler = endEditEH;

                break;
            case EventHandlerType.Type_ButtonClicked:

                ButtonClicked btnClickedEH = flowchart.gameObject.AddComponent<ButtonClicked>();
                btnClickedEH.ParentBlock = newBlock;
                newBlock._EventHandler = btnClickedEH;

                break;

            case EventHandlerType.Type_MessageReceived:

                MessageReceived msgEH = flowchart.gameObject.AddComponent<MessageReceived>();
                msgEH.ParentBlock = newBlock;
                newBlock._EventHandler = msgEH;

                break;
        }       

        newBlock.BlockName = blockNumber.ToString("D2") + " - " + blockBGName;
        newBlockPosition += new Vector2(0, 85);
        blockNumber++;
        return newBlock;
    }

    public void AddCommandToFlowBlock(Command newCommand)
    {
        flowchart.AddSelectedCommand(newCommand);
        newCommand.ParentBlock = currentBlock;
        newCommand.ItemId = flowchart.NextItemId();
        newCommand.OnCommandAdded(currentBlock);
        currentBlock.CommandList.Add(newCommand);
    }

    public View GetView(string viewName)
    {
        foreach (View v in views)
        {
            if (v.gameObject.name == viewName)
                return v;
        }

        return null;
    }

    public Character GetCharacter(string characterName)
    {
        foreach (Character c in characters)
        {
            if (c.gameObject.name == characterName)
                return c;
        }

        return null;
    }

    public SayDialog GetSayDialogue(string sayDialogue)
    {
        foreach (SayDialog s in sayDialogues)
        {
            if (s.gameObject.name == sayDialogue)
                return s;
        }

        return null;
    }

    public void CreateNewFadeToViewCommand(float duration, bool fadeout, string viewName)
    {
        View targetView = GetView(viewName);

        if (targetView == null)
        {
            Debug.Log("No View Of Selected Name " + viewName);
        }

        FadeToView newFadeToViewCommand = currentBlock.gameObject.AddComponent<FadeToView>();
        newFadeToViewCommand.FadeToViewInit(duration, fadeout, targetView);
        AddCommandToFlowBlock(newFadeToViewCommand);
    }

    public void CreateNewMoveToViewCommand(float duration, string viewName, bool waitFinish = true)
    {
        View targetView = GetView(viewName);

        if (targetView == null)
        {
            Debug.Log("No View Of Selected Name " + viewName);
        }

        MoveToView newMoveToViewCommand = currentBlock.gameObject.AddComponent<MoveToView>();
        newMoveToViewCommand.MoveToViewInit(duration, targetView, waitFinish);
        AddCommandToFlowBlock(newMoveToViewCommand);
    }

    public void CreateNewCallMethodCommand(GameObject targetObject, string MethodName)
    {
        CallMethod newCallMethodCommand = currentBlock.gameObject.AddComponent<CallMethod>();
        newCallMethodCommand.CallMethodInit(targetObject, MethodName);
        AddCommandToFlowBlock(newCallMethodCommand);
    }

    public void CreateNewWaitCommand(float duration = 1)
    {
        Wait newWaitCommand = currentBlock.gameObject.AddComponent<Wait>();
        newWaitCommand.WaitInit(duration);
        AddCommandToFlowBlock(newWaitCommand);
    }

    public void CreateNewCallMusicMethodCommand(GameObject targetObject, string MethodName, int musicIndex, float delay = 0)
    {
        CallMethodMusic newCallMusicMethodCommand = currentBlock.gameObject.AddComponent<CallMethodMusic>();
        newCallMusicMethodCommand.CallMethodMusicInit(targetObject, MethodName, delay, musicIndex);
        AddCommandToFlowBlock(newCallMusicMethodCommand);
    }

    public void CreateNewSayCommand(string dialogue, string newSayDialogue, bool fadeDone = true, bool waitClick = true, string newActor = "")
    {
        Character actor = null;

        if (newActor != "")
        {
            actor = GetCharacter(newActor);

            if (actor == null)
            {
                Debug.Log("No Character Of Selected Name " + actor);
            }
        }

        SayDialog sayDialogue = GetSayDialogue(newSayDialogue);

        if (sayDialogue == null)
        {
            Debug.Log("No SayDialogue Of Selected Name " + sayDialogue);
        }

        Say newSayCommand = currentBlock.gameObject.AddComponent<Say>();
        newSayCommand.SayInit(dialogue, actor, sayDialogue, fadeDone, waitClick);
        AddCommandToFlowBlock(newSayCommand);
    }

    public void CreateNewSetActiveCommand(GameObject target, bool isActive = true)
    {
        SetActive newSetActiveCommand = currentBlock.gameObject.AddComponent<SetActive>();
        newSetActiveCommand.SetActiveInit(target, isActive);
        AddCommandToFlowBlock(newSetActiveCommand);
    }

    public void CreateNewZoomCommand(float newStartdelay = 0, float newDuration = 6, float newTargetZoom = 19)
    {
        CameraZoom newZoomCommand = currentBlock.gameObject.AddComponent<CameraZoom>();
        newZoomCommand.ZoomInit(newStartdelay, newDuration, newTargetZoom);
        AddCommandToFlowBlock(newZoomCommand);
    }

    public void CreateNewWaitForClickCommand()
    {
        WaitPlayerInput newWaitForClickCommand = currentBlock.gameObject.AddComponent<WaitPlayerInput>();
        AddCommandToFlowBlock(newWaitForClickCommand);
    }

    public void CreateNewSetAudioVolumeCommand(float newVolume, float newFadeDuration, bool waitFinish = false)
    {
        SetAudioVolume newSetAudioVolumeCommand = currentBlock.gameObject.AddComponent<SetAudioVolume>();
        newSetAudioVolumeCommand.SetAudioVolumeInit(newVolume, newFadeDuration, waitFinish);
        AddCommandToFlowBlock(newSetAudioVolumeCommand);
    }

    public void CreateNewPlaySoundCommand(AudioClip newSoundClip, bool waitFinish = false)
    {
        PlaySound newPlaySoundCommand = currentBlock.gameObject.AddComponent<PlaySound>();
        newPlaySoundCommand.PlaySoundInit(newSoundClip, waitFinish);
        AddCommandToFlowBlock(newPlaySoundCommand);
    }

    public void CreateNewCallCommand(Flowchart newFlowChart, Block newlyCreatedBlock)
    {
        Call newCallCommand = currentBlock.gameObject.AddComponent<Call>();
        newCallCommand.CallInit(newFlowChart, newlyCreatedBlock);
        AddCommandToFlowBlock(newCallCommand);
    }

    public void CreateNewMenuCommand(string menuText, int diamondCost = 0)
    {
        Block newBlock = CreateNewBlock(menuText);
        MenuBlocksList.Add(newBlock);
        MenuMflix newMenuCommand = currentBlock.gameObject.AddComponent<MenuMflix>();
        newMenuCommand.MenuInit(menuText, newBlock, episodeInProgress.GetComponentInChildren<MenuDialogMflix>(), diamondCost);
        AddCommandToFlowBlock(newMenuCommand);
    }

    public void CreateNewPortraitCommand(bool newShow, string newCharacter, string toPositionName, bool facingLeft = true, bool newMove = true, bool newDefaultSettings = true, float newFadeDuration = 0.5f, string fromPositionName = "", string dress = "", string hair = "", string expression = "")
    {
        Stage s = episodeInProgress.GetComponentInChildren<Stage>();

        string actorName = "";
        string actorPortraitName = "";

        if (newCharacter == "Character_Male-LookType-0")
        {
            actorName = "Character_Male-LookType-0";
            actorPortraitName = "Boy";
        }
        else if (newCharacter == "Character_Female-LookType-0")
        {
            actorName = "Character_Female-LookType-0";
            actorPortraitName = "Girl";
        }
        else if (newCharacter == "Character_Male")
        {
            actorName = "Character_Male";
            actorPortraitName = "Boy";
        }
        else if (newCharacter == "Character_Female")
        {
            actorName = "Character_Female";
            actorPortraitName = "Girl";
        }
        else
        {
            actorName = newCharacter;
            actorPortraitName = newCharacter;
        }

        if (expression == "Phone")
            actorPortraitName = "z" + actorPortraitName;

        Character actor = GetCharacter(actorName);
        string portraitName = actorPortraitName + "_" + dress + "_" + hair + "_" + expression;
        portraitName = portraitName.Trim();

        if (portraitName.Contains("___"))
            portraitName = portraitName.Replace("___", "_");
        else if (portraitName.Contains("__"))
            portraitName = portraitName.Replace("__", "_");

        if (portraitName.EndsWith("_"))
            portraitName = portraitName.Replace("_", "");

        Sprite newPortrait = actor.GetPortrait(portraitName);

        if (actor == null)
        {
            Debug.Log("No Character Of Selected Name " + actor);
        }

        RectTransform newToPosition = s.GetPosition(toPositionName);
        RectTransform newFromPosition = s.GetPosition(fromPositionName);

        Portrait newPortraitCommand = currentBlock.gameObject.AddComponent<Portrait>();
        newPortraitCommand.PortraitInit(newShow, actor, newPortrait, facingLeft, newMove, newFromPosition, newToPosition, newDefaultSettings, newFadeDuration);
        AddCommandToFlowBlock(newPortraitCommand);
    }

    public void CreateNewGetTextCommand(GameObject nameInputField)
    {
        GetText newGetTextCommand = currentBlock.gameObject.AddComponent<GetText>();
        newGetTextCommand.GetTextInit(nameInputField);
        AddCommandToFlowBlock(newGetTextCommand);
    }

    public void CreateNewFadeCanvasGroupCommand(CanvasGroup newCanvas, bool blockRaycast, float newTargetAlpha)
    {
        FadeCanvasGroup newFadeCanvasGroupCommand = currentBlock.gameObject.AddComponent<FadeCanvasGroup>();
        newFadeCanvasGroupCommand.FadeCanvasGroupInit(newCanvas, blockRaycast, newTargetAlpha);
        AddCommandToFlowBlock(newFadeCanvasGroupCommand);
    }

    public void CreateNewSetInteractableCommand(GameObject newNameInputField, bool interactState)
    {
        SetInteractable newSetInteractableCommand = currentBlock.gameObject.AddComponent<SetInteractable>();
        newSetInteractableCommand.SetInteractableInit(newNameInputField, interactState);
        AddCommandToFlowBlock(newSetInteractableCommand);
    }

    public void CreateNewSetLanguageCommand()
    {
        SetLanguage newSetLanguageCommand = currentBlock.gameObject.AddComponent<SetLanguage>();
        newSetLanguageCommand.enabled = false;
        AddCommandToFlowBlock(newSetLanguageCommand);
    }

    public void CreateNewCommentCommand(string commentName, string commentText = "")
    {
        Comment newCommentCommand = currentBlock.gameObject.AddComponent<Comment>();
        newCommentCommand.CommentInit(commentName, commentText);
        AddCommandToFlowBlock(newCommentCommand);
    }

    public void CreateNewCameraShakeCommand(float newDuration = 1.5f, float amountX = 1, float amountY = 1, bool newWait = false)
    {
        ShakeCamera newCameraShakeCommand = currentBlock.gameObject.AddComponent<ShakeCamera>();
        newCameraShakeCommand.CameraShakeInit(newDuration, amountX, amountY, newWait);
        AddCommandToFlowBlock(newCameraShakeCommand);
    }

    public void CreateNewIFCommand()
    {
        If newIFCommand = currentBlock.gameObject.AddComponent<If>();
        AddCommandToFlowBlock(newIFCommand);
    }

    public void CreateNewELSECommand()
    {
        Else newELSECommand = currentBlock.gameObject.AddComponent<Else>();
        AddCommandToFlowBlock(newELSECommand);
    }

    public void CreateNewELSEIFCommand()
    {
        ElseIf newELSEIFCommand = currentBlock.gameObject.AddComponent<ElseIf>();
        AddCommandToFlowBlock(newELSEIFCommand);
    }

    public void CreateNewENDCommand()
    {
        End newENDCommand = currentBlock.gameObject.AddComponent<End>();
        AddCommandToFlowBlock(newENDCommand);
    }

    public void CreateNewSetAnimTriggerCommand(Animator a, string parameterName = "up")
    {
        SetAnimTrigger newAnimationCommand = currentBlock.gameObject.AddComponent<SetAnimTrigger>();
        newAnimationCommand.SetAnimTriggerInit(a, parameterName);
        AddCommandToFlowBlock(newAnimationCommand);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Automaton))]
    public class AddBlocksDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Automaton createEpisode = target as Automaton;

            if (GUILayout.Button("Create Episode"))
                createEpisode.CreateEpisode();
        }
    }
#endif
}
