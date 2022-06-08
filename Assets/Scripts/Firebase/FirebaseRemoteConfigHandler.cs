using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using HtmlAgilityPack;

public class FirebaseRemoteConfigHandler : MonoBehaviourSingletonPersistent<FirebaseRemoteConfigHandler>
{
    public bool testMode;
    public string testAppVersion = "0.1.4.2.2";
    public bool doVersionCheck;    

    //public static FirebaseRemoteConfigHandler instance;

    private const string gameAppVersionSetting = "GameAppVersion";
    private const string versionCheckTypeSetting = "VersionCheckType";

    private const string PLAYSTOREURL = "https://play.google.com/store/apps/details?id=com.culttales.maujflix";

    public static event Action OnAppVersionCorrect;
    public static event Action OnAppVersionIncorrect;
    public static event Action OnAppVersionIgnoreCheck;

    private bool isFirebaseRCInitialized;
    [HideInInspector] public bool isUpdateBtnClicked;

    private UISplashScreen splashScreen;

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }*/

    private void OnApplicationFocus(bool focus)
    {
        if(focus && isUpdateBtnClicked)
        {
            //FetchDataAsync();
            if (splashScreen == null)
                splashScreen = FindObjectOfType<UISplashScreen>();

            if (splashScreen != null)
            {
                splashScreen.StepOneVersionCheck();
                FetchDataAsync();
            }

            isUpdateBtnClicked = false;
        }
    }

    public void CheckVersionAgain()
    {
        if (isUpdateBtnClicked)
        {
            //FetchDataAsync();
            if (splashScreen == null)
                splashScreen = FindObjectOfType<UISplashScreen>();

            if (splashScreen != null)
            {
                splashScreen.StepOneVersionCheck();
                FetchDataAsync();
            }

            isUpdateBtnClicked = false;
        }
    }

    public void InitFirebaseRemoteConfigHandler()
    {
        if (isFirebaseRCInitialized)
            return;

        if (FirebaseApp.DefaultInstance != null)
        {
            Debug.Log("Firebase Remote Config: FirebaseApp Default Instance Found. Initialising...");
            InitFirebaseRemoteConfig();
        }
        else
            Debug.LogError("Firebase Remote Config: FirebaseApp Default Instance Not Found!");
        
        /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitFirebaseRemoteConfig();
            }
            else
            {
                Debug.LogError(string.Format("Firebase Remote Config: Could not resolve all dependencies: {0}", dependencyStatus));                
            }
        });*/
    }    

    private void InitFirebaseRemoteConfig()
    {
        isFirebaseRCInitialized = true;

        if (!doVersionCheck)
            return;

        //Check the App version
        //1. Get version from Firebase Remote Config using "GameAppVersion"
        //2. Compare it with current version using Application.version
        //3. Correct - then start splash screen sequence
        //4. Incorrect - Display Popup to update the game
        
        FetchDataAsync();
    }

    public Task FetchDataAsync()
    {

#if UNITY_EDITOR
        Debug.Log("Firebase Remote Config: Fetching data...");
#endif

        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }    

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: Fetch canceled.");
#endif
        }
        else if (fetchTask.IsFaulted)
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: Fetch encountered an error.");
#endif
        }
        else if (fetchTask.IsCompleted)
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: Fetch completed successfully!");
#endif
        }

        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task => 
                {
#if UNITY_EDITOR
                    Debug.Log(string.Format("Firebase Remote Config: Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
#endif
                    if(testMode)
                        CheckGameAppVersionTestMode();
                    else
                        CheckGameAppVersion();
                });

                break;

            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
#if UNITY_EDITOR
                        Debug.Log("Firebase Remote Config: Fetch failed for unknown reason");
#endif
                        break;

                    case FetchFailureReason.Throttled:
#if UNITY_EDITOR
                        Debug.Log("Firebase Remote Config: Fetch throttled until " + info.ThrottledEndTime);
#endif
                        break;
                }
                break;

            case LastFetchStatus.Pending:
#if UNITY_EDITOR
                Debug.Log("Firebase Remote Config: Latest Fetch call still pending.");
#endif
                break;
        }
    }

    private void CheckGameAppVersion()
    {
        string remoteAppVersion = string.Empty;
        
        int checkVersionType = (int)FirebaseRemoteConfig.DefaultInstance.GetValue(versionCheckTypeSetting).LongValue;
        switch(checkVersionType)
        {
            case 0:     //Check using Remote Config

                GetVersionUsingRemoteConfig(remoteAppVersion);

                break;

            case 1:     //Check using HTML Parser

                GetVersionUsingHTMLParser(remoteAppVersion);

                break;
        }        
    }

    private void GetVersionUsingRemoteConfig(string remoteAppVersion)
    {
        remoteAppVersion = FirebaseRemoteConfig.DefaultInstance.GetValue(gameAppVersionSetting).StringValue;

        if (remoteAppVersion.Length > 0)
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: [ 0 - Using Remote Config ]");
            Debug.Log("Firebase Remote Config: APP VERSION: " + Application.version);
            Debug.Log("Firebase Remote Config: REMOTE APP VERSION: " + remoteAppVersion);
#endif

            if (Application.version.Equals(remoteAppVersion))        //Version match
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Remote Config: VERSION MATCH! GO AHEAD!");
#endif
                OnAppVersionCorrect?.Invoke();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Remote Config: VERSION MISMATCH! SHOW UPDATE POPUP!");      //Version mismatch
#endif
                OnAppVersionIncorrect?.Invoke();
            }
        }
    }

    private void GetVersionUsingHTMLParser(string remoteAppVersion)
    {
        remoteAppVersion = GetVersionFromURL();

        if (remoteAppVersion.Length > 0)
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: [ 1 - Using HTML Parser Config ]");
            Debug.Log("Firebase Remote Config: APP VERSION: " + Application.version);
            Debug.Log("Firebase Remote Config: REMOTE APP VERSION: " + remoteAppVersion);
#endif

            if (Application.version.Equals(remoteAppVersion))        //Version match
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Remote Config: VERSION MATCH! GO AHEAD!");
#endif
                OnAppVersionCorrect?.Invoke();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Firebase Remote Config: VERSION MISMATCH! SHOW UPDATE POPUP!");      //Version mismatch
#endif
                OnAppVersionIncorrect?.Invoke();
            }
        }
        else
            GetVersionUsingRemoteConfig(remoteAppVersion);      //If HTML Parser doesn't work then fallback to Remote Config method
    }

    private string GetVersionFromURL()
    {
        string currentVersion = string.Empty;

        if (PLAYSTOREURL.Length <= 0)
            return currentVersion;

        HtmlWeb playStoreWeb = new HtmlWeb();
        HtmlDocument playStoreDoc = playStoreWeb.Load(PLAYSTOREURL);

        if(playStoreDoc != null)
        {
            var playStoreBody = playStoreDoc.DocumentNode.SelectSingleNode("//body");            

            //As long as Google dont change their Play Store webpage we are fine. Otherwise switch the CheckType to Remote Config Type
            foreach (var node in playStoreBody.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element)
                {                    
                    if (node.InnerText.Equals("Current Version"))
                    {
                        return node.NextSibling.InnerText;
                    }
                }
            }
        }

        return currentVersion;
    }
    
    private void CheckGameAppVersionTestMode()
    {
#if UNITY_EDITOR
        Debug.Log("Firebase Remote Config: APP VERSION: " + Application.version);
        Debug.Log("Firebase Remote Config: TEST APP VERSION: " + testAppVersion);
#endif

        if (Application.version.Equals(testAppVersion))        //Test Version match
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: TEST MODE VERSION MATCH! GO AHEAD!");
#endif
            OnAppVersionCorrect?.Invoke();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Firebase Remote Config: TEST MODE VERSION MISMATCH! SHOW UPDATE POPUP!");      //Test Version mismatch
#endif
            OnAppVersionIncorrect?.Invoke();
        }
    }
}