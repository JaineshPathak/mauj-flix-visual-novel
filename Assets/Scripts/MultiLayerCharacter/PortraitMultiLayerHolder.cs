using System;
using UnityEngine;
using UnityEngine.UI;

public class PortraitMultiLayerHolder : MonoBehaviour
{
    public Image portraitBaseBodyImage;
    //public Image portraitClothesImage;
    //public Image portraitFaceImage;

    [Space(15)]

    public Transform clothesContainer;
    public Transform facesContainer;

    [HideInInspector] public RectTransform rectTransform;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public Image AddNewClothImage(Sprite clothSprite)
    {
        if (clothesContainer == null)
            return null;

        Image newClothImage = new GameObject(clothSprite.name, typeof(Image)).GetComponent<Image>();

        newClothImage.sprite = clothSprite;
        newClothImage.transform.SetParent(clothesContainer, false);

        newClothImage.preserveAspect = true;
        newClothImage.color = new Color(1f, 1f, 1f, 0);

        //expand to fit parent
        RectTransform rt = newClothImage.rectTransform;
        rt.sizeDelta = Vector2.zero;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.one * 0.5f;
        rt.ForceUpdateRectTransforms();

        return newClothImage;
    }

    public Image AddNewFaceImage(Sprite faceSprite)
    {
        if (facesContainer == null)
            return null;

        Image newFaceImage = new GameObject(faceSprite.name, typeof(Image)).GetComponent<Image>();

        newFaceImage.sprite = faceSprite;
        newFaceImage.transform.SetParent(facesContainer, false);

        newFaceImage.preserveAspect = true;
        newFaceImage.color = new Color(1f, 1f, 1f, 0);

        //expand to fit parent
        RectTransform rt = newFaceImage.rectTransform;
        rt.sizeDelta = Vector2.zero;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.one * 0.5f;
        rt.ForceUpdateRectTransforms();

        return newFaceImage;
    }

    public void SetClothesAsLast(Image portraitClothesImage)
    {
        if (portraitClothesImage == null)
            return;

        int i = 0;
        while (i < clothesContainer.childCount)
        {
            if (portraitClothesImage.transform == clothesContainer.GetChild(i))
            {
                portraitClothesImage.transform.SetAsLastSibling();
                break;
            }

            i++;
        }
    }

    public void SetFaceImageAsLast(Image portraitFaceImage)
    {
        if (portraitFaceImage == null)
            return;

        int i = 0;
        while(i < facesContainer.childCount)
        {
            if(portraitFaceImage.transform == facesContainer.GetChild(i))
            {
                portraitFaceImage.transform.SetAsLastSibling();
                break;
            }

            i++;
        }
    }
}
