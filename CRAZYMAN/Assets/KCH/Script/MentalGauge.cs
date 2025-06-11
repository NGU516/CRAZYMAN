using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI; // UI 관련 사용

public class MentalGauge : MonoBehaviourPun
{
    public Slider mentalSlider; // 인스펙터에서 연결하거나 스크립트에서 찾을 슬라이더 변수
    [SerializeField] private float maxMental = 100f; // 최대 멘탈 값 (Inspector에서 설정)
    [SerializeField] private float decreaseRate = 2f; // 멘탈 감소 속도
    [SerializeField] private float increaseRate = 4f; // 멘탈 회복 속도 (현재 사용 안 함)
    private float currentMental; // 현재 멘탈 값
    public bool isDeath = false; // 죽음 상태 여부
    public Animator animator; // 플레이어 애니메이터
    public LightOff lightOff; // 손전등 스크립트 참조

    private ElectricTorchOnOff electricTorchOnOff;

    public System.Action<string> OnDeathRequest; // 죽음 요청 이벤트

    // UIInGame 인스턴스를 저장할 변수 (중복 생성 방지 및 참조용)
    private GameObject uiInGameInstance;

    void Awake()
    {
        // Animator와 LightOff는 Awake에서 찾아도 됩니다.
        // 플레이어 오브젝트에 MentalGauge가 붙어있다고 가정하고 FindWithTag로 Animator 찾기
        animator = GameObject.FindWithTag("Player")?.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("[MentalGauge] Awake: Animator is not assigned! Please ensure the player object has the 'Player' tag and an Animator component.");
        }

        electricTorchOnOff = GetComponent<ElectricTorchOnOff>();
        if (electricTorchOnOff == null)
        {
            // 만약 다른 곳에 있다면 아래처럼 FindObjectOfType으로 찾아봐
            electricTorchOnOff = FindObjectOfType<ElectricTorchOnOff>();
            if (electricTorchOnOff == null)
            {
                Debug.LogError("[MentalGauge] Awake: ElectricTorchOnOff script is not found in the scene or on this GameObject!");
            }
        }

        // 씬에서 LightOff 스크립트 찾기
        lightOff = FindObjectOfType<LightOff>();
        if (lightOff == null)
        {
            Debug.LogError("[MentalGauge] Awake: LightOff is not found in the scene! Please ensure LightOff script is attached to a GameObject in the scene.");
        }

        // *** Awake에서는 슬라이더를 찾지 않습니다. Start에서 Resources 로드 후 찾습니다. ***
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
                // 씬에 UIInGame 인스턴스 생성 (한 번만 생성되도록 관리 필요)
                uiInGameInstance = Instantiate(uiPrefab);
                uiInGameInstance.name = "UIInGame"; // 이름 중복 방지용

                // UIInGame 자식에서 Stamina_Slider 찾기
                Transform sliderTransform = uiInGameInstance.transform.Find("Stamina_Slider");
                if (sliderTransform != null)
                {
                    mentalSlider = sliderTransform.GetComponent<Slider>();
                    Debug.Log("[StaminaSystem] Stamina_Slider 연결 완료!");
                }
                else
                {
                    Debug.LogError("[StaminaSystem] UIInGame 내 Stamina_Slider를 찾지 못했습니다!");
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

        // *** 디버그 로그 추가: Update에서 슬라이더 MaxValue 변화 추적! ***
        if (mentalSlider != null && mentalSlider.maxValue != maxMental) // MaxValue가 100이 아닐 때 로그 출력
        {
            Debug.LogWarning($"[MentalGauge] Update: mentalSlider.maxValue가 예상치 못한 값으로 변경됨! 현재 값: {mentalSlider.maxValue}");
        }

        // 슬라이더가 null이 아닐 때만 값 업데이트 시도
        if (mentalSlider != null)
        {
            if (!(lightOff.isLightOn) && electricTorchOnOff != null && !electricTorchOnOff.IsFlashLightOn)
            {
                currentMental -= Time.deltaTime * decreaseRate; // 멘탈 감소 로직
                currentMental = Mathf.Clamp(currentMental, 0, maxMental);
                if (currentMental <= 0)
                {
                    currentMental = 0;
                    TriggerDeath("MentalDepleted");
                }
                // 멘탈 값 변화에 따라 슬라이더 값 업데이트
                mentalSlider.value = currentMental; // 0~1 값으로 슬라이더 업데이트
//                 Debug.Log(mentalSlider.value);
            }
            // TODO: 빛이 켜져 있을 때 멘탈 회복 로직 추가 (필요하다면)
            // else
            // {
            //     currentMental += Time.deltaTime * increaseRate;
            //     currentMental = Mathf.Clamp(currentMental, 0, maxMental);
            //     mentalSlider.value = currentMental / maxMental;
            // }
        }
        else
        {
            // Debug.LogWarning("[MentalGauge] mentalSlider is null in Update. Cannot update slider value."); // 너무 자주 출력될 수 있으니 필요할 때만 활성화
        }
    }

    // 정신력 회복 함수
    public void RecoveryMental(float amount)
    {
        Debug.Log($"MentalGauge: RecoveryMental 호출됨. 양: {amount}");
        currentMental += amount; // 전달받은 양만큼 정신력 즉시 증가
        currentMental = Mathf.Clamp(currentMental, 0, maxMental); // 0과 maxMental 사이로 제한

        // UI 슬라이더 업데이트 (슬라이더가 null이 아닐 때만)
        if (mentalSlider != null)
            mentalSlider.value = maxMental; // 0~1 값으로 슬라이더 업데이트
        else
            Debug.LogWarning("[MentalGauge] mentalSlider is null in RecoveryMental. Cannot update slider value.");


        Debug.Log($"MentalGauge: RecoveryMental - 현재 정신력: {currentMental}");
    }

    // ResetMentalGauge 함수 (슬라이더 null 체크 추가)
    public void ResetMentalGauge()
    {
        currentMental = maxMental;
        if (mentalSlider != null) // 슬라이더 null 체크 추가
            mentalSlider.value = currentMental / maxMental;
        else
            Debug.LogWarning("[MentalGauge] mentalSlider is null in ResetMentalGauge. Cannot update slider value.");

        isDeath = false;
        if (animator != null)
        {
            animator.SetBool("isDeath", false);
        }
    }

    // TriggerDeath 함수 (기존 코드 유지)
    public void TriggerDeath(string cause)
    {
        Debug.Log("TriggerDeath : " + isDeath);
        if (isDeath)
            return;

        isDeath = true;
        Debug.Log($"Death triggered! Cause: {cause}");
        if (animator != null)
        {
            Debug.Log("Trigger Animator");
            animator.SetBool("isDeath", true);
        }
        OnDeathRequest?.Invoke(cause);
    }

    // TriggerDeathByMentalDepletion 함수 (기존 코드 유지)
    public void TriggerDeathByMentalDepletion()
    {
        TriggerDeath("MentalDepleted");
    }

    // TriggerDeathByEnemy 함수 (기존 코드 유지)
    public void TriggerDeathByEnemy()
    {
        TriggerDeath("EnemyCollision");
    }

    // 씬 전환 시 UIInGame 인스턴스 파괴 (필요하다면)
    // 게임 매니저 등에서 UIInGame 인스턴스를 관리한다면 이 함수는 필요 없을 수 있습니다.
    void OnDestroy()
    {
        // 이 MentalGauge 스크립트가 파괴될 때, 만약 이 스크립트가 UIInGame 인스턴스를 직접 생성했다면 파괴합니다.
        // UIInGame 인스턴스 관리를 GameManager 등 다른 곳에서 한다면 이 로직은 제거해야 합니다.
        // 현재 코드 구조상 MentalGauge가 UIInGame을 생성하고 있으므로 남겨둡니다.
        if (uiInGameInstance != null)
        {
            Destroy(uiInGameInstance);
            Debug.Log("[MentalGauge] Destroyed UIInGame instance.");
        }
    }
}
