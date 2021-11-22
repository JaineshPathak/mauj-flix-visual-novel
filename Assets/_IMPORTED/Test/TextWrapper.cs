using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWrapper : MonoBehaviour 
{
	public string text;

	void Start () 
	{
		Text textField = GetComponent<Text>();
		if (textField != null) 
		{
			textField.text = HindiCorrector2.Correct(textField.text);
		}

		/*TextMeshProUGUI tm = GetComponent<TextMeshProUGUI>();
		if (tm != null)
			tm.text = HindiCorrector2.Correct(text);*/
	}
}
