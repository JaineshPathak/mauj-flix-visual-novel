using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player_Data))]
public class Player_DataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Player_Data playerData = target as Player_Data;

        DrawDefaultInspector();

        EditorGUILayout.Space(15f);

        if (GUILayout.Button("Reset Data"))
        {
            Undo.RecordObject(target, "Player Data Hard Reset");
            playerData.ResetPlayerData();
        }

        EditorGUILayout.Space(15f);

        GUIStyle buttonTextStyle = new GUIStyle(GUI.skin.button);
        buttonTextStyle.normal.textColor = Color.white;
        buttonTextStyle.hover.textColor = Color.white;

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete Settings File", buttonTextStyle))
            SaveLoadGame.DeletePlayerData();        
    }
}
