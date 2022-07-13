using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;

public class UICommentsSection : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private UICommentItem commentItemPrefab;
}
