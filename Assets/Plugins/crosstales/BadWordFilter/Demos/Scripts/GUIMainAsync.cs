using UnityEngine;

namespace Crosstales.BWF.Demo
{
   /// <summary>Main GUI controller for async calls.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_g_u_i_main_async.html")]
   public class GUIMainAsync : GUIBase
   {
      #region MonoBehaviour methods

      protected override void Start()
      {
         base.Start();

         BWFManager.Instance.OnGetAllComplete += onGetAllComplete;
         BWFManager.Instance.OnReplaceAllComplete += onReplaceAllComplete;
      }

      private void OnDestroy()
      {
         if (BWFManager.Instance != null)
         {
            BWFManager.Instance.OnGetAllComplete -= onGetAllComplete;
            BWFManager.Instance.OnReplaceAllComplete -= onReplaceAllComplete;
         }
      }

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

         if (Time.frameCount % 15 == 0)
         {
            Crosstales.BWF.Manager.BadWordManager.Instance.ReplaceChars = BadwordReplaceChars.text;

            Crosstales.BWF.Manager.DomainManager.Instance.ReplaceChars = DomainReplaceChars.text;

            bool res = int.TryParse(CapsTrigger.text, out int number);
            Crosstales.BWF.Manager.CapitalizationManager.Instance.CapitalizationCharsNumber = res ? number > 1 ? number : 1 : 1;
            CapsTrigger.text = Crosstales.BWF.Manager.CapitalizationManager.Instance.CapitalizationCharsNumber.ToString();

            res = int.TryParse(PuncTrigger.text, out number);
            Crosstales.BWF.Manager.PunctuationManager.Instance.PunctuationCharsNumber = res ? number > 1 ? number : 1 : 1;
            PuncTrigger.text = Crosstales.BWF.Manager.PunctuationManager.Instance.PunctuationCharsNumber.ToString();
         }
      }

      #endregion


      #region Public methods

      public override void Test()
      {
         BWFManager.Instance.GetAllAsync(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());
      }

      public override void Replace()
      {
         BWFManager.Instance.ReplaceAllAsync(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());
      }

      #endregion


      #region Private methods

      private void updateUI()
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

         BadWordCounter.text = badWords.Count.ToString();

         OutputText.text = BWFManager.Instance.Mark(Text.text, badWords);
      }

      #endregion


      #region Callbacks

      private void onReplaceAllComplete(string originaltext, string cleantext)
      {
         Text.text = cleantext;

         badWords.Clear();

         updateUI();
      }

      private void onGetAllComplete(string originaltext, System.Collections.Generic.List<string> badwords)
      {
         badWords = badwords;

         updateUI();
      }

      #endregion
   }
}
// © 2020-2022 crosstales LLC (https://www.crosstales.com)