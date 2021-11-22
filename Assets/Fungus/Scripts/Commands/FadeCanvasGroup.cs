using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace Fungus
{
    [CommandInfo("UI", "Fade Canvas Groups", "Fade Only Canvas Group in Parent Objects")]
    public class FadeCanvasGroup : Command
    {
        [SerializeField] protected List<CanvasGroup> targetCanvasGroups = new List<CanvasGroup>();

        [Tooltip("Type of tween easing to apply")]
        [SerializeField] protected LeanTweenType tweenType = LeanTweenType.easeOutQuad;

        [Tooltip("Wait until this command completes before continuing execution")]
        [SerializeField] protected BooleanData waitUntilFinished = new BooleanData(true);

        [SerializeField] protected BooleanData targetInteractable = new BooleanData(true);
        [SerializeField] protected BooleanData targetBlocksRaycast = new BooleanData(true);

        [SerializeField] protected FloatData targetAlpha = new FloatData(1f);

        [Tooltip("Time for the tween to complete")]
        [SerializeField] protected FloatData duration = new FloatData(1f);

        protected virtual void ApplyTween()
        {
            for (int i = 0; i < targetCanvasGroups.Count; i++)
            {
                CanvasGroup targetCanvas = targetCanvasGroups[i];
                if (targetCanvas == null)
                    continue;

                targetCanvas.interactable = targetInteractable;
                targetCanvas.blocksRaycasts = targetBlocksRaycast;
                LeanTween.alphaCanvas(targetCanvas, targetAlpha, duration).setEase(tweenType);
            }

            if(waitUntilFinished)
            {
                LeanTween.value(gameObject, 0f, 1f, duration).setOnComplete(OnComplete);
            }
        }

        protected virtual void OnComplete()
        {
            Continue();
        }

        protected virtual string GetSummaryValue()
        {
            return targetAlpha.Value.ToString() + " alpha";
        }

        #region Public members

        public override void OnEnter()
        {
            if (targetCanvasGroups.Count == 0)
            {
                Continue();
                return;
            }

            ApplyTween();

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add an empty slot by default. Saves an unnecessary user click.
            if (targetCanvasGroups.Count == 0)
            {
                targetCanvasGroups.Add(null);
            }
        }

        public override string GetSummary()
        {
            if (targetCanvasGroups.Count == 0)
            {
                return "Error: No targetObjects selected";
            }
            else if (targetCanvasGroups.Count == 1)
            {
                if (targetCanvasGroups[0] == null)
                {
                    return "Error: No targetObjects selected";
                }
                return targetCanvasGroups[0].name + " = " + GetSummaryValue();
            }

            string objectList = "";
            for (int i = 0; i < targetCanvasGroups.Count; i++)
            {
                var go = targetCanvasGroups[i];
                if (go == null)
                {
                    continue;
                }
                if (objectList == "")
                {
                    objectList += go.name;
                }
                else
                {
                    objectList += ", " + go.name;
                }
            }

            return objectList + " = " + GetSummaryValue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetCanvasGroups")
            {
                return true;
            }

            return false;
        }

        public override bool HasReference(Variable variable)
        {
            return waitUntilFinished.booleanRef == variable || duration.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}