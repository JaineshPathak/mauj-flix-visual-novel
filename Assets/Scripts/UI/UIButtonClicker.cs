using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonClicker : MonoBehaviour, IPointerDownHandler
{
    public bool lookForDetailsPanel;
    public AudioSource buttonSoundSource;
    public AudioClip buttonClickSound;

    private UIStoriesDetailsPanel detailsPanel;

    private void Start()
    {
        if (lookForDetailsPanel)
            detailsPanel = FindObjectOfType<UIStoriesDetailsPanel>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (detailsPanel != null)
            detailsPanel.PlayButtonClickSound();
        else if (buttonSoundSource != null && buttonClickSound != null)
            buttonSoundSource.PlayOneShot(buttonClickSound);
    }
}
