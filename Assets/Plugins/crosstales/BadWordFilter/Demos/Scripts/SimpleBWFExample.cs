using UnityEngine;
using UnityEngine.UI;
using Crosstales.BWF;

/// <summary>
/// Simple example to demonstrate the basic usage of BWF.
/// </summary>
public class SimpleBWFExample : MonoBehaviour
{
   public InputField Input;
   public Text Output;

   private void Start()
   {
      Output.text = "Not tested!";
   }

   public void Replace()
   {
      Output.text = string.IsNullOrEmpty(Input.text) ? "<color=red>No text to test!</color>" : BWFManager.Instance.ReplaceAll(Input.text);
   }
}
// © 2022 crosstales LLC (https://www.crosstales.com)