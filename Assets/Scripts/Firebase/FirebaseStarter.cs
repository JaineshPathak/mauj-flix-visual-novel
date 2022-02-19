using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;

public class FirebaseStarter : MonoBehaviour
{
    public static FirebaseStarter instance;

    public static bool firebaseGlobalInit;
    public FirebaseApp firebaseApp;

    public static event Action OnFirebaseInitDone;

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                OnFirebaseInitDone?.Invoke();
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(string.Format("Firebase Global: Could not resolve all Firebase Global dependencies: {0}", dependencyStatus));                
            }
        });
    }
}