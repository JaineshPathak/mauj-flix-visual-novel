using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
//using Firebase.Auth;
//using Firebase.Firestore;

public class UICommentsSection : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private UICommentItem commentItemPrefab;

    [Header("Comments Input Field")]
    [SerializeField] private TMP_InputField commentInputField;
    [SerializeField] private Button addCommentButton;
    [SerializeField] private Button cancelCommentButton;

    [Space(15)]

    [SerializeField] private GameObject loginToCommentText;
    [SerializeField] private GameObject buttonsGroup;

    private FirebaseAuthHandler fireAuthHandler;
    private FirebaseFirestoreHandler fireStoreHandler;
    private EpisodesSpawner episodesSpawner;

    private FirestoreCommentData commentData;

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
            userProfilePicUrl = fireAuthHandler.userCurrent.PhotoUrl.AbsoluteUri,
            userComment = commentInputField.text
        };

        string englishTitleNoSpace = storyItem.storyTitleEnglish;
        englishTitleNoSpace = englishTitleNoSpace.Replace(" ", "");
        fireStoreHandler.AddUserComment($"{storyItem.storyProgressFileName}_{englishTitleNoSpace}", fireAuthHandler.userCurrent.UserId, commentData, () =>
        {
            StartCoroutine(PostSetupCommentItemRoutine());            
        });
    }

    private IEnumerator PostSetupCommentItemRoutine()
    {
        UnityWebRequest wr = UnityWebRequestTexture.GetTexture(fireAuthHandler.userCurrent.PhotoUrl);

        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError)
            Debug.Log("Comment Section: Unable to Get User Profile Pic: " + wr.error);
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            tex = DownloadHandlerTexture.GetContent(wr);

            UICommentItem commentItem = Instantiate(commentItemPrefab, transform);
            commentItem.SetupItem(tex, commentData.userName, commentData.userComment);

            commentInputField.text = "";

#if UNITY_EDITOR
            Debug.Log("Comment Section: Comment Successfully Added!");
#endif
        }
    }

    private void CancelUserComment()
    {
        if (commentInputField == null)
            return;

        commentInputField.text = "";
    }
}
