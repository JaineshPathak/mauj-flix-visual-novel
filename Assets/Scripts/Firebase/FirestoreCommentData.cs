using Firebase.Firestore;

[FirestoreData]
public class FirestoreCommentData
{
    [FirestoreProperty]
    public string userID { get; set; }

    [FirestoreProperty]
    public string userName { get; set; }    

    [FirestoreProperty]
    public string userProfilePicUrl { get; set; }

    [FirestoreProperty]
    public string userComment { get; set; }
}
