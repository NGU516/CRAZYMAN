using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI; // UI ���� ���

public class MentalGauge : MonoBehaviourPun
{
    public Vector3 lastDeathPosition { get; private set; } // Save Death Position 
    public Slider mentalSlider; // �ν����Ϳ��� �����ϰų� ��ũ��Ʈ���� ã�� �����̴� ����
    [SerializeField] private float maxMental = 100f; // �ִ� ��Ż �� (Inspector���� ����)
    [SerializeField] private float decreaseRate = 2f; // ��Ż ���� �ӵ�
    [SerializeField] private float increaseRate = 4f; // ��Ż ȸ�� �ӵ� (���� ��� �� ��)
    private float currentMental; // ���� ��Ż ��
    public bool isDeath = false; // ���� ���� ����
    public Animator animator; // �÷��̾� �ִϸ�����
    public LightOff lightOff; // ������ ��ũ��Ʈ ����

    public System.Action<string> OnDeathRequest; // ���� ��û �̺�Ʈ

    // UIInGame �ν��Ͻ��� ������ ���� (�ߺ� ���� ���� �� ������)
    private GameObject uiInGameInstance;

    void Awake()
    {
        // Animator�� LightOff�� Awake���� ã�Ƶ� �˴ϴ�.
        // �÷��̾� ������Ʈ�� MentalGauge�� �پ��ִٰ� �����ϰ� FindWithTag�� Animator ã��
        animator = GameObject.FindWithTag("Player")?.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("[MentalGauge] Awake: Animator is not assigned! Please ensure the player object has the 'Player' tag and an Animator component.");
        }
        // ������ LightOff ��ũ��Ʈ ã��
        lightOff = FindObjectOfType<LightOff>();
        if (lightOff == null)
        {
            Debug.LogError("[MentalGauge] Awake: LightOff is not found in the scene! Please ensure LightOff script is attached to a GameObject in the scene.");
        }

        // *** Awake������ �����̴��� ã�� �ʽ��ϴ�. Start���� Resources �ε� �� ã���ϴ�. ***
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) return;

        if (mentalSlider == null)
        {
            GameObject uiPrefab = Resources.Load<GameObject>("Prefabs/UIInGame");
            if (uiPrefab != null)
            {
                // ���� UIInGame �ν��Ͻ� ���� (�� ���� �����ǵ��� ���� �ʿ�)
                uiInGameInstance = Instantiate(uiPrefab);
                uiInGameInstance.name = "UIInGame"; // �̸� �ߺ� ������

                // UIInGame �ڽĿ��� Stamina_Slider ã��
                Transform sliderTransform = uiInGameInstance.transform.Find("Stamina_Slider");
                if (sliderTransform != null)
                {
                    mentalSlider = sliderTransform.GetComponent<Slider>();
                    Debug.Log("[StaminaSystem] Stamina_Slider ���� �Ϸ�!");
                }
                else
                {
                    Debug.LogError("[StaminaSystem] UIInGame �� Stamina_Slider�� ã�� ���߽��ϴ�!");
                }
            }
            else
            {
                Debug.LogError("Stamina_Slider not found");
            }
        }
        currentMental = maxMental;
        if (mentalSlider != null)
        {
            mentalSlider.maxValue = maxMental;
            mentalSlider.value = currentMental;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeath)
            return;

        // *** ����� �α� �߰�: Update���� �����̴� MaxValue ��ȭ ����! ***
        if (mentalSlider != null && mentalSlider.maxValue != maxMental) // MaxValue�� 100�� �ƴ� �� �α� ���
        {
            Debug.LogWarning($"[MentalGauge] Update: mentalSlider.maxValue�� ����ġ ���� ������ �����! ���� ��: {mentalSlider.maxValue}");
        }

        // �����̴��� null�� �ƴ� ���� �� ������Ʈ �õ�
        if (mentalSlider != null)
        {
            if (!(lightOff.isLightOn))
            {
                currentMental -= Time.deltaTime * decreaseRate; // ��Ż ���� ����
                if (currentMental <= 0)
                {
                    currentMental = 0;
                    TriggerDeath("MentalDepleted");
                }
                // ��Ż �� ��ȭ�� ���� �����̴� �� ������Ʈ
                mentalSlider.value = currentMental; // 0~1 ������ �����̴� ������Ʈ
//                 Debug.Log(mentalSlider.value);
            }
            // TODO: ���� ���� ���� �� ��Ż ȸ�� ���� �߰� (�ʿ��ϴٸ�)
            // else
            // {
            //     currentMental += Time.deltaTime * increaseRate;
            //     currentMental = Mathf.Clamp(currentMental, 0, maxMental);
            //     mentalSlider.value = currentMental / maxMental;
            // }
        }
        else
        {
            // Debug.LogWarning("[MentalGauge] mentalSlider is null in Update. Cannot update slider value."); // �ʹ� ���� ��µ� �� ������ �ʿ��� ���� Ȱ��ȭ
        }
    }

    // ���ŷ� ȸ�� �Լ�
    public void RecoveryMental(float amount)
    {
        Debug.Log($"MentalGauge: RecoveryMental ȣ���. ��: {amount}");
        currentMental += amount; // ���޹��� �縸ŭ ���ŷ� ��� ����
        currentMental = Mathf.Clamp(currentMental, 0, maxMental); // 0�� maxMental ���̷� ����

        // UI �����̴� ������Ʈ (�����̴��� null�� �ƴ� ����)
        if (mentalSlider != null)
            mentalSlider.value = maxMental; // 0~1 ������ �����̴� ������Ʈ
        else
            Debug.LogWarning("[MentalGauge] mentalSlider is null in RecoveryMental. Cannot update slider value.");


        Debug.Log($"MentalGauge: RecoveryMental - ���� ���ŷ�: {currentMental}");
    }

    // ResetMentalGauge �Լ� (�����̴� null üũ �߰�)
    public void ResetMentalGauge()
    {
        currentMental = maxMental;
        if (mentalSlider != null) // �����̴� null üũ �߰�
            mentalSlider.value = currentMental / maxMental;
        else
            Debug.LogWarning("[MentalGauge] mentalSlider is null in ResetMentalGauge. Cannot update slider value.");

        isDeath = false;
        if (animator != null)
        {
            animator.SetBool("isDeath", false);
        }
    }

    // TriggerDeath �Լ� (���� �ڵ� ����)
    public void TriggerDeath(string cause)
    {
        Debug.Log("TriggerDeath : " + isDeath);
        if (isDeath)
            return;

        isDeath = true;
        lastDeathPosition = transform.position; // Death Position Save
        GameObject player = GameObject.FindWithTag("Player");
        player.tag = "Dead";
        
        PlayerPrefs.SetFloat("DeathX", lastDeathPosition.x);
        PlayerPrefs.SetFloat("DeathY", lastDeathPosition.y);
        PlayerPrefs.SetFloat("DeathZ", lastDeathPosition.z);

        Debug.Log($"Death triggered! Cause: {cause}");
        if (animator != null)
        {
            Debug.Log("Trigger Animator");
            animator.SetBool("isDeath", true);
        }
        Managers.UI.ShowPopupUI<UIDeath>("UIDeath");

        Destroy(player, 2f);
        OnDeathRequest?.Invoke(cause);
    }

    // TriggerDeathByMentalDepletion �Լ� (���� �ڵ� ����)
    public void TriggerDeathByMentalDepletion()
    {
        TriggerDeath("MentalDepleted");
    }

    // TriggerDeathByEnemy �Լ� (���� �ڵ� ����)
    public void TriggerDeathByEnemy()
    {
        TriggerDeath("EnemyCollision");
    }

    // �� ��ȯ �� UIInGame �ν��Ͻ� �ı� (�ʿ��ϴٸ�)
    // ���� �Ŵ��� ��� UIInGame �ν��Ͻ��� �����Ѵٸ� �� �Լ��� �ʿ� ���� �� �ֽ��ϴ�.
    void OnDestroy()
    {
        // �� MentalGauge ��ũ��Ʈ�� �ı��� ��, ���� �� ��ũ��Ʈ�� UIInGame �ν��Ͻ��� ���� �����ߴٸ� �ı��մϴ�.
        // UIInGame �ν��Ͻ� ������ GameManager �� �ٸ� ������ �Ѵٸ� �� ������ �����ؾ� �մϴ�.
        // ���� �ڵ� ������ MentalGauge�� UIInGame�� �����ϰ� �����Ƿ� ���ܵӴϴ�.
        if (uiInGameInstance != null)
        {
            Destroy(uiInGameInstance);
            Debug.Log("[MentalGauge] Destroyed UIInGame instance.");
        }
    }
}
