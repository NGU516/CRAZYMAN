using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSoundValue : MonoBehaviour
{
    public Text message;
    public Slider slider;

    private float min = -40f;
    private float max = 0f;

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
        tmp_value = (_value + 40f) * 2.5f;
        message.text = tmp_value.ToString("F0");
        Debug.Log("Slider Dragging!\n" + tmp_value);
    }

    public void ResetFunction_UI()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.maxValue = max;
        slider.minValue = min;
    }
}