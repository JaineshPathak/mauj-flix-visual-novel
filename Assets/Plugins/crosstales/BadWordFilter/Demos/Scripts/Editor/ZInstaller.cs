#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.BWF.Demo
{
   /// <summary>Installs the packages from Common and OnRadio.</summary>
   [InitializeOnLoad]
   public abstract class ZInstaller : Crosstales.Common.EditorTask.BaseInstaller
   {
      #region Constructor

      static ZInstaller()
      {
         string path = $"{Application.dataPath}{Crosstales.BWF.EditorUtil.EditorConfig.ASSET_PATH}";

#if !CT_UI && !CT_DEVELOP
         InstallUI(path);
#endif

#if !CT_BWF_DEMO && !CT_DEVELOP
         Crosstales.Common.EditorTask.BaseCompileDefines.AddSymbolsToAllTargets("CT_BWF_DEMO");
#endif
      }

      #endregion
   }
}
#endif
// © 2020-2022 crosstales LLC (https://www.crosstales.com)