using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICommentItem : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI profileUsernameText;
    [SerializeField] private TextMeshProUGUI profileCommentText;

    public void SetupItem(Sprite _profileImageSprite, string _profileUsername, string _userComment)
    {
        if (profileImage == null) return;
        profileImage.sprite = _profileImageSprite;

        if (profileUsernameText == null) return;
        profileUsernameText.text = _profileUsername;

        if (profileCommentText == null) return;
        profileCommentText.text = _userComment;
    }
}
