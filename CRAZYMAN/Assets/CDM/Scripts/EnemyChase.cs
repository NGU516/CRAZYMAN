// EnemyChase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 추적 담당 클래스
public class EnemyChase : MonoBehaviour
{
    public float chaseSpeed = 6f; // 추격 속도
    public float normalSpeed = 3.5f; // 기본 속도
    public float chaseRange = 5f; // 플레이어 감지 거리

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false; // NavMeshAgent 활성화
        agent.enabled = true; // NavMeshAgent 활성화
        agent.speed = normalSpeed; // 기본 속도로 설정
    }

    // 플레이어를 추격하는 동작
    public void Chase(Transform player)
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange)
        {
            agent.speed = normalSpeed; // 플레이어를 놓치면 다시 순찰 속도로 변경
        }
        else
        {
            agent.speed = chaseSpeed; // 추격 속도 증가
            agent.SetDestination(player.position);
        }
    }
}

