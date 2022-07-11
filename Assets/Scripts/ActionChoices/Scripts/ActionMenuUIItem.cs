using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenuUIItem : MonoBehaviour
{
    public int id;
    public Image itemImage;
    public Image itemImageBg;
    public TextMeshProUGUI itemNameText;
    public CanvasGroup itemCanvasGroup;

    [Space(15)]

    public Vector3 normalScale = Vector3.one;
    public Vector3 focusScale = Vector3.one * 1.1f;

    [Space(15)]

    public Color focusColor = Color.white;
    public Color unFocusColor = Color.black;

    private Transform _Transform;
    private RectTransform _rectTransform;
    private ActionMenuUI actionMenuUI;
    private Button itemButton;

    public RectTransform rectTransform
    {
        get { return _rectTransform; }
    }

    private void Awake()
    {
        _Transform = transform;
        _rectTransform = _Transform as RectTransform;

        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(OnItemClicked);

        ItemReset();
    }

    public void SetupItem(Sprite _itemSprite, string _itemName, ActionMenuUI _actionMenuUI)
    {
        if (_itemSprite == null || _actionMenuUI == null)
            return;

        itemImage.sprite = _itemSprite;
        itemNameText.text = _itemName;

        if (actionMenuUI == null)
            actionMenuUI = _actionMenuUI;

        //ItemUnfocus();
    }

    public void ItemReset()
    {
        _Transform.localScale = Vector3.zero;

        if(itemCanvasGroup)
        {
            itemCanvasGroup.alpha = 0;
            itemCanvasGroup.interactable = false;
            itemCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ItemShow()
    {
        itemImage.color = focusColor;
        itemImageBg.color = focusColor;

        _Transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, normalScale, 0.4f).setEaseOutBack();
        LeanTween.alphaCanvas(itemCanvasGroup, 1f, 0.2f);

        itemCanvasGroup.interactable = true;
        itemCanvasGroup.blocksRaycasts = true;
    }    

    public void ItemFocus()
    {
        LeanTween.color(itemImage.rectTransform, focusColor, 0.4f).setEaseInOutSine();
        LeanTween.color(itemImageBg.rectTransform, focusColor, 0.4f).setEaseInOutSine();
        LeanTween.scale(gameObject, focusScale, 0.4f).setEaseInOutBack();
    }

    public void ItemUnfocus()
    {
        itemImage.color = unFocusColor;
        itemImageBg.color = unFocusColor;
    }

    public void ItemHide()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.4f).setEaseInBack();
        LeanTween.alphaCanvas(itemCanvasGroup, 0, 0.4f).setEaseInOutSine();
        itemCanvasGroup.interactable = false;
        itemCanvasGroup.blocksRaycasts = false;
    }

    private void OnItemClicked()
    {
        if (actionMenuUI == null)
            return;

        actionMenuUI.OnItemFocusClicked(id);
    }
}
