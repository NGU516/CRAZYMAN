using UnityEngine;
using Photon.Pun; // Photon ����� ���� �ʿ�
using UnityEngine.UI; // UI ���� ��� (�ʿ��ϴٸ�)

// MonoBehaviourPun�� ��ӹ޾� ��Ʈ��ũ ����ȭ ����� ����մϴ�.
public class FlashLight : MonoBehaviourPun
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlightLight; // ���� ������ Light ������Ʈ (Inspector���� ����)
    [SerializeField] private KeyCode toggleKey = KeyCode.F; // ������ �Ѱ� ���� Ű
    [SerializeField] private float staminaDrainRate = 5f; // ������ ���� ���� �� ���׹̳� �Ҹ���

    private bool isFlashlightOn = false; // ������ ���� ����

    // �ʿ��� �ٸ� ��ũ��Ʈ ����
    private StaminaSystem staminaSystem; // ���׹̳� �ý��� ��ũ��Ʈ
    private MentalGauge mentalGauge; // ��Ż ������ ��ũ��Ʈ

    void Awake()
    {
        // ������ Light ������Ʈ ã�� (�ڽ� ������Ʈ�� �پ��ִٰ� ����)
        if (flashlightLight == null)
        {
            flashlightLight = GetComponentInChildren<Light>();
            if (flashlightLight != null)
            {
                Debug.Log("[FlashLight] Flashlight Light component found in children.");
            }
            else
            {
                Debug.LogError("[FlashLight] Flashlight Light component not found in children! Please assign it in the Inspector.");
            }
        }

        // Light ������Ʈ�� �ִٸ� �ʱ� ���´� �������� ����
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }
    }

    void Start()
    {
        // �� �÷��̾�Ը� ��ũ��Ʈ�� ����� �۵��ϵ��� PhotonView üũ
        if (!photonView.IsMine)
        {
            // �ٸ� �÷��̾��� ������ ��ũ��Ʈ�� ��Ȱ��ȭ (�Է� �� ���� ó�� ����)
            // ������ RPC�� �޾ƾ� �ϹǷ� ��ũ��Ʈ ������Ʈ ��ü�� ��Ȱ��ȭ������ �ʽ��ϴ�.
            // ��� Update ��� photonView.IsMine üũ�� �մϴ�.
            // �ٸ� �÷��̾��� Light ������Ʈ�� ���Ӵϴ�.
            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }
            return; // �� �÷��̾ �ƴϸ� Start ���� ���� ���� �� ��
        }

        // StaminaSystem�� MentalGauge ��ũ��Ʈ ã�� (���� ���� ������Ʈ�� �پ��ִٰ� ����)
        staminaSystem = GetComponent<StaminaSystem>();
        if (staminaSystem == null)
        {
            Debug.LogError("[FlashLight] StaminaSystem component not found on player object!");
        }

        mentalGauge = GetComponent<MentalGauge>();
        if (mentalGauge == null)
        {
            Debug.LogError("[FlashLight] MentalGauge component not found on player object!");
        }

        Debug.Log("[FlashLight] FlashLight script initialized for my player.");
    }

    void Update()
    {
        // �� �÷��̾�Ը� �Է� �� ���׹̳� �Ҹ� ���� ����
        if (!photonView.IsMine) return;

        // ������ �Ѱ� ���� �Է� ó��
        if (Input.GetKeyDown(toggleKey))
        {
            // ������ ���� ���
            isFlashlightOn = !isFlashlightOn;
            Debug.Log($"[FlashLight] Toggled flashlight to: {isFlashlightOn}");

            // ������ ���� ��ȭ�� ��Ʈ��ũ�� ����ȭ (RPC ȣ��)
            photonView.RPC("ToggleFlashlightRPC", RpcTarget.All, isFlashlightOn);

            // ������ ���¿� ���� ���׹̳� �Ҹ� ����/���� �˸�
            if (staminaSystem != null)
            {
                if (isFlashlightOn)
                {
                    staminaSystem.StartDraining(); // ���׹̳� �Ҹ� ����
                    Debug.Log("[FlashLight] Started stamina draining.");
                }
                else
                {
                    staminaSystem.StopDraining(); // ���׹̳� �Ҹ� ����
                    Debug.Log("[FlashLight] Stopped stamina draining.");
                }
            }

            // ������ ���¿� ���� ��Ż �������� �˸� (������� ���� ��Ż ���� ����)
            if (mentalGauge != null)
            {
                // TODO: MentalGauge ��ũ��Ʈ�� StopMentalDrainFromDarkness(bool stop) �Լ��� �߰��ϰ� ���⼭ ȣ��
                // mentalGauge.StopMentalDrainFromDarkness(isFlashlightOn);
                Debug.Log($"[FlashLight] Notified MentalGauge about flashlight state: {isFlashlightOn}");
            }
        }

        // �������� ���� �ְ� ���׹̳ʰ� �����ϸ� �ڵ����� ����
        if (isFlashlightOn && staminaSystem != null && !staminaSystem.HasEnoughStamina(0.1f)) // HasEnoughStamina�� �ּ� �ʿ䷮ üũ
        {
            Debug.Log("[FlashLight] Stamina too low, turning off flashlight automatically.");
            isFlashlightOn = false; // ���� ����
            photonView.RPC("ToggleFlashlightRPC", RpcTarget.All, isFlashlightOn); // ��Ʈ��ũ ����ȭ
            staminaSystem.StopDraining(); // ���׹̳� �Ҹ� ����

            // ��Ż �������� �˸� (������� ���� ��Ż ���� �ٽ� ����)
            if (mentalGauge != null)
            {
                // TODO: MentalGauge ��ũ��Ʈ�� StopMentalDrainFromDarkness(bool stop) �Լ��� �߰��ϰ� ���⼭ ȣ��
                // mentalGauge.StopMentalDrainFromDarkness(isFlashlightOn); // isFlashlightOn�� ���� false
            }
        }

        // TODO: ������ ���͸� �ý����� �ִٸ� ���⼭ ���͸� �Ҹ� ���� �߰�
        // ��: if (isFlashlightOn) { currentBattery -= Time.deltaTime * batteryDrainRate; ... }
    }

    // RPC �Լ�: ��� Ŭ���̾�Ʈ���� ȣ��Ǿ� ������ ���¸� ����ȭ�մϴ�.
    [PunRPC]
    void ToggleFlashlightRPC(bool state)
    {
        // ��� Ŭ���̾�Ʈ���� �ش� �÷��̾��� ������ Light ������Ʈ ���� ����
        if (flashlightLight != null)
        {
            flashlightLight.enabled = state;
            // isFlashlightOn = state; // RPC������ Light ������Ʈ ���¸� �ٲٰ�, isFlashlightOn ���´� ���� �Է¿����� �����ϴ� ���� �Ϲ����Դϴ�.
            // ������ ���� ����ȭ�� �ʿ��ϴٸ� ���⼭ isFlashlightOn = state; �� �� ���� �ֽ��ϴ�.
            // ���� ������ ���� �Է����� isFlashlightOn�� �ٲٰ� RPC�� Light ������Ʈ�� ����ȭ�մϴ�.
            // ���� �ٸ� �÷��̾ �������� ���� �� �� ȭ�鿡�� �� �÷��̾��� ������ ����(isFlashlightOn)�� �˾ƾ� �Ѵٸ� ���⼭ ���� ����ȭ�� �ʿ��մϴ�.
            // �����ϰ� Light ������Ʈ�� ����ȭ�ϴ� ������ �����غ��ô�.
            Debug.Log($"[FlashLight] RPC: Flashlight state set to {state} for player {photonView.OwnerActorNr}");
        }
    }

    // TODO: MentalGauge ��ũ��Ʈ�� �� �Լ��� �߰����ּ���!
    // �� �Լ��� FlashLight ��ũ��Ʈ���� ȣ��Ǿ� ������� ���� ��Ż ���Ҹ� ���߰ų� �ٽ� �����մϴ�.
    /*
    // MentalGauge.cs ��ũ��Ʈ�� �߰��� ����:
    private bool isMentalDrainStoppedByFlashlight = false; // ����� ���� ��Ż ���Ұ� ������� ����

    public void StopMentalDrainFromDarkness(bool stop)
    {
        isMentalDrainStoppedByFlashlight = stop;
        Debug.Log($"[MentalGauge] Mental drain stopped by flashlight: {isMentalDrainStoppedByFlashlight}");
    }

    // MentalGauge.cs ��ũ��Ʈ�� Update �Լ� ����:
    void Update()
    {
        if (isDeath) return;

        // ������� ���� ��Ż ���� ����
        // ����� ���� ��Ż ���Ұ� ������ �ʾҰ�, ȯ�� ���� ���� ���� ���� ��Ż ����
        if (!isMentalDrainStoppedByFlashlight && !(lightOff.isLightOn))
        {
            currentMental -= Time.deltaTime * decreaseRate;
            if (currentMental <= 0)
            {
                currentMental = 0;
                TriggerDeath("MentalDepleted");
            }
            if (mentalSlider != null)
                mentalSlider.value = currentMental / maxMental;
        }
        // TODO: ���� ���� ���� �� ��Ż ȸ�� ���� �߰� (�ʿ��ϴٸ�)
        // else if (!isMentalDrainStoppedByFlashlight && (lightOff.isLightOn)) // ���� ���� ���� �� ��Ż ȸ�� (������� ����)
        // {
        //     currentMental += Time.deltaTime * increaseRate;
        //     currentMental = Mathf.Clamp(currentMental, 0, maxMental);
        //     if (mentalSlider != null)
        //         mentalSlider.value = currentMental / maxMental;
        // }
    }
    */

    // TODO: ������ ���͸� �ý����� �ִٸ� ���͸� ���� �Լ� �߰�
    // ��: public void ChargeBattery(float amount) { ... }
}
