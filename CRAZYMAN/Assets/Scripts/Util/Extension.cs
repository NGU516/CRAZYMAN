using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UIBase.BindEvent(go, action, type);
    }

    public static void ResetVertical(this ScrollRect scrollRect)
    {
        scrollRect.verticalNormalizedPosition = 1;
    }

    public static void ResetHorizontal(this ScrollRect scrollRect)
    {
        scrollRect.horizontalNormalizedPosition = 1;
    }
}