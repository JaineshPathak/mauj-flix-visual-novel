using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class StringListSearchProvider : ScriptableObject, ISearchWindowProvider
{
    private string[] stringsList;
    private Action<int> actionCallback;

    public StringListSearchProvider()
    {
    }

    public StringListSearchProvider(string[] _list)
    {
        stringsList = _list;
    }

    public void Init(string[] _list, Action<int> callback)
    {
        stringsList = _list;
        actionCallback = callback;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        searchList.Add(new SearchTreeGroupEntry(new GUIContent("Textures DB"), 0));
        //searchList.Add(new SearchTreeGroupEntry(new GUIContent("Select Texture"), 1));

        for (int i = 0; i < stringsList.Length && (stringsList.Length > 0); i++)
        {
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(stringsList[i]));
            entry.level = 1;
            entry.userData = i;

            searchList.Add(entry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        actionCallback?.Invoke((int)SearchTreeEntry.userData);
        return true;
    }
}
