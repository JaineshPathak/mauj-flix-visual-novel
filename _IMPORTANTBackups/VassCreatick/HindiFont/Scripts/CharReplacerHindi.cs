using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class CharReplacerHindi : MonoBehaviour
{

    List<CharacterAttributesHindi> _CharacterAttributes = new List<CharacterAttributesHindi>();
    private TMP_Text _Text;
    private TMP_InputField _InputField;
    private TMP_Dropdown _DropDown;
    private string FontFile = "HindiReplacement";
    string Value = "";
    public string Convertedvalue = "";
    public string OriginalText = "";

    void Start()
    {
        LoadAttributes();
        _Text = GetComponent<TMP_Text>();
        _InputField = GetComponent<TMP_InputField>();
        _DropDown = GetComponent<TMP_Dropdown>();
        UpdateMe();
    }

    void OnEnable()
    {
        if (_Text == null) _Text = GetComponent<TMP_Text>();
        if (_InputField == null) _InputField = GetComponent<TMP_InputField>();
        if (_DropDown == null) _DropDown = GetComponent<TMP_Dropdown>();
    }

    void LoadAttributes()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(FontFile);
        if (textAsset != null)
        {
            _CharacterAttributes = JsonConvert.DeserializeObject<List<CharacterAttributesHindi>>(textAsset.text);

        }
    }
    /// <summary>
    /// Updates the dynamic text and converts broken letters from the dynamic texts either from local json file or from server.
    /// </summary>
   
    public void UpdateTextRuntime(string text)
    {
        if (_Text != null)
        {
             _Text.text= text;
           
        }
        else if (_InputField != null)
        {
            _InputField.text= text;
        }
        else if (_DropDown != null)
        {
            _DropDown.captionText.text= text;
        }
        UpdateMe();
    }

    public string GetFixedText(string _text)
    {
        string Value = _text;
        string AppendString = "";
        string AppendString2 = "";        

        for (int i = 0; i < _CharacterAttributes.Count; i++)
        {
            if (_CharacterAttributes.Count > 0)
            {

                if (_CharacterAttributes[i].CharacterHexValue == "")
                {

                    for (int j = 0; j < _CharacterAttributes[i].CharacterCombinations.Count; j++)
                    {


                        if (Value.Contains(_CharacterAttributes[i].CharacterCombinations[j]))
                        {

                            if (_CharacterAttributes[i].Append == true)
                            {

                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], _CharacterAttributes[i].CharName + AppendString);

                            }
                            else if (_CharacterAttributes[i].AppendBefore == true)
                            {
                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString + _CharacterAttributes[i].CharName);


                            }
                            else if (_CharacterAttributes[i].AppendFirst == true)
                            {

                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                if (_CharacterAttributes[i].CharName == "006B")
                                {
                                    int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                    AppendString2 = Convert.ToChar(appendValue2).ToString();
                                    Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString + AppendString2);

                                }
                                else
                                {
                                    int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                    AppendString2 = Convert.ToChar(appendValue2).ToString();


                                    if (_CharacterAttributes[i].Character == "ndh" ||
                                        _CharacterAttributes[i].Character == "ravo"
                                          || _CharacterAttributes[i].Character == "ravi" ||
                                          _CharacterAttributes[i].Character == "rathi" ||
                                          _CharacterAttributes[i].Character == "shki" ||
                                          _CharacterAttributes[i].Character == "kriy" ||
                                          _CharacterAttributes[i].Character == "iksii"
                                           || _CharacterAttributes[i].Character == "dhrii" || _CharacterAttributes[i].Character == "sthree")
                                    {
                                        int appenddot = Convert.ToInt32(_CharacterAttributes[i].CharRetain2, 16);
                                        string dot = Convert.ToChar(appenddot).ToString();
                                        Debug.Log("dot " + dot);
                                        if (_CharacterAttributes[i].Character == "iksii")
                                        {
                                            Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + _CharacterAttributes[i].CharRetain + dot);
                                        }
                                        else
                                        {
                                            Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + dot + _CharacterAttributes[i].CharRetain);
                                        }
                                    }
                                    else
                                    {

                                        Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + _CharacterAttributes[i].CharRetain);

                                    }

                                }
                            }
                            else if (_CharacterAttributes[i].AppendStart == true)
                            {
                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                AppendString2 = Convert.ToChar(appendValue2).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + _CharacterAttributes[i].CharRetain + AppendString);
                            }


                            else
                            {
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], _CharacterAttributes[i].CharName);
                            }

                        }
                    }
                }
                else
                {

                    int decValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue, 16);
                    string Converted = Convert.ToChar(decValue).ToString();
                    for (int j = 0; j < _CharacterAttributes[i].CharacterCombinations.Count; j++)
                    {
                        Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], @Converted);
                    }
                }
            }

        }

        return Value;
    }

   
    public void UpdateMe()
    {
        string Value = "";
        string AppendString = "";
        string AppendString2 = "";
        if (_Text != null)
        {
            OriginalText = _Text.text;
            Value = _Text.text;
        }
        else if (_InputField != null)
        {
            Value = _InputField.text;
        }
        else if (_DropDown != null)
        {
            Value = _DropDown.captionText.text;
        }

        for (int i = 0; i < _CharacterAttributes.Count; i++)
        {
            if (_CharacterAttributes.Count > 0)
            {

                if (_CharacterAttributes[i].CharacterHexValue == "")
                {

                    for (int j = 0; j < _CharacterAttributes[i].CharacterCombinations.Count; j++)
                    {


                        if (Value.Contains(_CharacterAttributes[i].CharacterCombinations[j]))
                        {

                            if (_CharacterAttributes[i].Append == true)
                            {

                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], _CharacterAttributes[i].CharName + AppendString);

                            }
                            else if (_CharacterAttributes[i].AppendBefore == true)
                            {
                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString + _CharacterAttributes[i].CharName);


                            }
                            else if (_CharacterAttributes[i].AppendFirst == true)
                            {

                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                if (_CharacterAttributes[i].CharName == "006B")
                                {
                                    int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                    AppendString2 = Convert.ToChar(appendValue2).ToString();
                                    Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString + AppendString2);

                                }
                                else
                                {
                                    int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                    AppendString2 = Convert.ToChar(appendValue2).ToString();


                                    if (_CharacterAttributes[i].Character == "ndh" ||
                                        _CharacterAttributes[i].Character == "ravo"
                                          || _CharacterAttributes[i].Character == "ravi" || 
                                          _CharacterAttributes[i].Character == "rathi" || 
                                          _CharacterAttributes[i].Character == "shki" || 
                                          _CharacterAttributes[i].Character == "kriy" || 
                                          _CharacterAttributes[i].Character == "iksii"
                                           || _CharacterAttributes[i].Character == "dhrii" || _CharacterAttributes[i].Character == "sthree")
                                    {
                                        int appenddot = Convert.ToInt32(_CharacterAttributes[i].CharRetain2, 16);
                                        string dot = Convert.ToChar(appenddot).ToString();
                                        Debug.Log("dot " + dot);
                                        if (_CharacterAttributes[i].Character == "iksii")
                                        {
                                            Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + _CharacterAttributes[i].CharRetain + dot);
                                        }
                                        else
                                        {
                                            Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + dot + _CharacterAttributes[i].CharRetain);
                                        }
                                    }
                                    else
                                    {

                                        Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + AppendString + _CharacterAttributes[i].CharRetain);

                                    }

                                }
                            }
                            else if (_CharacterAttributes[i].AppendStart == true)
                            {
                                int appendValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue2, 16);
                                AppendString = Convert.ToChar(appendValue).ToString();
                                int appendValue2 = Convert.ToInt32(_CharacterAttributes[i].CharName, 16);
                                AppendString2 = Convert.ToChar(appendValue2).ToString();
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], AppendString2 + _CharacterAttributes[i].CharRetain + AppendString);
                            }


                            else
                            {
                                Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], _CharacterAttributes[i].CharName);
                            }

                        }
                    }
                }
                else
                {

                    int decValue = Convert.ToInt32(_CharacterAttributes[i].CharacterHexValue, 16);
                    string Converted = Convert.ToChar(decValue).ToString();
                    for (int j = 0; j < _CharacterAttributes[i].CharacterCombinations.Count; j++)
                    {
                        Value = Value.Replace(_CharacterAttributes[i].CharacterCombinations[j], @Converted);
                    }
                }
            }

        }
        if (_Text != null)
        {
            _Text.text = Value;
            Convertedvalue = Value;
        }
        if (_InputField != null)
        {
            _InputField.text = Value;
            Convertedvalue = Value;
        }
        if (_DropDown != null)
        {
            _DropDown.captionText.text = Value;
            Convertedvalue = Value;
        }

    }

}

[Serializable]
public class CharacterAttributesHindi
{
    public string Character;
    public bool Append;
    public bool AppendBefore;
    public bool AppendFirst;
    public bool AppendStart;
    public List<string> CharacterCombinations;
    public string CharacterHexValue;
    public string CharacterHexValue2;
    public string CharRetain;
    public string CharRetain2;
    public string CharName;
}



