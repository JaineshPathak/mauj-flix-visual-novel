using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossFadeText : MonoBehaviour
{
    public Image thumbnailLoadingImagePrefab;

    [Space(15)]

    public int currentIndex;
    public Sprite[] spritesList;

    [Space(15)]

    public float timer;
    public float timerInterval = 3f;

    private Color whiteTransparent = new Color(1f, 1f, 1f, 0);
    private List<Image> thumbnailImagesList = new List<Image>();

    /*[Space(15)]

    public bool fadeFlag;

    private int fadeIndexPrevious;
    private int fadeIndexNext;

    public int FadeIndex
    {
        set
        {
            fadeIndexPrevious = value;
            fadeIndexNext = fadeIndexPrevious + 1;            
        }
    }

    private void Start()
    {
        FadeIndex = currentIndex;
        imageOne.sprite = spritesList[currentIndex];
        imageTwo.sprite = spritesList[currentIndex + 1];
    }*/

    private void Start()
    {
        //currentIndex = -1;

        currentIndex = Random.Range(0, spritesList.Length - 1);

        for (int i = 0; i < spritesList.Length; i++)
        {
            Image thumbnailLoading = Instantiate(thumbnailLoadingImagePrefab, transform);
            thumbnailLoading.color = (i == currentIndex) ? Color.white : whiteTransparent;
            thumbnailLoading.sprite = spritesList[i];
            thumbnailLoading.transform.name = spritesList[i].name;

            thumbnailImagesList.Add(thumbnailLoading);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timerInterval)
        {
            timer = 0;

            LeanTween.alpha(thumbnailImagesList[currentIndex].rectTransform, 0, 0.5f);

            if (currentIndex + 1 > spritesList.Length - 1)
                LeanTween.alpha(thumbnailImagesList[0].rectTransform, 1f, 0.5f);
            else
                LeanTween.alpha(thumbnailImagesList[currentIndex + 1].rectTransform, 1f, 0.5f);

            currentIndex++;
            if (currentIndex > spritesList.Length - 1)
                currentIndex = 0;

            /*fadeFlag = !fadeFlag;

            currentIndex++;
            if (currentIndex > spritesList.Length - 1)
                currentIndex = 0;



            FadeIndex = currentIndex;            

            if (fadeFlag)
            {
                imageOne.sprite = spritesList[currentIndex];

                LeanTween.alpha(imageOne.rectTransform, 1f, 0.5f);
                LeanTween.alpha(imageTwo.rectTransform, 0, 0.5f);
            }
            else
            {
                if(currentIndex + 1 > spritesList.Length - 1)
                    imageTwo.sprite = spritesList[0];
                else
                    imageTwo.sprite = spritesList[currentIndex + 1];

                LeanTween.alpha(imageOne.rectTransform, 0, 0.5f);
                LeanTween.alpha(imageTwo.rectTransform, 1f, 0.5f);
            }*/
        }
    }
}
