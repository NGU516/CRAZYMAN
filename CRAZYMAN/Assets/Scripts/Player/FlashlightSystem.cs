using UnityEngine;
using UnityEngine.UI;

public class FlashlightSystem : MonoBehaviour
{
    public Slider batterySlider; // �ν����Ϳ��� ����
    public float maxBattery = 100f;
    private float currentBattery;

    void Start()
    {
        currentBattery = maxBattery;
        if (batterySlider != null)
        {
            batterySlider.maxValue = maxBattery;
            batterySlider.value = currentBattery;
        }
    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);

        if (batterySlider != null)
        {
            batterySlider.value = currentBattery;
        }

        Debug.Log($"[FlashlightSystem] ���͸� ������. ���� ���͸�: {currentBattery}");
    }

    public void FullRecharge()
    {
        currentBattery = maxBattery;
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery;
        }

        Debug.Log("[FlashlightSystem] ���͸� �ִ� ���� �Ϸ�.");
    }
}
