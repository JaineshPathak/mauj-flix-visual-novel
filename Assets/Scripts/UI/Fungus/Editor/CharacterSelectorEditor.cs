using UnityEditor;
using Fungus.EditorUtils;

public class CharacterSelectorEditor : CommandEditor
{
    protected SerializedProperty topTextProp;    
    protected SerializedProperty selectionScreenProp;

    public override void OnEnable()
    {
        base.OnEnable();

        topTextProp = serializedObject.FindProperty("topText");        
        selectionScreenProp = serializedObject.FindProperty("selectionScreen");
    }

    public override void DrawCommandGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(topTextProp);        
        EditorGUILayout.PropertyField(selectionScreenProp);

        serializedObject.ApplyModifiedProperties();
    }
}
