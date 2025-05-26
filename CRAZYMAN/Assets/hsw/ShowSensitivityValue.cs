using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSensitivityValue : MonoBehaviour
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
        message.text = _value.ToString("F0");
        Debug.Log("Slider Dragging!\n" + _value);
    }

    public void ResetFunction_UI()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.maxValue = 2000;
        slider.minValue = 100;
    }
}