using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class MflixPickName : MonoBehaviour
{
    public InputField nameInputField;
    public Button submitNameButton;
    public CharacterGender characterGender;

    [HideInInspector] public Flowchart episodeFlowchart;
    [HideInInspector] public StringVariable nameVariableRef;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (submitNameButton)
            submitNameButton.onClick.AddListener(OnNameSubmit);

        canvasGroup = GetComponent<CanvasGroup>();
    }

    /*private void Start()
    {
        if(episodeFlowchart == null)
            episodeFlowchart = FindObjectOfType<Flowchart>();
    }*/

    private void OnNameSubmit()
    {
        if (EpisodesSpawner.instance != null && EpisodesSpawner.instance.storiesDBItem != null)
            if (!EpisodesSpawner.instance.storiesDBItem.isReworked)
                return;

        if (episodeFlowchart == null)
            episodeFlowchart = FindObjectOfType<EpisodesHandler>().episodeFlowchart;

        if (episodeFlowchart && nameInputField)
        {
            //Empty names not allowed
            if (nameInputField.text.Length <= 0)
                return;

            MessageReceived[] messagesReceived = episodeFlowchart.GetComponentsInChildren<MessageReceived>();

            //if(nameVariableRef.variable && nameVariableRef.variable.GetType() == typeof(StringVariable))
            //nameVariableRef.variable.Apply(SetOperator.Assign, nameInputField.text);

            if(nameVariableRef)
                nameVariableRef.Value = nameInputField.text;

            switch (characterGender)
            {
                case CharacterGender.Gender_Male:

                    for (int i = 0; i < messagesReceived.Length && (messagesReceived.Length > 0); i++)
                    {
                        if (messagesReceived[i] != null)
                        {
                            messagesReceived[i].OnSendFungusMessage("CharacterNameSubmitMale");

                            //Message sent time to disappear
                            DisengageNameDialogue();
                        }
                    }

                    break;

                case CharacterGender.Gender_Female:

                    for (int i = 0; i < messagesReceived.Length && (messagesReceived.Length > 0); i++)
                    {
                        if (messagesReceived[i] != null)
                        {
                            messagesReceived[i].OnSendFungusMessage("CharacterNameSubmitFemale");

                            //Message sent time to disappear
                            DisengageNameDialogue();
                        }
                    }

                    break;
            }
        }
    }

    private void DisengageNameDialogue()
    {
        nameInputField.interactable = false;
        submitNameButton.interactable = false;

        LeanTween.alphaCanvas(canvasGroup, 0, 0.5f).setEaseInOutSine().setOnComplete(() => gameObject.SetActive(false) );
    }
}
