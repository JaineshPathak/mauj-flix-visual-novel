using UnityEngine;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseDBHandler : MonoBehaviour
{
    public static FirebaseDBHandler instance;

    private DatabaseReference databaseReference;
    private FirebaseDatabase firebaseDB;

    private bool isFirebaseDBInitialized;

    public const string viewCountKeyEnd = "-View-Count";
    public const string likeCountKeyEnd = "-Like-Count";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void InitFirebaseDBHandler()
    {
        if (isFirebaseDBInitialized)
            return;

        //InitFirebaseDatabase();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitFirebaseDatabase();
            }
            else
            {
                Debug.LogError(string.Format("Firebase Database: Could not resolve all Firebase Database dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void InitFirebaseDatabase()
    {
        isFirebaseDBInitialized = true;

        firebaseDB = FirebaseDatabase.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void GetCountFromFirebaseDB(string reference, Action<string> OnValueReceived)
    {
        if (!isFirebaseDBInitialized)
        {
            Debug.LogError("Firebase Database not Initialized; Can't get the Count from Key: " + reference);
            return;
        }

        firebaseDB.GetReference(reference).ValueChanged += (object sender, ValueChangedEventArgs args) => 
        {
            if(args.DatabaseError != null)
            {
                Debug.LogError("Firebase Database Error: " + args.DatabaseError.Message);
                return;
            }
            else
            {
                DataSnapshot dataSnapshot = args.Snapshot;

                float valF = float.Parse(dataSnapshot.Value.ToString());                
                string valS = AbbrevationUtility.AbbreviateNumber(valF);

                OnValueReceived?.Invoke(valS);
            }

            //DataSnapshot dataSnapshot = args.Snapshot;
            //print(reference + ": " + dataSnapshot.Value.ToString());
        };        
    }

    public void UpdateViewCount(string reference)
    {
        if (!isFirebaseDBInitialized)
        {
            Debug.LogError("Firebase Database not Initialized; Can't update Key: " + reference);
            return;
        }

        firebaseDB.GetReference(reference).GetValueAsync().ContinueWith(task => 
        {
            if(task.IsFaulted)
            {
                Debug.LogError("Firebase Database Error: " + task + ", Reference: " + reference);
            }
            else if(task.IsCompleted)
            {
                DataSnapshot viewSnapShot = task.Result;
                int val = int.Parse(viewSnapShot.Value.ToString());
                val++;
                databaseReference.Child(reference).SetValueAsync(val);
            }
        });
    }

    public void LikesCountIncrement(string reference)
    {
        if (!isFirebaseDBInitialized)
        {
            Debug.LogError("Firebase Database not Initialized; Can't update Key: " + reference);
            return;
        }

        firebaseDB.GetReference(reference).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Database Error: " + task + ", Reference: " + reference);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot viewSnapShot = task.Result;
                int val = int.Parse(viewSnapShot.Value.ToString());
                val++;
                databaseReference.Child(reference).SetValueAsync(val);
            }
        });
    }

    public void LikesCountDecrement(string reference)
    {
        if (!isFirebaseDBInitialized)
        {
            Debug.LogError("Firebase Database not Initialized; Can't update Key: " + reference);
            return;
        }

        firebaseDB.GetReference(reference).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Database Error: " + task + ", Reference: " + reference);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot viewSnapShot = task.Result;
                int val = int.Parse(viewSnapShot.Value.ToString());
                val--;
                databaseReference.Child(reference).SetValueAsync(val);
            }
        });
    }

    public string GetReferenceFromStoryTitle(string storyTitleEng, string keySuffix)
    {
        string finalVal = "";

        finalVal = storyTitleEng;
        finalVal = finalVal.Replace(" ", "");

        return finalVal + keySuffix;
    }
}
