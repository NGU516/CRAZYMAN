using UnityEngine;

public class UIEnding : UIPopup
{
    // 엔딩 팝업에서 보여줄 텍스트, 버튼 등 필요에 따라 추가
    public override bool Init()
    {
        if (!base.Init())
            return false;

        return true;
    }

}
