using UnityEngine;
using Firebase.RemoteConfig;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SayDialogueModifier : MonoBehaviour
{
    public RectTransform mainPanel;

    [HideInInspector] public int yPosID;

    private FirebaseRemoteConfig remoteConfig;

    private void Awake()
    {
        if (FirebaseRemoteConfig.DefaultInstance != null)
            remoteConfig = FirebaseRemoteConfig.DefaultInstance;

        if (mainPanel != null && remoteConfig != null)
        {
            float newYPos = (float)remoteConfig.GetValue(DataPaths.dialogueYPosID[yPosID]).DoubleValue;
            mainPanel.anchoredPosition = new Vector2(mainPanel.anchoredPosition.x, newYPos);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SayDialogueModifier))]
public class SayDialogueModifierEditor : Editor
{
    private SayDialogueModifier modifier;

    private void OnEnable()
    {
        modifier = target as SayDialogueModifier;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        modifier.yPosID = EditorGUILayout.Popup(new GUIContent("Dialogue ID"), modifier.yPosID, DataPaths.dialogueYPosID);
    }
}
#endif