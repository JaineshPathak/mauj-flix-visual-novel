using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Camera",
                 "Zoom In/Out View",
                 "Zooms Camera based on Perspective or Orthographic View")]
    public class CameraZoom : Command
    {
        [Tooltip("Delay before camera starts zooming")]
        [SerializeField] protected float startDelay = 0;

        [Tooltip("Time for zoom effect to complete")]
        [SerializeField] protected float duration = 1f;

        [Tooltip("Target zoom FOV when camera zooms")]
        [SerializeField] protected float targetZoom = 60f;

        [Tooltip("Wait until the fade has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        [Tooltip("Camera to use for the fade. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;

        [Tooltip("Camera Zoom Tween Type")]
        [SerializeField] protected LeanTweenType zoomTweenType = LeanTweenType.easeInOutQuad;

        public void ZoomInit(float newStartdelay, float newDuration, float newTargetZoom)
        {
            startDelay = newStartdelay;
            duration = newDuration;
            targetZoom = newTargetZoom;
        }

        protected virtual void Start()
        {
            AcquireCamera();
        }

        protected virtual void AcquireCamera()
        {
            if (targetCamera != null)
            {
                return;
            }

            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = GameObject.FindObjectOfType<Camera>();
            }
        }

        public override void OnEnter()
        {
            AcquireCamera();

            if (targetCamera == null)                
            {
                Continue();
                return;
            }

            if( (targetCamera != null) && targetCamera.orthographic)   //Camera is Orthographic
            {
                if(Mathf.Approximately(startDelay, 0))
                {
                    LeanTween.value(targetCamera.orthographicSize, targetZoom, duration).setEase(zoomTweenType).setOnComplete(OnZoomComplete).setOnUpdate((float f) =>
                    {
                        if(targetCamera != null)
                            targetCamera.orthographicSize = f;

#if UNITY_EDITOR
                        if (FungusManager.Instance != null && FungusManager.Instance.CameraManager != null)
                            FungusManager.Instance.CameraManager.CameraIsZooming = true;
#endif
                    });
                }
                else
                {
                    LeanTween.value(targetCamera.orthographicSize, targetZoom, duration).setEase(zoomTweenType).setDelay(startDelay).setOnComplete(OnZoomComplete).setOnUpdate((float f) =>
                    {
                        if(targetCamera != null)
                            targetCamera.orthographicSize = f;
#if UNITY_EDITOR
                        if (FungusManager.Instance != null && FungusManager.Instance.CameraManager != null)
                            FungusManager.Instance.CameraManager.CameraIsZooming = true;
#endif
                    });
                }                
            }
            else //Camera is Perspective
            {
                if(Mathf.Approximately(startDelay, 0))
                {
                    LeanTween.value(targetCamera.fieldOfView, targetZoom, duration).setEase(zoomTweenType).setOnComplete(OnZoomComplete).setOnUpdate((float f) =>
                    {
                        if (targetCamera != null)
                            targetCamera.fieldOfView = f;

#if UNITY_EDITOR
                        if (FungusManager.Instance != null && FungusManager.Instance.CameraManager != null)
                            FungusManager.Instance.CameraManager.CameraIsZooming = true;
#endif
                    });
                }
                else
                {
                    LeanTween.value(targetCamera.fieldOfView, targetZoom, duration).setEase(zoomTweenType).setDelay(startDelay).setOnComplete(OnZoomComplete).setOnUpdate((float f) =>
                    {
                        if (targetCamera != null)
                            targetCamera.fieldOfView = f;

#if UNITY_EDITOR
                        if (FungusManager.Instance != null && FungusManager.Instance.CameraManager != null)
                            FungusManager.Instance.CameraManager.CameraIsZooming = true;
#endif
                    });
                }                
            }

            if (!waitUntilFinished)
                Continue();
        }

        private void OnZoomComplete()
        {
            if (!waitUntilFinished)
                return;

            if (FungusManager.Instance != null && FungusManager.Instance.CameraManager != null)
                FungusManager.Instance.CameraManager.CameraIsZooming = false;

            Continue();
        }

        public override string GetSummary()
        {
            return "Target Zoom: " + targetZoom;
        }

        public override Color GetButtonColor()
        {
            return Color.cyan;
        }
    }
}