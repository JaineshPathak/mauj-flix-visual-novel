using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fungus;

public class EpisodesLoadButton : MonoBehaviour
{
    public TextMeshProUGUI episodeText;

    [HideInInspector] public EpisodesTesting episodesTesting;
    [HideInInspector] public GameObject episodePrefab;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void LoadEpisode()
    {
        if (episodePrefab == null)
            return;        

        mainCamera.transform.position = Vector3.zero;
        mainCamera.transform.rotation = Quaternion.identity;

        /*GameObject episodeInstance = Instantiate(episodePrefab);
        episodeInstance.GetComponentInChildren<Flowchart>().ExecuteBlock("01 - Game Start");

        episodesTesting.currentEpisodePrefab = episodeInstance;
        episodesTesting.episodeButtonGroupLayout.gameObject.SetActive(false);
        episodesTesting.titleObject.SetActive(false);
        episodesTesting.backButton.SetActive(true);*/

        episodesTesting.SpawnEpisode(episodePrefab);
    }
}