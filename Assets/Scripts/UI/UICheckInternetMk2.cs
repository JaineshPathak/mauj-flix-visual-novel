using UnityEngine;
using UnityEngine.SceneManagement;

public class UICheckInternetMk2 : UICheckInternet
{
    [Header("Check Internet Mk2")]
    public RectTransform noInternetRect;

    private bool wasShownBefore;

    private void Start()
    {
        wasShownBefore = false;

        if (noInternetRect)
        {
            noInternetRect.sizeDelta = new Vector2(noInternetRect.sizeDelta.x, 0);
            noInternetRect.transform.localScale = new Vector3(noInternetRect.transform.localScale.x, 0, noInternetRect.transform.localScale.z);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex == 2)
            wasShownBefore = false;
    }

    protected override void GetInternetStatus(bool internetConnected)
    {
        if(!internetConnected && !wasShownBefore && noInternetRect)
        {
            wasShownBefore = true;

            LTSeq noInternetSeq = LeanTween.sequence();
            
            noInternetSeq.append(LeanTween.value(0, 70f, 0.3f).setEaseLinear().setOnStart( () => 
            {
                LeanTween.scaleY(noInternetRect.gameObject, 1f, 0.3f).setEaseLinear();
            }).setOnUpdate( (float val) => 
            {
                Vector2 sizeDel = noInternetRect.sizeDelta;
                sizeDel.y = val;
                noInternetRect.sizeDelta = sizeDel;
            }));

            noInternetSeq.append(3f);

            noInternetSeq.append(LeanTween.value(70f, 0, 0.3f).setEaseLinear().setOnStart(() =>
            {
                LeanTween.scaleY(noInternetRect.gameObject, 0, 0.3f).setEaseLinear();
            }).setOnUpdate((float val) =>
            {
                Vector2 sizeDel = noInternetRect.sizeDelta;
                sizeDel.y = val;
                noInternetRect.sizeDelta = sizeDel;
            }));
        }
    }
}
