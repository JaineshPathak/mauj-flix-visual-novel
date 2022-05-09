// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Character))]
    public class CharacterEditor : Editor
    {
        protected SerializedProperty nameTextProp;
        protected SerializedProperty nameColorProp;
        protected SerializedProperty soundEffectProp;
        protected SerializedProperty portraitsProp;
        protected SerializedProperty portraitsFaceProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty setSayDialogProp;

        //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
        protected SerializedProperty characterAtlasProp;
        protected SerializedProperty portraitsNamesListProp;

        private Sprite[] atlasSpritesList;
        private string[] atlasSpritesNamesList;

        private StringListSearchProvider searchProvider;
        //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll

        protected virtual void OnEnable()
        {
            nameTextProp = serializedObject.FindProperty ("nameText");
            nameColorProp = serializedObject.FindProperty ("nameColor");
            soundEffectProp = serializedObject.FindProperty ("soundEffect");
            portraitsProp = serializedObject.FindProperty ("portraits");
            portraitsFaceProp = serializedObject.FindProperty ("portraitsFace");
            descriptionProp = serializedObject.FindProperty ("description");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");

            //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
            characterAtlasProp = serializedObject.FindProperty("characterAtlas");
            portraitsNamesListProp = serializedObject.FindProperty("portraitsNamesList");
            //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            Character t = target as Character;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(nameTextProp, new GUIContent("Name Text", "Name of the character display in the dialog"));
            EditorGUILayout.PropertyField(nameColorProp, new GUIContent("Name Color", "Color of name text display in the dialog"));
            EditorGUILayout.PropertyField(soundEffectProp, new GUIContent("Sound Effect", "Sound to play when the character is talking. Overrides the setting in the Dialog."));
            EditorGUILayout.PropertyField(setSayDialogProp);
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description", "Notes about this story character (personality, attibutes, etc.)"));

            //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(characterAtlasProp);

            if (t.CharacterAtlas != null)
            {
                EditorGUILayout.BeginHorizontal();
                AddLabel("Select Portrait", "Select Portrait from Atlas");
                if (GUILayout.Button("Add Portrait", EditorStyles.popup, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                {
                    searchProvider = ScriptableObject.CreateInstance<StringListSearchProvider>();
                    searchProvider.Init(atlasSpritesNamesList, (index) =>
                    {
                        t.PortraitsNamesList.Add(atlasSpritesNamesList[index]);
                    });
                    //searchProvider = new StringListSearchProvider(atlasSpritesNamesList);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
                //imageLoader.currentTextureIndexToLoad = EditorGUILayout.Popup(imageLoader.currentTextureIndexToLoad, atlasSpritesNamesList);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
            }

            EditorGUILayout.PropertyField(portraitsNamesListProp);

            if (GUILayout.Button("Get Portraits Names"))
                t.FillupPortraitsNamesList();

            EditorGUILayout.Separator();
            //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll

            if (t.Portraits != null &&
                t.Portraits.Count > 0)
            {
                t.ProfileSprite = t.Portraits[0];
            }
            else
            {
                t.ProfileSprite = null;
            }
            
            if (t.ProfileSprite != null)
            {
                Texture2D characterTexture = t.ProfileSprite.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
            }

            EditorGUILayout.PropertyField(portraitsProp, new GUIContent("Portraits", "Character image sprites to display in the dialog"), true);

            EditorGUILayout.HelpBox("All portrait images should use the exact same resolution to avoid positioning and tiling issues.", MessageType.Info);

            EditorGUILayout.Separator();

            string[] facingArrows = new string[]
            {
                "FRONT",
                "<--",
                "-->",
            };
            portraitsFaceProp.enumValueIndex = EditorGUILayout.Popup("Portraits Face", (int)portraitsFaceProp.enumValueIndex, facingArrows);

            EditorGUILayout.Separator();

            if(EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(t);

            serializedObject.ApplyModifiedProperties();
        }

        private void AddLabel(string label, string labelToolTip)
        {
            GUIContent newContent = new GUIContent(label, labelToolTip);
            EditorGUILayout.LabelField(newContent, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        }
    }
}