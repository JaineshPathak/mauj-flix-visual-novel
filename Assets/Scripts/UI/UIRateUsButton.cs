using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRateUsButton : MonoBehaviour
{
    public RectTransform homeButton;

    private void Start()
    {
        if(EpisodesSpawner.instance != null)
        {
            if(EpisodesSpawner.instance.playerData != null)
            {
                if(EpisodesSpawner.instance.playerData.hasRatedGame)
                {
                    //homeButton.anchoredPosition = new Vector2(0, homeButton.anchoredPosition.y);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnRateUsClicked()
    {
        if (EpisodesSpawner.instance == null)
            return;

        EpisodesSpawner.instance.OnRateUsClicked();
    }
}
