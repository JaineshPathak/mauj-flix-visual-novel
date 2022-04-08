using UnityEngine;

public class UICheckInternet : MonoBehaviour
{
    public CanvasGroup checkInternetCanvas;

    private bool isShown;

    protected virtual void OnEnable()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus += GetInternetStatus;
    }

    protected virtual void OnDisable()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus -= GetInternetStatus;
    }

    protected virtual void OnDestroy()
    {
        CheckInternetConnection.BroadcastInternetConnectionStatus -= GetInternetStatus;
    }

    protected virtual void GetInternetStatus(bool internetConnected)
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
