using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class MenuUpdater : MonoBehaviour
{
    public CharReplacerHindi[] menuTextsReplacer;

    private void OnEnable()
    {
        MenuSignals.OnMenuStart += OnMenuStart;
    }

    private void OnDisable()
    {
        MenuSignals.OnMenuStart -= OnMenuStart;
    }

    private void OnDestroy()
    {
        MenuSignals.OnMenuStart -= OnMenuStart;
    }

    private void OnMenuStart(MenuDialog menu)
    {
        if (menuTextsReplacer.Length <= 0)
            return;

        StartCoroutine(UpdateOptionsTextRoutine());        
    }

    private IEnumerator UpdateOptionsTextRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < menuTextsReplacer.Length; i++)
        {
            if (menuTextsReplacer[i] != null)
            {
                menuTextsReplacer[i].UpdateMe();
            }
        }
    }
}
