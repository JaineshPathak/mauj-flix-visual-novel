using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FirebaseFirestoreHandler : MonoBehaviourSingletonPersistent<FirebaseFirestoreHandler>
{
    //public static FirebaseFirestoreHandler instance;

    public FirebaseUser firebaseUser;
    public FirestoreUserData firestoreUserData;

    private FirebaseFirestore firestoreDB;
    private DocumentReference userRef;
    //private ListenerRegistration firestoreDBListener;

    private FirebaseFirestoreOffline firestoreOffline;
    
    public static event Action<FirebaseFirestoreHandler> OnFirestoreLoaded;

    private bool firstTimeUser;
    private string devicePlatformCollection;
    private string deviceCommentsPlatformCollection;

    public override void Awake()
    {
        /*if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);*/

        base.Awake();

        firestoreOffline = GetComponent<FirebaseFirestoreOffline>();

#if UNITY_EDITOR
        devicePlatformCollection = "Users PC (TEST)";
        deviceCommentsPlatformCollection = "Comments PC (TEST)";
#elif UNITY_ANDROID
        devicePlatformCollection = "Users Android";
        deviceCommentsPlatformCollection = "Comments Android";
#elif UNITY_IOS
        devicePlatformCollection = "Users iOS";
        deviceCommentsPlatformCollection = "Comments iOS";
#endif
    }

    private void OnEnable()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded += OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted += OnFirebaseUserDeleted;
        FirebaseAuthHandler.OnFirebaseUserAccountSignedIn += OnFirebaseUserSignedIn;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded -= OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted -= OnFirebaseUserDeleted;
        FirebaseAuthHandler.OnFirebaseUserAccountSignedIn -= OnFirebaseUserSignedIn;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        FirebaseAuthHandler.OnFirebaseUserAccountLoaded -= OnFirebaseUserLoaded;
        FirebaseAuthHandler.OnFirebaseUserAccountDeleted -= OnFirebaseUserDeleted;
        FirebaseAuthHandler.OnFirebaseUserAccountSignedIn -= OnFirebaseUserSignedIn;

        //firestoreDBListener.Stop();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        //print(scene.name + " (" + scene.buildIndex + ")");
        if (scene.buildIndex == 2 && firstTimeUser && EpisodesSpawner.instance != null)
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
                    firestoreUserData.userProfilePicUrl = FirebaseAuthHandler.instance.GetProfileURLFromProviderID(DataPaths.firebaseGoogleProviderID);
                }
                else
                {
                    firestoreUserData.userName = "";
                    firestoreUserData.userEmail = "";
                    firestoreUserData.userProfilePicUrl = "";
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
                    firestoreUserData.userProfilePicUrl = FirebaseAuthHandler.instance.GetProfileURLFromProviderID(DataPaths.firebaseGoogleProviderID);
                }
                else
                {
                    firestoreUserData.userName = "";
                    firestoreUserData.userEmail = "";
                    firestoreUserData.userProfilePicUrl = "";
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

    private void OnFirebaseUserSignedIn(FirebaseUser user)
    {
        if (firestoreOffline != null)
        {
            userRef = firestoreDB.Collection(devicePlatformCollection).Document(user.UserId);
            userRef.GetSnapshotAsync().ContinueWith(getTask =>
            {
                DocumentSnapshot snapShot = getTask.Result;
                if (snapShot.Exists)
                {
                    firestoreUserData = snapShot.ConvertTo<FirestoreUserData>();
                    firestoreOffline.UpdateFirebaseFirestoreData(firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
                }
            });
        }
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

    public int GetUserTicketsAmountInt()
    {
        return Mathf.RoundToInt(firestoreUserData.ticketsAmount);
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

    //===================================================================================================================================================

    public void PushOfflineData(float _diamondsAmount, float _ticketsAmount)
    {
        if (firebaseUser == null)
            return;

        firestoreUserData.diamondsAmount = _diamondsAmount;
        firestoreUserData.ticketsAmount = _ticketsAmount;

        userRef = firestoreDB.Collection(devicePlatformCollection).Document(firebaseUser.UserId);
        userRef.SetAsync(firestoreUserData).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.LogFormat("Firebase Firestore: Existing User Data Updated {0}, {1}, {2}, {3}, {4}", firestoreUserData.userID, firestoreUserData.userName, firestoreUserData.userEmail, firestoreUserData.diamondsAmount, firestoreUserData.ticketsAmount);
        });
    }

    //===================================================================================================================================================

    //===================================================================================================================================================

    //Comments Related Section

    public void HasUserDocumentExists(string storyCollectionName, string userID, Action<DocumentSnapshot> callback)
    {
        DocumentReference storyDocRef = firestoreDB.Collection(storyCollectionName).Document(userID);
        storyDocRef.GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                callback?.Invoke(task.Result);
            }
        });
    }

    public void AddUserComment(string storyDocName, string userCollectionName, FirestoreCommentData commentData)
    {
        DocumentReference storyDocRef = firestoreDB.Collection(deviceCommentsPlatformCollection).Document(storyDocName).Collection(userCollectionName).Document(commentData.userID);
        storyDocRef.SetAsync(commentData).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.LogFormat("Firebase Firestore: New Comment Added {0}, {1}, {2}", commentData.userID, commentData.userName, commentData.userComment);
        });
        /*storyDocRef.GetSnapshotAsync().ContinueWith(getTask =>
        {
            if(getTask.IsCompleted)
            {
                DocumentSnapshot storyDocSnapshot = getTask.Result;

            }
            else if(getTask.IsFaulted)
                Debug.LogError($"Firebase Firestore: Unable to get Story Document Ref for name: {storyDocName}");
        });*/
    }

    //Adds a new User comment
    public void AddUserComment(string storyCollectionName, string userDocName, FirestoreCommentData commentData, Action callback)
    {
        //DocumentReference storyDocRef = firestoreDB.Collection(deviceCommentsPlatformCollection).Document(storyDocName).Collection(userCollectionName).Document(commentData.userID);
        DocumentReference storyDocRef = firestoreDB.Collection(storyCollectionName).Document(commentData.userID);
        storyDocRef.SetAsync(commentData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                callback?.Invoke();
                Debug.LogFormat("Firebase Firestore: New Comment Added {0}, {1}, {2}", commentData.userID, commentData.userName, commentData.userComment);
            }
        });
    }

    public void GetAllCommentDocs(string storyCollectionName, Action<List<DocumentSnapshot>> callback)
    {
        List<DocumentSnapshot> documentSnapshotsList = new List<DocumentSnapshot>();
        
        //Try to get all documents from cache. If error, then get it from firebase firestore server
        //REMOVED!
        Query storyCollectionQuery = firestoreDB.Collection(storyCollectionName);        
        storyCollectionQuery.GetSnapshotAsync().ContinueWith(commentTask => 
        {
            /*if(cachedTask.IsFaulted)
            {
#if UNITY_EDITOR
                Debug.LogError($"Firestore Handle: (VIA CACHE) FAILED GOT ALL DOCUMENTS OF TITLE: {storyCollectionName} | Exception: {cachedTask.Exception.Message} | TRYING TO FETCH LATEST FROM SERVER!");
#endif

                //Getting from cache failed, so try to get it from Server
                Query storyCollectionQueryServer = firestoreDB.Collection(storyCollectionName);
                storyCollectionQueryCached.GetSnapshotAsync(Source.Server).ContinueWith(serverTask =>
                {
                    if(serverTask.IsCompleted)
                    {
                        QuerySnapshot documentSnapshots = cachedTask.Result;
                        foreach (DocumentSnapshot documentSnapshot in documentSnapshots.Documents)
                            documentSnapshotsList.Add(documentSnapshot);

                        if (documentSnapshotsList.Count > 0)
                        {
#if UNITY_EDITOR
                            Debug.Log($"Firestore Handle: (VIA SERVER) SUCCESSFULLY GOT ALL DOCUMENTS OF TITLE: {storyCollectionName}, Count: {documentSnapshotsList.Count}");
#endif
                            callback?.Invoke(documentSnapshotsList);
                        }
                        else
                        {
#if UNITY_EDITOR
                            Debug.LogError("Firestore Handle: (VIA SERVER) BECAUSE LIST IS EMPTY! FAILED TO ALL DOCUMENTS OF TITLE: " + storyCollectionName);
#endif
                        }
                    }
                    else if(serverTask.IsFaulted)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"Firestore Handle: (VIA SERVER) BECAUSE TASK FAULTED! FAILED TO ALL DOCUMENTS OF TITLE: " + storyCollectionName);
#endif
                    }
                });
            }
            else*/ 
            if(commentTask.IsCompleted)
            {
                QuerySnapshot documentSnapshots = commentTask.Result;
                foreach (DocumentSnapshot documentSnapshot in documentSnapshots.Documents)                
                    documentSnapshotsList.Add(documentSnapshot);                

                if (documentSnapshotsList.Count > 0)
                {
#if UNITY_EDITOR
                    Debug.Log($"Firestore Handle: (VIA DEFAULT) SUCCESSFULLY GOT ALL DOCUMENTS OF TITLE: {storyCollectionName}, Count: {documentSnapshotsList.Count}");
#endif
                    callback?.Invoke(documentSnapshotsList);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Firestore Handle: (VIA DEFAULT) BECAUSE LIST IS EMPTY! FAILED TO ALL DOCUMENTS OF TITLE: " + storyCollectionName);
#endif
                }
            }
        });
    }

    //For testing purpose. Press 'V' key then check firestore console
    public void AddTestComments(string storyCollectionName, int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            DocumentReference storyDocRef = firestoreDB.Collection(storyCollectionName).Document(i.ToString("D2"));
            
            FirestoreCommentData commentData = new FirestoreCommentData();
            commentData.userID = i.ToString("D2");
            commentData.userName = i.ToString("D2");
            commentData.userComment = i.ToString("D2");
            commentData.userProfilePicUrl = "https://lh3.googleusercontent.com/a-/AFdZucpwMi1-aLCE3T7I32QH5BpAFJ2jPHQI4NtsBEsEAw=s96-c";

            List<string> userCommentsL = new List<string>();
            userCommentsL.Add(i.ToString("D2"));
            userCommentsL.Add(i.ToString("D2"));
            userCommentsL.Add(i.ToString("D2"));

            commentData.userComments = userCommentsL.ToArray();
            //commentData.userCommentsList.Add(i.ToString("D2"));
            //commentData.userCommentsList.Add(i.ToString("D2"));

            storyDocRef.SetAsync(commentData).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {                    
                   Debug.LogFormat("Firebase Firestore: Test Comment Added - " + i);
                }
            });
        }
    }

    /*public async void GetAllCommentDocs(string storyCollectionName, Action<DocumentSnapshot[]> callback)
    {
        List<DocumentSnapshot> documentSnapshotsList = new List<DocumentSnapshot>();

        Query storyCollectionQuery = firestoreDB.Collection(storyCollectionName);
        QuerySnapshot docsSnapshot = await storyCollectionQuery.GetSnapshotAsync();

        int i = 0;
        foreach (DocumentSnapshot documentSnapshot in docsSnapshot.Documents)
        {
            if(documentSnapshot.Exists)
                Debug.Log($"Firestore Handle: [{i}] Document EXISTS!");
            else
                Debug.LogError($"Firestore Handle: [{i}] Document NOT EXISTS!");

            i++;
            documentSnapshotsList.Add(documentSnapshot);
        }

        if (documentSnapshotsList.Count > 0)
        {
            Debug.LogError("Firestore Handle: SUCCESSFULLY GOT ALL DOCUMENTS OF TITLE: " + storyCollectionName);
            callback?.Invoke(documentSnapshotsList.ToArray());
        }
        else
            Debug.LogError("Firestore Handle: FAILED TO ALL DOCUMENTS OF TITLE: " + storyCollectionName);
    }*/

    //===================================================================================================================================================
}