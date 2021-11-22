using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class WriterUpdater : MonoBehaviour
{
    private enum HindiCorrectorType
    {
        Type_VassCreatick,
        Type_ClumsyDev,
        Type_Siddhanta
    };

    [SerializeField] private HindiCorrectorType languageCorrectorType;
    [SerializeField] private float waitDelay = 0.05f;

    [Header("Type - VassCreatick")]
    public CharReplacerHindi charReplacerHindi;

    [Header("Type - ClumsyDev")]
    public TextMeshProUGUI textObject;
    public TextMeshProUGUI characterNameTextObject;

    [Header("Type - Siddhanta")]
    public Text characterNameText;
    public Text storyTextObject;

    [Header("Hand Icon")]
    public bool playHandIcon;
    public float handIconDelay = 2f;
    public CanvasGroup handIconPanel;

    private int handSeqId;
    //private LTSeq handSeq;    

    void OnEnable()
    {
        // Register as listener for Writer state change events
        WriterSignals.OnWriterState += OnWriterState;        

        /*if (playHandIcon)
        {
            handSeq = LeanTween.sequence();
            handSeqId = handSeq.id;
        }*/
    }

    void OnDisable()
    {
        // Unregister as listener for Writer state change events
        WriterSignals.OnWriterState -= OnWriterState;        
    }

    void OnWriterState(Writer writer, WriterState writerState)
    {
        if (writerState == WriterState.Start)
        {
            //Debug.Log("Writing started");
            //Invoke("CharReplacerDelayed", 0.1f);
            StartCoroutine(CharReplacerDelayedRoutine());

            if(playHandIcon && handIconPanel != null)
            {
                handIconPanel.alpha = 0;

                if (LeanTween.isTweening(handSeqId))
                    LeanTween.cancel(handSeqId);

                handSeqId = LeanTween.alphaCanvas(handIconPanel, 1f, 0.5f).setDelay(handIconDelay).id;

                /*handSeq = LeanTween.sequence();
                handSeqId = handSeq.id;

                handSeq.append( () => 
                {
                    LeanTween.alphaCanvas(handIconPanel, 1f, 0.5f).setDelay(handIconDelay);
                });*/
            }
        }
    }

    private IEnumerator CharReplacerDelayedRoutine()
    {
        yield return new WaitForSeconds(waitDelay);

        CharReplacerDelayed();
    }

    private void CharReplacerDelayed()
    {
        switch (languageCorrectorType)
        {
            case HindiCorrectorType.Type_VassCreatick:

                charReplacerHindi.UpdateMe();

                break;

            case HindiCorrectorType.Type_ClumsyDev:

                textObject.SetHindiTMPro(textObject.text);

                //if (characterNameTextObject != null)
                    //characterNameTextObject.SetHindiTMPro(characterNameTextObject.text);

                break;

            case HindiCorrectorType.Type_Siddhanta:

                //if(storyTextObject != null)
                    //storyTextObject.text = HindiCorrector2.Correct(storyTextObject.text);

                if (characterNameText != null)
                    characterNameText.text = HindiCorrector2.Correct(characterNameText.text);

                break;
        }
    }
}