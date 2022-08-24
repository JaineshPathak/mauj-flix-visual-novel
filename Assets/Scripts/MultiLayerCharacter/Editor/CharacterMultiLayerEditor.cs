using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterMultiLayer))]
public class CharacterMultiLayerEditor : Editor
{
    protected SerializedProperty nameTextProp;
    protected SerializedProperty nameColorProp;
    protected SerializedProperty soundEffectProp;
    protected SerializedProperty portraitBaseBodyProp;
    protected SerializedProperty portraitClothesProp;
    protected SerializedProperty portraitFacesProp;
    protected SerializedProperty portraitsFaceProp;
    protected SerializedProperty descriptionProp;
    protected SerializedProperty setSayDialogProp;

    protected virtual void OnEnable()
    {
        nameTextProp = serializedObject.FindProperty("nameText");
        nameColorProp = serializedObject.FindProperty("nameColor");
        soundEffectProp = serializedObject.FindProperty("soundEffect");

        portraitBaseBodyProp = serializedObject.FindProperty("portraitBaseBody");
        portraitClothesProp = serializedObject.FindProperty("portraitClothes");
        portraitFacesProp = serializedObject.FindProperty("portraitFaces");

        portraitsFaceProp = serializedObject.FindProperty("portraitsFace");

        descriptionProp = serializedObject.FindProperty("description");
        setSayDialogProp = serializedObject.FindProperty("setSayDialog");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CharacterMultiLayer t = target as CharacterMultiLayer;
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(nameTextProp, new GUIContent("Name Text", "Name of the character display in the dialog"));
        EditorGUILayout.PropertyField(nameColorProp, new GUIContent("Name Color", "Color of name text display in the dialog"));
        EditorGUILayout.PropertyField(soundEffectProp, new GUIContent("Sound Effect", "Sound to play when the character is talking. Overrides the setting in the Dialog."));
        EditorGUILayout.PropertyField(setSayDialogProp);
        EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description", "Notes about this story character (personality, attibutes, etc.)"));

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(portraitBaseBodyProp);

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(portraitClothesProp);

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(portraitFacesProp);

        EditorGUILayout.Separator();

        string[] facingArrows = new string[]
        {
            "FRONT",
            "<--",
            "-->",
        };        
        portraitsFaceProp.enumValueIndex = EditorGUILayout.Popup("Portraits Face", (int)portraitsFaceProp.enumValueIndex, facingArrows);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(t);

        serializedObject.ApplyModifiedProperties();
    }
}
