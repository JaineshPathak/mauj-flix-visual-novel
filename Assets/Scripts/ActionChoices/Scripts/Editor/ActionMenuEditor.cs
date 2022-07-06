using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus.EditorUtils;

[CustomEditor(typeof(ActionMenu))]
public class ActionMenuEditor : CommandEditor
{
    protected SerializedProperty centerActionTextSerial;
    protected SerializedProperty actionItemsListSerial;

    public override void OnEnable()
    {
        base.OnEnable();

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

        if (EditorTools.DrawHeader("Actions List"))
        {
            for (int i = 0; i < actionItemsListSerial.arraySize; i++)
            {
                var itemProp = actionItemsListSerial.GetArrayElementAtIndex(i);

                var midFadeBlackBgSerial = itemProp.FindPropertyRelative("midFadeBgBlack");
                EditorGUILayout.PropertyField(midFadeBlackBgSerial, new GUIContent("Fade Background"));

                EditorGUI.indentLevel++;
                if (EditorTools.DrawHeader("Actions Choices"))
                {
                    var actionItemChoicesSerial = itemProp.FindPropertyRelative("actionItemChoices");
                    Debug.Log($"0 - {actionItemChoicesSerial.hasChildren}, {actionItemChoicesSerial.hasVisibleChildren}");                    
                    for (int j = 0; j < actionItemChoicesSerial.arraySize; j++)
                    {
                        var choiceProp = actionItemChoicesSerial.GetArrayElementAtIndex(j);
                        Debug.Log($"1 - {choiceProp != null}");

                        var choiceTextSerial = choiceProp.FindPropertyRelative("choiceText");
                        Debug.Log($"2 - {choiceTextSerial != null}");
                        EditorGUILayout.PropertyField(choiceTextSerial);

                        var targetBlockSerial = choiceProp.FindPropertyRelative("targetBlock");
                        Debug.Log($"3 - {targetBlockSerial != null}");
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

                        if (GUILayout.Button("Remove Action Item Choice"))
                        {
                            actionItemsListSerial.DeleteArrayElementAtIndex(j);
                        }
                    }
                }
                if (GUILayout.Button("Remove Action Item"))
                {
                    actionItemsListSerial.DeleteArrayElementAtIndex(i);
                }
            }

            if (GUILayout.Button("Add Action Item"))
            {
                actionItemsListSerial.InsertArrayElementAtIndex(actionItemsListSerial.arraySize);
            }
        }        

        serializedObject.ApplyModifiedProperties();
    }
}
