using System;
using UnityEngine.SceneManagement;

public class UIMain : UIPopup
{
    enum Texts
    {

    }

    enum Buttons
    {

    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

/*      �̷������� ��������~
        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.ScoreBoardButton).gameObject.BindEvent(OnClickScoreBoardButton);

        GetText((int)Texts.StartText).text = "����";
        GetText((int)Texts.ScoreBoardText).text = "���";*/

        return true;
    }

    void OnClickStartButton()
    {
        Managers.UI.ClosePopupUI(this);
        
        //Managers.UI.ShowPopupUI<UIModeSelect>();
    }

    void OnClickScoreBoardButton()
    {
        Managers.UI.ClosePopupUI(this);

        // UIScoreBoard �����ֱ�
        //var uiScoreBoard = Managers.UI.ShowPopupUI<UIScoreBoard>();

       /* uiScoreBoard.SetBoardDialog(
            () => {
                Managers.UI.ClosePopupUI(this);
                Managers.UI.ShowPopupUI<UIMain>();
            },
            null,
            "����",
            "",
            "���� �޴�",
            "�����",
            true
            );*/
    }

    void OnComplete()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
