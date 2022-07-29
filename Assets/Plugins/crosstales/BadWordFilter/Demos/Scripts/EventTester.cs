using UnityEngine;

namespace Crosstales.BWF.Demo
{
   /// <summary>Simple test script for all UnityEvent-callbacks.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_event_tester.html")]
   public class EventTester : MonoBehaviour
   {
      public void OnReady()
      {
         Debug.Log("OnReady");
      }

      public void OnContainsCompleted(string text, bool containsBadwords)
      {
         Debug.Log($"OnContainsCompleted: {text} - {containsBadwords}");
      }

      public void OnGetAllCompleted(string text, string allBadwords)
      {
         Debug.Log($"OnGetAllCompleted: {text} - {allBadwords}");
      }

      public void OnReplaceAllCompleted(string originalText, string cleanText)
      {
         Debug.Log($"OnReplaceAllCompleted: {originalText} - {cleanText}");
      }
   }
}
// © 2020-2022 crosstales LLC (https://www.crosstales.com)