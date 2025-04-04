using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIBase
{
    enum RectTransforms
    {
        SafeAreaContents
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, true);

        Bind<RectTransform>(typeof(RectTransforms));
        RectTransform safeAreaRectTransform = Get<RectTransform>((int)RectTransforms.SafeAreaContents);
        if (safeAreaRectTransform != null)
        {
            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeAreaRectTransform.anchorMin = anchorMin;
            safeAreaRectTransform.anchorMax = anchorMax;
        }

        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
