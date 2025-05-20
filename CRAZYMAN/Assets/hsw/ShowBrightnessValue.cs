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

    private void Function_Slider(float _value)//추후 정수 1~100으로 수정
    {
        float tmp_value;
        tmp_value = _value * 50f;
        message.text = tmp_value.ToString("F0");//tmp_value.ToString("F2")
        Debug.Log("Slider Dragging!\n" + _value);
    }

    public void ResetFunction_UI()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.maxValue = 2.00f;
        slider.minValue = 0.01f;
    }
}