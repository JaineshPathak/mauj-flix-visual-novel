using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public struct FirestoreCommentData
{
    [FirestoreProperty]
    public string userID { get; set; }

    [FirestoreProperty]
    public string userName { get; set; }

    [FirestoreProperty]
    public string userEmail { get; set; }

    [FirestoreProperty]
    public string userProfilePicUrl { get; set; }

    [FirestoreProperty]
    public string userComment { get; set; }

    [FirestoreProperty]
    public string[] userComments { get; set; }
}