using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player_Data))]
public class Player_DataEditor : Editor
{
    private Color defaultGUIColor;

    private void OnEnable()
    {
        defaultGUIColor = GUI.backgroundColor;
    }

    public override void OnInspectorGUI()
    {
        Player_Data playerData = target as Player_Data;

        DrawDefaultInspector();

        EditorGUILayout.Space(15f);

        GUIStyle buttonTextStyle = new GUIStyle(GUI.skin.button);
        buttonTextStyle.fontStyle = FontStyle.Bold;
        buttonTextStyle.normal.textColor = Color.white;
        buttonTextStyle.hover.textColor = Color.white;

        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Reset Data", buttonTextStyle, GUILayout.ExpandWidth(false)))
        {
            Undo.RecordObject(target, "Player Data Hard Reset");
            playerData.ResetPlayerData();
        }
        GUI.backgroundColor = defaultGUIColor;

        //EditorGUILayout.Space(5f);

        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Open File Location", buttonTextStyle, GUILayout.ExpandWidth(false)))
            SaveLoadGame.OpenPlayerDataFileLocation();
        GUI.backgroundColor = defaultGUIColor;

        //EditorGUILayout.Space(5f);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete Settings File", buttonTextStyle, GUILayout.ExpandWidth(false)))
            SaveLoadGame.DeletePlayerData();
        GUI.backgroundColor = defaultGUIColor;
        
        //EditorGUILayout.Space(5f);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete All Progresses", buttonTextStyle, GUILayout.ExpandWidth(false)))
            SaveLoadGame.DeleteAllProgress();
        GUI.backgroundColor = defaultGUIColor;

        EditorGUILayout.EndHorizontal();
    }
}
