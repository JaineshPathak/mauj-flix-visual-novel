using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Globalization;
using UnityEngine;
using Fungus;

[ExecuteInEditMode]
public class CharacterMultiLayer : MonoBehaviour, ILocalizable, IComparer<CharacterMultiLayer>
{
    [Tooltip("Character name as displayed in Say Dialog.")]
    [SerializeField] protected string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")

    [Tooltip("Color to display the character name in Say Dialog.")]
    [SerializeField] protected Color nameColor = Color.white;

    [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. This Say Dialog will be used whenever the character speaks.")]
    [SerializeField] protected SayDialog setSayDialog;

    [TextArea(5, 10)]
    [SerializeField] protected string description;

    [Tooltip("Sound effect to play when this character is speaking.")]
    [SerializeField] protected AudioClip soundEffect;

    [Space(15)]

    [Tooltip("Base Body Sprite of this character")]
    [SerializeField] protected Sprite portraitBaseBody;

    [Tooltip("List of Clothes Sprites of this character")]
    [SerializeField] protected List<Sprite> portraitClothes;

    [Tooltip("List of portrait images that can be displayed for this character.")]
    [SerializeField] protected List<Sprite> portraitFaces;

    [Tooltip("Direction that portrait sprites face.")]
    [SerializeField] protected FacingDirection portraitsFace;    

    protected PortraitStateMultiLayer portaitState = new PortraitStateMultiLayer();

    protected static List<CharacterMultiLayer> activeCharactersMultiLayer = new List<CharacterMultiLayer>();

    protected virtual void OnEnable()
    {
        if (!activeCharactersMultiLayer.Contains(this))
        {
            activeCharactersMultiLayer.Add(this);
            activeCharactersMultiLayer.Sort(this);
        }
    }

    protected virtual void OnDisable()
    {
        activeCharactersMultiLayer.Remove(this);
    }

    #region Public Members
    public static List<CharacterMultiLayer> ActiveCharactersMultiLayer { get { return activeCharactersMultiLayer; } }
    public virtual string NameText { get { return nameText; } }
    public virtual Color NameColor { get { return nameColor; } }
    public virtual AudioClip SoundEffect { get { return soundEffect; } }
    public virtual Sprite PortraitBaseBody { get { return portraitBaseBody; } set { portraitBaseBody = value; } }
    public virtual List<Sprite> PortraitClothes { get { return portraitClothes; } }
    public virtual List<Sprite> PortraitFaces { get { return portraitFaces; } }
    public virtual FacingDirection PortraitsFace { get { return portraitsFace; } }
    public virtual PortraitStateMultiLayer State { get { return portaitState; } }
    public virtual SayDialog SetSayDialog { get { return setSayDialog; } }
    public string GetObjectName() { return gameObject.name; }

    public virtual bool NameStartsWith(string matchString)
    {
#if NETFX_CORE
            return name.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase)
                || nameText.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase);
#else
        return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
            || nameText.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
#endif
    }
    #endregion

    public virtual bool NameMatch(string matchString)
    {
        return string.Compare(name, matchString, true, CultureInfo.CurrentCulture) == 0
            || string.Compare(nameText, matchString, true, CultureInfo.CurrentCulture) == 0;
    }

    public virtual Sprite GetPortraitFace(string portraitString)
    {
        if (string.IsNullOrEmpty(portraitString))
        {
            return null;
        }

        for (int i = 0; i < portraitFaces.Count; i++)
        {
            if (portraitFaces[i] != null && string.Compare(portraitFaces[i].name, portraitString, true) == 0)
            {
                return portraitFaces[i];
            }
        }
        return null;
    }

    public virtual Sprite GetPortraitClothes(string portraitString)
    {
        if (string.IsNullOrEmpty(portraitString))
        {
            return null;
        }

        for (int i = 0; i < portraitClothes.Count; i++)
        {
            if (portraitClothes[i] != null && string.Compare(portraitClothes[i].name, portraitString, true) == 0)
            {
                return portraitClothes[i];
            }
        }
        return null;
    }

    public int Compare(CharacterMultiLayer x, CharacterMultiLayer y)
    {
        if (x == y)
            return 0;
        if (y == null)
            return 1;
        if (x == null)
            return -1;

        return x.name.CompareTo(y.name);
    }

    #region ILocalizable implementation
    public string GetDescription()
    {
        return description;
    }

    public string GetStandardText()
    {
        return nameText;
    }

    public string GetStringId()
    {
        return "CHARACTER-MULTLAYER." + nameText;
    }

    public void SetStandardText(string standardText)
    {
        nameText = standardText;
    }
    #endregion
}
