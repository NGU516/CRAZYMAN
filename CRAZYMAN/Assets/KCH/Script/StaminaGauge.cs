using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float drainRate = 25f;
    public float regenRate = 5f;
    public Slider slider;

    private bool isDraining = false;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        if (slider == null)
            Debug.LogError("[StaminaGauge] Slider 컴포넌트를 찾을 수 없습니다!");
    }

    void Start()
    {
        currentStamina = maxStamina;
        if (slider != null)
        {
            slider.maxValue = maxStamina;
            slider.value = currentStamina;
        }
    }

    void Update()
    {
        if (isDraining)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += regenRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if (slider != null)
            slider.value = currentStamina;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isDraining = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isDraining = false;
    }
}
