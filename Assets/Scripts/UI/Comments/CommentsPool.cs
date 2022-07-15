using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CommentsPool : MonoBehaviourSingletonPersistent<CommentsPool>
{
    public int itemsCount = 100;
    public GameObject commentItemPrefab;

    [Space(15)]

    public List<UICommentItem> commentItemsList = new List<UICommentItem>();

    public UICommentItem GetCommentItem()
    {
        if (commentItemsList.Count <= 0)
            return null;

        for (int i = 0; i < commentItemsList.Count; i++)
        {
            if (!commentItemsList[i].gameObject.activeSelf)
                return commentItemsList[i];
        }

        return null;
    }

    public void AddNewCommentItem(UICommentItem commentItemNew)
    {
        if (commentItemsList.Contains(commentItemNew))
            return;

        commentItemNew.itemParent = transform;
        commentItemsList.Add(commentItemNew);
    }

    public void ResetItem(UICommentItem commentItem)
    {
        if (commentItem == null)
            return;

        commentItem.transform.parent = transform;
        commentItem.gameObject.SetActive(false);
    }

    public void ResetAllItems()
    {
        if (commentItemsList.Count <= 0)
            return;

        for (int i = 0; i < commentItemsList.Count; i++)
        {
            commentItemsList[i].transform.parent = transform;
            commentItemsList[i].gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    internal void LoadCommentItems()
    {
        if (itemsCount <= 0 || commentItemPrefab == null)
            return;

        for (int i = 0; i < itemsCount; i++)
        {
            GameObject itemInstance = PrefabUtility.InstantiatePrefab(commentItemPrefab) as GameObject;
            UICommentItem commentItemInstance = itemInstance.GetComponent<UICommentItem>();
            commentItemInstance.itemParent = transform;

            itemInstance.transform.parent = transform;
            itemInstance.SetActive(false);

            commentItemsList.Add(commentItemInstance);
        }
    }

    internal void RemoveCommentItems()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        commentItemsList.Clear();
    }
#endif    
}

#if UNITY_EDITOR
[CustomEditor(typeof(CommentsPool)), CanEditMultipleObjects]
public class CommentsPoolEditor : Editor
{
    private CommentsPool pool;

    public override void OnInspectorGUI()
    {
        pool = target as CommentsPool;

        DrawDefaultInspector();

        EditorGUILayout.Space(15f);

        if (GUILayout.Button("Load Comment Items"))
            pool.LoadCommentItems();

        if (GUILayout.Button("Remove Comment Items"))
            pool.RemoveCommentItems();
    }
}
#endif