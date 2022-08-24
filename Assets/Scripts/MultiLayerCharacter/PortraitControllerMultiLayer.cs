﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using MoonSharp.Interpreter;

//Copy of PortraitController with Extra Sprites used in CharacterMultiLayer.cs
//Line 145, 342, 390
public class PortraitControllerMultiLayer : MonoBehaviour
{
    [SerializeField] protected PortraitMultiLayerHolder portraitHolderMultiLayerPrefab;

    [Space(15)]

    protected float waitTimer;

    protected StageMultiLayer stageMultiLayer;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        stageMultiLayer = GetComponentInParent<StageMultiLayer>();
    }

    protected virtual void FinishCommand(PortraitOptionsMultiLayer options)
    {
        if (options.onComplete != null)
        {
            if (!options.waitUntilFinished)
            {
                options.onComplete();
            }
            else
            {
                StartCoroutine(WaitUntilFinished(options.fadeDuration, options.onComplete));
            }
        }
        else
        {
            StartCoroutine(WaitUntilFinished(options.fadeDuration));
        }
    }

    protected virtual PortraitOptionsMultiLayer CleanPortraitOptionsMult(PortraitOptionsMultiLayer options)
    {
        // Use default stage settings
        if (options.useDefaultSettings)
        {
            options.fadeDuration = stageMultiLayer.FadeDuration;
            options.moveDuration = stageMultiLayer.MoveDuration;
            options.shiftOffset = stageMultiLayer.ShiftOffset;
        }

        // if no previous portrait, use default portrait
        /*if (options.characterMultiLayer.State.portrait == null)
        {
            options.characterMultiLayer.State.SetPortraitImageBySprite(options.characterMultiLayer);
        }*/

        // Selected "use previous portrait"
        /*if (options.portrait == null)
        {
            options.portrait = options.character.State.portrait;
        }*/

        // if no previous position, use default position
        if (options.characterMultiLayer.PortraitMultiState.position == null)
        {
            options.characterMultiLayer.PortraitMultiState.position = stageMultiLayer.DefaultPosition.rectTransform;
        }

        // Selected "use previous position"
        if (options.toPosition == null)
        {
            options.toPosition = options.characterMultiLayer.PortraitMultiState.position;
        }

        if (options.replacedCharacterMultiLayer != null)
        {
            // if no previous position, use default position
            if (options.replacedCharacterMultiLayer.PortraitMultiState.position == null)
            {
                options.replacedCharacterMultiLayer.PortraitMultiState.position = stageMultiLayer.DefaultPosition.rectTransform;
            }
        }

        // If swapping, use replaced character's position
        if (options.display == DisplayType.Replace)
        {
            options.toPosition = options.replacedCharacterMultiLayer.PortraitMultiState.position;
        }

        // Selected "use previous position"
        if (options.fromPosition == null)
        {
            options.fromPosition = options.characterMultiLayer.PortraitMultiState.position;
        }

        // if portrait not moving, use from position is same as to position
        if (!options.move)
        {
            options.fromPosition = options.toPosition;
        }

        if (options.display == DisplayType.Hide)
        {
            options.fromPosition = options.characterMultiLayer.PortraitMultiState.position;
        }

        // if no previous facing direction, use default facing direction
        if (options.characterMultiLayer.PortraitMultiState.facing == FacingDirection.None)
        {
            options.characterMultiLayer.PortraitMultiState.facing = options.characterMultiLayer.PortraitsFace;
        }

        // Selected "use previous facing direction"
        if (options.facing == FacingDirection.None)
        {
            options.facing = options.characterMultiLayer.PortraitMultiState.facing;
        }

        if (options.characterMultiLayer.PortraitMultiState.portraitBaseBody == null)
        {
            CreatePortraitObject(options.characterMultiLayer, options.fadeDuration);
        }

        return options;
    }

    protected virtual void CreatePortraitObject(CharacterMultiLayer character, float fadeDuration)
    {
        if (character.PortraitMultiState.holder == null)
        {
            /*character.State.holder = new GameObject(character.name + " holder",
                                               typeof(RectTransform)
                                               //typeof(CanvasRenderer),
                                               //typeof(Image)
                                               ).GetComponent<RectTransform>();*/

            PortraitMultiLayerHolder newHolder = Instantiate(portraitHolderMultiLayerPrefab);
            newHolder.transform.name = character.name + "-Multi-Holder";
            newHolder.portraitBaseBodyImage.sprite = character.PortraitBaseBody;

            character.PortraitMultiState.mainHolder = newHolder;
            character.PortraitMultiState.holder = newHolder.rectTransform;
            character.PortraitMultiState.portraitBaseBodyImage = newHolder.portraitBaseBodyImage;

            // Set it to be a child of the stage
            character.PortraitMultiState.holder.transform.SetParent(stageMultiLayer.PortraitCanvas.transform, false);

            SetRectTransform(character.PortraitMultiState.holder, stageMultiLayer.DefaultPosition.GetComponent<RectTransform>());
        }

        //CLOTHES FIRST
        if(character.PortraitMultiState.allPortraitsClothes.Count == 0)
        {
            foreach(var clothesItem in character.PortraitClothes)
            {
                if (clothesItem == null)
                    continue;

                Image clothesImage = character.PortraitMultiState.mainHolder.AddNewClothImage(clothesItem);
                clothesImage.gameObject.SetActive(false);

                character.PortraitMultiState.allPortraitsClothes.Add(clothesImage);
            }
        }

        //FACES NEXT
        if(character.PortraitMultiState.allPortraitsFace.Count == 0)
        {
            foreach (var faceItem in character.PortraitFaces)
            {
                if (faceItem == null)
                    continue;

                Image faceImage = character.PortraitMultiState.mainHolder.AddNewFaceImage(faceItem);
                faceImage.gameObject.SetActive(false);

                character.PortraitMultiState.allPortraitsFace.Add(faceImage);
            }
        }


        #region REMOVED
        /*if (character.State.allPortraits.Count == 0)
        {
            foreach (var item in character.Portraits)
            {
                if (item == null)
                {
                    Debug.LogError("null in portrait list on character " + character.name);
                    continue;
                }
                // Create a new portrait object
                GameObject po = new GameObject(item.name,
                                                        typeof(RectTransform),
                                                        typeof(CanvasRenderer),
                                                        typeof(Image));

                // Set it to be a child of the stage
                po.transform.SetParent(character.State.holder, false);

                // Configure the portrait image
                Image pi = po.GetComponent<Image>();
                pi.preserveAspect = true;
                pi.sprite = item;
                pi.color = new Color(1f, 1f, 1f, 0f);

                if (item == character.ProfileSprite)
                {
                    character.State.portraitImage = pi;
                }

                //expand to fit parent
                RectTransform rt = po.GetComponent<RectTransform>();
                rt.sizeDelta = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = Vector2.one * 0.5f;
                rt.ForceUpdateRectTransforms();

                po.SetActive(false);

                character.State.allPortraits.Add(pi);
            }
        }*/
        #endregion
    }

    protected virtual void SetupPortrait(PortraitOptionsMultiLayer options)
    {
        if (options.characterMultiLayer.PortraitMultiState.holder == null)
            return;

        SetRectTransform(options.characterMultiLayer.PortraitMultiState.holder, options.fromPosition);

        if (options.characterMultiLayer.PortraitMultiState.facing != options.characterMultiLayer.PortraitsFace)
        {
            options.characterMultiLayer.PortraitMultiState.holder.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            options.characterMultiLayer.PortraitMultiState.holder.localScale = new Vector3(1f, 1f, 1f);
        }

        if (options.facing != options.characterMultiLayer.PortraitsFace)
        {
            options.characterMultiLayer.PortraitMultiState.holder.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            options.characterMultiLayer.PortraitMultiState.holder.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    protected virtual void DoMoveTween(PortraitOptionsMultiLayer options)
    {
        CleanPortraitOptionsMult(options);

        LeanTween.cancel(options.characterMultiLayer.PortraitMultiState.holder.gameObject);

        // LeanTween doesn't handle 0 duration properly
        float duration = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

        // LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
        LeanTween.move(options.characterMultiLayer.PortraitMultiState.holder.gameObject, options.toPosition.position, duration)
            .setEase(stageMultiLayer.FadeEaseType);

        if (options.waitUntilFinished)
        {
            waitTimer = duration;
        }
    }

    public static void SetRectTransform(RectTransform target, RectTransform from)
    {
        target.eulerAngles = from.eulerAngles;
        target.position = from.position;
        target.rotation = from.rotation;
        target.anchoredPosition = from.anchoredPosition;
        target.sizeDelta = from.sizeDelta;
        target.anchorMax = from.anchorMax;
        target.anchorMin = from.anchorMin;
        target.pivot = from.pivot;
        target.localScale = from.localScale;
    }

    public virtual void RunPortraitCommand(PortraitOptionsMultiLayer options, Action onComplete)
    {
        waitTimer = 0f;

        // If no character specified, do nothing
        if (options.characterMultiLayer == null)
        {
            onComplete();
            return;
        }

        // If Replace and no replaced character specified, do nothing
        if (options.display == DisplayType.Replace && options.replacedCharacterMultiLayer == null)
        {
            onComplete();
            return;
        }

        // Early out if hiding a character that's already hidden
        if (options.display == DisplayType.Hide &&
            !options.characterMultiLayer.PortraitMultiState.onScreen)
        {
            onComplete();
            return;
        }

        options = CleanPortraitOptionsMult(options);
        options.onComplete = onComplete;

        switch (options.display)
        {
            case (DisplayType.Show):
                Show(options);
                break;

            case (DisplayType.Hide):
                Hide(options);
                break;

            case (DisplayType.Replace):
                Show(options);
                Hide(options.replacedCharacterMultiLayer, options.replacedCharacterMultiLayer.PortraitMultiState.position.name);
                break;

            case (DisplayType.MoveToFront):
                MoveToFront(options);
                break;
        }
    }

    public virtual void MoveToFront(PortraitOptionsMultiLayer options)
    {
        //options.characterMultiLayer.State.portraitFaceImage.transform.SetSiblingIndex(options.characterMultiLayer.State.portraitFaceImage.transform.parent.childCount);

        options.characterMultiLayer.PortraitMultiState.mainHolder.SetClothesAsLast(options.characterMultiLayer.PortraitMultiState.portraitClothesImage);
        options.characterMultiLayer.PortraitMultiState.mainHolder.SetFaceImageAsLast(options.characterMultiLayer.PortraitMultiState.portraitFaceImage);
        options.characterMultiLayer.PortraitMultiState.display = DisplayType.MoveToFront;
        FinishCommand(options);
    }

    public virtual void Show(PortraitOptionsMultiLayer options)
    {
        options = CleanPortraitOptionsMult(options);

        if (options.shiftIntoPlace)
        {
            options.fromPosition = Instantiate(options.toPosition) as RectTransform;
            if (options.offset == PositionOffset.OffsetLeft)
            {
                options.fromPosition.anchoredPosition =
                    new Vector2(options.fromPosition.anchoredPosition.x - Mathf.Abs(options.shiftOffset.x),
                        options.fromPosition.anchoredPosition.y - Mathf.Abs(options.shiftOffset.y));
            }
            else if (options.offset == PositionOffset.OffsetRight)
            {
                options.fromPosition.anchoredPosition =
                    new Vector2(options.fromPosition.anchoredPosition.x + Mathf.Abs(options.shiftOffset.x),
                        options.fromPosition.anchoredPosition.y + Mathf.Abs(options.shiftOffset.y));
            }
            else
            {
                options.fromPosition.anchoredPosition = new Vector2(options.fromPosition.anchoredPosition.x, options.fromPosition.anchoredPosition.y);
            }
        }

        SetupPortrait(options);

        // LeanTween doesn't handle 0 duration properly
        float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

        //var prevPortrait = options.characterMultiLayer.State.portrait;

        if (options.characterMultiLayer.PortraitMultiState.portraitBaseBodyImage != null && options.characterMultiLayer.PortraitMultiState.portraitBaseBodyImage != options.portraitBaseBody)        
            HidePortrait(options.characterMultiLayer.PortraitMultiState.holder, duration);

        if (options.characterMultiLayer.PortraitMultiState.portraitClothesImage != null && options.characterMultiLayer.PortraitMultiState.portraitClothesImage != options.portraitClothes)
            HidePortrait(options.characterMultiLayer.PortraitMultiState.holder, duration);

        if (options.characterMultiLayer.PortraitMultiState.portraitFace != null && options.characterMultiLayer.PortraitMultiState.portraitFace != options.portraitFace)
            HidePortrait(options.characterMultiLayer.PortraitMultiState.holder, duration);

        options.characterMultiLayer.PortraitMultiState.portraitBaseBodyImage.gameObject.SetActive(true);

        options.characterMultiLayer.PortraitMultiState.SetPortraitClothesBySprite(options.portraitClothes);
        options.characterMultiLayer.PortraitMultiState.portraitClothesImage.gameObject.SetActive(true);

        options.characterMultiLayer.PortraitMultiState.SetPortraitFaceBySprite(options.portraitFace);
        options.characterMultiLayer.PortraitMultiState.portraitFaceImage.gameObject.SetActive(true);

        LeanTween.alpha(options.characterMultiLayer.PortraitMultiState.portraitBaseBodyImage.rectTransform, 1f, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
        LeanTween.alpha(options.characterMultiLayer.PortraitMultiState.portraitClothesImage.rectTransform, 1f, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
        LeanTween.alpha(options.characterMultiLayer.PortraitMultiState.portraitFaceImage.rectTransform, 1f, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
        /*options.characterMultiLayer.State.SetPortraitImageBySprite(options.portrait);
        options.characterMultiLayer.State.portraitImage.rectTransform.gameObject.SetActive(true);
        //LeanTween.color(options.character.State.portraitImage.rectTransform, Color.white, duration).setEase(stage.FadeEaseType).setRecursive(false);
        LeanTween.alpha(options.characterMultiLayer.State.portraitImage.rectTransform, 1f, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);*/

        DoMoveTween(options);

        FinishCommand(options);

        if (!stageMultiLayer.CharactersOnStage.Contains(options.characterMultiLayer))
        {
            stageMultiLayer.CharactersOnStage.Add(options.characterMultiLayer);
        }

        MoveToFront(options);

        // Update character state after showing
        options.characterMultiLayer.PortraitMultiState.onScreen = true;
        options.characterMultiLayer.PortraitMultiState.display = DisplayType.Show;
        options.characterMultiLayer.PortraitMultiState.facing = options.facing;
        options.characterMultiLayer.PortraitMultiState.position = options.toPosition;
    }

    protected virtual void HidePortrait(RectTransform rectTransform, float duration)
    {
        LeanTween.alpha(rectTransform, 0f, duration)
            .setEase(stageMultiLayer.FadeEaseType)
            .setRecursive(false)
            .setOnComplete(() => rectTransform.gameObject.SetActive(false));
    }

    /// <summary>
    /// Hide portrait with provided options
    /// </summary>
    public virtual void Hide(PortraitOptionsMultiLayer options)
    {
        CleanPortraitOptionsMult(options);

        if (options.characterMultiLayer.PortraitMultiState.display == DisplayType.None)
        {
            return;
        }

        SetupPortrait(options);

        // LeanTween doesn't handle 0 duration properly
        float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

        HidePortrait(options.characterMultiLayer.PortraitMultiState.portraitClothesImage.rectTransform, duration);
        HidePortrait(options.characterMultiLayer.PortraitMultiState.portraitFaceImage.rectTransform, duration);
        HidePortrait(options.characterMultiLayer.PortraitMultiState.portraitBaseBodyImage.rectTransform, duration);

        DoMoveTween(options);

        //update character state after hiding
        options.characterMultiLayer.PortraitMultiState.onScreen = false;
        options.characterMultiLayer.PortraitMultiState.facing = options.facing;
        options.characterMultiLayer.PortraitMultiState.position = options.toPosition;
        options.characterMultiLayer.PortraitMultiState.display = DisplayType.Hide;

        if (stageMultiLayer.CharactersOnStage.Remove(options.characterMultiLayer))
        {
        }

        FinishCommand(options);
    }

    /// <summary>
    /// Sets the dimmed state of a character on the stage.
    /// </summary>
    public virtual void SetDimmed(CharacterMultiLayer character, bool dimmedState)
    {
        if (character.PortraitMultiState.dimmed == dimmedState)
        {
            return;
        }

        character.PortraitMultiState.dimmed = dimmedState;

        Color targetColor = dimmedState ? stageMultiLayer.DimColor : Color.white;

        // LeanTween doesn't handle 0 duration properly
        float duration = (stageMultiLayer.FadeDuration > 0f) ? stageMultiLayer.FadeDuration : float.Epsilon;

        LeanTween.color(character.PortraitMultiState.portraitBaseBodyImage.rectTransform, targetColor, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
        LeanTween.color(character.PortraitMultiState.portraitClothesImage.rectTransform, targetColor, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
        LeanTween.color(character.PortraitMultiState.portraitFaceImage.rectTransform, targetColor, duration).setEase(stageMultiLayer.FadeEaseType).setRecursive(false);
    }

    protected virtual IEnumerator WaitUntilFinished(float duration, Action onComplete = null)
    {
        // Wait until the timer has expired
        // Any method can modify this timer variable to delay continuing.

        waitTimer = duration;
        while (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            yield return null;
        }

        // Wait until next frame just to be safe
        yield return new WaitForEndOfFrame();

        if (onComplete != null)
        {
            onComplete();
        }
    }

    #region Overloads and Helpers

    /// <summary>
    /// Shows character at a named position in the stage
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position">Named position on stage</param>
    public virtual void Show(CharacterMultiLayer character, string position)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;
        options.fromPosition = options.toPosition = stageMultiLayer.GetPosition(position);

        Show(options);
    }

    /// <summary>
    /// Shows character moving from a position to a position
    /// </summary>
    /// <param name="character"></param>
    /// <param name="portrait"></param>
    /// <param name="fromPosition">Where the character will appear</param>
    /// <param name="toPosition">Where the character will move to</param>
    public virtual void Show(CharacterMultiLayer character, string portraitClothes, string portraitFace, string fromPosition, string toPosition)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;
        options.portraitBaseBody = character.PortraitBaseBody;
        options.portraitClothes = character.GetPortraitClothes(portraitClothes);
        options.portraitFace = character.GetPortraitFace(portraitFace);
        options.fromPosition = stageMultiLayer.GetPosition(fromPosition);
        options.toPosition = stageMultiLayer.GetPosition(toPosition);
        options.move = true;

        Show(options);
    }

    /// <summary>
    /// From lua, you can pass an options table with named arguments
    /// example:
    ///     stage.show{character=jill, portrait="happy", fromPosition="right", toPosition="left"}
    /// Any option available in the PortraitOptions is available from Lua
    /// </summary>
    /// <param name="optionsTable">Moonsharp Table</param>
    /*public virtual void Show(Table optionsTable)
    {
        Show(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stageMultiLayer));
    }*/


    /// <summary>
    /// Simple show command that shows the character with an available named portrait
    /// </summary>
    /// <param name="character">CharacterMultiLayer to show</param>
    /// <param name="portrait">Named portrait to show for the character, i.e. "angry", "happy", etc</param>
    public virtual void ShowPortrait(CharacterMultiLayer character, string portraitClothes, string portraitFace)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;
        options.portraitBaseBody = character.PortraitBaseBody;
        options.portraitClothes = character.GetPortraitClothes(portraitClothes);
        options.portraitFace = character.GetPortraitFace(portraitFace);

        if (character.PortraitMultiState.position == null)
        {
            options.toPosition = options.fromPosition = stageMultiLayer.GetPosition("middle");
        }
        else
        {
            options.fromPosition = options.toPosition = character.PortraitMultiState.position;
        }

        Show(options);
    }

    /// <summary>
    /// Simple character hide command
    /// </summary>
    /// <param name="character">Character to hide</param>
    public virtual void Hide(CharacterMultiLayer character)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;

        Hide(options);
    }

    /// <summary>
    /// Move the character to a position then hide it
    /// </summary>
    /// <param name="character">Character to hide</param>
    /// <param name="toPosition">Where the character will disapear to</param>
    public virtual void Hide(CharacterMultiLayer character, string toPosition)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;
        options.toPosition = stageMultiLayer.GetPosition(toPosition);
        options.move = true;

        Hide(options);
    }

    /// <summary>
    /// From lua, you can pass an options table with named arguments
    /// example:
    ///     stage.hide{character=jill, toPosition="left"}
    /// Any option available in the PortraitOptions is available from Lua
    /// </summary>
    /// <param name="optionsTable">Moonsharp Table</param>
    /*public virtual void Hide(Table optionsTable)
    {
        Hide(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stageMultiLayer));
    }*/

    /// <summary>
    /// Moves Character in front of other characters on stage
    /// </summary>
    public virtual void MoveToFront(CharacterMultiLayer character)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;

        MoveToFront(CleanPortraitOptionsMult(options));
    }


    protected virtual void DoMoveTween(CharacterMultiLayer character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
    {
        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer(true);
        options.characterMultiLayer = character;
        options.fromPosition = fromPosition;
        options.toPosition = toPosition;
        options.moveDuration = moveDuration;
        options.waitUntilFinished = waitUntilFinished;

        DoMoveTween(options);
    }

    #endregion
}
