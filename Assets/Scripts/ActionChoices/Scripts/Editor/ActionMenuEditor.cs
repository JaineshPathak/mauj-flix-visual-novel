using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus;
using Fungus.EditorUtils;
using System;

[CustomEditor(typeof(ActionMenu))]
public class ActionMenuEditor : CommandEditor
{
    protected SerializedProperty centerActionTextSerial;
    protected SerializedProperty actionItemsListSerial;

    private ActionMenu actionMenu;

    private Color defaultGUIColor;
    private float buttonWidth = 170f;
    private float buttonHeight = 20f;

    private GUIStyle buttonTextStyle;

    private bool show = true;

    public override void OnEnable()
    {
        base.OnEnable();

        defaultGUIColor = GUI.backgroundColor;

        actionMenu = target as ActionMenu;

        buttonTextStyle = new GUIStyle(GUI.skin.button);
        buttonTextStyle.fontStyle = FontStyle.Bold;
        buttonTextStyle.wordWrap = true;
        buttonTextStyle.normal.textColor = Color.white;
        buttonTextStyle.hover.textColor = Color.white;

        centerActionTextSerial = serializedObject.FindProperty("centerActionText");
        actionItemsListSerial = serializedObject.FindProperty("actionItemsList");
    }

    public override void DrawCommandGUI()
    {
        var flowchart = FlowchartWindow.GetFlowchart();
        if (flowchart == null)        
            return;        

        serializedObject.Update();

        EditorGUILayout.PropertyField(centerActionTextSerial);

        EditorGUILayout.Space(10);

        //EditorGUILayout.PropertyField(actionItemsListSerial);

        if (EditorTools.DrawHeader($"Actions List (Total - {actionMenu.actionItemsList.Count})"))
        {
            EditorGUI.indentLevel += 2;
            for (int i = 0; i < actionItemsListSerial.arraySize; i++)
            {
                var itemProp = actionItemsListSerial.GetArrayElementAtIndex(i);

                var midFadeBlackBgSerial = itemProp.FindPropertyRelative("midFadeBgBlack");
                EditorGUILayout.PropertyField(midFadeBlackBgSerial, new GUIContent("Fade Background"));

                var itemSpriteSerial = itemProp.FindPropertyRelative("itemSprite");
                EditorGUILayout.PropertyField(itemSpriteSerial);
                
                if (EditorTools.DrawHeader($"Actions Choices - [{i}]"))
                {
                    var actionItemChoicesSerial = itemProp.FindPropertyRelative("actionItemChoices");                    

                    DrawActionItemChoices(flowchart, actionItemChoicesSerial);                    
                }

                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.red;
                GUIContent content = new GUIContent("Remove Action (-)", (Texture)Resources.Load("Icons/Icon_Action"));
                if (GUILayout.Button(content, buttonTextStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                    actionItemsListSerial.DeleteArrayElementAtIndex(i);                
                GUI.backgroundColor = defaultGUIColor;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);

            DrawDividerLine(Color.grey);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.blue;
            GUIContent content3 = new GUIContent("Add Action (+)", (Texture)Resources.Load("Icons/Icon_Action"));
            if (GUILayout.Button(content3, buttonTextStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                actionItemsListSerial.InsertArrayElementAtIndex(actionItemsListSerial.arraySize);
            GUI.backgroundColor = defaultGUIColor;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            DrawDividerLine(Color.grey);
        }        

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionItemChoices(Flowchart flowchart, SerializedProperty actionItemChoicesSerial)
    {
        for (int i = 0; i < actionItemChoicesSerial.arraySize; i++)
        {
            DrawDividerLine(Color.grey);

            show = EditorGUILayout.Foldout(show, $"Element - [{i}]", true, EditorStyles.foldout);
            if (show)
            {
                var choiceProp = actionItemChoicesSerial.GetArrayElementAtIndex(i);

                EditorGUILayout.Space(10);

                //EditorGUILayout.LabelField($"Element - [{i}]");

                var choiceTextSerial = choiceProp.FindPropertyRelative("choiceText");
                EditorGUILayout.PropertyField(choiceTextSerial);

                var targetBlockSerial = choiceProp.FindPropertyRelative("targetBlock");
                EditorGUILayout.BeginHorizontal();

                BlockEditor.BlockField(targetBlockSerial,
                                    new GUIContent("Target Block", "Block to call when option is selected"),
                                    new GUIContent("<None>"),
                                    flowchart);
                const int popupWidth = 17;
                if (targetBlockSerial.objectReferenceValue == null && GUILayout.Button("+", GUILayout.MaxWidth(popupWidth)))
                {
                    var fw = EditorWindow.GetWindow<FlowchartWindow>();
                    var t = (ActionMenu)target;
                    var activeFlowchart = t.GetFlowchart();
                    var newBlock = fw.CreateBlockSuppressSelect(activeFlowchart, t.ParentBlock._NodeRect.position - Vector2.down * 60);
                    targetBlockSerial.objectReferenceValue = newBlock;
                    activeFlowchart.SelectedBlock = t.ParentBlock;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.red;
                GUIContent content = new GUIContent("Remove Choice (-)", (Texture)Resources.Load("Icons/Icon_Choices"));
                if (GUILayout.Button(content, buttonTextStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                    actionItemChoicesSerial.DeleteArrayElementAtIndex(i);
                GUI.backgroundColor = defaultGUIColor;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }            

            DrawDividerLine(Color.grey);
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIContent content2 = new GUIContent("Add Choice (+)", (Texture)Resources.Load("Icons/Icon_Choices"));
        if (GUILayout.Button(content2, buttonTextStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            actionItemChoicesSerial.InsertArrayElementAtIndex(actionItemChoicesSerial.arraySize);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDividerLine(Color color)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 2;

        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }
}