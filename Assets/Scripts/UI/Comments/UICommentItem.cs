using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICommentItem : MonoBehaviour
{
    [SerializeField] private RawImage profileImage;
    [SerializeField] private TextMeshProUGUI profileUsernameText;
    [SerializeField] private TextMeshProUGUI profileCommentText;

    public void SetupItem(Texture _profileImage, string _profileUsername, string _userComment)
    {
        if (profileImage == null) return;
        profileImage.texture = _profileImage;

        if (profileUsernameText == null) return;
        profileUsernameText.text = _profileUsername;

        if (profileCommentText == null) return;
        profileCommentText.text = _userComment;
    }
}
