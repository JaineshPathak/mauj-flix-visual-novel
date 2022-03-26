using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;

public class FirebaseRemoteConfigHandler : MonoBehaviour
{
    public bool testMode;
    public string testAppVersion = "0.1.4.2.2";
    public bool doVersionCheck;    

    public static FirebaseRemoteConfigHandler instance;

    private const string gameAppVersionSetting = "GameAppVersion";

    public static event Action OnAppVersionCorrect;
    public static event Action OnAppVersionIncorrect;
    public static event Action OnAppVersionIgnoreCheck;

    private bool isFirebaseRCInitialized;
    [HideInInspector] public bool isUpdateBtnClicked;

    private UISplashScreen splashScreen;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

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

        //if (FirebaseApp.DefaultInstance != null)
            //InitFirebaseRemoteConfig();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
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
        });
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
        string remoteAppVersion = FirebaseRemoteConfig.DefaultInstance.GetValue(gameAppVersionSetting).StringValue;

#if UNITY_EDITOR
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