using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ThumbnailItem
{
    /*public enum ThumbnailItemType
    {
        Small,
        Big,
        Top,
        Shorts
    };*/

    public int itemId;
    public int itemCount = 10;
    public Transform itemParent;
    public GameObject itemPrefab;
    //public ThumbnailItemType itemType;
    public List<GameObject> itemPrefabList;

    /*[HideInInspector] public List<UIStoriesItemSmall> storiesItemSmallList = new List<UIStoriesItemSmall>();
    [HideInInspector] public List<UIStoriesItemSmall> storiesItemShortsList = new List<UIStoriesItemSmall>();
    [HideInInspector] public List<UIStoriesItemBig> storiesItemBigList = new List<UIStoriesItemBig>();*/

#if UNITY_EDITOR
    internal void LoadItems()
    {
        if (itemCount <= 0 || itemPrefab == null)
            return;

        itemPrefabList = new List<GameObject>();
        for (int i = 0; i < itemCount; i++)
        {
            GameObject itemInstance = PrefabUtility.InstantiatePrefab(itemPrefab) as GameObject;
            itemInstance.transform.parent = itemParent;
            itemInstance.SetActive(false);

            if (itemInstance.GetComponent<UIStoriesItemSmall>() != null)
                itemInstance.GetComponent<UIStoriesItemSmall>().isFromPool = true;

            itemPrefabList.Add(itemInstance);

            #region OLDCODE
            /*switch (itemType)
                {
                    case ThumbnailItemType.Small:
                        storiesItemSmallList.Add(itemInstance.GetComponent<UIStoriesItemSmall>());
                        break;

                    case ThumbnailItemType.Big:
                        storiesItemBigList.Add(itemInstance.GetComponent<UIStoriesItemBig>());
                        break;

                    case ThumbnailItemType.Top:
                        break;

                    case ThumbnailItemType.Shorts:
                        storiesItemShortsList.Add(itemInstance.GetComponent<UIStoriesItemSmall>());
                        break;
                }*/ 
            #endregion
        }
    }
#endif

    internal GameObject GetThumbnailItem()
    {
        for (int i = 0; i < itemPrefabList.Count; i++)
            if (!itemPrefabList[i].gameObject.activeSelf)
                return itemPrefabList[i].gameObject;

        return null;
    }

    internal void ResetItem(GameObject item)
    {
        if (!itemPrefabList.Contains(item) || item == null)
            return;

        item.SetActive(false);
        item.transform.parent = itemParent;
    }

    internal void ResetItems()
    {
        if (itemPrefabList.Count <= 0)
            return;

        for (int i = 0; i < itemPrefabList.Count; i++)
        {
            itemPrefabList[i].gameObject.SetActive(false);
            itemPrefabList[i].transform.parent = itemParent;
        }    
    }
}

public class ThumbnailItemsPool : MonoBehaviourSingletonPersistent<ThumbnailItemsPool>
{
    public ThumbnailItem[] thumbnailItems;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (thumbnailItems.Length <= 0)
            return;

        for (int i = 0; i < thumbnailItems.Length; i++)
        {
            thumbnailItems[i].itemId = i;
            if(thumbnailItems[i].itemParent == null)
            {
                GameObject newItemParent = new GameObject($"ItemParent_{thumbnailItems[i].itemId}");
                newItemParent.transform.parent = transform;
                newItemParent.transform.localPosition = Vector3.zero;
                newItemParent.transform.localRotation = Quaternion.identity;

                thumbnailItems[i].itemParent = newItemParent.transform;
            }
        }
    }

    public void LoadItems()
    {
        if (thumbnailItems.Length <= 0)
            return;

        for (int i = 0; i < thumbnailItems.Length; i++)
            thumbnailItems[i].LoadItems();
    }

    public void RemoveItems()
    {        
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        if (thumbnailItems.Length <= 0)
            return;

        for (int i = 0; i < thumbnailItems.Length; i++)
        {
            thumbnailItems[i].itemParent = null;
            thumbnailItems[i].itemPrefabList.Clear();
            //thumbnailItems[i].storiesItemSmallList.Clear();
            //thumbnailItems[i].storiesItemBigList.Clear();
        }

        OnValidate();
    }
#endif

    /// <summary>
    ///
    /// </summary>
    /// <param name="id">0 - Big Thumbnail</param>
    /// <param name="id">1 - Small Thumbnail</param>
    /// <param name="id">2 - Shorts Thumbnail</param>
    /// <returns></returns>
    public GameObject GetThumbnailItem(int id)
    {
        if (thumbnailItems.Length <= 0)
            return null;

        return thumbnailItems[id].GetThumbnailItem();
    }

    public void ResetThumbnailItem(int id, GameObject item)
    {
        thumbnailItems[id].ResetItem(item);
    }

    public void ResetThumbnailItems()
    {
        if (thumbnailItems.Length <= 0)
            return;

        for (int i = 0; i < thumbnailItems.Length; i++)
            thumbnailItems[i].ResetItems();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ThumbnailItemsPool)), CanEditMultipleObjects]
public class ThumbnailItemsPoolEditor : Editor
{
    private ThumbnailItemsPool pool;

    private float thumbnailWidth = 60f;
    private float thumbnailHeight = 100f;

    public override void OnInspectorGUI()
    {
        pool = target as ThumbnailItemsPool;

        DrawDefaultInspector();

        EditorGUILayout.Space(15f);

        GUILayout.BeginHorizontal();
        {            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Resources.Load<Texture>("Icons/Icon_ThumbnailItem_Add"), GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight)))
            {
                Undo.RecordObject(target, "Load Items");
                pool.LoadItems();
            }

            if (GUILayout.Button(Resources.Load<Texture>("Icons/Icon_ThumbnailItem_Remove"), GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight)))
            {
                Undo.RecordObject(target, "Remove Items");
                pool.RemoveItems();
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }
}
#endif