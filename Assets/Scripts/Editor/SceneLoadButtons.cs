#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityToolbarExtender;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };
    }
}

[InitializeOnLoad]
public class SceneLoadButtons
{
    static SceneLoadButtons()
    {
        ToolbarExtender.RightToolbarGUI.Add(() => 
        {
            //GUILayout.FlexibleSpace();

            GUIContent loadScene0Content = new GUIContent();
            loadScene0Content.text = "0";
            loadScene0Content.tooltip = "Loads SplashScreen Scene";

            GUIContent loadScene1Content = new GUIContent();
            loadScene1Content.text = "1";
            loadScene1Content.tooltip = "Loads MenuIndex Scene";

            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 24;

            GUILayout.Space(10);

            if (GUILayout.Button(loadScene0Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/SplashScreen.unity");
            }

            GUILayout.Space(5);

            if (GUILayout.Button(loadScene1Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/MenuIndex.unity");
            }
        });
    }
}
#endif