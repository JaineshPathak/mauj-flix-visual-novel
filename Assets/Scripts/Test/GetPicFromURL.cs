using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.Firestore;

public class GetPicFromURL : MonoBehaviour
{
    public string url;
    public RawImage rawImage;

    private void Start()
    {
        Debug.Log(FirebaseFirestore.DefaultInstance != null);

        CollectionReference collection = FirebaseFirestore.DefaultInstance.Collection("a31773004654d61_BhutiyaGaav");
        DocumentReference docRef = collection.Document("N9BVSP4TmSMFVfr5dnJv3BMrOl72");
        docRef.GetSnapshotAsync().ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("Completed");
                DocumentSnapshot doc = task.Result;
                Debug.Log(doc.Id);
                if (doc.Exists)
                {
                    Debug.Log($"Comment Data EXISTS: " + doc.Exists);

                    FirestoreCommentData commentData = doc.ConvertTo<FirestoreCommentData>();
                    StartCoroutine(GetProfilePic(commentData));

                    Debug.Log($"Comment Data URL: " + commentData.userProfilePicUrl);
                }
            }            
        });
    }

    private IEnumerator GetProfilePic(FirestoreCommentData firestoreComment)
    {
        UnityWebRequest wr = UnityWebRequestTexture.GetTexture(firestoreComment.userProfilePicUrl);

        yield return wr.SendWebRequest();
        
        if (wr.isNetworkError || wr.isHttpError)
            Debug.Log("Comment Section: Unable to Get User Profile Pic: " + wr.error);
        else if(wr.isDone)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex = DownloadHandlerTexture.GetContent(wr);

            rawImage.texture = tex;
        }
    }
}
