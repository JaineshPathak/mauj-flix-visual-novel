using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UISplashScreen : MonoBehaviour
{
    [Header("Version Check")]
    public CanvasGroup updateGameCanvas;

    [Header("Fade Timer")]
    public float timer;
    public float timerInterval = 4f;

    [Header("Canvas Groups")]
    public CanvasGroup titleCompanyCanvas;
    public CanvasGroup disclaimerCanvas;
    public CanvasGroup thumbnailLoadingCanvas;

    [Header("Thumbnails")]
    public Transform thumbnailsLoadingContainer;
    public Image thumbnailLoadingImagePrefab;
    public string[] thumbnailKeysList;
    public Sprite[] thumbnailsList;

    [Header("Progress Bar")]
    //public Animator loadingProgressBar;
    public Image loadingProgressBar;

    private int currentIndex;
    private float newFillAmount;

    private Color whiteTransparent = new Color(1f, 1f, 1f, 0);
    private List<Image> thumbnailImagesLoadedList = new List<Image>();

    public Sprite RandomThumbnailSprite
    {
        get
        {
            return thumbnailsList[Random.Range(0, thumbnailsList.Length)];
        }
    }    

    private void Awake()
    {
        titleCompanyCanvas.alpha = 0;
        thumbnailLoadingCanvas.alpha = 0;

        //thumbnailLoadingImage.sprite = RandomThumbnailSprite;

        newFillAmount = 1f / (float)thumbnailKeysList.Length;

        for (int i = 0; i < thumbnailsList.Length; i++)
        {
            Image thumbnailLoading = Instantiate(thumbnailLoadingImagePrefab, thumbnailsLoadingContainer);
            thumbnailLoading.color = whiteTransparent;
            thumbnailLoading.sprite = thumbnailsList[i];
            thumbnailLoading.transform.name = thumbnailsList[i].name;

            thumbnailImagesLoadedList.Add(thumbnailLoading);
        }
        
        //StartSequence();
    }

    private void Start()
    {
        if (!FirebaseRemoteConfigHandler.instance.doVersionCheck)
            StartSequence();
    }

    private void OnEnable()
    {
        FirebaseRemoteConfigHandler.OnAppVersionCorrect += OnAppVersionCorrect;
        FirebaseRemoteConfigHandler.OnAppVersionIncorrect += OnAppVersionIncorrect;
    }

    private void OnDisable()
    {
        FirebaseRemoteConfigHandler.OnAppVersionCorrect -= OnAppVersionCorrect;
        FirebaseRemoteConfigHandler.OnAppVersionIncorrect -= OnAppVersionIncorrect;
    }

    private void OnDestroy()
    {
        FirebaseRemoteConfigHandler.OnAppVersionCorrect -= OnAppVersionCorrect;
        FirebaseRemoteConfigHandler.OnAppVersionIncorrect -= OnAppVersionIncorrect;
    }

    private void OnAppVersionCorrect()
    {
        LeanTween.alphaCanvas(updateGameCanvas, 0, 0.3f);
        updateGameCanvas.interactable = false;
        updateGameCanvas.blocksRaycasts = false;

        StartSequence();        
    }

    private void OnAppVersionIncorrect()
    {
        LeanTween.alphaCanvas(updateGameCanvas, 1f, 0.3f);
        updateGameCanvas.interactable = true;
        updateGameCanvas.blocksRaycasts = true;
    }

    private IEnumerator UpdateFadeRoutine()
    {
        while(true)
        {
            timer += Time.deltaTime;
            if(timer >= timerInterval)
            {
                timer = 0;

                LeanTween.alpha(thumbnailImagesLoadedList[currentIndex].rectTransform, 0, 0.5f);

                if (currentIndex + 1 > thumbnailsList.Length - 1)
                    LeanTween.alpha(thumbnailImagesLoadedList[0].rectTransform, 1f, 0.5f);
                else
                    LeanTween.alpha(thumbnailImagesLoadedList[currentIndex + 1].rectTransform, 1f, 0.5f);

                currentIndex++;
                if (currentIndex > thumbnailsList.Length - 1)
                    currentIndex = 0;
            }

            yield return null;
        }
    }

    private void StartSequence()
    {
        currentIndex = Random.Range(0, thumbnailsList.Length - 1);

        LTSeq splashSeq = LeanTween.sequence();

        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(titleCompanyCanvas, 1f, 1f));
        splashSeq.append(3f);
        splashSeq.append(LeanTween.alphaCanvas(titleCompanyCanvas, 0, 1f));
        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(disclaimerCanvas, 1f, 1f));
        splashSeq.append(2f);
        splashSeq.append(LeanTween.alphaCanvas(disclaimerCanvas, 0, 1f));
        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(thumbnailLoadingCanvas, 1f, 1f).setOnStart( () => 
        {
            LeanTween.alpha(thumbnailImagesLoadedList[currentIndex].rectTransform, 1f, 1f).setOnComplete( () => 
            {
                StartCoroutine(UpdateFadeRoutine());
            });
        }).setOnComplete( () => 
        {
            if(thumbnailKeysList.Length > 0)
                StartCoroutine(EndSequenceRoutine());
        }));
    }

    private IEnumerator EndSequenceRoutine()
    {
        /*AsyncOperationHandle<IList<Sprite>> loadImagesKeysHandle = Addressables.LoadAssetsAsync<Sprite>(thumbnailKeysList, obj => 
        {
            Debug.Log(obj.name);
            loadingProgressBar.fillAmount += newFillAmount;
        }, Addressables.MergeMode.Union);

        yield return loadImagesKeysHandle;

        IList<Sprite> loadImagesKeysResult = loadImagesKeysHandle.Result;

        Addressables.Release(loadImagesKeysHandle);*/

        for (int i = 0; i < thumbnailKeysList.Length; i++)
        {
            AsyncOperationHandle<Sprite> loadImageFromKeyHandle = Addressables.LoadAssetAsync<Sprite>(thumbnailKeysList[i]);

            yield return loadImageFromKeyHandle;

            if(loadImageFromKeyHandle.Status == AsyncOperationStatus.Succeeded)
                loadingProgressBar.fillAmount += newFillAmount;
        }

        LeanTween.alphaCanvas(thumbnailLoadingCanvas, 0, 1f).setOnComplete( () => 
        {
            LeanTween.cancelAll();
            SceneManager.LoadScene(1);
        });
    }
    
    //Call from UI Update Button
    public void OnUpdateButtonClicked()
    {
        if (FirebaseRemoteConfigHandler.instance != null)
            FirebaseRemoteConfigHandler.instance.isUpdateBtnClicked = true;

        updateGameCanvas.alpha = 0;
        updateGameCanvas.interactable = false;
        updateGameCanvas.blocksRaycasts = false;

#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.culttales.maujflix");
#endif
    }

    /*private IEnumerator EndSequenceRoutine()
    {
        loadingProgressBar.SetTrigger("TriggerFakeLoading");

        yield return new WaitForSeconds(4f);

        LeanTween.alphaCanvas(thumbnailLoadingCanvas, 0, 1f).setOnComplete( () => 
        {
            SceneManager.LoadScene(1);
        } );
    }*/
}
