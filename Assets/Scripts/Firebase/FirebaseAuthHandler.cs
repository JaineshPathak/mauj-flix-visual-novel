using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthHandler : MonoBehaviour
{
    public static FirebaseAuthHandler instance;

    public FirebaseAuth auth;
    public FirebaseUser userCurrent;

    public string webClientID = "315332966088-tfmrn3krajj9vvmquoeog0ckvdouepov.apps.googleusercontent.com";

    private bool isFirebaseAuthInitialized;

    public static event Action<FirebaseUser> OnFirebaseUserAccountLoaded;
    public static event Action<FirebaseUser> OnFirebaseUserAccountDeleted;
    public static event Action<FirebaseUser> OnFirebaseUserAccountSignedIn;

    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        configuration = new GoogleSignInConfiguration();
        configuration.RequestEmail = true;
        configuration.RequestProfile = true;
        configuration.RequestIdToken = true;
        configuration.WebClientId = webClientID;
    }

    /*private void Start()
    {
        InitFirebaseAuthHandler();        
    }*/

    [ContextMenu("Delete User")]
    public void DeleteUser()
    {
        if (userCurrent == null)
            return;

        OnFirebaseUserAccountDeleted?.Invoke(userCurrent);
        Debug.LogFormat("Firebase Auth: Anonymous User Deletion STARTED: {0}", userCurrent.UserId);
        userCurrent.DeleteAsync().ContinueWith(task => 
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Firebase Auth: Anonymous User Deletion was CANCELED.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Auth: Anonymous User Deletion encountered an ERROR: " + task.Exception);
                return;
            }
            
            Debug.LogFormat("Firebase Auth: Anonymous User Deletion COMPLETED: {0}", userCurrent.UserId);
        });
    }

    public void InitFirebaseAuthHandler()
    {
        if (isFirebaseAuthInitialized)
            return;

        //if(FirebaseApp.DefaultInstance != null)
            //InitFirebaseAuth();        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitFirebaseAuth();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(string.Format("Firebase Auth: Could not resolve all Firebase Database dependencies: {0}", dependencyStatus));                
            }
        });
    }

    private void InitFirebaseAuth()
    {
        isFirebaseAuthInitialized = true;

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanged;        

        if (auth.CurrentUser == null)
        {
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Firebase Auth: SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Firebase Auth: SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                userCurrent = task.Result;
                OnFirebaseUserAccountLoaded?.Invoke(userCurrent);
                Debug.LogFormat("Firebase Auth: [NEW USER] User signed in anonymously successfully: {0}, ({1})", userCurrent.DisplayName, userCurrent.UserId);
            });
        }
        else if(auth.CurrentUser != null)
        {
            userCurrent = auth.CurrentUser;
            OnFirebaseUserAccountLoaded?.Invoke(userCurrent);
            Debug.LogFormat("Firebase Auth: [EXISTING USER] User signed in anonymously successfully: {0}, ({1})", userCurrent.DisplayName, userCurrent.UserId);                        
        }
    }

    private void OnAuthStateChanged(object sender, EventArgs e)
    {                
    }

    public void OnGoogleSignOut()
    {
        if (auth.CurrentUser == null || GetProviderID() != "google.com")
            return;

        auth.CurrentUser.UnlinkAsync("google.com").ContinueWith(unlinkTask =>
        {
            if (unlinkTask.IsCanceled)
                Debug.Log("Firebase Auth: Unlinking Google Account Canceled");
            else if (unlinkTask.IsFaulted)
                Debug.Log("Firebase Auth: Unlinking Google Account Failed: " + unlinkTask.Exception);
            else if (unlinkTask.IsCompleted)
            {
                Debug.Log("Firebase Auth: Unlinking Google Account Successful");

                userCurrent = unlinkTask.Result;
                OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                GoogleSignIn.DefaultInstance.SignOut();

                UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseAnonymousProviderID));
            }
        });
    }

    public void OnGoogleSignOut(Action<Task> unlinkTaskCallback)
    {
        if (auth.CurrentUser == null || GetProviderID() != "google.com")
            return;

        auth.CurrentUser.UnlinkAsync("google.com").ContinueWith(unlinkTask =>
        {
            if (unlinkTask.IsCanceled)
                Debug.Log("Firebase Auth: Unlinking Google Account Canceled");
            else if (unlinkTask.IsFaulted)
                Debug.Log("Firebase Auth: Unlinking Google Account Failed: " + unlinkTask.Exception);
            else if (unlinkTask.IsCompleted)
            {
                Debug.Log("Firebase Auth: Unlinking Google Account Successful");

                userCurrent = unlinkTask.Result;
                OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                GoogleSignIn.DefaultInstance.SignOut();

                UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseAnonymousProviderID));

                unlinkTaskCallback?.Invoke(unlinkTask);
            }
        });
    }

    public void OnGoogleSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
                signInCompleted.SetCanceled();
            else if (task.IsFaulted)
                signInCompleted.SetException(task.Exception);
            else
            {                
                Credential googleCredentials = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                /*auth.CurrentUser.LinkWithCredentialAsync(googleCredentials).ContinueWith(signTask =>
                {                    
                    if (signTask.IsCanceled)
                    {
                        Debug.LogError("Firebase Auth: LinkWithCredentialAsync was canceled.");
                        return;
                    }
                    else if (signTask.IsFaulted)
                    {
                        Debug.LogError("Firebase Auth: LinkWithCredentialAsync encountered an error: " + signTask.Exception);
                        return;
                    }
                    else if (signTask.IsCompleted)
                    {
                        userCurrent = signTask.Result;
                        Debug.LogFormat("Firebase Auth: Credentials successfully linked to Firebase user: {0} ({1})", userCurrent.DisplayName, userCurrent.UserId);

                        OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                        UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));

                        GetProvidersDataFromEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));
                    }
                });*/

                auth.SignInWithCredentialAsync(googleCredentials).ContinueWith(signTask => 
                {
                    if(signIn.IsCompleted)
                    {
                        userCurrent = signTask.Result;
                        Debug.LogFormat("Firebase Auth: Credentials successfully linked to Firebase user: {0} ({1})", userCurrent.UserId, userCurrent.DisplayName);

                        OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                        UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));
                        GetProvidersDataFromEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));

                        OnFirebaseUserAccountSignedIn?.Invoke(userCurrent);
                    }
                }); 
            }
        });
    }

    public void OnGoogleSignIn(Action<Task<GoogleSignInUser>> completedTaskCallback)
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
                signInCompleted.SetCanceled();
            else if (task.IsFaulted)
                signInCompleted.SetException(task.Exception);
            else
            {
                completedTaskCallback?.Invoke(signIn);

                Credential googleCredentials = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                /*auth.CurrentUser.LinkWithCredentialAsync(googleCredentials).ContinueWith(signTask =>
                {                    
                    if (signTask.IsCanceled)
                    {
                        Debug.LogError("Firebase Auth: LinkWithCredentialAsync was canceled.");
                        return;
                    }
                    else if (signTask.IsFaulted)
                    {
                        Debug.LogError("Firebase Auth: LinkWithCredentialAsync encountered an error: " + signTask.Exception);
                        return;
                    }
                    else if (signTask.IsCompleted)
                    {
                        userCurrent = signTask.Result;
                        Debug.LogFormat("Firebase Auth: Credentials successfully linked to Firebase user: {0} ({1})", userCurrent.DisplayName, userCurrent.UserId);

                        OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                        UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));

                        GetProvidersDataFromEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));
                    }
                });*/

                auth.SignInWithCredentialAsync(googleCredentials).ContinueWith(signTask =>
                {
                    if (signIn.IsCompleted)
                    {
                        userCurrent = signTask.Result;
                        Debug.LogFormat("Firebase Auth: Credentials successfully linked to Firebase user: {0} ({1})", userCurrent.UserId, userCurrent.DisplayName);

                        OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                        UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));
                        GetProvidersDataFromEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));

                        OnFirebaseUserAccountSignedIn?.Invoke(userCurrent);
                    }
                });
            }
        });
    }

    public void OnGoogleSignInSilent(Action<Task<GoogleSignInUser>> completedTaskCallback)
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignInSilently();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
                signInCompleted.SetCanceled();
            else if (task.IsFaulted)
                signInCompleted.SetException(task.Exception);
            else
            {
                completedTaskCallback?.Invoke(signIn);

                Credential googleCredentials = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);                

                auth.SignInWithCredentialAsync(googleCredentials).ContinueWith(signTask =>
                {
                    if (signIn.IsCompleted)
                    {
                        userCurrent = signTask.Result;
                        Debug.LogFormat("Firebase Auth: Google Credentials successfully linked to Firebase user: {0} ({1})", userCurrent.UserId, userCurrent.DisplayName);

                        OnFirebaseUserAccountLoaded?.Invoke(userCurrent);

                        UpdateUserEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));
                        GetProvidersDataFromEmail(GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID));

                        OnFirebaseUserAccountSignedIn?.Invoke(userCurrent);
                    }
                });
            }
        });
    }

    private void UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null || newEmail.Length == 0)
            return;

        auth.CurrentUser.UpdateEmailAsync(newEmail).ContinueWith(updateEmailTask =>
        {
            if (updateEmailTask.IsCanceled)
                Debug.Log($"Firebase Auth: Update Email: {newEmail}, Task Canceled: " + updateEmailTask.Exception);
            else if (updateEmailTask.IsFaulted)
                Debug.Log($"Firebase Auth: Update Email: {newEmail}, Task Failed: " + updateEmailTask.Exception);
            else if (updateEmailTask.IsCompleted)
                Debug.Log($"Firebase Auth: Update Email: {newEmail}, Task Completed");
        });
    }

    private void GetProvidersDataFromEmail(string email)
    {
        if (auth.CurrentUser == null || email.Length == 0)
            return;

        auth.FetchProvidersForEmailAsync(email).ContinueWith(dataTask => 
        {
            if (dataTask.IsCanceled)
                Debug.Log("Firebase Auth: Getting Providers from Email task Failed: " + dataTask.Exception);
            else if (dataTask.IsFaulted)
                Debug.Log("Firebase Auth: Getting Providers from Email task Failed: " + dataTask.Exception);
            else if (dataTask.IsCompleted)
            {
                Debug.Log("Firebase Auth: Getting Providers from Email Task completed GIVING NOW:");

                List<string> allProviders = ((IEnumerable<string>)dataTask.Result).ToList();

                for (int i = 0; i < allProviders.Count; i++)                
                    Debug.Log("Firebase Auth: FETCHED PROVIDER ID: " + allProviders[i]);                
            }
        });
    }

    public string GetProviderID()
    {
        var provider = string.Empty;
        var enumerator = userCurrent.ProviderData.GetEnumerator();

        while (enumerator.MoveNext())
        {
            provider = enumerator.Current.ProviderId;
            if (provider != "firebase")
                return provider;
        }

        return provider;
    }
    
    public string GetDisplayNameFromProviderID(string providerID)
    {
        var displayName = string.Empty;
        var enumerator = userCurrent.ProviderData.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var provider = enumerator.Current.ProviderId;
            if (provider == providerID)
            {
                displayName = enumerator.Current.DisplayName;
                return displayName;
            }
        }

        return displayName;
    }

    public string GetEmailFromProviderID(string providerID)
    {
        var userEmail = string.Empty;
        var enumerator = userCurrent.ProviderData.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var provider = enumerator.Current.ProviderId;
            if (provider == providerID)
            {                
                userEmail = enumerator.Current.Email;
                return userEmail;
            }
        }

        return userEmail;
    }
}
