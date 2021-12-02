using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodesTesting : MonoBehaviour
{
    public EpisodesLoadButton episodeLoadButtonPrefab;
    public GameObject titleObject;
    public GameObject backButton;
    public Transform episodeButtonGroupLayout;
    
    [Space(15)]

    public GameObject[] episodesToTest;

    [Space(15)]

    public GameObject currentEpisodePrefab;

    private void Start()
    {
        backButton.SetActive(false);
        LoadEpisodes();
    }

    private void LoadEpisodes()
    {
        if (episodeLoadButtonPrefab == null || episodesToTest.Length <= 0)
            return;

        for (int i = 0; i < episodesToTest.Length; i++)
        {
            EpisodesLoadButton buttonGameObject = Instantiate(episodeLoadButtonPrefab, episodeButtonGroupLayout);
            buttonGameObject.episodeText.text = "Episode " + (i + 1);
            buttonGameObject.episodesTesting = this;
            buttonGameObject.episodePrefab = episodesToTest[i];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentEpisodePrefab != null)
            OnBackButton();
    }

    public void OnBackButton()
    {
        if (currentEpisodePrefab == null)
            return;

        Destroy(currentEpisodePrefab);
        currentEpisodePrefab = null;

        LeanTween.cancelAll();

        backButton.SetActive(false);
        titleObject.SetActive(true);
        episodeButtonGroupLayout.gameObject.SetActive(true);
    }
}
