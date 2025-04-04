// EnemyChase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

// 추적 담당 클래스
public class EnemyChase : MonoBehaviour
{
    public float chaseSpeed = 6f; // 추격 속도
    public float normalSpeed = 3.5f; // 기본 속도
    public float chaseRange = 5f; // 플레이어 감지 거리
    public float chaseTimeLimit = 3f; // 추격 시간 제한 (초)
    public float chaseCoolDownTime = 5f; // 추격 쿨타임 (초)

    private NavMeshAgent agent;
    private float chaseTimer = 0f; // 추격 타이머
    private bool isChasing = false; // 추격 상태 플래그
    private bool isOnCooldown = false; // 추격 쿨타임 상태 플래그
    private EnemyPatrol patrol; // 순찰 스크립트 참조

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false; 
        agent.enabled = true; 
        agent.speed = normalSpeed; // 기본 속도로 설정

        patrol = GetComponent<EnemyPatrol>();
    }

    // 플레이어 추격 함수
    public void Chase(Transform player)
    {
        if (player == null || isOnCooldown) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어와의 거리와 추격 범위 비교
        if (distanceToPlayer > chaseRange)
        {
            StopChasing();
        }
        else
        {
            if (!isChasing)
            {
                Debug.Log("추격 시작");
                isChasing = true; // 추격 시작
                chaseTimer = 0f; // 타이머 초기화
            }

            agent.speed = chaseSpeed; // 추격 속도로 설정
            agent.SetDestination(player.position); // 플레이어 위치로 이동
            chaseTimer += Time.deltaTime; 

            if (chaseTimer >= chaseTimeLimit)
            {
                // 추격 시간 초과 시 쿨타임 시작
                isChasing = false;
                isOnCooldown = true; 
                StartCoroutine(ForcePatrolMode()); // 순찰 모드 강제 전환
            }

        }
    }

    private IEnumerator ForcePatrolMode()
    {
        Debug.Log("추격 시간 초과, 쿨타임 시작"); // 디버그 메시지

        isOnCooldown = true;
        isChasing = false;
        chaseTimer = 0f;

        agent.speed = normalSpeed;
        patrol.StartPatrol(); // 순찰 시작

        yield return new WaitForSeconds(chaseCoolDownTime); // 쿨타임 대기

        isOnCooldown = false;
    }

    private void StopChasing()
    {
        agent.speed = normalSpeed; // 기본 속도로 복귀
        isChasing = false;
        isOnCooldown = true; // 쿨타임 시작
        patrol.StartPatrol(); // 순찰 시작
    }
}

