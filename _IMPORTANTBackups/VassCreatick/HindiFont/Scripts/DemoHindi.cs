using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoHindi : MonoBehaviour
{
    // Start is called before the first frame update
    public CharReplacerHindi HindiInput =new CharReplacerHindi();
    public TMP_Text Hindi;
    public CharReplacerHindi _Text = new CharReplacerHindi(); //Use this for Getting text from One text to another
    public TMP_Text GetTexts;  //Display the text from _Text to this text;
    void Start()
    {
        GetOriginalText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Getting Text from InputField to Text
    public void GetInputText()
    {
        Hindi.text= HindiInput.Convertedvalue;
    }

    public void GetOriginalText()
    {
        _Text.UpdateMe();
        string originaltext = _Text.OriginalText;
        Debug.Log("Original " + originaltext);

    }

    //Getting Text From One Text(Tmp) to Another Text (Tmp)

    public void GetTextToAnother()
    {
        _Text.UpdateMe();
        GetTexts.text = _Text.Convertedvalue;
    }

    //Update your text from server or from local file or json file
    public void updatetextinruntime()
    {
        _Text.UpdateTextRuntime("भक्त");


    }
}
