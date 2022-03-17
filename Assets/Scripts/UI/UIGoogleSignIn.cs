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

    [Space(15)]

    public Button signInButton;
    public Button signOutButton;
    
    [Space(15)]

    public Sprite genericAvatarSprite;

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

                signInButton.gameObject.SetActive(true);
                signOutButton.gameObject.SetActive(false);

                displayPicImage.texture = genericAvatarSprite.texture;
            }
        }
    }

    private void OnGoogleLoginSilentDone(Task<GoogleSignInUser> UserResult)
    {
        signInButton.gameObject.SetActive(false);
        signOutButton.gameObject.SetActive(true);

        displayNameText.text = UserResult.Result.DisplayName;
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

        signInButton.gameObject.SetActive(true);
        signOutButton.gameObject.SetActive(false);

        displayPicImage.texture = genericAvatarSprite.texture;
    }
}
