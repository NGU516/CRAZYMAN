using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints; // 순찰 지점 목록
    public Transform player;        
    public float chaseRange = 5f;    // 플레이어 감지 거리

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private enum State { Patrolling, Chasing }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Patrolling;
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseRange)
            {
                currentState = State.Chasing;
            }
            else if (currentState == State.Chasing && distanceToPlayer > chaseRange)
            {
                currentState = State.Patrolling;
            }
        }
        else if (currentState != State.Patrolling) // Player가 없을 때 초기 순찰 상태로 설정
        {
            currentState = State.Patrolling;
        }

        // 순찰 상태일 때 새로운 경로로 이동 (NavMeshAgent 사용 시)
        if (currentState == State.Patrolling && agent.remainingDistance < 0.5f)
        {
            MoveToNextPatrolPoint();
        }

        // 추적 상태일 경우, 플레이어 위치로 이동
        if (currentState == State.Chasing)
        {
            agent.SetDestination(player.position);
        }
    }


    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned.");
            return; // 순찰 지점이 없으면 종료
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

        Debug.Log($"{patrolPoints[currentPatrolIndex].name}로 이동 중...");
    }
}

