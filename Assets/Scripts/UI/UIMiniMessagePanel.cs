using UnityEngine;
using TMPro;

public class UIMiniMessagePanel : MonoBehaviourSingleton<UIMiniMessagePanel>
{
    public CanvasGroup msgPanelCanvas;
    public TextMeshProUGUI msgText;

    private int msgSeqID;
    private LTSeq msgSeq;

    private void OnValidate()
    {
        if (msgPanelCanvas == null)
            msgPanelCanvas = GetComponent<CanvasGroup>();
    }

    public void ShowMiniMessage(string msg)
    {
        if (msgPanelCanvas == null || string.IsNullOrEmpty(msg))
            return;

        msgText.text = msg;

        if (LeanTween.isTweening(msgSeqID))
            LeanTween.cancel(msgSeqID);

        msgPanelCanvas.alpha = 0;

        msgSeq = LeanTween.sequence();

        msgSeqID = msgSeq.id;

        msgSeq.append(LeanTween.alphaCanvas(msgPanelCanvas, 1f, 0.3f).setEaseInOutSine());
        msgSeq.append(0.5f);
        msgSeq.append(LeanTween.alphaCanvas(msgPanelCanvas, 0, 0.3f).setEaseInOutSine());
    }
}
