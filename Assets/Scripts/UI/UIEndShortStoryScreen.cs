using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEndShortStoryScreen : MonoBehaviour
{
    private EpisodesSpawner episodesSpawner;

    [Header("Congrats Image")]
    public Image congratulationsImage;
    public ParticleSystem endParticleEffect;

    [Header("Part 1")]
    public CanvasGroup mainCanvasGroup;
    public RectTransform middlePanelPartOne;
    public CanvasGroup wonDiamondsText;
    public Transform collectDiamondsPanel;
    public TextMeshProUGUI collectDiamondsText;
    public Button collectDiamondsButton;

    [Space(10)]

    public ParticleSystem vfxDiamondsParticles;

    [Space(10)]

    public AnimationCurve outBackMore;

    private bool isTriggered;
    private LTSeq endSeq;

    private void Awake()
    {
        ResetStuffs();
    }

    public void ResetStuffs()
    {
        if (mainCanvasGroup)
        {
            mainCanvasGroup.alpha = 0;
            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;
        }

        if (wonDiamondsText)
        {
            wonDiamondsText.alpha = 0;
            wonDiamondsText.interactable = false;
            wonDiamondsText.blocksRaycasts = false;
        }

        if (congratulationsImage)
            congratulationsImage.transform.localScale = Vector3.zero;

        if (middlePanelPartOne)
            middlePanelPartOne.anchoredPosition = new Vector2(-1000f, middlePanelPartOne.anchoredPosition.y);

        if (collectDiamondsPanel)
            collectDiamondsPanel.localScale = Vector3.zero;

        if (collectDiamondsText)
            collectDiamondsText.text = "0";

        if (collectDiamondsButton)
        {
            collectDiamondsButton.interactable = true;
            collectDiamondsButton.onClick.RemoveAllListeners();
            collectDiamondsButton.onClick.AddListener(OnCollectDiamondsButton);
            collectDiamondsButton.transform.localScale = Vector3.zero;
        }
    }

    private void Start()
    {
        if (EpisodesSpawner.instance != null)
        {
            episodesSpawner = EpisodesSpawner.instance;
            //episodesSpawner.topPanel.HideTopPanel();
        }
    }

    [ContextMenu("Play Short Story End Screen")]
    public void PlayEndingShortStoryScreen()
    {
        ResetStuffs();

        if (!mainCanvasGroup.gameObject.activeSelf)
            mainCanvasGroup.gameObject.SetActive(true);

        mainCanvasGroup.interactable = true;
        mainCanvasGroup.blocksRaycasts = true;

        endSeq = LeanTween.sequence();
        endSeq.append(LeanTween.alphaCanvas(mainCanvasGroup, 1f, 1f));
        endSeq.append(LeanTween.scale(congratulationsImage.gameObject, Vector3.one, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            if (endParticleEffect != null)
                endParticleEffect.Play();
        }));
        endSeq.append(0.5f);
        endSeq.append(LeanTween.moveLocalX(middlePanelPartOne.gameObject, 0, 0.7f).setEase(outBackMore).setOnStart(() =>
        {
            if (vfxDiamondsParticles != null)
                vfxDiamondsParticles.Play(true);
        }).setOnComplete(OnMiddlePanelComplete));
    }

    private void OnMiddlePanelComplete()
    {
        if (wonDiamondsText == null)
            return;

        LeanTween.alphaCanvas(wonDiamondsText, 1f, 1f).setOnComplete(OnWonTextComplete);
    }

    private void OnWonTextComplete()
    {
        LTSeq endDiamondsPanelSeq = LeanTween.sequence();
        endDiamondsPanelSeq.append(0.3f);
        endDiamondsPanelSeq.append(LeanTween.scale(collectDiamondsPanel.gameObject, Vector3.one, 0.5f).setEase(outBackMore));
        endDiamondsPanelSeq.append(LeanTween.value(0, 10f, 0.8f).setOnUpdate((float val) => collectDiamondsText.text = "+" + Mathf.RoundToInt(val).ToString()).setEase(LeanTweenType.linear));
        endDiamondsPanelSeq.append(LeanTween.scale(collectDiamondsButton.gameObject, Vector3.one, 0.4f).setEase(outBackMore));
        endDiamondsPanelSeq.append(() =>
        {
            if (episodesSpawner == null && EpisodesSpawner.instance)
                episodesSpawner = EpisodesSpawner.instance;

            episodesSpawner.topPanel.ShowTopPanel();
        });
    }

    private void OnCollectDiamondsButton()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        collectDiamondsButton.interactable = false;
        LeanTween.scale(collectDiamondsButton.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInBack);

        episodesSpawner.diamondsPool.PlayDiamondsAnimationDeposit(collectDiamondsPanel, episodesSpawner.topPanel.diamondsPanelIcon, 10, 10, OnDiamondCollected);
    }

    private void OnDiamondCollected()
    {
        if (episodesSpawner == null && EpisodesSpawner.instance)
            episodesSpawner = EpisodesSpawner.instance;

        if (episodesSpawner == null)
            return;

        episodesSpawner.topPanel.HideTopPanel(() => { episodesSpawner.LoadEpisodesMainMenu(false); }, 0.3f, 1f);        
    }
}