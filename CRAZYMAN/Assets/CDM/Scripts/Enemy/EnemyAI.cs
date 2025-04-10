// EnemyAI.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 상태 관리 클래스
public class EnemyAI : MonoBehaviour
{
    private Animator EnemyAnimator;
    private EnemyPatrol patrol;
    private EnemyChase chase;

    public Transform player;
    public float chaseRange = 5f;
    public float attackRange = 2.5f;
    public float fieldOfView = 120f;
    public float attackCooldown = 2f;

    private float lastAttackTime;

    private enum EnemyState
    {
        Patrol = 0,
        Chase = 1,
        Blind = 2,
        Attack = 3,
    }

    private EnemyState currentState;

    void Start()
    {
        EnemyAnimator = GetComponent<Animator>();
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();

        if (patrol == null || chase == null)
        {
            Debug.LogError("EnemyPatrol 또는 EnemyChase 컴포넌트가 필요합니다.");
            return;
        }

        SetState(EnemyState.Patrol);
        EnemyAnimator.applyRootMotion = false;
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 공격 조건
        if (angle <= fieldOfView * 0.5f && distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                SetState(EnemyState.Attack);
                return;
            }
        }

        // 추적/순찰 상태 판별
        EnemyState nextState = (distanceToPlayer <= chaseRange)
            ? EnemyState.Chase : EnemyState.Patrol;

        if (currentState != nextState)
        {
            SetState(nextState);
        }

        // 상태별 동작 수행
        switch (currentState)
        {
            case EnemyState.Patrol:
                patrol.Patrol();
                break;
            case EnemyState.Chase:
                chase.Chase(player);
                break;
        }
    }

    void SetState(EnemyState newState)
    {
        currentState = newState;
        EnemyAnimator.SetInteger("EnemyState", (int)newState);

        switch (newState)
        {
            case EnemyState.Patrol:
                EnemyAnimator.ResetTrigger("Attack");
                EnemyAnimator.SetInteger("AttackIndex", -1);
                patrol.Patrol();
                Debug.Log("순찰 상태");
                break;
            case EnemyState.Chase:
                EnemyAnimator.ResetTrigger("Attack");
                EnemyAnimator.SetInteger("AttackIndex", -1);
                chase.Chase(player);
                Debug.Log("추적 상태");
                break;
            case EnemyState.Attack:
                int randomAttack = Random.Range(0, 3);
                EnemyAnimator.SetInteger("AttackIndex", randomAttack); // 먼저 설정
                EnemyAnimator.SetTrigger("Attack");                    // 마지막에 트리거
                Debug.Log($"공격 상태 - 패턴: {randomAttack}");
                lastAttackTime = Time.time;
                break;
            case EnemyState.Blind:
                Debug.Log("블라인드 상태");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 충돌 발생! 플레이어 사망!");
            StartCoroutine(patrol.WaitAtPatrolPoint());
        }
        else
        {
            Debug.Log($"{other.gameObject.name}와 충돌.");
        }
    }

    public void ForceStateToPatrol()
    {
        SetState(EnemyState.Patrol);
    }
}
