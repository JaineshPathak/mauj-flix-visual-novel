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

        commentData = new FirestoreCommentData()
        {
            userID = fireAuthHandler.userCurrent.UserId,
            userName = fireAuthHandler.userCurrent.DisplayName,
            userEmail = fireAuthHandler.userCurrent.Email,
            userProfilePicUrl = fireAuthHandler.userCurrent.PhotoUrl.AbsoluteUri,
            userComment = commentInputField.text
        };

        string englishTitleNoSpace = storyItem.storyTitleEnglish;
        englishTitleNoSpace = englishTitleNoSpace.Replace(" ", "");
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

            StartCoroutine(GetUserProfilePicRoutine(profileURL, (Texture profilePicTex) =>
            {
                //SpawnCommentItem(transform, profilePicTex, otherCommentData);
                SpawnCommentItem(transform, profileName, profileComment, profilePicTex);
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

            StartCoroutine(GetUserProfilePicRoutine(profileURL, (Texture profilePicTex) =>
            {
                //SpawnCommentItem(transform, profilePicTex, otherCommentData);
                SpawnCommentItem(transform, profileName, profileComment, profilePicTex);
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
        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_tex, commentData.userName, commentData.userComment);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, commentData.userName, commentData.userComment);

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
            commentItem.SetupItem(_tex, commentData.userName, commentData.userComment);

            Invoke("RebuildSectionLayout", 0.5f);
        }
    }

    public void SpawnCommentItem(Transform _parent, FirestoreCommentData _commentData)
    {
        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_commentData.userName, _commentData.userComment);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_commentData.userName, _commentData.userComment);

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
            commentItem.SetupItem(_commentData.userName, _commentData.userComment);

            Invoke("RebuildSectionLayout", 0.5f);
        }
    }

    public void SpawnCommentItem(Transform _parent, string _profileName, string _profileComment, Texture _tex)
    {
        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_tex, _profileName, _profileComment);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, _profileName, _profileComment);

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
            commentItem.SetupItem(_tex, _profileName, _profileComment);

            Invoke("RebuildSectionLayout", 0.5f);
        }
    }

    public void SpawnCommentItem(Transform _parent, Texture _tex, FirestoreCommentData _commentData)
    {
        UICommentItem commentItem = null;
        if (CommentsPool.instance != null)
        {
            commentItem = CommentsPool.instance.GetCommentItem();
            if (commentItem != null)
            {
                commentItem.gameObject.SetActive(true);
                commentItem.transform.parent = _parent;
                commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment);

                Invoke("RebuildSectionLayout", 0.5f);
            }
            else
            {
                commentItem = Instantiate(commentItemPrefab, _parent);
                commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment);

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
            commentItem.SetupItem(_tex, _commentData.userName, _commentData.userComment);

            //RebuildSectionLayout();
            Invoke("RebuildSectionLayout", 0.5f);
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
}
