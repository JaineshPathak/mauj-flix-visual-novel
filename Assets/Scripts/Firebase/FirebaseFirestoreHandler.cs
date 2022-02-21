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
        devicePlatformCollection = "Users PC (TEST)";
#elif UNITY_ANDROID
        devicePlatformCollection = "Users Android";
#elif UNITY_IOS
        devicePlatformCollection = "Users iOS";
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
        Debug.Log("Firebase Firestore: UserID: " + firebaseUser.UserId);
        Debug.Log("Firebase Firestore: User Name: " + firebaseUser.DisplayName);
        Debug.Log("Firebase Firestore: User Email: " + firebaseUser.Email);
        Debug.Log("Firebase Firestore: User Main Provider: " + firebaseUser.ProviderId);
        foreach(var providerData in firebaseUser.ProviderData)
        {
            Debug.Log("Firebase Firestore: User Provider Display Name: " + providerData.DisplayName);
            Debug.Log("Firebase Firestore: User Provider ID: " + providerData.ProviderId);
        }       

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
            firestoreUserData.ticketsAmount = 10f;

            if (FirebaseAuthHandler.instance.GetProviderID() == "google.com")
            {
                firestoreUserData.userName = FirebaseAuthHandler.instance.GetDisplayNameFromProviderID(DataPaths.firebaseGoogleProviderID);
                firestoreUserData.userEmail = FirebaseAuthHandler.instance.GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID);
            }
            else
            {
                firestoreUserData.userName = "";
                firestoreUserData.userEmail = "";
            }

            userRef.SetAsync(firestoreUserData).ContinueWith(task => 
            {
                if(task.IsCompleted)
                    Debug.LogFormat("Firebase Firestore: New User Data added {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
            });

            if (mainMenuDiamondsText != null)
                mainMenuDiamondsText.text = GetUserDiamondsAmount();
        }
        else                            //Existing User
        {            
            Debug.LogFormat("Firebase Firestore: Existing User Data loaded/changed {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
            firestoreUserData = snapshot.ConvertTo<FirestoreUserData>();
            firestoreUserData.userID = firebaseUser.UserId;
            firestoreUserData.userName = firebaseUser.DisplayName;
            firestoreUserData.userEmail = firebaseUser.Email;

            if (FirebaseAuthHandler.instance.GetProviderID() == "google.com")
            {
                firestoreUserData.userName = FirebaseAuthHandler.instance.GetDisplayNameFromProviderID(DataPaths.firebaseGoogleProviderID);
                firestoreUserData.userEmail = FirebaseAuthHandler.instance.GetEmailFromProviderID(DataPaths.firebaseGoogleProviderID);
            }
            else
            {
                firestoreUserData.userName = "";
                firestoreUserData.userEmail = "";
            }

            userRef.SetAsync(firestoreUserData).ContinueWith(task =>
            {
                if (task.IsCompleted)
                    Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
            });

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
        //return Mathf.RoundToInt(firestoreUserData.diamondsAmount).ToString();
        return AbbrevationUtility.AbbreviateNumber(firestoreUserData.diamondsAmount);
    }
    
    public void DepositDiamondsAmount(int addAmount)
    {
        firestoreUserData.diamondsAmount += addAmount;        
    }

    public void DebitDiamondsAmount(int deductAmount)
    {
        firestoreUserData.diamondsAmount -= deductAmount;
        if (firestoreUserData.diamondsAmount <= 0)
            firestoreUserData.diamondsAmount = 0;
    }
}
