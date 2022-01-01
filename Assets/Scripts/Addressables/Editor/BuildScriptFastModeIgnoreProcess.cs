using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Build;

namespace UnityEditor.AddressableAssets.Build.DataBuilders
{
    [CreateAssetMenu(
        fileName = "BuildScriptFastModeIgnoreProcess.asset",
        menuName = "Addressables/Content Builders/Use Asset Database (Ignore Process)")]

    public class BuildScriptFastModeIgnoreProcess : BuildScriptFastMode
    {
        public override string Name => "Use Asset Database (faster, Ignore Process)";

        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput context)
        {
            TResult result = default(TResult);
            return result;
        }
    }
}