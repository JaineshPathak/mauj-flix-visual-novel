using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICommentItem : MonoBehaviour
{
    [SerializeField] private RawImage profileImage;
    [SerializeField] private TextMeshProUGUI profileUsernameText;
    [SerializeField] private TextMeshProUGUI profileCommentText;
    [SerializeField] private string profileUserID;

    [HideInInspector] public Transform itemParent;

    public void SetupItem(Texture _profileImage, string _profileUsername, string _userComment, string _userID)
    {
        if (profileImage == null) return;
        profileImage.texture = _profileImage;

        if (profileUsernameText == null) return;
        profileUsernameText.text = _profileUsername;

        if (profileCommentText == null) return;
        profileCommentText.text = _userComment;

        profileUserID = _userID;
    }

    public void SetupItem(string _profileUsername, string _userComment)
    {
        if (profileUsernameText == null) return;
        profileUsernameText.text = _profileUsername;

        if (profileCommentText == null) return;
        profileCommentText.text = _userComment;
    }

    public void OnProfileImageClicked()
    {
        if (UserControl.instance == null || !UserControl.instance.AdminMode)
            return;

        UniClipboard.SetText(profileUserID);

        if(UIMiniMessagePanel.instance != null)
            UIMiniMessagePanel.instance.ShowMiniMessage("User ID Copied!");
    }
}