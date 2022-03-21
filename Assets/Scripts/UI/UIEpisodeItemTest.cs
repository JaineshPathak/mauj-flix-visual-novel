using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEpisodeItemTest : MonoBehaviour
{
    public CanvasGroup testLoadingScreen;
    public Image testLoadingBar;
    public TextMeshProUGUI testLoadingText;

    public CanvasGroup testBlackScreen;

    public void TestLoadingScreen()
    {
        LTSeq testSeq = LeanTween.sequence();

        testSeq.append(LeanTween.alphaCanvas(testLoadingScreen, 1f, 0.5f).setEaseInOutSine());
        testSeq.append(LeanTween.value(0, 0.23f, 1f).setOnUpdate(UpdateBar).setEaseLinear());
        testSeq.append(1f);
        testSeq.append( () => UpdateBar(0.6f) );
        testSeq.append(1f);
        testSeq.append(LeanTween.value(0.6f, 0.7f, 1f).setOnUpdate(UpdateBar).setEaseLinear());
        testSeq.append(0.5f);
        testSeq.append( () => UpdateBar(1f) );
        testSeq.append(0.5f);
        testSeq.append(LeanTween.alphaCanvas(testBlackScreen, 1f, 0.5f).setEaseInOutSine());
    }

    private void UpdateBar(float val)
    {
        if (testLoadingBar == null || testLoadingText == null)
            return;

        testLoadingBar.fillAmount = val;
        testLoadingText.text = Mathf.RoundToInt(testLoadingBar.fillAmount * 100f).ToString() + "%";
    }
}
