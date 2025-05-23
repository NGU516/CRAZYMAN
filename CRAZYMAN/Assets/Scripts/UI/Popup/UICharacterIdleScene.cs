using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UICharacterIdleScene : UIPopup
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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

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
