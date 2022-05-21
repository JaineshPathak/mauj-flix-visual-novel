using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Fungus.EditorUtils;

[CustomEditor(typeof(PlayEnding))]
public class PlayEndingEditor : CommandEditor
{
    protected SerializedProperty endScreenTypeSerial;
    protected SerializedProperty nextEpisodeAtlasSerial;
    protected SerializedProperty currentTextureIndexToLoadSerial;
    protected SerializedProperty currentTextureNameToLoadSerial;

    private float textureScale = 2f;

    private float buttonWidth = 120f;
    private float buttonHeight = 60f;

    private Sprite[] atlasSpritesList;
    private string[] atlasSpritesNamesList;

    private StringListSearchProvider searchProvider;

    public override void OnEnable()
    {
        base.OnEnable();

        endScreenTypeSerial = serializedObject.FindProperty("endScreenType");
        nextEpisodeAtlasSerial = serializedObject.FindProperty("nextEpisodesSpriteAtlas");
        currentTextureIndexToLoadSerial = serializedObject.FindProperty("currentTextureIndexToLoad");
        currentTextureNameToLoadSerial = serializedObject.FindProperty("currentTextureNameToLoad");
    }

    private void OnDisable()
    {
        Array.Resize(ref atlasSpritesList, 0);
    }

    public override void DrawCommandGUI()
    {
        PlayEnding playEnding = target as PlayEnding;

        serializedObject.Update();

        if (playEnding.nextEpisodesSpriteAtlas != null)
        {
            atlasSpritesList = playEnding.GetSpritesList();
            if (atlasSpritesList.Length > 0)
            {
                Array.Resize(ref atlasSpritesNamesList, atlasSpritesList.Length);
                for (int i = 0; i < atlasSpritesList.Length; i++)
                    atlasSpritesNamesList[i] = atlasSpritesList[i].texture.name;
            }
        }

        EditorGUILayout.PropertyField(endScreenTypeSerial, new GUIContent("End Screen Type"));

        EditorGUILayout.Space(10f);

        EditorGUILayout.PropertyField(currentTextureIndexToLoadSerial, new GUIContent("Current Texture Index Load"));
        EditorGUILayout.PropertyField(currentTextureNameToLoadSerial, new GUIContent("Current Texture Name Load"));
        
        EditorGUILayout.Space(10f);

        if(playEnding.endScreenType == EndScreenType.EpisodeEndScreen)
        {
            EditorGUILayout.PropertyField(nextEpisodeAtlasSerial, new GUIContent("Next Eps Atlas"));
            EditorGUILayout.Space(15f);
            
            DrawDividerLine(Color.grey);

            if(playEnding.nextEpisodesSpriteAtlas != null && atlasSpritesList.Length > 0)
            {
                playEnding.currentTextureNameToLoad = atlasSpritesNamesList[playEnding.currentTextureIndexToLoad];

                EditorGUILayout.BeginHorizontal();
                AddLabel("Image Scale", "Image Scaling for adjusting purpose. Nothing to do with actual scaling of Sprite Renderer.");
                textureScale = EditorGUILayout.Slider(textureScale, 1f, 2f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5f);

                EditorGUILayout.BeginHorizontal();
                AddLabel("Select Texture", "Select Texture from List");
                if (GUILayout.Button(atlasSpritesNamesList[playEnding.currentTextureIndexToLoad], EditorStyles.popup, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                {
                    searchProvider = ScriptableObject.CreateInstance<StringListSearchProvider>();
                    searchProvider.Init(atlasSpritesNamesList, (index) => playEnding.currentTextureIndexToLoad = index);
                    //searchProvider = new StringListSearchProvider(atlasSpritesNamesList);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
                //imageLoader.currentTextureIndexToLoad = EditorGUILayout.Popup(imageLoader.currentTextureIndexToLoad, atlasSpritesNamesList);
                EditorGUILayout.EndHorizontal();

                //------------------------------------------------------------------------------------------------
                EditorGUILayout.BeginVertical();

                Texture2D textureToLoad = atlasSpritesList[playEnding.currentTextureIndexToLoad].texture;

                Rect texRect = GUILayoutUtility.GetRect(textureToLoad.width / textureScale, textureToLoad.height / textureScale);
                GUI.DrawTexture(texRect, textureToLoad, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.FlexibleSpace();
                    GUILayout.FlexibleSpace();
                    AddLabel("[" + playEnding.currentTextureIndexToLoad + "] " + playEnding.currentTextureNameToLoad, true);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(5f);

                DrawDividerLine(Color.grey);

                EditorGUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("<", GUILayout.Width(buttonWidth), GUILayout.Width(buttonHeight)))
                    {
                        playEnding.currentTextureIndexToLoad--;
                        playEnding.currentTextureIndexToLoad = Mathf.Clamp(playEnding.currentTextureIndexToLoad, 0, atlasSpritesList.Length - 1);
                    }

                    if (GUILayout.Button(">", GUILayout.Width(buttonWidth), GUILayout.Width(buttonHeight)))
                    {
                        playEnding.currentTextureIndexToLoad++;
                        playEnding.currentTextureIndexToLoad = Mathf.Clamp(playEnding.currentTextureIndexToLoad, 0, atlasSpritesList.Length - 1);
                    }

                    GUILayout.FlexibleSpace();
                }
                //GUILayout.EndHorizontal();
                EditorGUILayout.Space(5f);
                //------------------------------------------------------------------------------------------------
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddLabel(string label, bool bold = false)
    {
        GUIContent newContent = new GUIContent(label);

        GUIStyle newStyle = new GUIStyle(GUI.skin.label);

        if (bold)
            newStyle.fontStyle = FontStyle.Bold;
        else
            newStyle.fontStyle = FontStyle.Normal;

        EditorGUILayout.LabelField(newContent, newStyle);
    }

    private void AddLabel(string label, string labelToolTip)
    {
        GUIContent newContent = new GUIContent(label, labelToolTip);
        EditorGUILayout.LabelField(newContent, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
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
