using System;
using UnityEngine;

public static class DateTimeExtension
{
    public static void SaveDate(this DateTime _date, string Key = "SavedDate")
    { 
        string d = Convert.ToString(_date); 
        PlayerPrefs.SetString(Key, d); 
    }

    public static DateTime GetSavedDate(string key = "SavedDate") 
    { 
        if (PlayerPrefs.HasKey(key)) 
        { 
            string d = PlayerPrefs.GetString(key); 
            return Convert.ToDateTime(d); 
        } 
        else 
        { 
            return DateTime.Now; 
        } 
    }
}