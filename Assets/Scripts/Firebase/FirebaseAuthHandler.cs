using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;

public class FirebaseAuthHandler : MonoBehaviour
{
    public static FirebaseAuthHandler instance;

    public FirebaseAuth auth;
    public FirebaseUser userCurrent;

    private bool isFirebaseAuthInitialized;


    public static event Action<FirebaseUser> OnFirebaseUserAccountLoaded;
    public static event Action<FirebaseUser> OnFirebaseUserAccountDeleted;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitFirebaseAuthHandler();        
    }

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
            Debug.LogFormat("Firebase Auth: [CURRENT USER] User signed in anonymously successfully: {0}, ({1})", userCurrent.DisplayName, userCurrent.UserId);                        
        }
    }

    private void OnAuthStateChanged(object sender, EventArgs e)
    {                
    }
}
