using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIDiamondsPool : MonoBehaviour
{
    [Header("Prefabs")]
    public int diamondsCount = 20;
    public Transform diamondsSmallPrefab;
    public Transform diamondsSmallParent;
    public List<Transform> diamondsList = new List<Transform>();
    public List<TextMeshProUGUI> diamondDebitTextList = new List<TextMeshProUGUI>();

    [Header("Middle Paths")]
    //public Transform[] middleTransformPoints;
    public List<Vector3> diamondsPathLeft = new List<Vector3>();
    public List<Vector3> diamondsPathMiddle = new List<Vector3>();
    public List<Vector3> diamondsPathRight = new List<Vector3>();

    private UITopPanel topPanel;

    private Camera mainCamera;

    private void Awake()
    {
        if (topPanel == null)
            topPanel = GetComponent<UITopPanel>();
    }

#if UNITY_EDITOR
    public void SpawnSmallDiamonds()
    {
        if (diamondsList.Count > 0 || diamondsSmallPrefab == null)
            return;

        for (int i = 0; i < diamondsCount; i++)
        {
            //Transform coinsSmallInstance = Instantiate(coinsSmallPrefab, coinsSmallParent);
            Transform diamondsSmallInstance = PrefabUtility.InstantiatePrefab(diamondsSmallPrefab, diamondsSmallParent) as Transform;
            diamondsSmallInstance.gameObject.SetActive(false);

            diamondsList.Add(diamondsSmallInstance);
            diamondDebitTextList.Add(diamondsSmallInstance.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    public void ClearSmallDiamonds()
    {
        if (diamondsList.Count <= 0)
            return;

        while (diamondsSmallParent.childCount > 0)
            DestroyImmediate(diamondsSmallParent.GetChild(0).gameObject);

        diamondsList.Clear();
    }
#endif    

    private Vector2 GetRandomPositionInsideCircle(Transform start, float spreadRadius)
    {
        Vector2 vector2 = new Vector2(start.position.x, start.position.y) + UnityEngine.Random.insideUnitCircle.normalized * spreadRadius;
        return vector2;
    }

    #region Removed Codes
    /*private Vector3[] GetDiamondPath(Transform diamond, Transform end)
    {
        List<Vector3> newPath = new List<Vector3>();

        //newPath.Add(coin.position);
        newPath.Add(GetRandomMiddlePoint(diamond));
        newPath.Add(end.position);

        return newPath.ToArray();
    }

    private Vector3 GetRandomMiddlePoint(Transform diamond)
    {
        Vector3 middlePoint = Vector3.zero;

        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 viewportPoint = mainCamera.ScreenToViewportPoint(diamond.position);

        if (viewportPoint.x >= 0.5f)
            middlePoint = middleTransformPoints[2].position;
        else if (viewportPoint.x < 0.5f)
            middlePoint = middleTransformPoints[0].position;

        //return middlePathsList[Random.Range(0, middlePathsList.Length)].position;
        return middlePoint;
    }

    private void GetDiamondsPath(Transform start, Transform end)
    {
        if (start == null || end == null)
            return;

        if (middleTransformPoints.Length <= 0)
            return;

        diamondsPathLeft.Clear();
        diamondsPathMiddle.Clear();
        diamondsPathRight.Clear();

        diamondsPathLeft.Add(start.position);
        diamondsPathMiddle.Add(start.position);
        diamondsPathRight.Add(start.position);

        diamondsPathLeft.Add(middleTransformPoints[0].position);
        diamondsPathMiddle.Add(middleTransformPoints[1].position);
        diamondsPathRight.Add(middleTransformPoints[2].position);

        diamondsPathLeft.Add(end.position);
        diamondsPathMiddle.Add(end.position);
        diamondsPathRight.Add(end.position);
    }*/
    #endregion

    //============================================================================================================
    
    //When player gets Diamond rewards
    public void PlayDiamondsAnimationDeposit(Transform start, Transform end, int totalDiamonds, Action callback, float spreadRadius = 300f)
    {
        StartCoroutine(PlayDiamondsAnimationRoutineDeposit(start, end, totalDiamonds, callback, spreadRadius));
    }

    private IEnumerator PlayDiamondsAnimationRoutineDeposit(Transform start, Transform end, int totalDiamonds, Action callback, float spreadRadius)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < totalDiamonds && (diamondsList.Count > 0); i++)
        {
            MoveDiamond(diamondsList[i], false, diamondDebitTextList[i], start, end, totalDiamonds, spreadRadius);

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        callback?.Invoke();
    }

    //============================================================================================================

    //When player buys something using Diamonds
    public void PlayDiamondsAnimationDebit(Transform start, Transform end, int totalDiamonds, Action callback, float spreadRadius = 300f, Color? textColor = null)
    {
        if(FirebaseFirestoreOffline.instance != null)
        {
            if (FirebaseFirestoreOffline.instance.GetDiamondsAmountInt() < totalDiamonds)
                return;
        }

        StartCoroutine(PlayDiamondsAnimationDebitRoutine(start, end, totalDiamonds, callback, spreadRadius, textColor));
    }

    private IEnumerator PlayDiamondsAnimationDebitRoutine(Transform start, Transform end, int totalDiamonds, Action callback, float spreadRadius, Color? textColor = null)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < totalDiamonds && (diamondsList.Count > 0); i++)
        {
            MoveDiamond(diamondsList[i], true, diamondDebitTextList[i], start, end, totalDiamonds, spreadRadius, textColor);

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        callback?.Invoke();
    }

    //============================================================================================================

    private void MoveDiamond(Transform diamond, bool debitMode, TextMeshProUGUI diamondDebitText, Transform start, Transform end, int totalDiamonds, float spreadRadius, Color? textColor = null)
    {
        if (!diamond.gameObject.activeSelf)
            diamond.gameObject.SetActive(true);

        diamondDebitText.gameObject.SetActive(debitMode);
        diamondDebitText.color = textColor ?? Color.white;

        float perDiamondAmount = totalDiamonds / totalDiamonds;
        if (debitMode)
            diamondDebitText.text = "-" + Mathf.RoundToInt(perDiamondAmount).ToString();

        diamond.position = start.position;

        LeanTween.move(diamond.gameObject, GetRandomPositionInsideCircle(start, spreadRadius), 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
        {
            //LTBezierPath diamondPath = new LTBezierPath(GetDiamondPath(diamond, end));            
            LeanTween.move(diamond.gameObject, end.position, 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
            {
                if (FirebaseFirestoreOffline.instance != null)
                {
                    if(!debitMode)
                        FirebaseFirestoreOffline.instance.DepositDiamondsAmount(perDiamondAmount);
                    else
                        FirebaseFirestoreOffline.instance.DebitDiamondsAmount(perDiamondAmount);
                }

                if (MMVibrationManager.HapticsSupported())
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);

                if (topPanel != null)
                    topPanel.PlayDiamondIconCollectAnim(1f, 1.8f, !debitMode);

                diamond.gameObject.SetActive(false);
            });
        });
    }

    //============================================================================================================
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIDiamondsPool))]
public class UIDiamondsPoolEditor : Editor
{
    private float thumbnailWidth = 70f;
    private float thumbnailHeight = 70f;

    public override void OnInspectorGUI()
    {
        UIDiamondsPool pool = target as UIDiamondsPool;

        DrawDefaultInspector();

        EditorGUILayout.Space(15f);                

        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(EditorGUIUtility.labelWidth);
            if (GUILayout.Button(Resources.Load<Texture>("Icons/Icon_Diamond_Editor_Spawn"), GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight)))
            {
                Undo.RecordObject(target, "Spawn Diamonds");
                pool.SpawnSmallDiamonds();
            }

            if (GUILayout.Button(Resources.Load<Texture>("Icons/Icon_Diamond_Editor_Clear"), GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight)))
            {
                Undo.RecordObject(target, "Clear Diamonds");
                pool.ClearSmallDiamonds();
            }
        }

        GUILayout.EndHorizontal();
    }
}
#endif
