using UnityEngine;
using Photon.Pun; // Photon 사용을 위해 필요
using UnityEngine.UI; // UI 관련 사용 (필요하다면)

// MonoBehaviourPun을 상속받아 네트워크 동기화 기능을 사용합니다.
public class FlashLight : MonoBehaviourPun
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlightLight; // 실제 손전등 Light 컴포넌트 (Inspector에서 연결)
    [SerializeField] private KeyCode toggleKey = KeyCode.F; // 손전등 켜고 끄는 키
    [SerializeField] private float staminaDrainRate = 5f; // 손전등 켜져 있을 때 스테미너 소모율

    private bool isFlashlightOn = false; // 손전등 현재 상태

    // 필요한 다른 스크립트 참조
    private StaminaSystem staminaSystem; // 스테미너 시스템 스크립트
    private MentalGauge mentalGauge; // 멘탈 게이지 스크립트

    void Awake()
    {
        // 손전등 Light 컴포넌트 찾기 (자식 오브젝트에 붙어있다고 가정)
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

        // Light 컴포넌트가 있다면 초기 상태는 꺼짐으로 설정
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }
    }

    void Start()
    {
        // 내 플레이어에게만 스크립트가 제대로 작동하도록 PhotonView 체크
        if (!photonView.IsMine)
        {
            // 다른 플레이어의 손전등 스크립트는 비활성화 (입력 및 로직 처리 방지)
            // 하지만 RPC는 받아야 하므로 스크립트 컴포넌트 자체를 비활성화하지는 않습니다.
            // 대신 Update 등에서 photonView.IsMine 체크를 합니다.
            // 다른 플레이어의 Light 컴포넌트는 꺼둡니다.
            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }
            return; // 내 플레이어가 아니면 Start 이후 로직 실행 안 함
        }

        // StaminaSystem과 MentalGauge 스크립트 찾기 (같은 게임 오브젝트에 붙어있다고 가정)
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
        // 내 플레이어에게만 입력 및 스테미너 소모 로직 적용
        if (!photonView.IsMine) return;

        // 손전등 켜고 끄기 입력 처리
        if (Input.GetKeyDown(toggleKey))
        {
            // 손전등 상태 토글
            isFlashlightOn = !isFlashlightOn;
            Debug.Log($"[FlashLight] Toggled flashlight to: {isFlashlightOn}");

            // 손전등 상태 변화를 네트워크로 동기화 (RPC 호출)
            photonView.RPC("ToggleFlashlightRPC", RpcTarget.All, isFlashlightOn);

            // 손전등 상태에 따라 스테미너 소모 시작/중지 알림
            if (staminaSystem != null)
            {
                if (isFlashlightOn)
                {
                    staminaSystem.StartDraining(); // 스테미너 소모 시작
                    Debug.Log("[FlashLight] Started stamina draining.");
                }
                else
                {
                    staminaSystem.StopDraining(); // 스테미너 소모 중지
                    Debug.Log("[FlashLight] Stopped stamina draining.");
                }
            }

            // 손전등 상태에 따라 멘탈 게이지에 알림 (어둠으로 인한 멘탈 감소 방지)
            if (mentalGauge != null)
            {
                // TODO: MentalGauge 스크립트에 StopMentalDrainFromDarkness(bool stop) 함수를 추가하고 여기서 호출
                // mentalGauge.StopMentalDrainFromDarkness(isFlashlightOn);
                Debug.Log($"[FlashLight] Notified MentalGauge about flashlight state: {isFlashlightOn}");
            }
        }

        // 손전등이 켜져 있고 스테미너가 부족하면 자동으로 끄기
        if (isFlashlightOn && staminaSystem != null && !staminaSystem.HasEnoughStamina(0.1f)) // HasEnoughStamina는 최소 필요량 체크
        {
            Debug.Log("[FlashLight] Stamina too low, turning off flashlight automatically.");
            isFlashlightOn = false; // 상태 변경
            photonView.RPC("ToggleFlashlightRPC", RpcTarget.All, isFlashlightOn); // 네트워크 동기화
            staminaSystem.StopDraining(); // 스테미너 소모 중지

            // 멘탈 게이지에 알림 (어둠으로 인한 멘탈 감소 다시 시작)
            if (mentalGauge != null)
            {
                // TODO: MentalGauge 스크립트에 StopMentalDrainFromDarkness(bool stop) 함수를 추가하고 여기서 호출
                // mentalGauge.StopMentalDrainFromDarkness(isFlashlightOn); // isFlashlightOn은 이제 false
            }
        }

        // TODO: 손전등 배터리 시스템이 있다면 여기서 배터리 소모 로직 추가
        // 예: if (isFlashlightOn) { currentBattery -= Time.deltaTime * batteryDrainRate; ... }
    }

    // RPC 함수: 모든 클라이언트에서 호출되어 손전등 상태를 동기화합니다.
    [PunRPC]
    void ToggleFlashlightRPC(bool state)
    {
        // 모든 클라이언트에서 해당 플레이어의 손전등 Light 컴포넌트 상태 변경
        if (flashlightLight != null)
        {
            flashlightLight.enabled = state;
            // isFlashlightOn = state; // RPC에서는 Light 컴포넌트 상태만 바꾸고, isFlashlightOn 상태는 로컬 입력에서만 관리하는 것이 일반적입니다.
            // 하지만 상태 동기화가 필요하다면 여기서 isFlashlightOn = state; 를 할 수도 있습니다.
            // 현재 로직은 로컬 입력으로 isFlashlightOn을 바꾸고 RPC로 Light 컴포넌트만 동기화합니다.
            // 만약 다른 플레이어가 손전등을 켰을 때 내 화면에서 그 플레이어의 손전등 상태(isFlashlightOn)를 알아야 한다면 여기서 상태 동기화도 필요합니다.
            // 간단하게 Light 컴포넌트만 동기화하는 것으로 시작해봅시다.
            Debug.Log($"[FlashLight] RPC: Flashlight state set to {state} for player {photonView.OwnerActorNr}");
        }
    }

    // TODO: MentalGauge 스크립트에 이 함수를 추가해주세요!
    // 이 함수는 FlashLight 스크립트에서 호출되어 어둠으로 인한 멘탈 감소를 멈추거나 다시 시작합니다.
    /*
    // MentalGauge.cs 스크립트에 추가할 내용:
    private bool isMentalDrainStoppedByFlashlight = false; // 손전등에 의해 멘탈 감소가 멈췄는지 여부

    public void StopMentalDrainFromDarkness(bool stop)
    {
        isMentalDrainStoppedByFlashlight = stop;
        Debug.Log($"[MentalGauge] Mental drain stopped by flashlight: {isMentalDrainStoppedByFlashlight}");
    }

    // MentalGauge.cs 스크립트의 Update 함수 수정:
    void Update()
    {
        if (isDeath) return;

        // 어둠으로 인한 멘탈 감소 로직
        // 손전등에 의해 멘탈 감소가 멈추지 않았고, 환경 빛이 꺼져 있을 때만 멘탈 감소
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
        // TODO: 빛이 켜져 있을 때 멘탈 회복 로직 추가 (필요하다면)
        // else if (!isMentalDrainStoppedByFlashlight && (lightOff.isLightOn)) // 빛이 켜져 있을 때 멘탈 회복 (손전등과 별개)
        // {
        //     currentMental += Time.deltaTime * increaseRate;
        //     currentMental = Mathf.Clamp(currentMental, 0, maxMental);
        //     if (mentalSlider != null)
        //         mentalSlider.value = currentMental / maxMental;
        // }
    }
    */

    // TODO: 손전등 배터리 시스템이 있다면 배터리 충전 함수 추가
    // 예: public void ChargeBattery(float amount) { ... }
}
