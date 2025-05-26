using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBrightnessValue : MonoBehaviour
{
    public Text message;
    public Slider slider;

    void Start()
    {
        SetFunction_UI();
    }

    private void SetFunction_UI()
    {
        ResetFunction_UI();

        slider.onValueChanged.AddListener(Function_Slider);
    }

    private void Function_Slider(float _value)
    {
        float tmp_value;
        tmp_value = _value * 50f;
        message.text = tmp_value.ToString("F0");
        Debug.Log("Slider Dragging!\n" + _value);
    }

    public void ResetFunction_UI()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.maxValue = 2.00f;
        slider.minValue = 0.01f;
    }
}