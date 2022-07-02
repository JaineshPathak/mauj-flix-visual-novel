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
                    Debug.Log("Firebase Starter Engine: App available: " + task.Result);
                    StartCoroutine(LoadSceneAtIndex(1));
                }
                else
                {
                    Debug.LogError(string.Format("Firebase Starter Engine: Could not resolve all Firebase dependencies: {0}", task.Result));
                }
            }
        });

#if UNITY_EDITOR
        Caching.ClearCache();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
#endif

        yield return null;        
    }

    private IEnumerator LoadSceneAtIndex(int index)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(index);
    }
}
