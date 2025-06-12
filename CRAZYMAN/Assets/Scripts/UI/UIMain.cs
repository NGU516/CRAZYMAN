using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMain : UIPopup
{
    enum Texts
    {
        GameTitle
    }

    enum Buttons
    {
        GameStartButton,
        SettingButton,
        GameQuitButton
    }

    [SerializeField] private AudioListener uiAudioListener;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnClickSettingButton);
        GetButton((int)Buttons.GameQuitButton).gameObject.BindEvent(OnClickGameQuitButton);

        GetText((int)Texts.GameTitle).text = "CRAZY MAN";

        if (uiAudioListener != null)
            uiAudioListener.enabled = true;
        Managers.SoundManager.Play(Define.Sound.StartBgm);

        return true;
    }

    void OnClickStartButton()
    {
        Debug.Log("���� ����");

        if (uiAudioListener != null)
            uiAudioListener.enabled = false;

        Managers.UI.ClosePopupUI(this);
        Managers.SoundManager.Clear();
        
        // ������ �ٷ� ĳ���� ������ �Ѿ
        // Managers.UI.ShowCharacterIdleScene();
        // �κ�(UISelect)�� �Ѿ
        Managers.UI.ShowPopupUI<UISelect>("UISelect");
    }

    void OnClickSettingButton()
    {
        Managers.UI.ClosePopupUI(this);

        Managers.UI.ShowSettingPopup();
    }

    void OnComplete()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickGameQuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
