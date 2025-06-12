// EnemyAI.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// 상태 관리 클래스
public class EnemyAI : MonoBehaviour
{
    // 필요한 컴포넌트들 참조
    private Animator EnemyAnimator; // 애니메이션
    private EnemyPatrol patrol; // 순찰
    private EnemyChase chase;   // 추적
    private EnemyAttack attack; // 공격 
    private MentalGauge mentalGauge; // 정신 게이지
    private NavMeshAgent agent;
    private PhotonView photonView;

    public Transform player;
    public float chaseRange = 5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    private float health = 100f;

    private enum EnemyState
    {
        Patrol = 0,
        Chase = 1,
        Blind = 2,
        Attack = 3,
    }

    private EnemyState currentState;
    private Coroutine blindCoroutine;

    // 안정성을 위해 IEnumerator 사용
    IEnumerator Start()
    {
        EnemyAnimator = GetComponent<Animator>();
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();
        attack = GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();

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
        
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
            }
            else
            {
                return; // 아직 못 찾았으면 동작 보류
            }
        }
        
        if (agent != null)
        {
            // Debug.Log($"괴인 현재 속도: {agent.velocity.magnitude:F2} (실제 이동 속도)");
        }

        if (attack != null && attack.player == null)
        {
            attack.player = player;
        }

        // 공격 조건
        if (attack != null && attack.IsPlayerInAttackCone())
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                SetState(EnemyState.Attack);
                return;
            }
        }
        
        // 시야 내 감지 → 추적
        bool playerInSight = attack != null && attack.IsPlayerInSight(attack.viewDistance); // viewDistance를 EnemyAttack에서 가져옴
        EnemyState nextState = playerInSight ? EnemyState.Chase : EnemyState.Patrol;

        if (currentState != nextState)
            SetState(nextState);

        // 상태별 처리
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
        // Debug.Log($"괴인 상태 변경: {currentState}");
        EnemyAnimator.SetInteger("EnemyState", (int)newState);

        // 
        if (PhotonNetwork.IsMasterClient)
        {
            NetworkEnemy network = GetComponent<NetworkEnemy>();
            if (network != null)
                network.photonView.RPC("SetState", RpcTarget.Others, (int)newState);
        }

        switch (newState)
        {
            case EnemyState.Patrol:
                EnemyAnimator.ResetTrigger("Attack");
                EnemyAnimator.SetInteger("AttackIndex", -1);
                patrol.Patrol();
                break;
            case EnemyState.Chase:
                EnemyAnimator.ResetTrigger("Attack");
                EnemyAnimator.SetInteger("AttackIndex", -1);
                chase.Chase(player);
                break;
            case EnemyState.Attack:
                int randomAttack = Random.Range(0, 3);
                EnemyAnimator.SetInteger("AttackIndex", randomAttack); // 먼저 설정
                EnemyAnimator.SetTrigger("Attack");                    // 마지막에 트리거
                lastAttackTime = Time.time;
                if (mentalGauge != null)
                {
                    mentalGauge.TriggerDeath("플레이어 사망");
                }
                break;
            case EnemyState.Blind:
                EnemyAnimator.SetBool("BlindState", true);
                SetBlind(true);
                break;
        }
    }

    // 플레이어 타겟 설정 메서드
    public void SetTarget(Transform target)
    {
        player = target;
    }

    // 데미지 처리 메서드
    public void TakeDamage(float damage, int attackerViewID)
    {
        if (mentalGauge != null)
        {
            mentalGauge.TriggerDeath("플레이어 사망");
        }
    }

    // 충돌 감지
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (mentalGauge != null)
            {
                mentalGauge.TriggerDeath("플레이어 사망");
            }
            // 기존 플레이어 충돌 처리
            StartCoroutine(patrol.WaitAtPatrolPoint());
        }
        else if (other.GetComponent<DoorController>() != null)
        {
            DoorController door = other.GetComponent<DoorController>();
            // 플레이어가 문 근처에 있으면 문 열기
            if (door.IsPlayerInRange && !door.isOpen && !door.isLocked)
            {
                door.ToggleDoor();
            }
            // 플레이어가 근처에 없고 문이 닫혀 있으면 다른 곳 순찰
            else if (!door.isOpen)
            {
                patrol.Patrol();
            }
        }
    }

    // 강제 상태 변경
    public void ForceStateToPatrol()
    {
        SetState(EnemyState.Patrol);
    }

    public void SetBlind(bool isBlind)
    {
        Debug.Log("[EnemyAI] SetBlind: " + isBlind);
        if (isBlind)
        {
            SetState(EnemyState.Blind);
            EnemyAnimator.SetBool("BlindState", true);
            if (agent != null) agent.isStopped = true; // 움직임 멈춤

            // 이미 코루틴이 돌고 있다면 중지
            if (blindCoroutine != null)
                StopCoroutine(blindCoroutine);
            // 3~5초 랜덤 멈춤
            blindCoroutine = StartCoroutine(BlindDurationCoroutine());
        }
        else
        {
            SetState(EnemyState.Patrol); // 필요시 이전 상태로 복귀
            EnemyAnimator.SetBool("BlindState", false);
            if (agent != null) agent.isStopped = false; // 다시 움직임

            if (blindCoroutine != null)
            {
                StopCoroutine(blindCoroutine);
                blindCoroutine = null;
            }
        }
    }

    private IEnumerator BlindDurationCoroutine()
    {
        float waitTime = Random.Range(3f, 5f);
        yield return new WaitForSeconds(waitTime);
        SetBlind(false); // 자동으로 Blind 해제
    }

}
