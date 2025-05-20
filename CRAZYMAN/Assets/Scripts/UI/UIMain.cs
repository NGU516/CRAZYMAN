using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMain : UIPopup
{
    enum Texts
    {
        GameTitle
    }

    enum Buttons
    {
        GameStartButton,
        SettingButton,
        GameQuitButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnClickScoreBoardButton);

        GetText((int)Texts.GameTitle).text = "CRAZY MAN";

        return true;
    }

    void OnClickStartButton()
    {
        Debug.Log("게임 시작");

        Managers.UI.ClosePopupUI(this);
        
        Managers.UI.ShowCharacterIdleScene();
    }

    void OnClickScoreBoardButton()
    {
        Managers.UI.ClosePopupUI(this);

        // UIScoreBoard 보여주기
        //var uiScoreBoard = Managers.UI.ShowPopupUI<UIScoreBoard>();

       /* uiScoreBoard.SetBoardDialog(
            () => {
                Managers.UI.ClosePopupUI(this);
                Managers.UI.ShowPopupUI<UIMain>();
            },
            null,
            "점수",
            "",
            "메인 메뉴",
            "재시작",
            true
            );*/
    }

    void OnComplete()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
