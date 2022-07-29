﻿#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;

[CreateAssetMenu(fileName = "StoryDBItem", menuName = "Stories/Create Story Item", order = 0)]
public class StoriesDBItemSO : ScriptableObject
{
    [Header("Reworked or Not?")]
    public bool isReworked;

    [Header("Story Type")]
    public bool isNewStory;
    public bool isStoryEnabled = true;
    public bool isShortStory;

    [Header("Titles")]
    public string storyTitle;
    public string storyTitleEnglish;

    [Header("Description")]
    [TextArea]
    public string storyDescription = "Coming Soon";    

    [Header("Total Blocks")]
    public int storyTotalBlocksCount;

    [Header("Sounds Bucket")]
    public AssetReferenceGameObject soundsBucketKey;            //if isReworked = true

    [Header("Art Atlas")]
    public AssetReference atlasDBKey;                           //if isReworked = true

    [Header("Images Keys")]
    public AssetReferenceSprite storyThumbnailSmallKey;    
    public AssetReferenceSprite storyThumbnailBigKey;
    public AssetReferenceSprite storyThumbnailLoadingKey;
    public AssetReferenceSprite storyTitleImageKey;

    [Space(15)]

    public string storyThumbnailSmallName;
    public string storyThumbnailBigName;
    public string storyThumbnailLoadingName;
    public string storyThumbnailTitleName;
    public string storyThumbnailTrendingName;

    [Header("Flowchart Keys")]
    public AssetReferenceGameObject storyFlowchartKey;

    [Header("Story Filename")]
    [ReadOnly]
    public string storyProgressFileName;

    [Header("Story Episodes Keys")]
    public AssetReferenceGameObject[] storyEpisodesKeys;

    [Header("Story Episodes Description")]
    public string[] storyEpisodesDescriptions;

    [Header("Story Episodes Branch Keys")]
    public AssetReferenceGameObject[] storyEpisodesBranchkeys;

    //private StoriesDBItem item = null;

    /*private void OnValidate()
    {
        if (Application.isPlaying || !Application.isEditor)
            return;

        UpdateStoryProgressFileName();
    }*/

    public void UpdateStoryProgressFileName()
    {
        storyProgressFileName = string.Empty;
        if (storyProgressFileName.Length <= 0 && storyEpisodesKeys.Length >= 3)
        {
            string finalName = string.Empty;

            for (int i = 0; i < 3; i++)
            {
                if (storyEpisodesKeys[i] != null)
                {
                    char[] episodeKeyArr = storyEpisodesKeys[i].RuntimeKey.ToString().ToCharArray();
                    if (episodeKeyArr.Length > 5)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            finalName = finalName + episodeKeyArr[j];
                        }
                    }
                }
            }

            storyProgressFileName = finalName;
        }
    }

    private void OnValidate()
    {
        storyThumbnailSmallName = storyThumbnailSmallKey.editorAsset != null ? storyThumbnailSmallKey.editorAsset.name : "";
        storyThumbnailBigName = storyThumbnailBigKey.editorAsset != null ? storyThumbnailBigKey.editorAsset.name : "";
        storyThumbnailLoadingName = storyThumbnailLoadingKey.editorAsset != null ? storyThumbnailLoadingKey.editorAsset.name : "";
        storyThumbnailTitleName = storyTitleImageKey.editorAsset != null ? storyTitleImageKey.editorAsset.name : "";

        Array.Resize(ref storyEpisodesDescriptions, storyEpisodesKeys.Length);
    }

    public StoriesDBItem GetStoriesDBItem()
    {
        StoriesDBItem item = new StoriesDBItem();

        item.isReworked = isReworked;

        item.isNewStory = isNewStory;
        item.isStoryEnabled = isStoryEnabled;
        item.isShortStory = isShortStory;

        item.storyTitle = storyTitle;
        item.storyTitleEnglish = storyTitleEnglish;

        item.storyDescription = storyDescription;

        item.storyTotalBlocksCount = storyTotalBlocksCount;

        item.atlasDBKey = (atlasDBKey != null) ? atlasDBKey.RuntimeKey.ToString() : string.Empty;

        item.soundsBucketKey = (soundsBucketKey != null) ? soundsBucketKey.RuntimeKey.ToString() : string.Empty;

        item.storyThumbnailSmallKey = (storyThumbnailSmallKey != null) ? storyThumbnailSmallKey.RuntimeKey.ToString() : string.Empty;
        item.storyThumbnailBigKey = (storyThumbnailBigKey != null) ? storyThumbnailBigKey.RuntimeKey.ToString() : string.Empty;
        item.storyThumbnailLoadingKey = (storyThumbnailLoadingKey != null) ? storyThumbnailLoadingKey.RuntimeKey.ToString() : string.Empty;
        item.storyTitleImageKey = (storyTitleImageKey != null) ? storyTitleImageKey.RuntimeKey.ToString() : string.Empty;

        item.storyThumbnailSmallName = storyThumbnailSmallName;
        item.storyThumbnailBigName = storyThumbnailBigName;
        item.storyThumbnailLoadingName = storyThumbnailLoadingName;
        item.storyThumbnailTitleName = storyThumbnailTitleName;
        item.storyThumbnailTrendingName = storyThumbnailTrendingName;

        item.storyFlowchartKey = (storyFlowchartKey != null) ? storyFlowchartKey.RuntimeKey.ToString() : string.Empty;

        item.storyProgressFileName = storyProgressFileName;

        //item.storyEpisodesKeys = storyEpisodesKeys;
        Array.Resize(ref item.storyEpisodesKeys, storyEpisodesKeys.Length);
        if(storyEpisodesKeys.Length > 0)
        {
            for (int i = 0; i < storyEpisodesKeys.Length; i++)
            {
                item.storyEpisodesKeys[i] = storyEpisodesKeys[i].RuntimeKey.ToString();
            }
        }

        item.storyEpisodesDescriptions = storyEpisodesDescriptions;

        /*try
        {
            Array.Resize(ref item.storyBranchEpisodesKeys, this.storyEpisodesBranchkeys.Length);            
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("Array Problem in: item.storyBranchEpisodesKeys + [" + e.Message + " - " + e.Source + "]");
        }*/

        //item.storyBranchEpisodesKeys = new string[0];
        if (storyEpisodesBranchkeys.Length > 0)
        {
            Array.Resize(ref item.storyBranchEpisodesKeys, storyEpisodesBranchkeys.Length);
            for (int i = 0; i < storyEpisodesBranchkeys.Length; i++)
            {
                if(storyEpisodesBranchkeys[i] != null)
                    item.storyBranchEpisodesKeys[i] = storyEpisodesBranchkeys[i].RuntimeKey.ToString();
            }
        }

        return item;
    }
}

[CustomEditor(typeof(StoriesDBItemSO))]
public class StoriesDBItemSOEditor : Editor
{
    private StoriesDBItemSO item;

    private SerializedProperty isReworkedSerial;

    private SerializedProperty isNewStorySerial;
    private SerializedProperty storyEnabledSerial;
    private SerializedProperty storyTypeSerial;

    private SerializedProperty storyTitleSerial;
    private SerializedProperty storyTitleEnglishSerial;

    private SerializedProperty storyDescriptionSerial;

    private SerializedProperty storyTotalBlocksCountSerial;

    private SerializedProperty soundsBucketKeySerial;

    private SerializedProperty atlasDBKeySerial;

    private SerializedProperty storyThumbnailSmallKeySerial;
    private SerializedProperty storyThumbnailBigKeySerial;
    private SerializedProperty storyThumbnailLoadingKeySerial;
    private SerializedProperty storyThumbnailTitleKeySerial;

    private SerializedProperty storyThumbnailSmallNameSerial;
    private SerializedProperty storyThumbnailBigNameSerial;
    private SerializedProperty storyThumbnailLoadingNameSerial;
    private SerializedProperty storyThumbnailTitleNameSerial;
    private SerializedProperty storyThumbnailTrendingNameSerial;

    private SerializedProperty storyFlowchartKeySerial;

    private SerializedProperty storyProgressFileNameSerial;

    private SerializedProperty storyEpisodesKeysSerial;
    private SerializedProperty storyEpisodesDescriptionsSerial;
    private SerializedProperty storyBranchEpisodesKeysSerial;

    private Texture2D thumbnailSmallTex;
    private Texture2D thumbnailBigTex;
    private Texture2D thumbnailLoadTex;

    private GUIStyle thumbnailLabelStyle;

    private float imageScaleDivide = 9f;
    private float labelYOffset = 35f;

    private bool showThumbnails;

    private enum ThumbnailType
    {
        Type_Small,
        Type_Big,
        Type_Loading
    };

    private void OnEnable()
    {
        isReworkedSerial = serializedObject.FindProperty("isReworked");

        isNewStorySerial = serializedObject.FindProperty("isNewStory");
        storyEnabledSerial = serializedObject.FindProperty("isStoryEnabled");
        storyTypeSerial = serializedObject.FindProperty("isShortStory");

        storyTitleSerial = serializedObject.FindProperty("storyTitle");
        storyTitleEnglishSerial = serializedObject.FindProperty("storyTitleEnglish");

        storyDescriptionSerial = serializedObject.FindProperty("storyDescription");

        storyTotalBlocksCountSerial = serializedObject.FindProperty("storyTotalBlocksCount");

        atlasDBKeySerial = serializedObject.FindProperty("atlasDBKey");

        soundsBucketKeySerial = serializedObject.FindProperty("soundsBucketKey");

        storyThumbnailSmallKeySerial = serializedObject.FindProperty("storyThumbnailSmallKey");
        storyThumbnailBigKeySerial = serializedObject.FindProperty("storyThumbnailBigKey");
        storyThumbnailLoadingKeySerial = serializedObject.FindProperty("storyThumbnailLoadingKey");
        storyThumbnailTitleKeySerial = serializedObject.FindProperty("storyTitleImageKey");

        storyThumbnailSmallNameSerial = serializedObject.FindProperty("storyThumbnailSmallName");
        storyThumbnailBigNameSerial = serializedObject.FindProperty("storyThumbnailBigName");
        storyThumbnailLoadingNameSerial = serializedObject.FindProperty("storyThumbnailLoadingName");
        storyThumbnailTitleNameSerial = serializedObject.FindProperty("storyThumbnailTitleName");
        storyThumbnailTrendingNameSerial = serializedObject.FindProperty("storyThumbnailTrendingName");

        storyFlowchartKeySerial = serializedObject.FindProperty("storyFlowchartKey");

        storyProgressFileNameSerial = serializedObject.FindProperty("storyProgressFileName");

        storyEpisodesKeysSerial = serializedObject.FindProperty("storyEpisodesKeys");
        storyEpisodesDescriptionsSerial = serializedObject.FindProperty("storyEpisodesDescriptions");
        storyBranchEpisodesKeysSerial = serializedObject.FindProperty("storyEpisodesBranchkeys");
    }

    public override void OnInspectorGUI()
    {
        item = target as StoriesDBItemSO;

        serializedObject.Update();

        AddPropertyLabel(isReworkedSerial, "Is Reworked?", "If this story is reworked, then assign Sounds Bucket and Atlas DB");

        AddPropertyLabel(isNewStorySerial, "Is New Story", "Check if Story is new so in order to add 'New' stamp banner in Thumbnails");
        AddPropertyLabel(storyEnabledSerial, "Story Enabled", "Is Story Enabled? Helpful when you want to disable a story for any changes or issues found.");
        AddPropertyLabel(storyTypeSerial, "Short Story", "Is it a short story? False: Normal 9-10 episodes story");

        AddPropertyLabel(storyTitleSerial, "Story Title", "Story Title in Hindi or others (except English)");
        AddPropertyLabel(storyTitleEnglishSerial, "Story Title English", "Story Title in English only");

        EditorGUILayout.Space(10);

        AddLabel("Description", true);
        AddLabel("Story Description", "Story Description (Appears in Details Panel)");
        AddPropertyTextArea(storyDescriptionSerial);

        AddPropertyLabel(storyTotalBlocksCountSerial, "Story Total Blocks Count", "Total Blocks Count. Get it from 'MaujFlix-Episodes-DB' Excel sheet");

        if (item.isReworked)
        {
            AddPropertyLabel(soundsBucketKeySerial, "Sounds Bucket Key", "Sounds Bucket Asset Ref");
            AddPropertyLabel(atlasDBKeySerial, "Atlas DB", "Sprite Atlas DB");
        }

        AddPropertyLabel(storyThumbnailSmallKeySerial, "Story Image Small", "Story Thumbnail Small from Addressable (Eg: Thumbnail_Padosan_S.png)");
        AddPropertyLabel(storyThumbnailBigKeySerial, "Story Image Big", "Story Thumbnail Big from Addressable (Eg: Thumbnail_Padosan_B.png)");
        AddPropertyLabel(storyThumbnailLoadingKeySerial, "Story Image Loading", "Story Thumbnail Loading from Addressable (Eg: Thumbnail_Padosan_L.png)");
        AddPropertyLabel(storyThumbnailTitleKeySerial, "Story Image Title", "Story Title Image from Addressable (Eg: Thumbnail_Padosan_Title.png)");

        AddPropertyLabel(storyThumbnailSmallNameSerial, "Story Image Small Name", "Story Thumbnail Small from Addressable (Eg: Thumbnail_Padosan_S.png)");
        AddPropertyLabel(storyThumbnailBigNameSerial, "Story Image Big Name", "Story Thumbnail Big from Addressable (Eg: Thumbnail_Padosan_B.png)");
        AddPropertyLabel(storyThumbnailLoadingNameSerial, "Story Image Loading Name", "Story Thumbnail Loading from Addressable (Eg: Thumbnail_Padosan_L.png)");
        AddPropertyLabel(storyThumbnailTitleNameSerial, "Story Image Title Name", "Story Title Image from Addressable (Eg: Thumbnail_Padosan_Title.png)");
        AddPropertyLabel(storyThumbnailTrendingNameSerial, "Story Image Trending Name", "Story Title Image from Addressable (Eg: Thumbnail_Padosan_Title.png)");

        AddPropertyLabel(storyFlowchartKeySerial, "Story Flowchart", "Story Flowchart GameObject prefab (Eg: ST-Padosan-Flowchart)");
        
        AddPropertyLabel(storyProgressFileNameSerial, "Story Progress File name", "File name of the Story Progress. Hit the 'Update' button next to it after assigning at least three Episode prefabs in below list.");        

        AddPropertyLabel(storyEpisodesKeysSerial, "Story Episodes", "List of Story Episodes Prefabs (Eg: ST-Padosan-Ep1.prefab). This appears in Details Panel depending on array size.");
        AddPropertyLabel(storyEpisodesDescriptionsSerial, "Story Episodes Descriptions", "Small description of each episodes");
        AddPropertyLabel(storyBranchEpisodesKeysSerial, "Story Branch Episodes", "List of Story Episodes Prefabs With Branch Endings (Eg: ST-Padosan-Ep1.prefab). This appears in Details Panel depending on array size.");
        
        //DrawDefaultInspector();

        thumbnailLabelStyle = new GUIStyle(GUI.skin.label);
        thumbnailLabelStyle.alignment = TextAnchor.LowerCenter;        
        thumbnailLabelStyle.fontStyle = FontStyle.Bold;        

        showThumbnails = EditorGUILayout.Toggle("Show Thumbnails", showThumbnails);
        if(showThumbnails)
        {
            EditorGUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();

            CheckForThumbnail(ThumbnailType.Type_Small);

            CheckForThumbnail(ThumbnailType.Type_Big);

            CheckForThumbnail(ThumbnailType.Type_Loading);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(40);
        }        

        serializedObject.ApplyModifiedProperties();
    }

    private void AddLabel(string label, string labelToolTip)
    {
        GUIContent newContent = new GUIContent(label, labelToolTip);
        EditorGUILayout.LabelField(newContent);
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

    private void AddPropertyLabel(SerializedProperty property, string label, string labelToolTip)
    {
        GUIContent newContent = new GUIContent(label, labelToolTip);
        EditorGUILayout.PropertyField(property, newContent, GUILayout.ExpandHeight(false));
    }    

    private void AddPropertyLabel(SerializedProperty property, bool allowChildren, string label, string labelToolTip)
    {
        GUIContent newContent = new GUIContent(label, labelToolTip);
        EditorGUILayout.PropertyField(property, newContent, allowChildren, GUILayout.ExpandHeight(false));
    }

    private void AddPropertyTextArea(SerializedProperty property)
    {
        GUIContent textContent = new GUIContent(property.stringValue);

        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.CalcSize(textContent);
        textAreaStyle.wordWrap = true;

        property.stringValue = EditorGUILayout.TextArea(property.stringValue, textAreaStyle);
    }

    private void CheckForThumbnail(ThumbnailType _thumbnailType)
    {
        switch (_thumbnailType)
        {
            case ThumbnailType.Type_Small:

                if (item.storyThumbnailSmallKey != null)
                {
                    thumbnailSmallTex = item.storyThumbnailSmallKey.editorAsset as Texture2D;

                    if (thumbnailSmallTex)
                    {
                        EditorGUILayout.BeginVertical();

                        Rect texRect = GUILayoutUtility.GetRect(thumbnailSmallTex.width / imageScaleDivide, thumbnailSmallTex.height / imageScaleDivide);
                        GUI.DrawTexture(texRect, thumbnailSmallTex, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 5f);

                        Rect labelRect = new Rect(texRect.x, texRect.y + labelYOffset, texRect.width, texRect.height);
                        EditorGUI.LabelField(labelRect, $"Thumbnail Small \n {thumbnailSmallTex.width}x{thumbnailSmallTex.height}", thumbnailLabelStyle);

                        EditorGUILayout.EndVertical();
                    }
                    //lastRect = GUILayoutUtility.GetLastRect();
                }

                break;

            case ThumbnailType.Type_Big:

                if (item.storyThumbnailBigKey != null)
                {
                    thumbnailBigTex = item.storyThumbnailBigKey.editorAsset as Texture2D;

                    if (thumbnailBigTex)
                    {
                        EditorGUILayout.BeginVertical();

                        Rect texRect = GUILayoutUtility.GetRect(thumbnailBigTex.width / imageScaleDivide, thumbnailBigTex.height / imageScaleDivide);
                        GUI.DrawTexture(texRect, thumbnailBigTex, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 5f);

                        Rect labelRect = new Rect(texRect.x, texRect.y + labelYOffset, texRect.width, texRect.height);
                        EditorGUI.LabelField(labelRect, $"Thumbnail Big \n {thumbnailBigTex.width}x{thumbnailBigTex.height}", thumbnailLabelStyle);

                        EditorGUILayout.EndVertical();
                    }
                    //lastRect = GUILayoutUtility.GetLastRect();
                }

                break;

            case ThumbnailType.Type_Loading:

                if (item.storyThumbnailLoadingKey != null)
                {
                    thumbnailLoadTex = item.storyThumbnailLoadingKey.editorAsset as Texture2D;

                    if (thumbnailLoadTex)
                    {
                        EditorGUILayout.BeginVertical();

                        Rect texRect = GUILayoutUtility.GetRect(thumbnailLoadTex.width / imageScaleDivide, thumbnailLoadTex.height / imageScaleDivide);
                        GUI.DrawTexture(texRect, thumbnailLoadTex, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 5f);

                        Rect labelRect = new Rect(texRect.x, texRect.y + labelYOffset, texRect.width, texRect.height);
                        EditorGUI.LabelField(labelRect, $"Thumbnail Loading \n {thumbnailLoadTex.width}x{thumbnailLoadTex.height}", thumbnailLabelStyle);

                        EditorGUILayout.EndVertical();
                    }
                }

                break;
        }
    }
}
#endif