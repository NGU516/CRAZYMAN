using UnityEngine;
using TMPro;

public class UIJoin : UIPopup
{
    enum Buttons
    {
        JoinButton
    }
    enum GameObjects
    {
        InputKey // 입력부분 오브젝트 이름
    }

    private bool isCreateMode = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.JoinButton).gameObject.BindEvent(OnClickJoin);

        return true;
    }

    public void SetMode(bool createMode)
    {
        isCreateMode = createMode;
    }

    void OnClickJoin()
    {
        string roomCode = GetObject((int)GameObjects.InputKey).GetComponent<TMP_InputField>().text.Trim();
        Debug.Log("roomCode: " + roomCode);

        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogWarning("방 코드를 입력하세요.");
            return;
        }

        // 6자리 숫자 확인
        if (roomCode.Length != 6 || !int.TryParse(roomCode, out _))
        {
            Debug.LogWarning("6자리 숫자 코드를 입력하세요.");
            return;
        }

        Managers.UI.ClosePopupUI(this);

        // roomCode를 NetworkManager의 roomName으로 설정
        NetworkManager.Instance.roomName = roomCode;

        // 네트워크 방 참가
        NetworkManager.Instance.JoinRoom(roomCode);

        Managers.UI.ShowPopupUI<UICreateRoom>("UICreateRoom");
    }
}
