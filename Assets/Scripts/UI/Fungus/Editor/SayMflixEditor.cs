using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;
using Fungus;

[CustomEditor(typeof(SayMflix))]
public class SayMflixEditor : SayEditor
{
    protected SerializedProperty sayDialogueStyleSerialized;
    protected SerializedProperty characterNameSerialized;
    protected SerializedProperty characterNameRefSerialized;

    public override void OnEnable()
    {
        base.OnEnable();

        sayDialogueStyleSerialized = serializedObject.FindProperty("sayDialogStyle");
        characterNameSerialized = serializedObject.FindProperty("characterGender");
        characterNameRefSerialized = serializedObject.FindProperty("nameVariableRef");
    }

    public override void DrawCommandGUI()
    {
        serializedObject.Update();

        bool showPortraits = false;
        CommandEditor.ObjectField<Character>(characterProp,
                                            new GUIContent("Character", "Character that is speaking"),
                                            new GUIContent("<None>"),
                                            Character.ActiveCharacters);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        characterProp.objectReferenceValue = (Character)EditorGUILayout.ObjectField(characterProp.objectReferenceValue, typeof(Character), true);
        EditorGUILayout.EndHorizontal();

        SayMflix t = target as SayMflix;

        // Only show portrait selection if...
        if (t._Character != null &&              // Character is selected
            t._Character.Portraits != null &&    // Character has a portraits field
            t._Character.Portraits.Count > 0)   // Selected Character has at least 1 portrait
        {
            showPortraits = true;
        }

        if (showPortraits)
        {
            CommandEditor.ObjectField<Sprite>(portraitProp,
                                              new GUIContent("Portrait", "Portrait representing speaking character"),
                                              new GUIContent("<None>"),
                                              t._Character.Portraits);
        }
        else
        {
            if (!t.ExtendPrevious)
            {
                t.Portrait = null;
            }
        }

        EditorGUILayout.PropertyField(storyTextProp);

        EditorGUILayout.PropertyField(descriptionProp);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(extendPreviousProp);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Tag Help", "View available tags"), new GUIStyle(EditorStyles.miniButton)))
        {
            showTagHelp = !showTagHelp;
        }
        EditorGUILayout.EndHorizontal();

        if (showTagHelp)
        {
            DrawTagHelpLabel();
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(voiceOverClipProp,
                                      new GUIContent("Voice Over Clip", "Voice over audio to play when the text is displayed"));

        EditorGUILayout.PropertyField(showAlwaysProp);

        if (showAlwaysProp.boolValue == false)
        {
            EditorGUILayout.PropertyField(showCountProp);
        }

        GUIStyle centeredLabel = new GUIStyle(EditorStyles.label);
        centeredLabel.alignment = TextAnchor.MiddleCenter;
        GUIStyle leftButton = new GUIStyle(EditorStyles.miniButtonLeft);
        leftButton.fontSize = 10;
        leftButton.font = EditorStyles.toolbarButton.font;
        GUIStyle rightButton = new GUIStyle(EditorStyles.miniButtonRight);
        rightButton.fontSize = 10;
        rightButton.font = EditorStyles.toolbarButton.font;

        EditorGUILayout.PropertyField(fadeWhenDoneProp);
        EditorGUILayout.PropertyField(waitForClickProp);
        EditorGUILayout.PropertyField(stopVoiceoverProp);
        EditorGUILayout.PropertyField(sayDialogueStyleSerialized, new GUIContent("Dialogue Type"));

        if (t.sayDialogStyle == SayMflix.SayDialogStyle.Type_CharacterName)
        {
            EditorGUILayout.PropertyField(characterNameSerialized, new GUIContent("Character Gender"));
            EditorGUILayout.PropertyField(characterNameRefSerialized, new GUIContent("Character Variable"));
        }

        EditorGUILayout.PropertyField(waitForVOProp);

        if (showPortraits && t.Portrait != null)
        {
            Texture2D characterTexture = t.Portrait.texture;
            float aspect = (float)characterTexture.width / (float)characterTexture.height;
            Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
            if (characterTexture != null)
            {
                GUI.DrawTexture(previewRect, characterTexture, ScaleMode.ScaleToFit, true, aspect);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
