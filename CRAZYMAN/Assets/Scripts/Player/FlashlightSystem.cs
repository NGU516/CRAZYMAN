using UnityEngine;
using UnityEngine.UI;

public class FlashlightSystem : MonoBehaviour
{
    public Slider batterySlider; // 인스펙터에서 연결
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

        Debug.Log($"[FlashlightSystem] 배터리 충전됨. 현재 배터리: {currentBattery}");
    }

    public void FullRecharge()
    {
        currentBattery = maxBattery;
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery;
        }

        Debug.Log("[FlashlightSystem] 배터리 최대 충전 완료.");
    }
}
