using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;
public static class Extension
{
    public static List<T> Shuffle<T>(this List<T> origin)
    {
        List<T> shuffled = new(origin);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int r = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[r]) = (shuffled[r], shuffled[i]);
        }

        return shuffled;
    }

    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public static void ShowAtRightOfPos(this GameObject _obj, Vector2 targetScreenPos, float offsetFactor = 100)
    {
        RectTransform obj = _obj.GetComponent<RectTransform>();
        Canvas canvas = PlayerUI.Instance.GetComponent<Canvas>();
        float offsetX = offsetFactor * canvas.scaleFactor;

        Vector3[] corners = new Vector3[4];
        obj.GetWorldCorners(corners);
        float followerScreenWidth = Vector3.Distance(
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[0]),
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3])
        );

        float halfWidth = followerScreenWidth / 2f;

        Vector2 screenPos = targetScreenPos + new Vector2(offsetX + halfWidth, 0);

        RectTransform followerParent = obj.parent as RectTransform;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(followerParent, screenPos, canvas.worldCamera, out localPoint))
        {
            obj.localPosition = localPoint;
        }
    }
    public static void ShowAtRightOfUI(this GameObject _obj, Transform target, float offsetFactor = 100)
    {
        Canvas canvas = PlayerUI.Instance.GetComponent<Canvas>();
        _obj.ShowAtRightOfPos(RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, target.position), offsetFactor);
    }
}
