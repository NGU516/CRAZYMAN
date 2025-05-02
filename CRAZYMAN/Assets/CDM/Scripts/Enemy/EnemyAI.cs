// EnemyAI.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 상태 관리 클래스
public class EnemyAI : MonoBehaviour
{
    // 필요한 컴포넌트들 참조
    private Animator EnemyAnimator; // 애니메이션
    private EnemyPatrol patrol; // 순찰
    private EnemyChase chase;   // 추적
    private EnemyAttack attack; // 공격 
    private MentalGauge mentalGauge; // 정신 게이지

    public Transform player;
    public float chaseRange = 5f;
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

    // 안정성을 위해 IEnumerator 사용
    IEnumerator Start()
    {
        EnemyAnimator = GetComponent<Animator>();
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();
        attack = GetComponent<EnemyAttack>();

        if (patrol == null || chase == null)
        {
            Debug.LogError("EnemyPatrol 또는 EnemyChase 컴포넌트가 필요합니다.");
            yield break;
        }

        yield return null; // 한 프레임 대기하여 초기화 완료

        SetState(EnemyState.Patrol);
        EnemyAnimator.applyRootMotion = false;
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        // 부채꼴 공격 판정 검사 (EnemyAttack.cs에서 구현)
        if (attack != null && attack.IsPlayerInAttackCone())
        {
            // 쿨타임이 지났다면 공격 상태로
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                SetState(EnemyState.Attack);
                return;  
            }
        }

        // 추적/순찰 상태 판별
        // (공격 조건 아니거나 쿨타임 중이면)
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        EnemyState nextState = (distanceToPlayer <= chaseRange)
            ? EnemyState.Chase
            : EnemyState.Patrol;

        if (currentState != nextState)
        {
            SetState(nextState);
        }

        // 상태별 매 프레임 동작
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

    // 상태 변경 메서드
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
                // mentalGauge.TriggerDeath("플레이어 사망");
                break;
            case EnemyState.Blind:
                Debug.Log("블라인드 상태");
                break;
        }
    }

    // 충돌 감지
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 충돌 발생! 플레이어 사망!");
            // mentalGauge.TriggerDeath("플레이어 사망");
            StartCoroutine(patrol.WaitAtPatrolPoint());
        }

        else
        {
            // Debug.Log($"{other.gameObject.name}와 충돌.");
        }
    }

    // 강제 상태 변경
    public void ForceStateToPatrol()
    {
        SetState(EnemyState.Patrol);
    }
    
}
