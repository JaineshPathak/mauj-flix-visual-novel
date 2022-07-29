using UnityEngine;

namespace Crosstales.BWF.Demo
{
   /// <summary>Main GUI controller.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_g_u_i_main.html")]
   public class GUIMain : GUIBase
   {
      #region Variables

      private bool tested;

      #endregion


      #region MonoBehaviour methods

      private void Update()
      {
         elapsedTimeCheck += Time.deltaTime;
         elapsedTimeReplace += Time.deltaTime;

         if (AutoTest && !AutoReplace && elapsedTimeCheck > IntervalCheck)
         {
            Test();

            elapsedTimeCheck = 0f;
         }

         if (AutoReplace && elapsedTimeReplace > IntervalReplace)
         {
            Replace();

            elapsedTimeReplace = 0f;
         }

         Crosstales.BWF.Manager.BadWordManager.Instance.ReplaceChars = BadwordReplaceChars.text;

         Crosstales.BWF.Manager.DomainManager.Instance.ReplaceChars = DomainReplaceChars.text;

         if (tested)
         {
            if (badWords.Count > 0)
            {
               BadWordList.text = string.Join(System.Environment.NewLine, badWords.ToArray());
               BadWordListImage.color = BadColor;
            }
            else
            {
               BadWordList.text = "No bad words found";
               BadWordListImage.color = GoodColor;
            }

            tested = false;
         }

         if (Time.frameCount % 15 == 0)
         {
            bool res = int.TryParse(CapsTrigger.text, out int number);
            Crosstales.BWF.Manager.CapitalizationManager.Instance.CapitalizationCharsNumber = res ? number > 1 ? number : 1 : 1;
            CapsTrigger.text = Crosstales.BWF.Manager.CapitalizationManager.Instance.CapitalizationCharsNumber.ToString();

            res = int.TryParse(PuncTrigger.text, out number);
            Crosstales.BWF.Manager.PunctuationManager.Instance.PunctuationCharsNumber = res ? number > 1 ? number : 1 : 1;
            PuncTrigger.text = Crosstales.BWF.Manager.PunctuationManager.Instance.PunctuationCharsNumber.ToString();

            BadWordCounter.text = badWords.Count.ToString();

            OutputText.text = BWFManager.Instance.Mark(Text.text, badWords);
         }
      }

      #endregion


      #region Public methods

      public override void Test()
      {
         tested = true;

         badWords = BWFManager.Instance.GetAll(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());
      }

      public override void Replace()
      {
         tested = true;

         Text.text = BWFManager.Instance.ReplaceAll(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());

         badWords.Clear();
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)