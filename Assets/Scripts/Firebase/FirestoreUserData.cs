using Firebase.Firestore;

[FirestoreData]
public struct FirestoreUserData
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
    public float diamondsAmount { get; set; }

    [FirestoreProperty]
    public float ticketsAmount { get; set; }
}