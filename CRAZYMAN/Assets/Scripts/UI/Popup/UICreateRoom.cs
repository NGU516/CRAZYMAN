using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class UICreateRoom : UIPopup
{
    enum Texts
    {
        Players
    }

    enum Buttons
    {
        StartButton,
        ReadyButton
    }

    enum GameObjects
    {
        Player
    }

    public string RoomKeyValue { get; private set; }
    private bool isReady = false;

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

        // 버튼 이벤트 바인딩
        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.ReadyButton).gameObject.BindEvent(OnClickReadyButton);

        // Start 버튼은 마스터 클라이언트만 활성화
        GetButton((int)Buttons.StartButton).interactable = PhotonNetwork.IsMasterClient;

        return true;
    }

    void OnClickReadyButton()
    {
        if (!PhotonNetwork.InRoom) return;

        isReady = !isReady;
        var readyButtonText = GetButton((int)Buttons.ReadyButton).GetComponentInChildren<TextMeshProUGUI>();
        readyButtonText.text = isReady ? "준비 완료" : "준비하기";
        
        // 플레이어의 Ready 상태를 Photon Custom Properties에 저장
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 모든 플레이어가 준비되었는지 확인
        bool allPlayersReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("IsReady", out object isReady))
            {
                if (!(bool)isReady)
                {
                    allPlayersReady = false;
                    break;
                }
            }
            else
            {
                allPlayersReady = false;
                break;
            }
        }

        if (!allPlayersReady)
        {
            Debug.Log("모든 플레이어가 준비되지 않았습니다.");
            return;
        }

        Debug.Log("게임 시작");

        // 명시적 Join (이미 방에 들어가 있지 않다면)
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("방 참가 시도: " + RoomKeyValue);
            NetworkManager.Instance.JoinRoom(RoomKeyValue);
        }

        // 방을 닫음 (마스터 클라이언트만 실행)
        NetworkManager.Instance.CloseRoom();

        Managers.UI.ClosePopupUI(this);

        // 모든 Player_Camera 오브젝트 찾기
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            // 각 플레이어의 PhotonControl 스크립트 활성화
            PhotonControl controlScript = playerObject.GetComponent<PhotonControl>();
            if (controlScript != null)
            {
                controlScript.enabled = true;
            }
        }

        Time.timeScale = 1;
    }
}
