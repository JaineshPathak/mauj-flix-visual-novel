using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Crosstales.BWF.Demo
{
   /// <summary>Generates a scrollable list of sources.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_g_u_i_source.html")]
   public class GUISource : MonoBehaviour
   {
      #region Variables

      public GameObject ItemPrefab;
      public GameObject Target;
      public Scrollbar Scroll;
      public GUIBase GuiMain;
      public int ColumnCount = 1;
      public Vector2 SpaceWidth = new Vector2(8, 8);
      public Vector2 SpaceHeight = new Vector2(8, 8);

      #endregion


      #region MonoBehaviour methods

      private void Awake()
      {
         BWFManager.CreateInstance();
      }

      private void Start()
      {
         BWFManager.Instance.OnBWFReady += onBWFReady;
      }

      private void OnDestroy()
      {
         if (BWFManager.Instance != null)
            BWFManager.Instance.OnBWFReady -= onBWFReady;
      }

      #endregion


      #region Private methods

      private IEnumerator buildLanguageList()
      {
         if (ItemPrefab != null && Scroll != null)
         {
            RectTransform rowRectTransform = ItemPrefab.GetComponent<RectTransform>();
            RectTransform containerRectTransform = Target.GetComponent<RectTransform>();

            for (int ii = Target.transform.childCount - 1; ii >= 0; ii--)
            {
               Transform child = Target.transform.GetChild(ii);
               child.SetParent(null);
               Destroy(child.gameObject);
            }

            yield return null;

            System.Collections.Generic.List<Crosstales.BWF.Data.Source> items = BWFManager.Instance.Sources(Crosstales.BWF.Model.Enum.ManagerMask.BadWord);
            //Debug.Log("ITEMS: " + items.Count, this);

            //calculate the width and height of each child item.
            float width = containerRectTransform.rect.width / ColumnCount - (SpaceWidth.x + SpaceWidth.y) * ColumnCount;

            float height = rowRectTransform.rect.height - (SpaceHeight.x + SpaceHeight.y);

            int rowCount = items.Count / ColumnCount;

            if (rowCount > 0 && items.Count % rowCount > 0)
            {
               rowCount++;
            }

            //adjust the height of the container so that it will just barely fit all its children
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            int j = 0;
            for (int ii = 0; ii < items.Count; ii++)
            {
               //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
               if (ii % ColumnCount == 0)
                  j++;

               //create a new item, name it, and set the parent
               GameObject newItem = Instantiate(ItemPrefab, Target.transform, true);
               newItem.name = $"{Target.name} item at ({ii},{j})";
               newItem.transform.localScale = Vector3.one;

               newItem.GetComponent<SourceEntry>().Source = items[ii];
               newItem.GetComponent<SourceEntry>().GuiMain = GuiMain;


               //move and size the new item
               RectTransform rectTransform = newItem.GetComponent<RectTransform>();

               Rect rect = containerRectTransform.rect;
               float x = -rect.width / 2 + (width + SpaceWidth.x) * (ii % ColumnCount) + SpaceWidth.x * ColumnCount;
               float y = rect.height / 2 - (height + SpaceHeight.y) * j;

               Vector2 offsetMin = new Vector2(x, y);
               rectTransform.offsetMin = offsetMin;

               x = offsetMin.x + width;
               y = offsetMin.y + height;
               rectTransform.offsetMax = new Vector2(x, y);
            }

            Scroll.value = 1f;
         }
      }

      #endregion


      #region Callbacks

      private void onBWFReady()
      {
         StartCoroutine(buildLanguageList());
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)