using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingPopup : UIPopup
{
    private KeyManager keyManager;
    private ShowSoundValue showSoundValue;
    private ShowBrightnessValue showBrightnessValue;
    public Text soundValueText;//UISettingpopup obj 인스펙터에서 brightnessValueText에 BrightnessValue obj 할당
    public Text brightnessValueText;//UISettingpopup obj 인스펙터에서 soundValueText에 SoundValue obj 할당

    enum Texts
    {
        SoundValue,
        BrightnessValue,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        INTERACT,
        RUN,
        SIT,
        ITEM1,
        ITEM2
    }

    enum Buttons
    {
        ButtonBackToMain,
        ButtonUp,
        ButtonDown,
        ButtonLeft,
        ButtonRight,
        ButtonInteract,
        ButtonRun,
        ButtonSit,
        ButtonItem1,
        ButtonItem2
    }

    enum GameObjects
    {
        MainCameraTmp,
        CameraTmp,
        Scroll_Keyboard,
        BackGround,
        Sound,
        Brightness,
    }

    enum Sliders
    {
        SoundSlider,
        BrightnessSlider,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (keyManager == null)
            keyManager = FindObjectOfType<KeyManager>();
        showSoundValue = FindObjectOfType<ShowSoundValue>();
        showBrightnessValue = FindObjectOfType<ShowBrightnessValue>();
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.ButtonBackToMain).gameObject.BindEvent(OnClickBackToMainButton);
        GetButton((int)Buttons.ButtonUp).gameObject.BindEvent(OnClickUpButton);
        GetButton((int)Buttons.ButtonDown).gameObject.BindEvent(OnClickDownButton);
        GetButton((int)Buttons.ButtonLeft).gameObject.BindEvent(OnClickLeftButton);
        GetButton((int)Buttons.ButtonRight).gameObject.BindEvent(OnClickRightButton);
        GetButton((int)Buttons.ButtonInteract).gameObject.BindEvent(OnClickInteractButton);
        GetButton((int)Buttons.ButtonRun).gameObject.BindEvent(OnClickRunButton);
        GetButton((int)Buttons.ButtonSit).gameObject.BindEvent(OnClickSitButton);
        GetButton((int)Buttons.ButtonItem1).gameObject.BindEvent(OnClickItem1Button);
        GetButton((int)Buttons.ButtonItem2).gameObject.BindEvent(OnClickItem2Button);

        if (showSoundValue != null && soundValueText != null)
            showSoundValue.message = soundValueText;
        if (showBrightnessValue != null && brightnessValueText != null)
            showBrightnessValue.message = brightnessValueText;
        //GetText((int)Texts.SoundValue).text = "";
        //GetText((int)Texts.BrightnessValue).text = "";

        GetText((int)Texts.UP).text = "Up";
        GetText((int)Texts.DOWN).text = "Down";
        GetText((int)Texts.LEFT).text = "Left";
        GetText((int)Texts.RIGHT).text = "Right";
        GetText((int)Texts.INTERACT).text = "Interact";
        GetText((int)Texts.RUN).text = "Run";
        GetText((int)Texts.SIT).text = "Sit";
        GetText((int)Texts.ITEM1).text = "Item1";
        GetText((int)Texts.ITEM2).text = "Item2";

        return true;
    }
    
    void OnClickStartButton()
    {
        //@
    }
    void OnClickScoreBoardButton()//OnClickSettingsButton
    {
        //@
    }
    void OnClickQuitButton()
    {
        //@
    }
    void OnClickBackToMainButton()
    {
        // UI 메인으로 돌아가기
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UIMain>("UIMain");
    }
    void OnClickUpButton()
    {
        keyManager.ChangeKey((int)KeyInput.UP);

    }
    void OnClickDownButton()
    {
        keyManager.ChangeKey((int)KeyInput.DOWN);
    }
    void OnClickLeftButton()
    {
        keyManager.ChangeKey((int)KeyInput.LEFT);
    }
    void OnClickRightButton()
    {
        keyManager.ChangeKey((int)KeyInput.RIGHT);
    }
    void OnClickInteractButton()
    {
        keyManager.ChangeKey((int)KeyInput.INTERACT);
    }
    void OnClickRunButton()
    {
        keyManager.ChangeKey((int)KeyInput.RUN);
    }
    void OnClickSitButton()
    {
        keyManager.ChangeKey((int)KeyInput.SIT);
    }
    void OnClickItem1Button()
    {
        keyManager.ChangeKey((int)KeyInput.Item1);
    }
    void OnClickItem2Button()
    {
        keyManager.ChangeKey((int)KeyInput.Item2);
    }
}
