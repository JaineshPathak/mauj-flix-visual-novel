using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.AddressableAssets.ResourceLocators;
using TMPro;

public class UISplashScreen : MonoBehaviour
{
    [Header("Stories Load Images Database")]
    public string storiesLoadImagesKey;
    public CanvasGroup imageLoadingCanvasGroup;
    public Image imageLoadingBarActual;
    public TextMeshProUGUI imageLoadingStatusText;

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
    //public Sprite[] thumbnailsList;

    [Header("Progress Bar")]
    //public Animator loadingProgressBar;
    public Image loadingProgressBar;
    public TextMeshProUGUI loadingProgressText;

    private int currentIndex;
    private float newFillAmount;
    private float loadNewFillAmount;

    private Color whiteTransparent = new Color(1f, 1f, 1f, 0);
    private List<Image> thumbnailImagesLoadedList = new List<Image>();

    private StoriesLoadImagesDB storiesLoadImagesDB;

    /*public Sprite RandomThumbnailSprite
    {
        get
        {
            return thumbnailsList[Random.Range(0, thumbnailsList.Length)];
        }
    }*/   

    private void Awake()
    {
        imageLoadingCanvasGroup.alpha = 0;
        titleCompanyCanvas.alpha = 0;
        thumbnailLoadingCanvas.alpha = 0;

        if(thumbnailKeysList.Length > 0)
            newFillAmount = 1f / (float)thumbnailKeysList.Length;

        #region OLD CODE v0.1.4.2.1
        //thumbnailLoadingImage.sprite = RandomThumbnailSprite;

        /*newFillAmount = 1f / (float)thumbnailKeysList.Length;

        for (int i = 0; i < thumbnailsList.Length; i++)
        {
            Image thumbnailLoading = Instantiate(thumbnailLoadingImagePrefab, thumbnailsLoadingContainer);
            thumbnailLoading.color = whiteTransparent;
            thumbnailLoading.sprite = thumbnailsList[i];
            thumbnailLoading.transform.name = thumbnailsList[i].name;

            thumbnailImagesLoadedList.Add(thumbnailLoading);
        }*/

        //StartSequence();
        #endregion
    }

    /*private void Start()
    {
        if (!FirebaseRemoteConfigHandler.instance.doVersionCheck)
            StartSequence();
    }*/

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

    private void Start()
    {
        StepOneVersionCheck();
    }

    public void StepOneVersionCheck()
    {
        imageLoadingStatusText.text = "";

        LeanTween.alphaCanvas(imageLoadingCanvasGroup, 1f, 0.3f).setOnComplete( () => 
        {
            if (FirebaseRemoteConfigHandler.instance != null)
                FirebaseRemoteConfigHandler.instance.InitFirebaseRemoteConfigHandler();

            imageLoadingStatusText.text = "Checking Version...";
            imageLoadingBarActual.fillAmount = 0.5f;
        });
    }

    private void OnAppVersionCorrect()
    {
        LeanTween.alphaCanvas(updateGameCanvas, 0, 0.3f);
        updateGameCanvas.interactable = false;
        updateGameCanvas.blocksRaycasts = false;

        //StartSequence();
        imageLoadingBarActual.fillAmount = 1f;

        Invoke("StepTwoAdditionalResources", 1f);
        //StepTwoAdditionalResources();
    }

    private void OnAppVersionIncorrect()
    {
        LeanTween.alphaCanvas(updateGameCanvas, 1f, 0.3f);
        updateGameCanvas.interactable = true;
        updateGameCanvas.blocksRaycasts = true;

        imageLoadingCanvasGroup.alpha = 0;
    }

    private void StepTwoAdditionalResources()
    {
        imageLoadingBarActual.fillAmount = 0;
        imageLoadingStatusText.text = "Checking Additional Resources...";

        StartCoroutine(StepTwoAdditionalResourcesRoutine());
    }

    private IEnumerator StepTwoAdditionalResourcesRoutine()
    {
        //1. Download Text Asset first
        AsyncOperationHandle<TextAsset> storiesLoadImagesDBHandle = Addressables.LoadAssetAsync<TextAsset>(storiesLoadImagesKey);

        yield return storiesLoadImagesDBHandle;

        //2. Download Images from Json if available
        switch (storiesLoadImagesDBHandle.Status)
        {
            case AsyncOperationStatus.None:
                break;

            case AsyncOperationStatus.Succeeded:

#if FCM_DEBUG
                Debug.Log("SPLASH SCREEN: Stories Load Images DB Successfully loaded!");
#endif

                string dataString = storiesLoadImagesDBHandle.Result.text;
                storiesLoadImagesDB = JsonUtility.FromJson<StoriesLoadImagesDB>(dataString);

                //3. Download the images from Keys once DB is downloaded
                if(storiesLoadImagesDB != null)
                {
                    loadNewFillAmount = 1f / (float)storiesLoadImagesDB.storiesLoadKeys.Length;

                    /*var loadResLoc = Addressables.LoadResourceLocationsAsync(storiesLoadImagesDB.storiesLoadKeys, Addressables.MergeMode.Union);
                    yield return loadResLoc;

                    var locs = loadResLoc.Result;
                    
                    long dSize = 0;
                    float dSizeKb = 0;
                    float dSizeMb = 0;

                    AsyncOperationHandle<long> downloadSize = Addressables.GetDownloadSizeAsync(locs);
                    yield return downloadSize;
                    dSize += downloadSize.Result;
                    dSizeKb = BToKb(dSize);
                    dSizeMb = KbToMb(dSizeKb);
                    print("DOWNLOAD SIZE: " + dSizeMb + "Mb");*/

                    #region IMAGE LOADING REMOVED
                    /*for (int i = 0; i < storiesLoadImagesDB.storiesLoadKeys.Length; i++)
                    {
                        if(storiesLoadImagesDB.storiesLoadKeys[i].Length > 0)
                        {
                            AsyncOperationHandle<Sprite> spriteImageFromKeyHandle = Addressables.LoadAssetAsync<Sprite>(storiesLoadImagesDB.storiesLoadKeys[i]);

                            yield return spriteImageFromKeyHandle;

                            if(spriteImageFromKeyHandle.Status == AsyncOperationStatus.Succeeded)
                            {
                                Image thumbnailLoading = Instantiate(thumbnailLoadingImagePrefab, thumbnailsLoadingContainer);
                                thumbnailLoading.color = whiteTransparent;
                                thumbnailLoading.sprite = spriteImageFromKeyHandle.Result;
                                thumbnailLoading.transform.name = spriteImageFromKeyHandle.Result.name;

                                thumbnailImagesLoadedList.Add(thumbnailLoading);

                                imageLoadingBarActual.fillAmount += loadNewFillAmount;
                            }
                        }
                    }*/

                    string randomImageKey = storiesLoadImagesDB.storiesLoadKeys[Random.Range(0, storiesLoadImagesDB.storiesLoadKeys.Length)];
                    AsyncOperationHandle<Sprite> spriteImageFromKeyHandle = Addressables.LoadAssetAsync<Sprite>(randomImageKey);

                    yield return spriteImageFromKeyHandle;

                    if(spriteImageFromKeyHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Image thumbnailLoading = Instantiate(thumbnailLoadingImagePrefab, thumbnailsLoadingContainer);
                        thumbnailLoading.color = whiteTransparent;
                        thumbnailLoading.sprite = spriteImageFromKeyHandle.Result;
                        thumbnailLoading.transform.name = spriteImageFromKeyHandle.Result.name;

                        thumbnailImagesLoadedList.Add(thumbnailLoading);

                        imageLoadingBarActual.fillAmount = 1f;
                    }
                    #endregion

                    yield return new WaitForSeconds(1f);

                    imageLoadingStatusText.text = "DONE";
                    LeanTween.alphaCanvas(imageLoadingCanvasGroup, 0, 0.3f).setOnComplete( () => 
                    {
                        StartSplashSequence();
                    });
                }

                break;

            case AsyncOperationStatus.Failed:

                imageLoadingStatusText.text = "Something Went Wrong..";
                imageLoadingBarActual.fillAmount = 0.1f;
#if FCM_DEBUG
                Debug.LogError("SPLASH SCREEN: Stories Load Images DB failed!");
#endif
                break;
        }
    }

    static float BToKb(long bytes)
    {
        return bytes / 1000f;
    }

    static float KbToMb(float kb)
    {
        return kb / 1000f;
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

                if (currentIndex + 1 > storiesLoadImagesDB.storiesLoadKeys.Length - 1 /*thumbnailsList.Length - 1*/)
                    LeanTween.alpha(thumbnailImagesLoadedList[0].rectTransform, 1f, 0.5f);
                else
                    LeanTween.alpha(thumbnailImagesLoadedList[currentIndex + 1].rectTransform, 1f, 0.5f);

                currentIndex++;
                if (currentIndex > storiesLoadImagesDB.storiesLoadKeys.Length - 1)
                    currentIndex = 0;
            }

            yield return null;
        }
    }

    private void StartSplashSequence()
    {
        //currentIndex = Random.Range(0, thumbnailsList.Length - 1);
        currentIndex = Random.Range(0, thumbnailImagesLoadedList.Count - 1);

        LTSeq splashSeq = LeanTween.sequence();

        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(titleCompanyCanvas, 1f, 1f));
        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(titleCompanyCanvas, 0, 1f));
        splashSeq.append(1f);
        splashSeq.append(LeanTween.alphaCanvas(disclaimerCanvas, 1f, 1f));
        splashSeq.append(1f);
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

            if (loadImageFromKeyHandle.Status == AsyncOperationStatus.Succeeded)
            {
                loadingProgressBar.fillAmount += newFillAmount;
                loadingProgressText.text = (loadingProgressBar.fillAmount * 100f).ToString("0") + "%";
            }
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

#if UNITY_EDITOR
        if (FirebaseRemoteConfigHandler.instance != null)
            FirebaseRemoteConfigHandler.instance.CheckVersionAgain();
#elif UNITY_ANDROID
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
