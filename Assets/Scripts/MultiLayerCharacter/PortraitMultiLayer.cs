using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
                 "Portrait (Multi-Layer)",
                 "Controls a character portrait. (Multi-Layer support)")]
public class PortraitMultiLayer : ControlWithDisplay<DisplayType>
{
    [Tooltip("Stage to display portrait on")]
    [SerializeField] protected StageMultiLayer stage;

    [Tooltip("Character to display")]
    [SerializeField] protected CharacterMultiLayer character;

    [Tooltip("Character to swap with")]
    [SerializeField] protected CharacterMultiLayer replacedCharacter;

    [Tooltip("Portrait Base Body to display")]
    [SerializeField] protected Sprite portraitBaseBody;

    [Tooltip("Portrait Clothes to display")]
    [SerializeField] protected Sprite portraitClothes;

    [Tooltip("Portrait Face to display")]
    [SerializeField] protected Sprite portraitFace;

    [Tooltip("Move the portrait from/to this offset position")]
    [SerializeField] protected PositionOffset offset;

    [Tooltip("Move the portrait from this position")]
    [SerializeField] protected RectTransform fromPosition;

    [Tooltip("Move the portrait to this position")]
    [SerializeField] protected RectTransform toPosition;

    [Tooltip("Direction character is facing")]
    [SerializeField] protected FacingDirection facing;

    [Tooltip("Use Default Settings")]
    [SerializeField] protected bool useDefaultSettings = true;

    [Tooltip("Fade Duration")]
    [SerializeField] protected float fadeDuration = 0.5f;

    [Tooltip("Movement Duration")]
    [SerializeField] protected float moveDuration = 1f;

    [Tooltip("Shift Offset")]
    [SerializeField] protected Vector2 shiftOffset;

    [Tooltip("Move portrait into new position")]
    [SerializeField] protected bool move;

    [Tooltip("Start from offset position")]
    [SerializeField] protected bool shiftIntoPlace;

    [Tooltip("Wait until the tween has finished before executing the next command")]
    [SerializeField] protected bool waitUntilFinished = false;

    #region Public
    public virtual StageMultiLayer _Stage { get { return stage; } set { stage = value; } }
    public virtual CharacterMultiLayer _Character { get { return character; } set { character = value; } }
    public virtual Sprite _PortraitBaseBody { get { return portraitBaseBody; } set { portraitBaseBody = value; } }
    public virtual Sprite _PortraitClothes { get { return portraitClothes; } set { portraitClothes = value; } }
    public virtual Sprite _PortraitFace { get { return portraitFace; } set { portraitFace = value; } }
    public virtual PositionOffset Offset { get { return offset; } set { offset = value; } }
    public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value; } }
    public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value; } }
    public virtual FacingDirection Facing { get { return facing; } set { facing = value; } }
    public virtual bool UseDefaultSettings { get { return useDefaultSettings; } set { useDefaultSettings = value; } }
    public virtual bool Move { get { return move; } set { move = value; } }
    public virtual bool ShiftIntoPlace { get { return shiftIntoPlace; } set { shiftIntoPlace = value; } }

    public override void OnEnter()
    {
        //if (character != null && display == DisplayType.Show)
            //portrait = character.GetPortrait(_PortraitName);

        // Selected "use default Portrait Stage"
        if (stage == null)
        {
            // If no default specified, try to get any portrait stage in the scene
            stage = StageMultiLayer.GetActiveStageMult();
            // If portrait stage does not exist, do nothing
            if (stage == null)
            {
                Continue();
                return;
            }

        }

        // If no display specified, do nothing
        if (IsDisplayNone(display))
        {
            Continue();
            return;
        }

        PortraitOptionsMultiLayer options = new PortraitOptionsMultiLayer();

        options.characterMultiLayer = character;
        options.replacedCharacterMultiLayer = replacedCharacter;
        options.portraitBaseBody = portraitBaseBody;
        options.portraitClothes = portraitClothes;
        options.portraitFace = portraitFace;
        options.display = display;
        options.offset = offset;
        options.fromPosition = fromPosition;
        options.toPosition = toPosition;
        options.facing = facing;
        options.useDefaultSettings = useDefaultSettings;
        options.fadeDuration = fadeDuration;
        options.moveDuration = moveDuration;
        options.shiftOffset = shiftOffset;
        options.move = move;
        options.shiftIntoPlace = shiftIntoPlace;
        options.waitUntilFinished = waitUntilFinished;

        stage.RunPortraitCommand(options, Continue);
    }

    public override string GetSummary()
    {
        if (display == DisplayType.None && character == null)
        {
            return "Error: No character or display selected";
        }
        else if (display == DisplayType.None)
        {
            return "Error: No display selected";
        }
        else if (character == null)
        {
            return "Error: No character selected";
        }

        string displaySummary = "";
        string characterSummary = "";
        string fromPositionSummary = "";
        string toPositionSummary = "";
        string stageSummary = "";
        string portraitSummary = "";
        string facingSummary = "";

        displaySummary = StringFormatter.SplitCamelCase(display.ToString());

        if (display == DisplayType.Replace)
        {
            if (replacedCharacter != null)
            {
                displaySummary += " \"" + replacedCharacter.name + "\" with";
            }
        }

        characterSummary = character.name;
        if (stage != null)
        {
            stageSummary = " on \"" + stage.name + "\"";
        }

        /*if (portrait != null)
        {
            portraitSummary = " " + portrait.name;
        }*/

        if (shiftIntoPlace)
        {
            if (offset != 0)
            {
                fromPositionSummary = offset.ToString();
                fromPositionSummary = " from " + "\"" + fromPositionSummary + "\"";
            }
        }
        else if (fromPosition != null)
        {
            fromPositionSummary = " from " + "\"" + fromPosition.name + "\"";
        }

        if (toPosition != null)
        {
            string toPositionPrefixSummary = "";
            if (move)
            {
                toPositionPrefixSummary = " to ";
            }
            else
            {
                toPositionPrefixSummary = " at ";
            }

            toPositionSummary = toPositionPrefixSummary + "\"" + toPosition.name + "\"";
        }

        if (facing != FacingDirection.None)
        {
            if (facing == FacingDirection.Left)
            {
                facingSummary = "<--";
            }
            if (facing == FacingDirection.Right)
            {
                facingSummary = "-->";
            }

            facingSummary = " facing \"" + facingSummary + "\"";
        }

        return displaySummary + " \"" + characterSummary + portraitSummary + "\"" + stageSummary + facingSummary + fromPositionSummary + toPositionSummary;
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 200, 250, 255);
    }

    public override void OnCommandAdded(Block parentBlock)
    {
        //Default to display type: show
        //display = DisplayType.Show;

        display = DisplayType.Show;
    }
    #endregion
}
