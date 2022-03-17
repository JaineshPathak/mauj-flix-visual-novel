using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Auth;

public class FirebaseFirestoreHandler : MonoBehaviour
{
    public static FirebaseFirestoreHandler instance;

    public FirebaseUser firebaseUser;
    public FirestoreUserData firestoreUserData;

    private FirebaseFirestore firestoreDB;
    private DocumentReference userRef;
    //private ListenerRegistration firestoreDBListener;

    private FirebaseFirestoreOffline firestoreOffline;
    
    public static event Action<FirebaseFirestoreHandler> OnFirestoreLoaded;

    private bool firstTimeUser;
    private string devicePlatformCollection;    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        firestoreOffline = GetComponent<FirebaseFirestoreOffline>();

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

        //firestoreDBListener.Stop();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        //print(scene.name + " (" + scene.buildIndex + ")");
        if (scene.buildIndex == 1 && firstTimeUser && EpisodesSpawner.instance != null)
        {
            firstTimeUser = false;
            EpisodesSpawner.instance.ShowFirstTimeReward();

            //OnFirestoreLoaded?.Invoke(this);
            //GetUserDiamondsOnSceneLoad();
        }
    }

    private void OnFirebaseUserLoaded(FirebaseUser user)
    {
        if (user == null)
            return;

        firebaseUser = user;
#if UNITY_EDITOR
        Debug.Log("Firebase Firestore: UserID: " + firebaseUser.UserId);
        Debug.Log("Firebase Firestore: User Name: " + firebaseUser.DisplayName);
        Debug.Log("Firebase Firestore: User Email: " + firebaseUser.Email);
        Debug.Log("Firebase Firestore: User Main Provider: " + firebaseUser.ProviderId);
        foreach(var providerData in firebaseUser.ProviderData)
        {
            Debug.Log("Firebase Firestore: User Provider Display Name: " + providerData.DisplayName);
            Debug.Log("Firebase Firestore: User Provider ID: " + providerData.ProviderId);
        }
#endif

        if (firestoreDB == null)
            firestoreDB = FirebaseFirestore.DefaultInstance;

        userRef = firestoreDB.Collection(devicePlatformCollection).Document(user.UserId);
        userRef.GetSnapshotAsync().ContinueWith(getTask =>
        {
            DocumentSnapshot snapShot = getTask.Result;
            if (!snapShot.Exists)          //New User
            {
                firstTimeUser = true;

                firestoreUserData = new FirestoreUserData();
                firestoreUserData.userID = firebaseUser.UserId;
                firestoreUserData.userName = firebaseUser.DisplayName;
                firestoreUserData.userEmail = firebaseUser.Email;
                firestoreUserData.diamondsAmount = 0;
                firestoreUserData.ticketsAmount = 0;

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

                userRef.SetAsync(firestoreUserData).ContinueWith(setTask =>
                {
                    if (setTask.IsCompleted)
                        Debug.LogFormat("Firebase Firestore: New User Data added {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
                });

                if (firestoreOffline != null)
                {
                    Debug.Log("Firebase Firestore: Offline Handler Initialized with New User: " + firestoreUserData.userID);
                    firestoreOffline.InitFirebaseFirestoreOffline(this, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
                }
            }
            else
            {
                Debug.LogFormat("Firebase Firestore: Existing User Data loaded/changed {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);

                firestoreUserData = snapShot.ConvertTo<FirestoreUserData>();
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

                userRef.SetAsync(firestoreUserData).ContinueWith(setTask =>
                {
                    if (setTask.IsCompleted)
                        Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
                });

                if (firestoreOffline != null)
                {
                    Debug.Log("Firebase Firestore: Offline Handler Initialized with Existing User: " + firestoreUserData.userID);
                    firestoreOffline.InitFirebaseFirestoreOffline(this, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
                }
            }
        });        
        //firestoreDBListener = userRef.Listen(OnFirestoreUserDataListener);
    }

    /*private void OnFirestoreUserDataListener(DocumentSnapshot snapshot)
    {
        if(!snapshot.Exists)            //New User
        {            
            firestoreUserData = new FirestoreUserData();
            firestoreUserData.userID = firebaseUser.UserId;
            firestoreUserData.userName = firebaseUser.DisplayName;
            firestoreUserData.userEmail = firebaseUser.Email;            
            firestoreUserData.diamondsAmount = 50f;
            firestoreUserData.ticketsAmount = 15f;

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
    }*/

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

                /*if (mainMenuDiamondsText != null)
                    mainMenuDiamondsText.text = GetUserDiamondsAmount();*/
            }
        });
    }

    //=======================================================================================

    //Diamonds Public methods

    public string GetUserDiamondsAmount()
    {
        //return Mathf.RoundToInt(firestoreUserData.diamondsAmount).ToString();
        return AbbrevationUtility.AbbreviateNumber(firestoreUserData.diamondsAmount);
    }

    public int GetUserDiamondsAmountInt()
    {
        return Mathf.RoundToInt(firestoreUserData.diamondsAmount);
    }
    
    public void DepositDiamondsAmount(float addAmount)
    {
        firestoreUserData.diamondsAmount += addAmount;        
    }

    public void DebitDiamondsAmount(float deductAmount)
    {
        firestoreUserData.diamondsAmount -= deductAmount;
        if (firestoreUserData.diamondsAmount <= 0)
            firestoreUserData.diamondsAmount = 0;

        userRef.SetAsync(firestoreUserData).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
        });
    }

    //=======================================================================================

    public string GetUserTicketsAmount()
    {
        return AbbrevationUtility.AbbreviateNumber(firestoreUserData.ticketsAmount);
    }

    public void DepositTicketsAmount(float addAmount)
    {
        firestoreUserData.ticketsAmount += addAmount;
    }

    public void DebitTicketsAmount(float deductAmount)
    {
        firestoreUserData.ticketsAmount -= deductAmount;
        if (firestoreUserData.ticketsAmount <= 0)
            firestoreUserData.ticketsAmount = 0;

        userRef.SetAsync(firestoreUserData).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
        });
    }

    //=======================================================================================

    public void PushOfflineData(float _diamondsAmount, float _ticketsAmount)
    {
        firestoreUserData.diamondsAmount = _diamondsAmount;
        firestoreUserData.ticketsAmount = _ticketsAmount;

        userRef.SetAsync(firestoreUserData).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
        });
    }
}
