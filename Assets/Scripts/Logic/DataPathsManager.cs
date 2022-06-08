using UnityEngine;

public class DataPathsManager : MonoBehaviourSingletonPersistent<DataPathsManager>
{
    //public static DataPathsManager instance;

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }*/

    private void Start()
    {
        DataPaths.progressDirectoryPath = Application.persistentDataPath + "/progress";
        DataPaths.loadProgressPath = Application.persistentDataPath + "/progress/";
    }
}
