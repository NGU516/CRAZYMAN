using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMouseSensitivity : MonoBehaviour
{
    public Slider sensitivitySlider;
    public FirstPerson firstPersonScript;

    void Start()
    {
        if (sensitivitySlider != null && firstPersonScript != null)
        {
            // 초기 슬라이더 값 세팅
            sensitivitySlider.value = firstPersonScript.mouseSensitivity;

            // 슬라이더 값이 변경될 때 이벤트 연결
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    void UpdateSensitivity(float value)
    {
        firstPersonScript.mouseSensitivity = value;
    }
}
