using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.BWF.Demo
{
   /// <summary>Wrapper for sources.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_source_entry.html")]
   public class SourceEntry : MonoBehaviour
   {
      #region Variables

      public Text Text;
      public Image Icon;
      public Image Main;

      public Crosstales.BWF.Data.Source Source;

      public GUIBase GuiMain;

      public Color32 EnabledColor = new Color32(0, 255, 0, 192);
      private Color32 disabledColor;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         disabledColor = Main.color;
      }

      private void Update()
      {
         Text.text = Source.SourceName;
         Icon.sprite = Source.Icon;

         Main.color = GuiMain.Sources.Contains(Source.SourceName) ? EnabledColor : disabledColor;
      }

      #endregion


      #region Public methods

      public void Click()
      {
         if (GuiMain.Sources.Contains(Source.SourceName))
         {
            GuiMain.Sources.Remove(Source.SourceName);
         }
         else
         {
            GuiMain.Sources.Add(Source.SourceName);
         }
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)