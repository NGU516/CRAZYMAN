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
    public bool isDeath = false;
    public Animator animator;
    public LightOff lightOff;

    public System.Action<string> OnDeathRequest;

    // Start is called before the first frame update
    void Start()
    {
        if(animator == null)
        {
            Debug.LogError("Animator is not assigned in MentalGauge! Please assign the player's Animator in the Inspector");
        }

        currentMental = maxMental;
        mentalSlider.value = currentMental / maxMental;
    }

    // Update is called once per frame

    void Update()
    {
        if (isDeath)
            return;

        if (!(lightOff.isLightOn))
        {
            currentMental -= Time.deltaTime * decreaseRate; // ???? 4?? ????
            if (currentMental <= 0)
            {
                currentMental = 0;
                TriggerDeath("MentalDepleted");
            }
            mentalSlider.value = currentMental / maxMental; // 0~1 ???? ?????? ???????? ????????
        }
    }

  // ???????? ???????? ?????? ????
    public void ChargeMental(float amount)
    {
        currentMental += amount;
        currentMental = Mathf.Clamp(currentMental, 0, maxMental);
        mentalSlider.value = currentMental / maxMental;
    }

    public void ResetMentalGauge()
    {
        currentMental = maxMental;
        mentalSlider.value = currentMental / maxMental;
        isDeath = false;
        if(animator != null)
        {
            animator.SetBool("isDeath", false);
        }
    }

    public void TriggerDeath(string cause)
    {
        if (isDeath)
            return;

        isDeath = true;
        Debug.Log($"Death triggered! Cause: {cause}");
        if(animator != null)
        {
            Debug.Log("check ppint");
            animator.SetBool("isDeath", true);
        }
        OnDeathRequest?.Invoke(cause);
    }

    public void TriggerDeathByMentalDepletion()
    {
        TriggerDeath("MentalDepleted");
    }

    public void TriggerDeathByEnemy()
    {
        TriggerDeath("EnemyCollision");
    }
}
