using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UICreateRoom : UIPopup
{
    enum Texts
    {
        Players
    }

    enum Buttons
    {
        StartButton
    }

    enum GameObjects
    {
        Player
    }

    public string RoomKeyValue { get; private set; }

    public void SetRoomKeyValue(string key)
    {
        RoomKeyValue = key;
        Debug.Log("방 코드(RoomKeyValue): " + RoomKeyValue);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        // RoomKeyValue 오브젝트에 roomCode 표시
        var roomKeyValueObj = GameObject.Find("RoomKeyValue");
        if (roomKeyValueObj != null)
        {
            var text = roomKeyValueObj.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = NetworkManager.Instance.LastRoomKeyValue;
            }
        }

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);

        return true;
    }

    void OnClickStartButton()
    {
        Debug.Log("게임 시작");

        Managers.UI.ClosePopupUI(this);

        GameObject playerObject = GameObject.Find("Player_Camera");

        FirstPerson firstPersonScript = playerObject.GetComponent<FirstPerson>();

        firstPersonScript.enabled = true;

        Managers.UI.ShowPopupUI<UIInGame>("UIInGame");

        Time.timeScale = 1;
    }
}
