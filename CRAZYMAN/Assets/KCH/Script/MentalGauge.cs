using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MentalGauge : MonoBehaviour
{
    public Slider mentalSlider;
    [SerializeField] private float maxMental = 100f;
    [SerializeField] private float decreaseRate = 4f;
    [SerializeField] private float increaseRate = 4f;
    private float currentMental;
    private bool isDark = false;

    // Start is called before the first frame update
    void Start()
    {
        currentMental = maxMental;
        mentalSlider.value = currentMental / maxMental; // �ʱⰪ 1 (�ִ�)
    }

    // Update is called once per frame

    void Update()
    {
        // ���ŷ� �׽�Ʈ�� F ������ isDark ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetDarkState(!isDark);
        }

        if (isDark)
        {
            currentMental -= Time.deltaTime * decreaseRate; // �ʴ� 4�� ����
            if (currentMental < 0)
                currentMental = 0;
        }
        else
        {
            currentMental += Time.deltaTime * increaseRate;
            if(currentMental >= 100)
                currentMental = 100;
        }

        mentalSlider.value = currentMental / maxMental; // 0~1 ���� ������ �����̴� ������Ʈ
    }

  // ���ŷ��� ���̰ų� �ø��� �Լ�
    public void ChangeMental(float amount)
    {
        currentMental += amount;
        currentMental = Mathf.Clamp(currentMental, 0, maxMental);
        mentalSlider.value = currentMental / maxMental;
    }

    public void SetDarkState(bool dark)
    {
        isDark = dark;
    }
}
