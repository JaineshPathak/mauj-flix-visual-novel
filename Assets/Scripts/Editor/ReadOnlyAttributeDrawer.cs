using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    private Object targetObject;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*if (Event.current.type == EventType.Repaint)
        {
            var previousGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGUIState;            
        }*/

        targetObject = property.serializedObject.targetObject;        
        MethodInfo method = targetObject.GetType().GetMethod("UpdateStoryProgressFileName");        

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        int indent = EditorGUI.indentLevel;        
        EditorGUI.indentLevel = 0;

        //var previousGUIState = GUI.enabled;
        //GUI.enabled = false;
        Rect unitRect = new Rect(position.x, position.y, position.width / 2f, position.height);
        EditorGUI.PropertyField(unitRect, property, GUIContent.none);
        //GUI.enabled = previousGUIState;

        StoriesDBItemSO storyItem = targetObject as StoriesDBItemSO;
        if (storyItem != null)
        {
            if (storyItem.storyEpisodesKeys.Length >= 3)
            {
                Rect buttonRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f, position.height);
                if (GUI.Button(buttonRect, "Update"))
                    method.Invoke(targetObject, null);
            }
        }
        //EditorGUI.PropertyField(position, property, GUIContent.none);

        EditorGUI.indentLevel = indent;
        
        EditorGUI.EndProperty();
    }
}