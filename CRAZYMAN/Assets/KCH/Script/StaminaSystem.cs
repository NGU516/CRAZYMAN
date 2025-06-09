using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] public float maxStamina = 100f;
    [SerializeField] public float currentStamina = 100f;
    [SerializeField] public float staminaDrainRate = 25f;
    [SerializeField] public float staminaRegenRate = 5f;
    [SerializeField] public float recoveryDelay = 3f;  // ?????
    public Slider staminaSlider;

    private bool isDraining = false;
    private bool isRecoveryDelayed = false; // ???? ???? ????
    private bool isExhausted = false; // ???????? ???? ????

    public bool IsExhausted => isExhausted; // ???????? ???? ???? ????

    void Start()
    {
        currentStamina = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        else
        {
            Debug.LogError("StaminaSlider is not assigned in the Inspector! Attempting to find it automatically.");
            StartCoroutine(WaitForSliderAndInitialize());
        }
    }

    private IEnumerator WaitForSliderAndInitialize()
    {
        // 슬라이더가 Instantiate될 때까지 대기
        while (staminaSlider == null)
        {
        //     Debug.Log("Stamina_Slider 연결 중");
            Slider[] allSliders = GameObject.FindObjectsOfType<Slider>(true); // 비활성 포함
            foreach (Slider s in allSliders)
            {
                if (s.name == "Stamina_Slider")
                {
                    staminaSlider = s;
                    Debug.Log("Stamina_Slider 자동 연결 완료!");
                    break;
                }
            }

            yield return null; // 다음 프레임까지 대기
        }

        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    void Update()
    {
        if (isDraining)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
            if(currentStamina <= 0 && !isRecoveryDelayed && !isExhausted)
            {
                isExhausted = true;
                StartCoroutine(DelayRecovery());
            }
        } else if(!isRecoveryDelayed && !isExhausted)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    private IEnumerator DelayRecovery()
    {
        isRecoveryDelayed = true;
        yield return new WaitForSeconds(recoveryDelay);
        isRecoveryDelayed = false;
        isExhausted = false;
    }

    public void RecoverStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Min(currentStamina, maxStamina);

        isRecoveryDelayed = false;
        isExhausted = false;

        // UI 슬라이더도 바로 업데이트
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;

        Debug.Log($"StaminaSystem: 스테미너 회복됨! 현재 스테미너: {currentStamina}");
    }

    public void StartDraining() => isDraining = true;
    public void StopDraining() => isDraining = false;
    public bool HasEnoughStamina(float amount = 0.1f) => currentStamina >= amount;
}
