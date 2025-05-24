// EnemyChase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

// 추적 담당 클래스
public class EnemyChase : MonoBehaviour
{
    public float chaseSpeed = 5f; // 추적 속도
    public float normalSpeed = 2.5f; // 기본 속도
    public float chaseRange = 10f; // 플레이어 감지 거리
    public float chaseTimeLimit = 20f; // 추적 시간 제한 (초)
    public float chaseCoolDownTime = 5f; // 추적 쿨타임 (초)

    private EnemyAI enemyAI; // EnemyAI 스크립트 참조

    private NavMeshAgent agent;
    private float chaseTimer = 0f; // 추적 타이머
    private bool isChasing = false; // 추적 상태 플래그
    private bool isOnCooldown = false; // 추적 쿨타임 상태 플래그
    private EnemyPatrol patrol; // 순찰 스크립트 참조
    private EnemyAttack attack; // 시야 체크용

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false; 
        agent.enabled = true; 
        agent.speed = normalSpeed; // 기본 속도로 설정
        agent.updateRotation = true; // 회전 업데이트 활성화
        agent.updatePosition = true; // 위치 업데이트 활성화
        agent.updateUpAxis = true; // Y축 업데이트 활성화
        agent.angularSpeed = 720f; // 회전 속도 설정

        patrol = GetComponent<EnemyPatrol>();
        enemyAI = GetComponent<EnemyAI>();
        attack = GetComponent<EnemyAttack>(); // 추가
    }

    // 플레이어 추적 함수
    public void Chase(Transform player)
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool inSight = false;
        if (attack != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            inSight = angleToPlayer <= attack.fieldOfView * 0.5f && distanceToPlayer <= attack.viewDistance;
        }

        // 쿨타임 중에는 강제로 Patrol 상태 유지
        if (isOnCooldown)
        {
            if (enemyAI != null)
            {
                enemyAI.ForceStateToPatrol(); // 애니메이션 + 순찰 동작
            }
            return;
        }

        // 감지 범위 + 시야 내 → 추적 시작
        if (distanceToPlayer <= chaseRange && inSight)
        {
            if (!isChasing)
            {
                Debug.Log("플레이어 추적 시작");
                isChasing = true;
                chaseTimer = 0f;
            }

            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            chaseTimer += Time.deltaTime;

            if (chaseTimer >= chaseTimeLimit)
            {
                Debug.Log("추적 시간 초과 → 순찰 상태 강제 전환");
                isChasing = false;
                isOnCooldown = true;
                StartCoroutine(ForcePatrolMode());
                
            }
        }
        else
        {
            // 감지 범위 벗어남 또는 시야 밖 → 추적 종료
            if (isChasing)
            {
                StopChasing();
            }
        }
    }

    private IEnumerator ForcePatrolMode()
    {
        isOnCooldown = true;
        isChasing = false;
        chaseTimer = 0f;

        agent.speed = normalSpeed;

        if (enemyAI != null)
        {
            enemyAI.ForceStateToPatrol(); // 애니메이션 + 순찰
        }

        yield return new WaitForSeconds(chaseCoolDownTime);

        isOnCooldown = false;
    }

    private void StopChasing()
    {
        agent.speed = normalSpeed; // 기본 속도로 복귀
        isChasing = false;
        isOnCooldown = true; // 쿨타임 시작
        patrol.Patrol(); // 순찰 시작
    }
}

