using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
//using Firebase.Auth;
using Firebase.Firestore;

public class UICommentsSection : MonoBehaviour
{
    [Header("Details Panel")]
    [SerializeField] private UIStoriesDetailsPanel detailsPanel;

    [Header("Prefab")]
    [SerializeField] private UICommentItem commentItemPrefab;

    [Header("Comments Input Field")]
    [SerializeField] private TMP_InputField commentInputField;
    [SerializeField] private Button addCommentButton;
    [SerializeField] private Button cancelCommentButton;

    [Header("Buttons n Texts")]
    [SerializeField] private GameObject loginToCommentText;
    [SerializeField] private GameObject buttonsGroup;

    [Header("Section Fitters")]
    [SerializeField] private RectTransform sectionRect;
    [SerializeField] private ContentSizeFitter sectionSizeFitter;

    [Header("Comments Load Buttons")]
    [SerializeField] private Button loadMoreCommentButton;    

    private FirebaseAuthHandler fireAuthHandler;
    private FirebaseFirestoreHandler fireStoreHandler;
    private EpisodesSpawner episodesSpawner;

    private FirestoreCommentData commentData;
    private List<DocumentSnapshot> commentDataListActual = new List<DocumentSnapshot>();

    private int startIndex;
    private int loadCount;

    private void OnEnable()
    {
        FirebaseAuthHandler.OnUserGoogleSignIn += OnGoogleSignIn;
        FirebaseAuthHandler.OnUserGoogleSignOut += OnGoogleSignOut;
    }

    private void OnDisable()
    {
        FirebaseAuthHandler.OnUserGoogleSignIn -= OnGoogleSignIn;
        FirebaseAuthHandler.OnUserGoogleSignOut -= OnGoogleSignOut;
    }

    private void OnDestroy()
    {
        FirebaseAuthHandler.OnUserGoogleSignIn -= OnGoogleSignIn;
        FirebaseAuthHandler.OnUserGoogleSignOut -= OnGoogleSignOut;
    }

    private void OnGoogleSignIn() => CheckSignInStatus();

    private void OnGoogleSignOut() => CheckSignInStatus();

    private void Awake()
    {
        if (commentInputField)
        {            
            commentInputField.onValueChanged.AddListener((string val) =>
            {
                addCommentButton.interactable = val.Length > 0;
                cancelCommentButton.interactable = val.Length > 0;
            });
        }

        if (addCommentButton)
            addCommentButton.onClick.AddListener(AddUserComment);

        if (cancelCommentButton)
            cancelCommentButton.onClick.AddListener(CancelUserComment);

        if (loadMoreCommentButton)
            loadMoreCommentButton.onClick.AddListener(LoadExtraComments);

        loadMoreCommentButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (FirebaseAuthHandler.instance != null)
            fireAuthHandler = FirebaseAuthHandler.instance;

        if (FirebaseFirestoreHandler.instance != null) 
            fireStoreHandler = FirebaseFirestoreHandler.instance;

        if (EpisodesSpawner.instance != null)
            episodesSpawner = EpisodesSpawner.instance;

        CheckSignInStatus();
    }

    private void CheckSignInStatus()
    {
        if (buttonsGroup == null || loginToCommentText == null)
            return;

        buttonsGroup.SetActive(fireAuthHandler.GetProviderID() == DataPaths.firebaseGoogleProviderID);
        loginToCommentText.SetActive(!(fireAuthHandler.GetProviderID() == DataPaths.firebaseGoogleProviderID));

        if(addCommentButton && cancelCommentButton && commentInputField)
        {
            addCommentButton.interactable = commentInputField.text.Length > 0;
            cancelCommentButton.interactable = commentInputField.text.Length > 0;
        }
    }

    private void AddUserComment()
    {
        if (commentInputField == null || commentInputField.text.Length <= 0)
            return;

        if (fireAuthHandler.GetProviderID() != DataPaths.firebaseGoogleProviderID)
            return;

        if (episodesSpawner == null || episodesSpawner.storiesDBItem == null)
            return;

        StoriesDBItem storyItem = episodesSpawner.storiesDBItem;
        string englishTitleNoSpace = storyItem.storyTitleEnglish;
        englishTitleNoSpace = englishTitleNoSpace.Replace(" ", "");

        //First check if the User Document already exists, if not make a new comment item
        fireStoreHandler.HasUserDocumentExists($"{englishTitleNoSpace}_{storyItem.storyProgressFileName}", fireAuthHandler.userCurrent.UserId, (DocumentSnapshot docSnap) =>
        {
            Debug.Log($"Comment Section: {fireAuthHandler.userCurrent.UserId} Status: {docSnap.Exists}");
            if(docSnap.Exists)
            {
                commentData = docSnap.ConvertTo<FirestoreCommentData>();

                Debug.Log($"Comment Section: {commentData.userID} Document Exists!");
                
                List<string> userCommentsList = new List<string>(commentData.userComments);
                userCommentsList.Add(commentInputField.text);
                commentData.userComments = userCommentsList.ToArray();

                fireStoreHandler.AddUserComment($"{englishTitleNoSpace}_{storyItem.storyProgressFileName}", fireAuthHandler.userCurrent.UserId, commentData, () =>
                {
                    //StartCoroutine(PostSetupCommentItemRoutine());

                    commentInputField.text = "";

                    StartCoroutine(GetUserProfilePicRoutine((Texture profilePicTex) =>
                    {
                        SpawnCommentItem(transform, profilePicTex);
#if UNITY_EDITOR
                        Debug.Log("Comment Section: Comment Successfully Added!");
#endif
                    }));
                });
            }
            else
            {
                commentData = new FirestoreCommentData();
                commentData.userID = fireAuthHandler.userCurrent.UserId;
                commentData.userName = fireAuthHandler.userCurrent.DisplayName;
                commentData.userEmail = fireAuthHandler.userCurrent.Email;
                commentData.userProfilePicUrl = fireAuthHandler.userCurrent.PhotoUrl.AbsoluteUri;
                commentData.userComment = commentInputField.text;
                commentData.userComments = new string[] { commentInputField.text };

                /*List<string> userCommentsL = new List<string>();
                userCommentsL.Add(commentInputField.text);
                commentData.userComments = userCommentsL.ToArray();*/
                //commentData.userCommentsList.Add(commentInputField.text);

                fireStoreHandler.AddUserComment($"{englishTitleNoSpace}_{storyItem.storyProgressFileName}", fireAuthHandler.userCurrent.UserId, commentData, () =>
                {
                    //StartCoroutine(PostSetupCommentItemRoutine());

                    commentInputField.text = "";

                    StartCoroutine(GetUserProfilePicRoutine((Texture profilePicTex) =>
                    {
                        SpawnCommentItem(transform, profilePicTex);
#if UNITY_EDITOR
                        Debug.Log("Comment Section: Comment Successfully Added!");
#endif
                    }));
                });
            }
        });        
    }

    private IEnumerator GetUserProfilePicRoutine(Action<Texture> callback)
    {
        UnityWebRequest wr = UnityWebRequestTexture.GetTexture(fireAuthHandler.userCurrent.PhotoUrl);

        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError)
            Debug.Log("Comment Section: Unable to Get User Profile Pic: " + wr.error);
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            tex = DownloadHandlerTexture.GetContent(wr);

            callback?.Invoke(tex);
            //UICommentItem commentItem = Instantiate(commentItemPrefab, transform);
            //commentItem.SetupItem(tex, commentData.userName, commentData.userComment);

            //SpawnCommentItem(transform, tex);      
        }
    }

    private IEnumerator GetUserProfilePicRoutine(string photoUrl, Action<Texture> callback)
    {
        UnityWebRequest wr = UnityWebRequestTexture.GetTexture(photoUrl);

        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError)
            Debug.Log("Comment Section: Unable to Get User Profile Pic: " + wr.error);
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            tex = DownloadHandlerTexture.GetContent(wr);

            callback?.Invoke(tex);
            //UICommentItem commentItem = Instantiate(commentItemPrefab, transform);
            //commentItem.SetupItem(tex, commentData.userName, commentData.userComment);

            //SpawnCommentItem(transform, tex);      
        }
    }

    public void PopulateSection()
    {
        if (fireStoreHandler == null)
            return;

        if (episodesSpawner == null || episodesSpawner.storiesDBItem == null)
            return;

        StoriesDBItem storyItem = episodesSpawner.storiesDBItem;
        
        string englishTitleNoSpace = storyItem.storyTitleEnglish;
        englishTitleNoSpace = englishTitleNoSpace.Replace(" ", "");

        commentDataListActual.Clear();
        fireStoreHandler.GetAllCommentDocs($"{englishTitleNoSpace}_{storyItem.storyProgressFileName}", (List<DocumentSnapshot> commentDataList) => 
        {
            commentDataListActual = commentDataList;
            if (commentDataListActual.Count > 0)
                LoadStartComments();
        });
    }

    private void LoadStartComments()
    {
        if (commentDataListActual.Count <= 0)
        {
            loadMoreCommentButton.gameObject.SetActive(false);
            return;
        }
        
        loadMoreCommentButton.gameObject.SetActive(commentDataListActual.Count > 10);

        loadCount = commentDataListActual.Count > 10 ? 10 : commentDataListActual.Count;
        startIndex = 0;
        for (int i = startIndex; i < loadCount; i++)
        {
            FirestoreCommentData otherCommentData = commentDataListActual[i].ConvertTo<FirestoreCommentData>();
            string profileID = otherCommentData.userID;
            string profileName = otherCommentData.userName;
            string profileEmail = otherCommentData.userEmail;
            string profileURL = otherCommentData.userProfilePicUrl;
            string profileComment = otherCommentData.userComment;
            string[] profileComments = otherCommentData.userComments;

            StartCoroutine(GetUserProfilePicRoutine(profileURL, (Texture profilePicTex) =>
            {
                //SpawnCommentItem(transform, profilePicTex, otherCommentData);
                //SpawnCommentItem(transform, profileName, profileComment, profilePicTex, profileID);
                SpawnCommentItem(transform, profileName, profileComments, profilePicTex, profileID);
            }));
        }

        startIndex = loadCount;
        loadCount += 10;
        if (loadCount >= commentDataListActual.Count)
            loadCount = commentDataListActual.Count;
    }

    private void LoadMoreComments()
    {
        for (int i = startIndex; i < loadCount; i++)
        {
            FirestoreCommentData otherCommentData = commentDataListActual[i].ConvertTo<FirestoreCommentData>();
            string profileID = otherCommentData.userID;
            string profileName = otherCommentData.userName;
            string profileEmail = otherCommentData.userEmail;
            string profileURL = otherCommentData.userProfilePicUrl;
            string profileComment = otherCommentData.userComment;
            string[] profileComments = otherCommentData.userComments;

            StartCoroutine(GetUserProfilePicRoutine(profileURL, (Texture profilePicTex) =>
            {
                //SpawnCommentItem(transform, profilePicTex, otherCommentData);
                //SpawnCommentItem(transform, profileName, profileComment, profilePicTex, profileID);
                SpawnCommentItem(transform, profileName, profileComments, profilePicTex, profileID);
            }));
        }

        startIndex = loadCount;
        loadCount += 10;
        if (loadCount >= commentDataListActual.Count)
            loadCount = commentDataListActual.Count;

        loadMoreCommentButton.gameObject.SetActive(!(startIndex >= loadCount));
    }

    /*private void PopulateComments()
    {
        Debug.Log("Comment Section: snapshotsList.Length : " + commentDataListActual.Count);
        for (int i = 0; i < commentDataListActual.Count; i++)
        {
            //UICommentItem commentItemInstance = Instantiate(commentItemPrefab, transform);
            //commentItemInstance.SetupItem(commentDataList[i].userName, commentDataList[i].userComment);

            Debug.Log("Comment Section: Snapshot ID : " + commentDataListActual[i].userID);
            //FirestoreCommentData otherCommentData = commentDataList[i].ConvertTo<FirestoreCommentData>();

            Debug.Log("Comment Section: otherCommentData ID : " + commentDataListActual[i].userID);
            Debug.Log("Comment Section: otherCommentData Name : " + commentDataListActual[i].userName);

            StartCoroutine(GetUserProfilePicRoutine(commentDataListActual[i].userProfilePicUrl, (Texture profilePicTex) =>
            {
                SpawnCommentItem(transform, profilePicTex, commentDataListActual[i]);
            }));            
        }
    }*/

    public void EmptySection()
    {
        if (CommentsPool.instance == null)
            return;

        CommentsPool.instance.ResetAllItems();

        Invoke("RebuildSectionLayout", 0.5f);
    }

    public void SpawnCommentItem(Transform _parent, Texture _tex)
    {
        string latestComment = commentData.userComments[commentData.userComments.Length - 1];

        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_tex, commentData.userName, latestComment, commentData.userID);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, commentData.userName, latestComment, commentData.userID);

                CommentsPool.instance.AddNewCommentItem(commentItem);

                Invoke("RebuildSectionLayout", 0.5f);

#if UNITY_EDITOR
                Debug.Log("Comment Section: New Comment Item was added to Pool!");
#endif
            }
        }
        else
        {
            commentItem = Instantiate(commentItemPrefab, _parent);
            commentItem.SetupItem(_tex, commentData.userName, latestComment, commentData.userID);

            Invoke("RebuildSectionLayout", 0.5f);
        }
    }

    public void SpawnCommentItem(Transform _parent, FirestoreCommentData _commentData)
    {
        for (int i = 0; i < _commentData.userComments.Length && (_commentData.userComments.Length > 0); i++)
        {
            UICommentItem commentItem = null;
            if (CommentsPool.instance != null)
            {
                commentItem = CommentsPool.instance.GetCommentItem();
                if (commentItem != null)
                {
                    commentItem.gameObject.SetActive(true);
                    commentItem.transform.parent = _parent;
                    commentItem.SetupItem(_commentData.userName, _commentData.userComments[i]);

                    Invoke("RebuildSectionLayout", 0.5f);
                }
                else
                {
                    commentItem = Instantiate(commentItemPrefab, _parent);
                    commentItem.SetupItem(_commentData.userName, _commentData.userComments[i]);

                    CommentsPool.instance.AddNewCommentItem(commentItem);

                    Invoke("RebuildSectionLayout", 0.5f);

#if UNITY_EDITOR
                    Debug.Log("Comment Section: New Comment Item was added to Pool!");
#endif
                }
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_commentData.userName, _commentData.userComments[i]);

                Invoke("RebuildSectionLayout", 0.5f);
            }
        }        
    }

    public void SpawnCommentItem(Transform _parent, string _profileName, string _profileComment, Texture _tex, string _profileID)
    {
        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_tex, _profileName, _profileComment, _profileID);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, _profileName, _profileComment, _profileID);

                CommentsPool.instance.AddNewCommentItem(commentItem);

                Invoke("RebuildSectionLayout", 0.5f);

#if UNITY_EDITOR
                Debug.Log("Comment Section: New Comment Item was added to Pool!");
#endif
            }
        }
        else
        {
            commentItem = Instantiate(commentItemPrefab, _parent);
            commentItem.SetupItem(_tex, _profileName, _profileComment, _profileID);

            Invoke("RebuildSectionLayout", 0.5f);
        }
    }

    public void SpawnCommentItem(Transform _parent, string _profileName, string[] _profileComments, Texture _tex, string _profileID)
    {
        for (int i = 0; i < _profileComments.Length && (_profileComments.Length > 0); i++)
        {
            UICommentItem commentItem = null;
            if (CommentsPool.instance != null)
            {
                commentItem = CommentsPool.instance.GetCommentItem();
                if (commentItem != null)
                {
                    commentItem.gameObject.SetActive(true);
                    commentItem.transform.parent = _parent;
                    commentItem.SetupItem(_tex, _profileName, _profileComments[i], _profileID);

                    Invoke("RebuildSectionLayout", 0.5f);
                }
                else
                {
                    commentItem = Instantiate(commentItemPrefab, _parent);
                    commentItem.SetupItem(_tex, _profileName, _profileComments[i], _profileID);

                    CommentsPool.instance.AddNewCommentItem(commentItem);

                    Invoke("RebuildSectionLayout", 0.5f);

#if UNITY_EDITOR
                    Debug.Log("Comment Section: New Comment Item was added to Pool!");
#endif
                }
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, _profileName, _profileComments[i], _profileID);

                Invoke("RebuildSectionLayout", 0.5f);
            }
        }        
    }

    public void SpawnCommentItem(Transform _parent, Texture _tex, FirestoreCommentData _commentData)
    {
        for (int i = 0; i < _commentData.userComments.Length && (_commentData.userComments.Length > 0); i++)
        {
            UICommentItem commentItem = null;
            if (CommentsPool.instance != null)
            {
                commentItem = CommentsPool.instance.GetCommentItem();
                if (commentItem != null)
                {
                    commentItem.gameObject.SetActive(true);
                    commentItem.transform.parent = _parent;
                    commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment, _commentData.userID);

                    Invoke("RebuildSectionLayout", 0.5f);
                }
                else
                {
                    commentItem = Instantiate(commentItemPrefab, _parent);
                    commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment, _commentData.userID);

                    CommentsPool.instance.AddNewCommentItem(commentItem);

                    Invoke("RebuildSectionLayout", 0.5f);

#if UNITY_EDITOR
                    Debug.Log("Comment Section: New Comment Item was added to Pool!");
#endif
                }
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment, _commentData.userID);

                //RebuildSectionLayout();
                Invoke("RebuildSectionLayout", 0.5f);
            }
        }
    }

    private void RebuildSectionLayout()
    {
        if (sectionSizeFitter == null || sectionRect == null)
            return;

        sectionSizeFitter.enabled = false;
        sectionSizeFitter.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(sectionRect);
        sectionSizeFitter.enabled = true;        

        loadMoreCommentButton.transform.SetAsLastSibling();

        detailsPanel.OnCommentAdded();
    }

    private void CancelUserComment()
    {
        if (commentInputField == null)
            return;

        commentInputField.text = "";
    }

    private void LoadExtraComments()
    {
        if (detailsPanel)
            detailsPanel.PlayButtonClickSound();

        LoadMoreComments();        
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V) && episodesSpawner.storiesDBItem != null)
        {
            StoriesDBItem storyItem = episodesSpawner.storiesDBItem;

            string englishTitleNoSpace = storyItem.storyTitleEnglish;
            englishTitleNoSpace = englishTitleNoSpace.Replace(" ", "");

            fireStoreHandler.AddTestComments($"{englishTitleNoSpace}_{storyItem.storyProgressFileName}", 20);
        }
    }
#endif
}
