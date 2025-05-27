using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISettingPopup : UIPopup
{
    enum Texts
    {
        SoundValueText,
        BrightnessValueText,
        MouseSensitivityValueText
    }

    enum Buttons
    {
        BackToMainButton
    }

    enum GameObjects
    {
        MainCamera,
        DirectionalLight,
        EventSystem,
        SettingWindow,
        Scroll_Keyboard
    }

    enum Sliders
    {
        SoundSlider,
        BrightnessSlider,
        MouseSensitivitySlider
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.BackToMainButton).gameObject.BindEvent(OnClickBackToMainButton);

        return true;
    }

    void OnClickBackToMainButton()
    {

    }
}
