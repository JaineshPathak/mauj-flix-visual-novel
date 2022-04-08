using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Google;

public class UIGoogleSignIn : MonoBehaviour
{
    public RawImage displayPicImage;
    public TextMeshProUGUI displayNameText;
    public TextMeshProUGUI displayIDText;

    [Space(15)]

    public Button signInButton;
    public Button signOutButton;
    public Button userIDCopyClipboardButton;
    
    [Space(15)]

    public Sprite genericAvatarSprite;

    [Space(15)]

    public CanvasGroup copiedIDPanel;

    private string userID;

    private LTSeq copySeq;
    private int copySeqID;

    private void Awake()
    {
        if (signInButton)
        {
            signInButton.onClick.AddListener(OnGoogleLogin);
            signInButton.gameObject.SetActive(false);
        }

        if (signOutButton)
        {
            signOutButton.onClick.AddListener(OnGoogleLogOut);
            signOutButton.gameObject.SetActive(false);
        }

        if (userIDCopyClipboardButton)
        {
            userIDCopyClipboardButton.onClick.AddListener(OnUserIDCopyButton);
            userIDCopyClipboardButton.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(FirebaseAuthHandler.instance != null)
        {
            //User is already Google Logged In
            if (FirebaseAuthHandler.instance.GetProviderID() == "google.com")
                FirebaseAuthHandler.instance.OnGoogleSignInSilent(OnGoogleLoginSilentDone);
            else
            {
                //User is not Google Logged In
                displayNameText.text = "Guest";
                if (FirebaseAuthHandler.instance.userCurrent != null)
                {
                    //userIDCopyClipboardButton.gameObject.SetActive(true);
                    userIDCopyClipboardButton.gameObject.SetActive(false);
                    displayIDText.text = "ID: " + FirebaseAuthHandler.instance.userCurrent.UserId;

                    userID = FirebaseAuthHandler.instance.userCurrent.UserId;
                }
                else
                    userIDCopyClipboardButton.gameObject.SetActive(false);

#if UNITY_EDITOR
                signInButton.gameObject.SetActive(false);
                signOutButton.gameObject.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS
                signInButton.gameObject.SetActive(true);
                signOutButton.gameObject.SetActive(false);
#endif

                displayPicImage.texture = genericAvatarSprite.texture;
            }
        }
    }

    private void OnGoogleLoginSilentDone(Task<GoogleSignInUser> UserResult)
    {
        signInButton.gameObject.SetActive(false);
        signOutButton.gameObject.SetActive(true);

        displayNameText.text = UserResult.Result.DisplayName;

        //userIDCopyClipboardButton.gameObject.SetActive(true);
        userIDCopyClipboardButton.gameObject.SetActive(false);
        displayIDText.text = "ID: " + UserResult.Result.UserId;

        userID = UserResult.Result.UserId;

        StartCoroutine(GetProfilePic(UserResult.Result.ImageUrl));
    }

    private void OnGoogleLogin()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignIn(OnLoginDone);
    }

    private void OnLoginDone(Task<GoogleSignInUser> UserResult)
    {
        displayNameText.text = UserResult.Result.DisplayName;

        //userIDCopyClipboardButton.gameObject.SetActive(true);
        userIDCopyClipboardButton.gameObject.SetActive(false);
        displayIDText.text = "ID: " + UserResult.Result.UserId;

        userID = UserResult.Result.UserId;

        signInButton.gameObject.SetActive(false);
        signOutButton.gameObject.SetActive(true);

        StartCoroutine(GetProfilePic(UserResult.Result.ImageUrl));
    }

    private IEnumerator GetProfilePic(Uri url)
    {
        UnityWebRequest wr = UnityWebRequestTexture.GetTexture(url);

        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError)        
            Debug.Log("Google Sign In: Unable to Get User Profile Pic: " + wr.error);        
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            tex = DownloadHandlerTexture.GetContent(wr);

            if(tex != null)
                displayPicImage.texture = tex;
        }
    }

    //========================================================================================================

    private void OnGoogleLogOut()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignOut(OnLogoutDone);
    }

    private void OnLogoutDone(Task task)
    {
        displayNameText.text = "Guest";
        if (FirebaseAuthHandler.instance.userCurrent != null)
        {
            //userIDCopyClipboardButton.gameObject.SetActive(true);
            userIDCopyClipboardButton.gameObject.SetActive(false);
            displayIDText.text = "ID: " + FirebaseAuthHandler.instance.userCurrent.UserId;

            userID = FirebaseAuthHandler.instance.userCurrent.UserId;
        }
        else
            userIDCopyClipboardButton.gameObject.SetActive(false);

        signInButton.gameObject.SetActive(true);
        signOutButton.gameObject.SetActive(false);

        displayPicImage.texture = genericAvatarSprite.texture;
    }

    //========================================================================================================

    private void OnUserIDCopyButton()
    {
        if (userID.Length <= 0)
            return;

        UniClipboard.SetText(userID);

        ShowCopyIDWindow();
    }

    private void ShowCopyIDWindow()
    {
        if (copiedIDPanel == null)
            return;

        if (LeanTween.isTweening(copySeqID))
            LeanTween.cancel(copySeqID);

        copiedIDPanel.alpha = 0;

        copySeq = LeanTween.sequence();

        copySeqID = copySeq.id;

        copySeq.append(LeanTween.alphaCanvas(copiedIDPanel, 1f, 0.3f).setEaseInOutSine());
        copySeq.append(0.5f);
        copySeq.append(LeanTween.alphaCanvas(copiedIDPanel, 0, 0.3f).setEaseInOutSine());
    }
    //========================================================================================================
}
