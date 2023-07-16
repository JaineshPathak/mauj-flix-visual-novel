public static class DataPaths
{
    public static string progressDirectoryPath;
    public static string loadProgressPath;
    public static readonly string loadProgressFileExtension = ".json";

    public static readonly string OngoingPath = loadProgressPath + "OnGoing.json";
    public static readonly string OngoingFilename = "OnGoing.json";

    public static readonly string storyDatabaseFileName = "StoriesDB.json";
    public static readonly string storyDatabaseFileNameTest = "StoriesDBTest.json";
    public static readonly string storyImagesLoadDatabaseName = "StoriesLoadImagesDB.json";

    public static readonly string firebaseGoogleProviderID = "google.com";
    public static readonly string firebaseAnonymousProviderID = "(anonymous)";

    public static readonly string dialogueCharacterSayYPosID = "Dialogue_CharacterSay_YPos";
    public static readonly string[] dialogueYPosID = new string[] { "Dialogue_CharacterSay_YPos", 
                                                                    "Dialogue_TutorialSay_YPos",
                                                                    "Dialogue_NarrativeSay_YPos",
                                                                    "Dialogue_NarrativeBlackSay_YPos"};

    public static readonly string socialLinkFB = "Social_Link_FB";
    public static readonly string socialLinkInsta = "Social_Link_Insta";
    public static readonly string socialLinkWhatsapp = "Social_Link_Whatsapp";
    public static readonly string socialLinkTelegram = "Social_Link_Telegram";

    public static readonly string m_UICommentsSectionStatus = "UI_CommentsSection_Status";
    public static readonly string m_ShortDescriptionStatus = "UI_ShortDesc_Status";
}