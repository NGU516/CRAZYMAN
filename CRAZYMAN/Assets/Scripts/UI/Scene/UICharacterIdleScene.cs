using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterIdleScene : UIScene
{
    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        Debug.Log("ĳ���� ��� ȭ�� �ʱ�ȭ");

        // ĳ���� ��� ȭ�� ���� �߰�

        return true;
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
