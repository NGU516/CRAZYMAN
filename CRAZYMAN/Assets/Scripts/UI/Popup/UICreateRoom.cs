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

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);

        return true;
    }

    void OnClickStartButton()
    {
        Debug.Log("���� ����");

        // ����� Join (�̹� �濡 �� ���� �ʴٸ�)
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("�� ���� �õ�: " + RoomKeyValue);
            NetworkManager.Instance.JoinRoom(RoomKeyValue);
        }

        Managers.UI.ClosePopupUI(this);

        // ��� Player_Camera ������Ʈ ã��
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            // �� �÷��̾��� Control ��ũ��Ʈ Ȱ��ȭ
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
