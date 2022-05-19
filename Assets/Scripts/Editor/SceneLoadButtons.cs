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

            GUIContent loadSceneRootContent = new GUIContent();
            loadSceneRootContent.text = "0";
            loadSceneRootContent.tooltip = "Loads Root Scene";

            GUIContent loadScene0Content = new GUIContent();
            loadScene0Content.text = "1";
            loadScene0Content.tooltip = "Loads SplashScreen Scene";

            GUIContent loadScene1Content = new GUIContent();
            loadScene1Content.text = "2";
            loadScene1Content.tooltip = "Loads MenuIndex Scene";

            GUIContent loadScene2Content = new GUIContent();
            loadScene2Content.text = "3";
            loadScene2Content.tooltip = "Loads Story Scene (Reworked)";

            GUIContent loadScene3Content = new GUIContent();
            loadScene3Content.text = "4";
            loadScene3Content.tooltip = "Loads Story Scene (Non-Reworked)";

            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 24;

            GUILayout.Space(10);

            if (GUILayout.Button(loadSceneRootContent, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/00_Root.unity");
            }

            GUILayout.Space(5);

            if (GUILayout.Button(loadScene0Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/01_SplashScreen.unity");
            }

            GUILayout.Space(5);

            if (GUILayout.Button(loadScene1Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/02_MenuIndex.unity");
            }

            GUILayout.Space(5);

            if (GUILayout.Button(loadScene2Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/03_StoryScene.unity");
            }

            GUILayout.Space(5);

            if (GUILayout.Button(loadScene3Content, style))
            {
                if (EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/04_StoryScene_NonReworked.unity");
            }
        });
    }
}
#endif