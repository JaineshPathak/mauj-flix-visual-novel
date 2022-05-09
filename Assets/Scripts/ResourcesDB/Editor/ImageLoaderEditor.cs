#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(ImageLoader)), CanEditMultipleObjects]
public class ImageLoaderEditor : Editor
{
    private float textureScale;

    private float buttonWidth = 120f;
    private float buttonHeight = 60f;

    private Sprite[] atlasSpritesList;
    private string[] atlasSpritesNamesList;

    private StringListSearchProvider searchProvider;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageLoader imageLoader = target as ImageLoader;

        serializedObject.Update();

        if (imageLoader.bgSpriteAtlas != null)
        {
            atlasSpritesList = imageLoader.GetSpritesList();
            if (atlasSpritesList.Length > 0)
            {
                Array.Resize(ref atlasSpritesNamesList, atlasSpritesList.Length);
                for (int i = 0; i < atlasSpritesList.Length; i++)
                    atlasSpritesNamesList[i] = atlasSpritesList[i].texture.name;
            }
        }

        if (/*imageLoader.testMode &&*/ imageLoader.bgSpriteAtlas != null && atlasSpritesList.Length > 0)
        {
            //Array.Resize(ref atlasSpritesList, imageLoader.bgSpriteAtlas.spriteCount);
            //imageLoader.bgSpriteAtlas.GetSprites(atlasSpritesList);

            //imageLoader.spritesList = atlasSpritesList;

            imageLoader.currentTextureNameToLoad = atlasSpritesNamesList[imageLoader.currentTextureIndexToLoad];

            EditorGUILayout.Space(15f);

            DrawDividerLine(Color.grey);

            EditorGUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            AddLabel("Image Scale", "Image Scaling for adjusting purpose. Nothing to do with actual scaling of Sprite Renderer.");
            textureScale = EditorGUILayout.Slider(textureScale, 1f, 2f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            AddLabel("Select Texture", "Select Texture from List");
            if (GUILayout.Button(atlasSpritesNamesList[imageLoader.currentTextureIndexToLoad], EditorStyles.popup, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
            {
                searchProvider = ScriptableObject.CreateInstance<StringListSearchProvider>();
                searchProvider.Init(atlasSpritesNamesList, (index) => imageLoader.currentTextureIndexToLoad = index);
                //searchProvider = new StringListSearchProvider(atlasSpritesNamesList);
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
            }
            //imageLoader.currentTextureIndexToLoad = EditorGUILayout.Popup(imageLoader.currentTextureIndexToLoad, atlasSpritesNamesList);
            EditorGUILayout.EndHorizontal();

            //------------------------------------------------------------------------------------------------
            EditorGUILayout.BeginVertical();

            Texture2D textureToLoad = atlasSpritesList[imageLoader.currentTextureIndexToLoad].texture;

            Rect texRect = GUILayoutUtility.GetRect(textureToLoad.width / textureScale, textureToLoad.height / textureScale);
            GUI.DrawTexture(texRect, textureToLoad, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 5f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                AddLabel("[" + imageLoader.currentTextureIndexToLoad + "] " + imageLoader.currentTextureNameToLoad, true);
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
                    imageLoader.currentTextureIndexToLoad--;
                    imageLoader.currentTextureIndexToLoad = Mathf.Clamp(imageLoader.currentTextureIndexToLoad, 0, atlasSpritesList.Length - 1);
                }

                if (GUILayout.Button(">", GUILayout.Width(buttonWidth), GUILayout.Width(buttonHeight)))
                {
                    imageLoader.currentTextureIndexToLoad++;
                    imageLoader.currentTextureIndexToLoad = Mathf.Clamp(imageLoader.currentTextureIndexToLoad, 0, atlasSpritesList.Length - 1);
                }

                GUILayout.FlexibleSpace();
            }
            //GUILayout.EndHorizontal();
            EditorGUILayout.Space(5f);
            //------------------------------------------------------------------------------------------------
        }

        //if(imagesDatabase != null)
        //{            
        //}

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

    private void AddPropertyField(SerializedProperty property, string label, string tooltip)
    {
        if (property == null)
            return;

        GUIContent propertyContent = new GUIContent(label, tooltip);
        EditorGUILayout.PropertyField(property, propertyContent, GUILayout.ExpandHeight(false));
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
#endif