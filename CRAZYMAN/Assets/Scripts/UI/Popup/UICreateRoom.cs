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
        Debug.Log("�� �ڵ�(RoomKeyValue): " + RoomKeyValue);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        // RoomKeyValue ������Ʈ�� roomCode ǥ��
        var roomKeyValueObj = GameObject.Find("RoomKeyValue");
        if (roomKeyValueObj != null)
        {
            var text = roomKeyValueObj.GetComponent<TMP_Text>();
            if (text != null)
            {
                // NetworkManager�� roomName ���
                text.text = NetworkManager.Instance.roomName;
                SetRoomKeyValue(NetworkManager.Instance.roomName);
            }
        }

        // ��ư �̺�Ʈ ���ε�
        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.ReadyButton).gameObject.BindEvent(OnClickReadyButton);

        // Start ��ư�� ������ Ŭ���̾�Ʈ�� Ȱ��ȭ
        GetButton((int)Buttons.StartButton).interactable = PhotonNetwork.IsMasterClient;

        return true;
    }

    void OnClickReadyButton()
    {
        if (!PhotonNetwork.InRoom) return;

        isReady = !isReady;
        var readyButtonText = GetButton((int)Buttons.ReadyButton).GetComponentInChildren<TextMeshProUGUI>();
        readyButtonText.text = isReady ? "�غ� �Ϸ�" : "�غ��ϱ�";
        
        // �÷��̾��� Ready ���¸� Photon Custom Properties�� ����
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // ��� �÷��̾ �غ�Ǿ����� Ȯ��
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
            Debug.Log("��� �÷��̾ �غ���� �ʾҽ��ϴ�.");
            return;
        }

        Debug.Log("���� ����");

        // ����� Join (�̹� �濡 �� ���� �ʴٸ�)
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("�� ���� �õ�: " + RoomKeyValue);
            NetworkManager.Instance.JoinRoom(RoomKeyValue);
        }

        // ���� ���� (������ Ŭ���̾�Ʈ�� ����)
        NetworkManager.Instance.CloseRoom();

        Managers.UI.ClosePopupUI(this);

        // ��� Player_Camera ������Ʈ ã��
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            // �� �÷��̾��� PhotonControl ��ũ��Ʈ Ȱ��ȭ
            PhotonControl controlScript = playerObject.GetComponent<PhotonControl>();
            if (controlScript != null)
            {
                controlScript.enabled = true;
            }
        }

        Time.timeScale = 1;
    }
}
