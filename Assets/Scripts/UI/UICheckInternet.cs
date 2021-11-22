using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICheckInternet : MonoBehaviour
{
    public CanvasGroup checkInternetCanvas;

    private bool isShown;

    private void OnEnable()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus += GetInternetStatus;
    }

    private void OnDisable()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus -= GetInternetStatus;
    }

    private void OnDestroy()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus -= GetInternetStatus;
    }

    private void GetInternetStatus(bool internetConnected)
    {
        if (checkInternetCanvas == null)
            return;

        if (internetConnected && isShown)
        {
            isShown = false;
            LeanTween.alphaCanvas(checkInternetCanvas, 0, 0.3f);
        }
        else if(!internetConnected && !isShown)
        {
            isShown = true;
            LeanTween.alphaCanvas(checkInternetCanvas, 1f, 0.3f);
        }
    }
}
