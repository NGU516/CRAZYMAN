using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;

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
                // NetworkManager의 roomName 사용
                text.text = NetworkManager.Instance.roomName;
                SetRoomKeyValue(NetworkManager.Instance.roomName);
            }
        }

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);

        return true;
    }

    void OnClickStartButton()
    {
        Debug.Log("게임 시작");

        // 명시적 Join (이미 방에 들어가 있지 않다면)
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("방 참가 시도: " + RoomKeyValue);
            NetworkManager.Instance.JoinRoom(RoomKeyValue);
        }

        Managers.UI.ClosePopupUI(this);

        // 모든 Player_Camera 오브젝트 찾기
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            // 각 플레이어의 Control 스크립트 활성화
            Control controlScript = playerObject.GetComponent<Control>();
            if (controlScript != null)
            {
                controlScript.enabled = true;
            }
        }

        Managers.UI.ShowPopupUI<UIInGame>("UIInGame");

        Time.timeScale = 1;
    }
}
