using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;

public class FirebaseStarterEngine : MonoBehaviour
{
    private IEnumerator Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Firebase App available: " + task.Result);
                }
                else
                {
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", task.Result));
                }
            }
        });

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1);
    }    
}
