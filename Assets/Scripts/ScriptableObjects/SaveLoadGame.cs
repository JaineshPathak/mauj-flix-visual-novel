using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System;

public class SaveLoadGame : MonoBehaviour
{
    public static Player_Data playerData;

    private static string configFilePath;

    public static void SetPlayerDataInstance(Player_Data _newPlayerData)
    {
        playerData = _newPlayerData;
    }

    public static Player_Data LoadPlayerData()
    {
        configFilePath = Application.persistentDataPath + "/Settings.JSON";

        if (File.Exists(configFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream settingsFile = File.Open(configFilePath, FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(settingsFile) as string, playerData);
            settingsFile.Close();
        }

        return playerData;
    }

    public static void SavePlayerData(Player_Data savePlayerData)
    {
        configFilePath = Application.persistentDataPath + "/Settings.JSON";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream settingsFile = File.Create(configFilePath);
        string json = JsonUtility.ToJson(savePlayerData);
        bf.Serialize(settingsFile, json);
        settingsFile.Close();
    }

    public static void DeletePlayerData()
    {
        string configFilePath = Application.persistentDataPath + "/Settings.JSON";

        if (File.Exists(configFilePath))
        {
            UnityEngine.Debug.Log(configFilePath + " - File is deleted!");
            File.Delete(configFilePath);
        }
        else
            UnityEngine.Debug.Log(configFilePath + " - No such File Found!");
    }

    public static void DeleteAllProgress()
    {
        string progressPath = Path.Combine(Application.persistentDataPath, "progress");

        DirectoryInfo progressDirInfo = new DirectoryInfo(progressPath);
        
        if(progressDirInfo.GetFiles().Length <= 0)
        {
            UnityEngine.Debug.Log(progressPath + " - Directory is Empty!");
            return;
        }

        int count = 0;
        foreach (FileInfo file in progressDirInfo.GetFiles())
        {
            file.Delete();
            count++;
        }

        UnityEngine.Debug.Log(progressPath + (count > 0 ? " - Directory Cleared with Total : " + count : "- Directory is Empty!"));
    }

    public static bool CheckForFile()
    {
        configFilePath = Application.persistentDataPath + "/Settings.JSON";

        if (File.Exists(configFilePath))
            return true;

        return false;
    }

    public static void OpenPlayerDataFileLocation()
    {
        Process.Start(Application.persistentDataPath);
    }
}
