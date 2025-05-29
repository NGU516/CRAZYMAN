using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AI; // NavMesh 관련 기능을 사용하기 위해 추가

public class Control : MonoBehaviour
{
    public float v = 0.0f;
    public float h = 0.0f;
    public float moveSpeed = 2.5f;
    private float defaultSpeed = 2.5f;
    private float runSpeed = 5.0f;
    private float crouchSpeed = 1.0f;
    private float exhaustedSpeed = 1.5f; // 스테미나 소진 시 속도
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
    IEnumerator Start()
    {
        //PlayerTr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        staminaSystem = GetComponent<StaminaSystem>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        while (mentalGauge == null || gameManager == null)
        {
            mentalGauge = FindObjectOfType<MentalGauge>();
            gameManager = FindObjectOfType<GameManager>();
            yield return null;
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
        Vector3 targetPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;

        // NavMesh 위에 있는지 확인
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 0.1f, NavMesh.AllAreas))
        {
            // NavMesh 위에 있다면 이동
            rb.MovePosition(targetPosition);
        }
        else
        {
            // NavMesh 위에 없다면 현재 위치 유지
            rb.MovePosition(rb.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        if(other.CompareTag("Monster"))
        {
            if(mentalGauge != null && gameManager != null)
            {
                mentalGauge.TriggerDeath("EnemyCollision");
                gameManager.RequestDeath("EnemyCollision");
            }
        }
        // 문 상호작용 디버그
        DoorController door = other.GetComponent<DoorController>();
        if (door != null)
        {
            var obstacle = door.doorObstacle;
            Debug.Log($"[Control] 플레이어가 상호작용한 문: {door.gameObject.name}, Obstacle.enabled: {obstacle?.enabled}");
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
