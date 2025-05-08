using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Control : MonoBehaviour
{
    public float v = 0.0f;
    public float h = 0.0f;
    public float moveSpeed = 5.0f;
    private float defaultSpeed = 5.0f;
    private float runSpeed = 10.0f;
    private float crouchSpeed = 2.0f;
    private float exhaustedSpeed = 3.0f; // 스테미나 소진 시 속도
    //public Transform PlayerTr;
    private Rigidbody rb;
    public Transform cameraTransform;

    public Animator animator;
    public bool canRun = false;
    public bool canCrouch = false;

    public MentalGauge mentalGauge;
    public GameManager gameManager;

    private StaminaSystem staminaSystem;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerTr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        staminaSystem = GetComponent<StaminaSystem>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        mentalGauge = FindObjectOfType<MentalGauge>();
        gameManager = FindObjectOfType<GameManager>();

        if(mentalGauge == null)
        {
            Debug.LogError("MentalGauge is not assigned in Control! Please assign it in the Inspector.");
        }
        if(gameManager == null)
        {
            Debug.LogError("MentalGauge is not assigned in Control! Please assign it in the Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 사망 상태면 이동 중지
        if (mentalGauge != null && mentalGauge.isDeath)
            return;

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        moveSpeed = defaultSpeed;

        bool isExhausted = staminaSystem != null && staminaSystem.IsExhausted;
        bool hasEnoughStamina = (staminaSystem != null) && staminaSystem.HasEnoughStamina(0.1f);

        if (isExhausted)
        {
            // 스테미나 소진 시 느린 속도 적용
            moveSpeed = exhaustedSpeed;
            canRun = false;
            canCrouch = false;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            canCrouch = true;
            canRun = false; // 앉는 중에는 달릴 수 없음
            moveSpeed = crouchSpeed;
            if (staminaSystem != null)
                staminaSystem.StopDraining();
        }
        else if (Input.GetKey(KeyCode.LeftShift) && v > 0.01f && hasEnoughStamina)
        {
            canRun = true;
            canCrouch = false; // 달리는 중에는 앉을 수 없음
            moveSpeed = runSpeed;
            if (staminaSystem != null)
                staminaSystem.StartDraining();
        }
        else
        {
            canRun = false;
            canCrouch = false;
            if (staminaSystem != null)
                staminaSystem.StopDraining();
        }

        bool isMoving = v > 0.01f || v < -0.01f;
        animator.SetBool("isMoving", isMoving);

        animator.SetFloat("v", v);
        animator.SetBool("canRun", canRun && hasEnoughStamina);
        animator.SetBool("canCrouch", canCrouch);
    }

    void FixedUpdate()
    {
        if (mentalGauge != null && mentalGauge.isDeath)
            return;

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
        if(other.CompareTag("Monster"))
        {
            Debug.Log("Player collided with Enemy! Triggering death.");
            if(mentalGauge != null && gameManager != null)
            {
                mentalGauge.TriggerDeath("EnemyCollision");
                gameManager.RequestDeath("EnemyCollision");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("OnCollisionEnter " + collision.gameObject.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        // Debug.Log("OnCollisionStay " + collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Debug.Log("OnCollisionExit " + collision.gameObject.name);
    }
}
