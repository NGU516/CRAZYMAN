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
        string roomCode = GetObject((int)GameObjects.InputKey).GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogWarning("방 코드를 입력하세요.");
            return;
        }

        // 네트워크 방 생성/참가
        if (isCreateMode)
            NetworkManager.Instance.CreateRoom(roomCode);
        else
            NetworkManager.Instance.JoinRoom(roomCode);

        // roomCode는 NetworkManager에 저장
        NetworkManager.Instance.LastRoomKeyValue = roomCode;

        Managers.UI.ClosePopupUI(this);

        // UICreateRoom으로 이동
        Managers.UI.ShowCharacterIdleScene();
    }
}
