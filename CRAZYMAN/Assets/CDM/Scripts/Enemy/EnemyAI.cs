// EnemyAI.cs
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// 상태 관리 클래스
public class EnemyAI : MonoBehaviour
{
    private Animator EnemyAnimator; // 애니메이터 컴포넌트
    private EnemyPatrol patrol;
    private EnemyChase chase;
    
    public Transform player;
    public float chaseRange = 5f;    // 플레이어 감지 거리

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

        SetState(EnemyState.Patrol); // 초기 상태 설정

        EnemyAnimator.applyRootMotion = false; // 루트 모션 비활성화
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        EnemyState nextState = (distanceToPlayer <= chaseRange)
            ? EnemyState.Chase
            : EnemyState.Patrol;

        if (currentState != nextState)
        {
            SetState(nextState);
        }

        // 상태에 따른 행동을 매 프레임 수행
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
        currentState = newState; // 현재 상태 업데이트
        EnemyAnimator.SetInteger("EnemyState", (int)newState); // 애니메이터 상태 변경

        // 0: Patrol, 1: Chase, 2: Blind, 3: Attack
        switch (newState)
        {
            case EnemyState.Patrol:
                patrol.Patrol(); 
                Debug.Log("순찰 상태");
                break;
            case EnemyState.Chase:
                chase.Chase(player); 
                Debug.Log("추적 상태");
                break;
            case EnemyState.Blind:
                // 블라인드 상태 처리 (구현 필요)
                break;
            case EnemyState.Attack:
                // 공격 상태 처리 (구현 필요)
                break;
            default:
                Debug.LogError("알 수 없는 상태입니다.");
                break;
        }
    }

    // 플레이어와 충돌했을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 충돌 발생! 플레이어 사망!");  

            // 플레이어 사망 처리 (구현 필요)

            // 일정 시간 대기 후 다시 순찰 시작
            StartCoroutine(patrol.WaitAtPatrolPoint());
        }
        else
        {
            Debug.Log($"{other.gameObject.name}와 충돌.");
        }
    }
}
