// 멀티 플레이어 카메라, 정신력, 스테미너 게이지 UI 연결
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.UI;

public class PhotonControl : MonoBehaviourPunCallbacks
{
    public float v = 0.0f;
    public float h = 0.0f;
    public float moveSpeed = 2.5f;
    private float defaultSpeed = 2.5f;
    private float runSpeed = 5.0f;
    private float crouchSpeed = 1.0f;
    private float exhaustedSpeed = 1.5f;

    private Rigidbody rb;
    public Transform cameraTransform;

    public Animator animator;
    public bool canRun = false;
    public bool canCrouch = false;

    private MentalGauge mentalGauge;
    public GameManager gameManager;
    private StaminaSystem staminaSystem;
    public Camera myCam;

    private bool isDead = false;

    private bool isSettingActive = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        staminaSystem = GetComponent<StaminaSystem>();

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>(true);
            if (cam != null)
            {
                cameraTransform = cam.transform;
                Debug.Log("[PhotonControl] cameraTransform assigned: " + cameraTransform.name);
            } else
            {
                Debug.LogError("No camera found in player prefab");
            }
        }
    }

    IEnumerator Start()
    {
        // 내 플레이어라면 레이어 변경, 카메라 설정
        if (photonView.IsMine)
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("head"));
            myCam = transform.parent.GetComponentInChildren<Camera>(true);
            if (myCam != null)
            {
                myCam.enabled = true;
                myCam.cullingMask &= ~(1 << LayerMask.NameToLayer("head"));
                myCam.tag = "MainCamera";

                PhotonCameraMove camScript = myCam.GetComponent<PhotonCameraMove>();
                if (camScript != null)
                {
                    camScript.enabled = true;
                    camScript.SetTarget(this.transform);
                    myCam.transform.position = this.transform.position + Vector3.up * 1.6f;
                }
                else
                {
                    Debug.LogError("PhotonCameraMove component not found on camera!");
                }
            }
            else
            {
                Debug.LogError("Camera component not found in player prefab!");
            }

            // MentalGauge 찾기
            int attempts = 0;
            while (mentalGauge == null)
            {
                mentalGauge = FindObjectOfType<MentalGauge>();
                attempts++;
                if (attempts > 100) break;
                yield return null;
            }

            // GameManager 찾기
            attempts = 0;
            while (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
                attempts++;
                if (attempts > 100) break;
                yield return null;
            }
        }
        else
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
            // 다른 유저의 카메라/오디오 제거
            Camera otherCam = transform.parent.GetComponentInChildren<Camera>();
            if (otherCam != null)
            {
                otherCam.enabled = false;
                otherCam.tag = "Untagged";

                PhotonCameraMove camScript = otherCam.GetComponent<PhotonCameraMove>();
                if (camScript != null)
                {
                    camScript.enabled = false;
                }
            }

            AudioListener listener = transform.parent.GetComponentInChildren<AudioListener>();
            if (listener != null) listener.enabled = false;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // 죽음 상태 체크
        if (mentalGauge != null)
        {
            
            if (mentalGauge.isDeath)
            {
                if (!isDead)
                {
                    isDead = true;
                    Debug.Log("[PhotonControl] Player is now dead - Initializing death state");
                    // 죽음 상태일 때 모든 입력과 움직임을 중지
                    v = 0f;
                    h = 0f;
                    moveSpeed = 0f;
                    canRun = false;
                    canCrouch = false;
                    
                    // 애니메이션 업데이트
                    animator.SetBool("isMoving", false);
                    animator.SetFloat("v", 0f);
                    animator.SetBool("canRun", false);
                    animator.SetBool("canCrouch", false);

                    // Rigidbody 정지
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        Debug.Log("[PhotonControl] Rigidbody movement stopped");
                    }
                }
                return;
            }
        }
        else
        {
            Debug.LogWarning("[PhotonControl] MentalGauge is null!");
        }

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        moveSpeed = defaultSpeed;

        bool isExhausted = staminaSystem != null && staminaSystem.IsExhausted;
        bool hasEnoughStamina = (staminaSystem != null) && staminaSystem.HasEnoughStamina(0.1f);

        if (isExhausted)
        {
            moveSpeed = exhaustedSpeed;
            canRun = false;
            canCrouch = false;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            canCrouch = true;
            canRun = false;
            moveSpeed = crouchSpeed;
            staminaSystem?.StopDraining();
        }
        else if (Input.GetKey(KeyCode.LeftShift) && v > 0.01f && hasEnoughStamina)
        {
            canRun = true;
            canCrouch = false;
            moveSpeed = runSpeed;
            staminaSystem?.StartDraining();
        }
        else
        {
            canRun = false;
            canCrouch = false;
            staminaSystem?.StopDraining();
        }

        bool isMoving = Mathf.Abs(v) > 0.01f;
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("v", v);
        animator.SetBool("canRun", canRun && hasEnoughStamina);
        animator.SetBool("canCrouch", canCrouch);

        if (Input.GetKey(KeyCode.Escape) && isSettingActive == false)
        {
            isSettingActive = true;
            Managers.UI.ShowPopupUI<UISettingPopup>("UISettingPopup");
        }

        if (Input.GetKey(KeyCode.Escape) && isSettingActive == true)
        {
            isSettingActive = false;
            Managers.UI.ClosePopupUI();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (mentalGauge != null && mentalGauge.isDeath) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camRight * h + camForward * v).normalized;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Monster"))
        {
            if (mentalGauge != null)
            {
                Debug.Log("[PhotonControl] Triggering death through MentalGauge");
                mentalGauge.TriggerDeath("EnemyCollision");
            }
            else
            {
                Debug.LogError("[PhotonControl] MentalGauge is null!");
            }

            if (gameManager != null)
            {
                Debug.Log("[PhotonControl] Requesting death through GameManager");
                gameManager.RequestDeath("EnemyCollision");
            }
            else
            {
                Debug.LogError("[PhotonControl] GameManager is null!");
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void RecoverStamina(float amount)
    {
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }
        Debug.Log($"PhotonControl: 스테미너 회복 요청 받음! 양: {amount}");

        if (staminaSystem != null)
        {
            staminaSystem.RecoverStamina(amount); // StaminaSystem 스크립트의 RecoverStamina 함수 호출!
        }
        else
        {
            Debug.LogError("PhotonControl: StaminaSystem 스크립트 참조가 null입니다! 스테미너 회복 불가.");
        }
    }

    public void RecoverMental(float amount)
    {
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }
        Debug.Log($"PhotonControl: 정신력 회복 요청 받음! 양: {amount}");

        if (mentalGauge != null)
        {
            mentalGauge.RecoveryMental(amount); // 예: MentalGauge 스크립트에 RecoverSanity 함수가 있다면 호출
        }
        else
        {
            Debug.LogError("PhotonControl: MentalGauge 스크립트 참조가 null입니다! 정신력 회복 불가.");
        }
    }
}