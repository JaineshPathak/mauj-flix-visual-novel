using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

//Copy of PortraitOptions in PortraitUtils.cs with extra sprites
public class PortraitOptionsMultiLayer
{
    public CharacterMultiLayer characterMultiLayer;
    public CharacterMultiLayer replacedCharacterMultiLayer;
    public Sprite portraitBaseBody;
    public Sprite portraitClothes;
    public Sprite portraitFace;
    public DisplayType display;
    public PositionOffset offset;
    public RectTransform fromPosition;
    public RectTransform toPosition;
    public FacingDirection facing;
    public bool useDefaultSettings;
    public float fadeDuration;
    public float moveDuration;
    public Vector2 shiftOffset;
    public bool move; //sets to position to be the same as from
    public bool shiftIntoPlace;
    public bool waitUntilFinished;
    public System.Action onComplete;

    public PortraitOptionsMultiLayer(bool useDefaultSettings = true)
    {
        characterMultiLayer = null;
        replacedCharacterMultiLayer = null;
        portraitBaseBody = null;
        portraitClothes = null;
        portraitFace = null;
        display = DisplayType.None;
        offset = PositionOffset.None;
        fromPosition = null;
        toPosition = null;
        facing = FacingDirection.None;
        shiftOffset = new Vector2(0, 0);
        move = false;
        shiftIntoPlace = false;
        waitUntilFinished = false;
        onComplete = null;

        // Special values that can be overridden
        fadeDuration = 0.5f;
        moveDuration = 1f;
        this.useDefaultSettings = useDefaultSettings;
    }
}

public class PortraitStateMultiLayer
{
    public bool onScreen;
    public bool dimmed;
    public DisplayType display;
    public RectTransform position, holder;
    public PortraitMultiLayerHolder mainHolder;
    public FacingDirection facing;

    public Image portraitBaseBodyImage;
    public Sprite portraitBaseBody { get { return portraitBaseBodyImage != null ? portraitBaseBodyImage.sprite : null; } }

    public Image portraitClothesImage;
    public Sprite portraitClothes { get { return portraitClothesImage != null ? portraitClothesImage.sprite : null; } }

    public Image portraitFaceImage;
    public Sprite portraitFace { get { return portraitFaceImage != null ? portraitFaceImage.sprite : null; } }

    //public List<Image> allPortraitsBaseBody = new List<Image>();
    public List<Image> allPortraitsClothes = new List<Image>();
    public List<Image> allPortraitsFace = new List<Image>();

    public void SetPortraitBaseBodyBySprite(Sprite baseBodySprite)
    {

    }

    public void SetPortraitClothesBySprite(Sprite clothSprite)
    {
        portraitClothesImage = allPortraitsClothes.Find(x => x.sprite == clothSprite);
    }

    public void SetPortraitFaceBySprite(Sprite faceSprite)
    {
        portraitFaceImage = allPortraitsFace.Find(x => x.sprite == faceSprite);
    }

    /*public void SetPortraitImageBySprite(Sprite portrait)
    {
        portraitImage = allPortraits.Find(x => x.sprite == portrait);
    }*/
}

public static class PortraitUtilsMultiLayer
{
}
