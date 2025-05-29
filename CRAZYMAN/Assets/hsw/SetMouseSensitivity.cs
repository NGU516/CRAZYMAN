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
            // �ʱ� �����̴� �� ����
            sensitivitySlider.value = firstPersonScript.mouseSensitivity;

            // �����̴� ���� ����� �� �̺�Ʈ ����
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    void UpdateSensitivity(float value)
    {
        firstPersonScript.mouseSensitivity = value;
    }
}
