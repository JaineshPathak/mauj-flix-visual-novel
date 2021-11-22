using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if(!Directory.Exists(DataPaths.progressDirectoryPath))
            Directory.CreateDirectory(DataPaths.progressDirectoryPath);

        string path = DataPaths.loadProgressPath + saveName + DataPaths.loadProgressFileExtension;
        
        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);
        file.Close();

        return true;
    }

    public static void SaveAsTextFile(string saveName, string saveData)
    {
        //string path = Application.persistentDataPath + "/progress/" + saveName + ".json";

        if (!Directory.Exists(DataPaths.progressDirectoryPath))
            Directory.CreateDirectory(DataPaths.progressDirectoryPath);

        string path = DataPaths.loadProgressPath + saveName + DataPaths.loadProgressFileExtension;
        
        File.WriteAllText(path, saveData);

#if UNITY_EDITOR
        Debug.Log(path + " - FILE SAVED!");
#endif
    }

    public static void SaveAsTextFile(string path, string saveName, string saveData)
    {
        string fullPath = path + "/" + saveName;
        File.WriteAllText(fullPath, saveData);

#if UNITY_EDITOR
        Debug.Log(path + " - FILE SAVED!");
#endif
    }

    public static object Load(string path)
    {
        if(!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
#if UNITY_EDITOR
            Debug.LogError($"Failed to load file at path: {path}");
#endif
            file.Close();
            return null;
        }
    }

    public static string LoadFromTextFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            return saveString;
        }
        else
            return null;
    }

    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public static void DeleteIfFileExists(string path)
    {
        if(FileExists(path))
        {
#if UNITY_EDITOR
            Debug.Log(path + " - File is deleted!");
#endif
            File.Delete(path);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log(path + " - No such File Found!");
#endif
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
