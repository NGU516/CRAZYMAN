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
        message.text = _value.ToString();
        Debug.Log("Slider Dragging!\n" + _value);//추후 수정
    }

    public void ResetFunction_UI()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.maxValue = max;
        slider.minValue = min;
    }
}