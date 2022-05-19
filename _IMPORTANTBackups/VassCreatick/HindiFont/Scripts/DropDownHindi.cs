using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropDownHindi : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text Hinditext;
    public CharReplacerHindi _HindiDropdown = new CharReplacerHindi();
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetHindiText()
    {
        Hinditext.text = _HindiDropdown.Convertedvalue;
    }

   
}
