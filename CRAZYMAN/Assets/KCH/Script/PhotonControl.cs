using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PhotonControl : MonoBehaviourPun
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

    public MentalGauge mentalGauge;
    public GameManager gameManager;

    private StaminaSystem staminaSystem;

    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        staminaSystem = GetComponent<StaminaSystem>();

        // 내 플레이어라면 레이어 변경, 카메라 설정
        if (photonView.IsMine)
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("MyPlayer"));

            Camera myCam = GetComponentInChildren<Camera>();
            if (myCam != null)
            {
                myCam.cullingMask &= ~(1 << LayerMask.NameToLayer("MyPlayer"));
            }

            Camera.main.GetComponent<PhotonCameraMove>()?.SetTarget(this.transform);
        }
        else
        {
            // 다른 유저의 카메라/오디오 제거
            Camera otherCam = GetComponentInChildren<Camera>();
            if (otherCam != null) otherCam.gameObject.SetActive(false);
            AudioListener listener = GetComponentInChildren<AudioListener>();
            if (listener != null) listener.enabled = false;
        }

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        while (mentalGauge == null || gameManager == null)
        {
            mentalGauge = FindObjectOfType<MentalGauge>();
            gameManager = FindObjectOfType<GameManager>();
            yield return null;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (mentalGauge != null && mentalGauge.isDeath) return;

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
            if (mentalGauge != null && gameManager != null)
            {
                mentalGauge.TriggerDeath("EnemyCollision");
                gameManager.RequestDeath("EnemyCollision");
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
}