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
    }

    void Update()
    {
        if (isDraining)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
            if(!isRecoveryDelayed)
            {
                isExhausted = true;
                StartCoroutine(DelayRecovery());
            }
        } else if(!isRecoveryDelayed)
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

    public void StartDraining() => isDraining = true;
    public void StopDraining() => isDraining = false;
    public bool HasEnoughStamina(float amount = 0.1f) => currentStamina >= amount;
}
