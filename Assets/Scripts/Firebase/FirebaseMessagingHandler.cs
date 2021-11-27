using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using System;
using System.Threading.Tasks;

public class FirebaseMessagingHandler : MonoBehaviour
{
    public static FirebaseMessagingHandler instance;

    private bool isFirebaseInitialized = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void InitFirebaseMessagingHandler()
    {
        if (isFirebaseInitialized)
            return;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //var app = Firebase.FirebaseApp.DefaultInstance;

                InitFirebaseMessaging();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase Messaging dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void InitFirebaseMessaging()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;

        isFirebaseInitialized = true;

        FirebaseMessaging.RequestPermissionAsync().ContinueWith( task => 
        {
            LogTaskCompletion(task, "RequestPermissionAsync");
        });
    }

    private void OnDestroy()
    {
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
#if FCM_DEBUG
        Debug.Log("Firebase: " + "Received Registration Token: " + token.Token);
#endif
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
#if FCM_DEBUG
        Debug.Log("Firebase: " + "Received a new message from: " + e.Message.From);
#endif
    }

    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;

        if (task.IsCanceled)
        {
#if FCM_DEBUG
            Debug.Log("Firebase: " + operation + " canceled.");
#endif
        }
        else if (task.IsFaulted)
        {
#if FCM_DEBUG
            Debug.Log("Firebase: " + operation + " encounted an error.");
#endif
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                FirebaseException firebaseEx = exception as FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = String.Format("Error.{0}: ", ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
#if FCM_DEBUG
            Debug.Log("Firebase: "+operation + " completed");
#endif
            complete = true;
        }

        return complete;
    }
}
