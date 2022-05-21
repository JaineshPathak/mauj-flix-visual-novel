#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Fungus;

public class CharacterReworker : MonoBehaviour
{
    public void DoIt()
    {
        Character[] characters = GetComponentsInChildren<Character>();

        if(characters.Length <= 0)
        {
            Debug.LogError("Character Reworker: Oops! Looks like I couldn't get Characters as Child Objects!");
            return;
        }

        for (int i = 0; i < characters.Length && (characters.Length > 0); i++)
        {
            characters[i].FillupPortraitsNamesList();
            characters[i].Portraits.Clear();
        }

        if (characters.Length > 0)
            Debug.Log("Character Reworker: REWORKING DONE!");
    }
}

[CustomEditor(typeof(CharacterReworker)), CanEditMultipleObjects]
public class CharacterReworkerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterReworker characterReworker = target as CharacterReworker;

        EditorGUILayout.HelpBox("This will fill the names of the Portrait Names and Clear all Portraits names\n Proceed ahead with CAUTION!", MessageType.Warning);

        EditorGUILayout.Space(10f);

        if(GUILayout.Button("DO IT!"))
        {
            characterReworker.DoIt();
        }
    }
}
#endif