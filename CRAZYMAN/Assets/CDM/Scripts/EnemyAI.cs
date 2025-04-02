// EnemyAI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 상태 관리 클래스
public class EnemyAI : MonoBehaviour
{
    private EnemyPatrol patrol;
    private EnemyChase chase;
    
    public Transform player;
    public float chaseRange = 5f;    // 플레이어 감지 거리

    private enum State { Patrolling, Chasing }
    private State currentState;

    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();

        if (patrol == null || chase == null)
        {
            Debug.LogError("EnemyPatrol 또는 EnemyChase 컴포넌트가 필요합니다.");
            return;
        }

        currentState = State.Patrolling;
        patrol.StartPatrol(); // 처음에는 순찰 상태
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // chaseRange 이내에 플레이어가 있으면 추적 시작
        if (distanceToPlayer <= chaseRange)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Patrolling;
        }

        switch (currentState)
        {
            case State.Patrolling:
                patrol.Patrol();
                break;
            case State.Chasing:
                chase.Chase(player);
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
            currentState = State.Patrolling;
            StartCoroutine(patrol.WaitAtPatrolPoint());
        }
        else
        {
            Debug.Log($"{other.gameObject.name}와 충돌.");
        }
    }
}
