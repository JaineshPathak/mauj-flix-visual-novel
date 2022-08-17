using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus;
using Fungus.EditorUtils;

[CustomEditor(typeof(PortraitMultiLayer))]
public class PortraitMultiLayerEditor : CommandEditor
{
    protected SerializedProperty stageProp;
    protected SerializedProperty displayProp;
    protected SerializedProperty characterProp;
    protected SerializedProperty replacedCharacterProp;
    protected SerializedProperty portraitBaseBodyProp;
    protected SerializedProperty portraitClothesProp;
    protected SerializedProperty portraitFaceProp;
    //protected SerializedProperty portraitNameProp;
    protected SerializedProperty offsetProp;
    protected SerializedProperty fromPositionProp;
    protected SerializedProperty toPositionProp;
    protected SerializedProperty facingProp;
    protected SerializedProperty useDefaultSettingsProp;
    protected SerializedProperty fadeDurationProp;
    protected SerializedProperty moveDurationProp;
    protected SerializedProperty shiftOffsetProp;
    protected SerializedProperty waitUntilFinishedProp;
    protected SerializedProperty moveProp;
    protected SerializedProperty shiftIntoPlaceProp;

    public override void OnEnable()
    {
        base.OnEnable();

        stageProp = serializedObject.FindProperty("stage");
        displayProp = serializedObject.FindProperty("display");
        characterProp = serializedObject.FindProperty("character");
        replacedCharacterProp = serializedObject.FindProperty("replacedCharacter");
        
        portraitBaseBodyProp = serializedObject.FindProperty("portraitBaseBody");
        portraitClothesProp = serializedObject.FindProperty("portraitClothes");
        portraitFaceProp = serializedObject.FindProperty("portraitFace");
        
        //portraitNameProp = serializedObject.FindProperty("_PortraitName");
        offsetProp = serializedObject.FindProperty("offset");
        fromPositionProp = serializedObject.FindProperty("fromPosition");
        toPositionProp = serializedObject.FindProperty("toPosition");
        facingProp = serializedObject.FindProperty("facing");
        useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
        fadeDurationProp = serializedObject.FindProperty("fadeDuration");
        moveDurationProp = serializedObject.FindProperty("moveDuration");
        shiftOffsetProp = serializedObject.FindProperty("shiftOffset");
        waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
        moveProp = serializedObject.FindProperty("move");
        shiftIntoPlaceProp = serializedObject.FindProperty("shiftIntoPlace");
    }

    public override void DrawCommandGUI()
    {
        serializedObject.Update();

        PortraitMultiLayer t = target as PortraitMultiLayer;

        if (StageMultiLayer.ActiveStages.Count > 1)
        {
            CommandEditor.ObjectField<StageMultiLayer>(stageProp,
                                                     new GUIContent("Portrait Stage", "Stage to display the character portraits on"),
                                                     new GUIContent("<Default>"),
                                                     StageMultiLayer.ActiveStages);
        }
        else
        {
            t._Stage = null;
        }
        // Format Enum names
        string[] displayLabels = StringFormatter.FormatEnumNames(t.Display, "<None>");
        displayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)displayProp.enumValueIndex, displayLabels);

        //EditorGUILayout.PropertyField(portraitNameProp);

        string characterLabel = "Character";
        if (t.Display == DisplayType.Replace)
        {
            CommandEditor.ObjectField<CharacterMultiLayer>(replacedCharacterProp,
                                                 new GUIContent("Replace", "Character to replace"),
                                                 new GUIContent("<None>"),
                                                 CharacterMultiLayer.ActiveCharactersMultiLayer);
            characterLabel = "With";
        }

        CommandEditor.ObjectField<CharacterMultiLayer>(characterProp,
                                             new GUIContent(characterLabel, "Character to display"),
                                             new GUIContent("<None>"),
                                             CharacterMultiLayer.ActiveCharactersMultiLayer);

        bool showOptionalFields = true;
        StageMultiLayer s = t._Stage;
        // Only show optional portrait fields once required fields have been filled...
        if (t._Character != null)                // Character is selected
        {
            if (t._Character.PortraitBaseBody == null ||    // Character has a portraits field
                t._Character.PortraitClothes.Count <= 0 || t._Character.PortraitFaces.Count <= 0)   // Character has at least one portrait
            {
                EditorGUILayout.HelpBox("This character has no portraits. Please add portraits to the character's prefab before using this command.", MessageType.Error);
                showOptionalFields = false;
            }
            if (t._Stage == null)            // If default portrait stage selected
            {
                if (t._Stage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                    s = GameObject.FindObjectOfType<StageMultiLayer>();
                }
            }
            if (s == null)
            {
                EditorGUILayout.HelpBox("No portrait stage has been set.", MessageType.Error);
                showOptionalFields = false;
            }
        }





        if (t.Display != DisplayType.None && t._Character != null && showOptionalFields)
        {
            if (t.Display != DisplayType.Hide && t.Display != DisplayType.MoveToFront)
            {
                // PORTRAIT BASE BODY
                EditorGUILayout.PropertyField(portraitBaseBodyProp, new GUIContent("Portrait Base Body", "Portrait Base Body"));

                // PORTRAIT CLOTHES
                CommandEditor.ObjectField<Sprite>(portraitClothesProp,
                                                  new GUIContent("Portrait Clothes", "Portrait Clothes"),
                                                  new GUIContent("<NULL>"),
                                                  t._Character.PortraitClothes);

                // PORTRAIT FACE
                CommandEditor.ObjectField<Sprite>(portraitFaceProp,
                                                  new GUIContent("Portrait Face", "Portrait Face"),
                                                  new GUIContent("<NULL>"),
                                                  t._Character.PortraitFaces);

                if (t._Character.PortraitsFace != FacingDirection.None)
                {
                    // FACING
                    // Display the values of the facing enum as <-- and --> arrows to avoid confusion with position field
                    string[] facingArrows = new string[]
                    {
                            "<Previous>",
                            "<--",
                            "-->",
                    };
                    facingProp.enumValueIndex = EditorGUILayout.Popup("Facing", (int)facingProp.enumValueIndex, facingArrows);
                }
                else
                {
                    t.Facing = FacingDirection.None;
                }
            }
            else
            {
                t._PortraitBaseBody = null;
                t.Facing = FacingDirection.None;
            }
            string toPositionPrefix = "";
            if (t.Move)
            {
                // MOVE
                EditorGUILayout.PropertyField(moveProp);
            }
            if (t.Move)
            {
                if (t.Display != DisplayType.Hide)
                {
                    // START FROM OFFSET
                    EditorGUILayout.PropertyField(shiftIntoPlaceProp);
                }
            }
            if (t.Move)
            {
                if (t.Display != DisplayType.Hide)
                {
                    if (t.ShiftIntoPlace)
                    {
                        t.FromPosition = null;
                        // OFFSET
                        // Format Enum names
                        string[] offsetLabels = StringFormatter.FormatEnumNames(t.Offset, "<Previous>");
                        offsetProp.enumValueIndex = EditorGUILayout.Popup("From Offset", (int)offsetProp.enumValueIndex, offsetLabels);
                    }
                    else
                    {
                        t.Offset = PositionOffset.None;
                        // FROM POSITION
                        CommandEditor.ObjectField<RectTransform>(fromPositionProp,
                                                                 new GUIContent("From Position", "Move the portrait to this position"),
                                                                 new GUIContent("<Previous>"),
                                                                 s.Positions);
                    }
                }
                toPositionPrefix = "To ";
            }
            else
            {
                t.ShiftIntoPlace = false;
                t.FromPosition = null;
                toPositionPrefix = "At ";
            }
            if (t.Display == DisplayType.Show || (t.Display == DisplayType.Hide && t.Move))
            {
                // TO POSITION
                CommandEditor.ObjectField<RectTransform>(toPositionProp,
                                                         new GUIContent(toPositionPrefix + "Position", "Move the portrait to this position"),
                                                         new GUIContent("<Previous>"),
                                                         s.Positions);
            }
            else
            {
                t.ToPosition = null;
            }
            if (!t.Move && t.Display != DisplayType.MoveToFront)
            {
                // MOVE
                EditorGUILayout.PropertyField(moveProp);
            }
            if (t.Display != DisplayType.MoveToFront)
            {

                EditorGUILayout.Separator();

                // USE DEFAULT SETTINGS
                EditorGUILayout.PropertyField(useDefaultSettingsProp);
                if (!t.UseDefaultSettings)
                {
                    // FADE DURATION
                    EditorGUILayout.PropertyField(fadeDurationProp);
                    if (t.Move)
                    {
                        // MOVE SPEED
                        EditorGUILayout.PropertyField(moveDurationProp);
                    }
                    if (t.ShiftIntoPlace)
                    {
                        // SHIFT OFFSET
                        EditorGUILayout.PropertyField(shiftOffsetProp);
                    }
                }
            }
            else
            {
                t.Move = false;
                t.UseDefaultSettings = true;
                EditorGUILayout.Separator();
            }

            EditorGUILayout.PropertyField(waitUntilFinishedProp);


            //REMOVED TEXTURES DISPLAY & DOWN BUTTON (FOR NOW)
            #region REMOVED
            /*if (t._Portrait != null && t.Display != DisplayType.Hide)
            {
                Texture2D characterTexture = t._Portrait.texture;

                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));

                if (characterTexture != null)
                {
                    GUI.DrawTexture(previewRect, characterTexture, ScaleMode.ScaleToFit, true, aspect);
                }
            }

            if (t.Display != DisplayType.Hide)
            {
                string portraitName = "<Previous>";
                if (t._Portrait != null)
                {
                    portraitName = t._Portrait.name;
                }
                string portraitSummary = " " + portraitName;
                int toolbarInt = 1;
                string[] toolbarStrings = { "<--", portraitSummary, "-->" };
                toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.MinHeight(20));
                int portraitIndex = -1;

                if (toolbarInt != 1)
                {
                    for (int i = 0; i < t._Character.Portraits.Count; i++)
                    {
                        if (portraitName == t._Character.Portraits[i].name)
                        {
                            portraitIndex = i;
                        }
                    }
                }

                if (toolbarInt == 0)
                {
                    if (portraitIndex > 0)
                    {
                        t._Portrait = t._Character.Portraits[--portraitIndex];
                    }
                    else
                    {
                        t._Portrait = null;
                    }
                }
                if (toolbarInt == 2)
                {
                    if (portraitIndex < t._Character.Portraits.Count - 1)
                    {
                        t._Portrait = t._Character.Portraits[++portraitIndex];
                    }
                }
            }*/
            #endregion
        }



        serializedObject.ApplyModifiedProperties();
    }
}
