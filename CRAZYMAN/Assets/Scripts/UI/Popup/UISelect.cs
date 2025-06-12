using UnityEngine;

public class UISelect : UIPopup
{
    enum Buttons
    {
        CreateButton,
        JoinButton,
        BackButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.CreateButton).gameObject.BindEvent(OnClickCreate);
        GetButton((int)Buttons.JoinButton).gameObject.BindEvent(OnClickJoin);
        GetButton((int)Buttons.BackButton).gameObject.BindEvent(OnClickBackButton);

        return true;
    }

    // 방 생성
    void OnClickCreate()
    {
        Managers.UI.ClosePopupUI(this);
        NetworkManager.Instance.CreateRoom(""); // 내부에서 랜덤 코드 생성
        Managers.UI.ShowPopupUI<UICreateRoom>("UICreateRoom");
    }

    // 방 참가
    void OnClickJoin()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UIJoin>("UIJoin").SetMode(false);
    }

    void OnClickBackButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UIMain>("UIMain");
    }
}