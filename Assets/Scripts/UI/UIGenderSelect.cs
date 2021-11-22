using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGenderSelect : MonoBehaviour
{
    public int selectedGender = 0;

    [Space(15)]

    public CanvasGroup canvasGroup;   

    [Space(15)]

    public UIStoriesDetailsPanel detailsPanel;

    private EpisodesSpawner episodesSpawner;

    private void Start()
    {
        if (EpisodesSpawner.instance != null && EpisodesSpawner.instance.playerData != null)
        {
            episodesSpawner = EpisodesSpawner.instance;

            if(!episodesSpawner.playerData.hasGivenGenderType)
            {
                LeanTween.alphaCanvas(canvasGroup, 1f, 0.3f);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;                
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }        
    }

    public void SelectGender(int index)
    {
        selectedGender = index;

        if (detailsPanel != null)
            detailsPanel.PlayButtonClickSound();

        SubmitGender();

        /*switch (index)
        {
            case 0:     //Male

                LeanTween.alpha(selectedGenderFrames[0].rectTransform, 1f, 0.3f);
                LeanTween.alpha(selectedGenderFrames[1].rectTransform, 0, 0.3f);
                selectedGenderFrames[0].gameObject.SetActive(true);
                selectedGenderFrames[1].gameObject.SetActive(false);

                break;

            case 1:     //Female

                LeanTween.alpha(selectedGenderFrames[0].rectTransform, 0, 0.3f);
                LeanTween.alpha(selectedGenderFrames[1].rectTransform, 1f, 0.3f);

                break;
        }*/
    }

    public void SubmitGender()
    {
        if (episodesSpawner == null)
            return;

        if (episodesSpawner.playerData == null)
            return;

        if(detailsPanel != null)
            detailsPanel.PlayButtonClickSound();

        episodesSpawner.playerData.hasGivenGenderType = true;
        episodesSpawner.playerData.genderType = selectedGender;

        SaveLoadGame.SavePlayerData(episodesSpawner.playerData);

        LeanTween.alphaCanvas(canvasGroup, 0, 0.1f);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
