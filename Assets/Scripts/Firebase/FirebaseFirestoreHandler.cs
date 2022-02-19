using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Auth;
using TMPro;

public class FirebaseFirestoreHandler : MonoBehaviour
{
    public static FirebaseFirestoreHandler instance;

    public FirebaseUser firebaseUser;
    public FirestoreUserData firestoreUserData;


    private FirebaseFirestore firestoreDB;
    private DocumentReference userRef;
    private ListenerRegistration firestoreDBListener;

    private string devicePlatformCollection;

    [HideInInspector] public TextMeshProUGUI mainMenuDiamondsText;
    public static event Action<FirebaseFirestoreHandler> OnFirestoreLoaded;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        devicePlatformCollection = "PC Users (TEST)";
#elif UNITY_ANDROID
        devicePlatformCollection = "Android Users";
#elif UNITY_IOS
        devicePlatformCollection = "iOS Users";
#endif
    }

    private void OnEnable()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded += OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted += OnFirebaseUserDeleted;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded -= OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted -= OnFirebaseUserDeleted;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded -= OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted -= OnFirebaseUserDeleted;

        firestoreDBListener.Stop();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        //print(scene.name + " (" + scene.buildIndex + ")");
        if (scene.buildIndex == 1)
        {
            OnFirestoreLoaded?.Invoke(this);

            GetUserDiamondsOnSceneLoad();
        }
        else if (mainMenuDiamondsText != null)
            mainMenuDiamondsText = null;
    }

    private void OnFirebaseUserLoaded(FirebaseUser user)
    {
        if (user == null)
            return;

        firebaseUser = user;

        if (firestoreDB == null)
            firestoreDB = FirebaseFirestore.DefaultInstance;

        userRef = firestoreDB.Collection(devicePlatformCollection).Document(user.UserId);
        firestoreDBListener = userRef.Listen(OnFirestoreUserDataListener);
    }

    private void OnFirestoreUserDataListener(DocumentSnapshot snapshot)
    {
        if(!snapshot.Exists)            //New User
        {            
            firestoreUserData = new FirestoreUserData();
            firestoreUserData.userID = firebaseUser.UserId;
            firestoreUserData.userName = firebaseUser.DisplayName;
            firestoreUserData.userEmail = firebaseUser.Email;
            firestoreUserData.diamondsAmount = 50f;
            
            userRef.SetAsync(firestoreUserData).ContinueWith(task => 
            {
                if(task.IsCompleted)
                    Debug.LogFormat("Firebase Firestore: New User Data added {0}, {1}, {2}, {3}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount);
            });

            if (mainMenuDiamondsText != null)
                mainMenuDiamondsText.text = GetUserDiamondsAmount();
        }
        else                            //Existing User
        {            
            Debug.LogFormat("Firebase Firestore: Existing User Data loaded/changed {0}, {1}, {2}, {3}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount);
            firestoreUserData = snapshot.ConvertTo<FirestoreUserData>();

            if (mainMenuDiamondsText != null)
                mainMenuDiamondsText.text = GetUserDiamondsAmount();
        }
    }

    private void OnFirebaseUserDeleted(FirebaseUser user)
    {
        /*userRef = firestoreDB.Collection(devicePlatformCollection).Document(user.UserId);
        userRef.DeleteAsync().ContinueWith(task => 
        {
            if (task.IsCompleted)
                Debug.Log("Firebase Firestore: Existing User Document Deleted");
        });*/
    }

    public void GetUserDiamondsOnSceneLoad()
    {
        userRef = firestoreDB.Collection(devicePlatformCollection).Document(firebaseUser.UserId);
        userRef.GetSnapshotAsync().ContinueWith(task => 
        { 
            if(task.IsCompleted)
            {
                firestoreUserData = task.Result.ConvertTo<FirestoreUserData>();

                if (mainMenuDiamondsText != null)
                    mainMenuDiamondsText.text = GetUserDiamondsAmount();
            }
        });
    }

    public string GetUserDiamondsAmount()
    {
        return Mathf.RoundToInt(firestoreUserData.diamondsAmount).ToString();
    }
    
    public void AddDiamondsAmount(int addAmount)
    {
        firestoreUserData.diamondsAmount += addAmount;        
    }

    public void DeductDiamondsAmount(int deductAmount)
    {
        firestoreUserData.diamondsAmount -= deductAmount;
        if (firestoreUserData.diamondsAmount <= 0)
            firestoreUserData.diamondsAmount = 0;
    }
}
