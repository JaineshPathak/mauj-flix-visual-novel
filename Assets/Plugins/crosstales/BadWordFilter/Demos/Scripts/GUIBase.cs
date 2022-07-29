using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crosstales.BWF.Demo
{
   /// <summary>Base-class for "GUIMain" and "GUIMainAsync".</summary>
   public abstract class GUIBase : MonoBehaviour
   {
      #region Variables

      [Header("General Settings")] public bool AutoTest = true;
      public bool AutoReplace;
      public bool ReplaceLeet = true;
      public bool SimpleCheck = true;

      [Range(0.5f, 3f)] public float IntervalCheck = 0.8f;

      [Range(0.5f, 5f)] public float IntervalReplace = 2.5f;

      public Color32 GoodColor = new Color32(0, 255, 0, 192);
      public Color32 BadColor = new Color32(255, 0, 0, 192);

      [Header("Managers")] public Crosstales.BWF.Model.Enum.ManagerMask BadwordManager = Crosstales.BWF.Model.Enum.ManagerMask.BadWord;
      public Crosstales.BWF.Model.Enum.ManagerMask DomManager = Crosstales.BWF.Model.Enum.ManagerMask.Domain;
      public Crosstales.BWF.Model.Enum.ManagerMask CapsManager = Crosstales.BWF.Model.Enum.ManagerMask.Capitalization;
      public Crosstales.BWF.Model.Enum.ManagerMask PuncManager = Crosstales.BWF.Model.Enum.ManagerMask.Punctuation;

      public System.Collections.Generic.List<string> Sources = new System.Collections.Generic.List<string>(30);

      [Header("UI Components")] public InputField Text;
      public Text OutputText;

      public Text BadWordList;
      public Text BadWordCounter;

      public Text Name;
      public Text Version;
      public Text Scene;

      public Toggle TestEnabled;
      public Toggle ReplaceEnabled;

      public Toggle Badword;
      public Toggle Domain;
      public Toggle Capitalization;
      public Toggle Punctuation;

      public InputField BadwordReplaceChars;
      public InputField DomainReplaceChars;
      public InputField CapsTrigger;
      public InputField PuncTrigger;

      public Toggle LeetReplace;
      public Toggle SimpleCheckToggle;

      public Image BadWordListImage;


      protected System.Collections.Generic.List<string> badWords = new System.Collections.Generic.List<string>();

      protected float elapsedTimeCheck = 0f;
      protected float elapsedTimeReplace = 0f;

      #endregion


      #region MonoBehaviour methods

      protected virtual void Start()
      {
         if (Name != null)
            Name.text = Crosstales.BWF.Util.Constants.ASSET_NAME;

         if (Version != null)
            Version.text = Crosstales.BWF.Util.Constants.ASSET_VERSION;

         if (Scene != null)
            Scene.text = SceneManager.GetActiveScene().name;

         if (!AutoTest && TestEnabled != null)
            TestEnabled.isOn = false;

         if (!AutoReplace && ReplaceEnabled != null)
            ReplaceEnabled.isOn = false;

         if (BadwordManager != Crosstales.BWF.Model.Enum.ManagerMask.BadWord && Badword != null)
            Badword.isOn = false;

         if (DomManager != Crosstales.BWF.Model.Enum.ManagerMask.Domain && Domain != null)
            Domain.isOn = false;

         if (CapsManager != Crosstales.BWF.Model.Enum.ManagerMask.Capitalization && Capitalization != null)
            Capitalization.isOn = false;

         if (PuncManager != Crosstales.BWF.Model.Enum.ManagerMask.Punctuation && Punctuation != null)
            Punctuation.isOn = false;

         Crosstales.BWF.Manager.BadWordManager.Instance.Mode = ReplaceLeet ? Crosstales.BWF.Model.Enum.ReplaceMode.LeetSpeakAdvanced : Crosstales.BWF.Model.Enum.ReplaceMode.Default;
         if (!ReplaceLeet && LeetReplace != null)
            LeetReplace.isOn = false;

         Crosstales.BWF.Manager.BadWordManager.Instance.SimpleCheck = SimpleCheck;
         if (!SimpleCheck && SimpleCheckToggle != null)
            SimpleCheckToggle.isOn = false;

         if (BadwordReplaceChars != null)
            BadwordReplaceChars.text = Crosstales.BWF.Manager.BadWordManager.Instance.ReplaceChars;

         if (DomainReplaceChars != null)
            DomainReplaceChars.text = Crosstales.BWF.Manager.DomainManager.Instance.ReplaceChars;

         if (CapsTrigger != null)
            CapsTrigger.text = Crosstales.BWF.Manager.CapitalizationManager.Instance.CapitalizationCharsNumber.ToString();

         if (PuncTrigger != null)
            PuncTrigger.text = Crosstales.BWF.Manager.PunctuationManager.Instance.PunctuationCharsNumber.ToString();

         if (BadWordList != null)
            BadWordList.text = badWords.Count > 0 ? string.Empty : "Not tested";

         if (Text != null)
            Text.text = string.Empty;
      }

      #endregion


      #region Public methods

      public abstract void Test();

      public abstract void Replace();

      public void TestChanged(bool val)
      {
         AutoTest = val;
      }

      public void ReplaceChanged(bool val)
      {
         AutoReplace = val;
      }

      public void BadwordChanged(bool val)
      {
         BadwordManager = val ? Crosstales.BWF.Model.Enum.ManagerMask.BadWord : Crosstales.BWF.Model.Enum.ManagerMask.None;
      }

      public void DomainChanged(bool val)
      {
         DomManager = val ? Crosstales.BWF.Model.Enum.ManagerMask.Domain : Crosstales.BWF.Model.Enum.ManagerMask.None;
      }

      public void CapitalizationChanged(bool val)
      {
         CapsManager = val ? Crosstales.BWF.Model.Enum.ManagerMask.Capitalization : Crosstales.BWF.Model.Enum.ManagerMask.None;
      }

      public void PunctuationChanged(bool val)
      {
         PuncManager = val ? Crosstales.BWF.Model.Enum.ManagerMask.Punctuation : Crosstales.BWF.Model.Enum.ManagerMask.None;
      }

      public void LeetChanged(bool val)
      {
         Crosstales.BWF.Manager.BadWordManager.Instance.Mode = val ? Crosstales.BWF.Model.Enum.ReplaceMode.LeetSpeakAdvanced : Crosstales.BWF.Model.Enum.ReplaceMode.Default;
      }

      public void SimpleChanged(bool val)
      {
         Crosstales.BWF.Manager.BadWordManager.Instance.SimpleCheck = val;
      }

      public void FullscreenChanged(bool val)
      {
         Screen.fullScreen = val;
      }

      public void OpenAssetURL()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.BWF.Util.Constants.ASSET_CT_URL);
      }

      public void OpenCTURL()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.BWF.Util.Constants.ASSET_AUTHOR_URL);
      }

      public void Quit()
      {
         if (Application.isEditor)
         {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
         }
         else
         {
            Application.Quit();
         }
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)