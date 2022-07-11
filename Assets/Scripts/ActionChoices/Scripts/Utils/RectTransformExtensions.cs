using UnityEngine;

public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,
    BottomStretch,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}

public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}

public enum ItemAnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}

public static class RectTransformExtensions
{
    public static void SetItemAnchor(this RectTransform source, ItemAnchorPresets allign)
    {
        float width = source.sizeDelta.x;
        float height = source.sizeDelta.y;

        float offsetX = width / 2f;
        float offsetY = height / 2f;

        switch (allign)
        {
            case ItemAnchorPresets.TopLeft:
                source.anchorMin = new Vector2(0, 1);
                source.anchorMax = new Vector2(0, 1);

                offsetY = -offsetY;
                break;

            case ItemAnchorPresets.TopCenter:
                source.anchorMin = new Vector2(0.5f, 1);
                source.anchorMax = new Vector2(0.5f, 1);

                offsetX = 0;
                offsetY = -offsetY;
                break;

            case ItemAnchorPresets.TopRight:
                source.anchorMin = new Vector2(1, 1);
                source.anchorMax = new Vector2(1, 1);

                offsetX = -offsetX;
                offsetY = -offsetY;
                break;

            case ItemAnchorPresets.MiddleLeft:
                source.anchorMin = new Vector2(0, 0.5f);
                source.anchorMax = new Vector2(0, 0.5f);

                offsetY = 0;
                break;

            case ItemAnchorPresets.MiddleCenter:
                source.anchorMin = new Vector2(0.5f, 0.5f);
                source.anchorMax = new Vector2(0.5f, 0.5f);

                offsetX = 0;
                offsetY = 0;
                break;

            case ItemAnchorPresets.MiddleRight:
                source.anchorMin = new Vector2(1, 0.5f);
                source.anchorMax = new Vector2(1, 0.5f);

                offsetX = -offsetX;
                offsetY = 0;
                break;

            case ItemAnchorPresets.BottomLeft:
                source.anchorMin = new Vector2(0, 0);
                source.anchorMax = new Vector2(0, 0);
                break;

            case ItemAnchorPresets.BottomCenter:
                source.anchorMin = new Vector2(0.5f, 0);
                source.anchorMax = new Vector2(0.5f, 0);

                offsetX = 0;
                break;

            case ItemAnchorPresets.BottomRight:
                source.anchorMin = new Vector2(1, 0);
                source.anchorMax = new Vector2(1, 0);

                offsetX = -offsetX;                
                break;
        }

        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);
    }

    public static void SetAnchor(this RectTransform source, AnchorPresets allign, float offsetX = 0, float offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }

        //source.anchorMin = anchorMin;
        //source.anchorMax = anchorMax;
        //source.DOAnchorMin(anchorMin, duration).SetEase(ease);
        //source.DOAnchorMax(anchorMax, duration).SetEase(ease);        

        //return source.DOAnchorPos(new Vector2(offsetX, offsetY), duration).SetEase(ease);
    }

    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {
        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }
}