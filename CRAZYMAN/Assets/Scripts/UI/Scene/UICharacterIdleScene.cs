using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterIdleScene : UIScene
{
    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        Debug.Log("캐릭터 대기 화면 초기화");

        // 캐릭터 대기 화면 로직 추가

        return true;
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
