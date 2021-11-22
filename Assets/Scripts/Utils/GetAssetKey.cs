using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

[DisallowMultipleComponent]
public class GetAssetKey : MonoBehaviour
{
    public AssetReference assetReference;
    public string assetKey;
}

#if UNITY_EDITOR
[CustomEditor(typeof(GetAssetKey))]
public class GetAssetKeyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GetAssetKey getAssetKey = target as GetAssetKey;

        EditorGUILayout.Space(5f);

        if(GUILayout.Button("Get Asset Key"))
        {
            if(getAssetKey.assetReference != null)
                getAssetKey.assetKey = getAssetKey.assetReference.RuntimeKey.ToString();
        }
    }
}
#endif