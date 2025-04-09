// EnemyPatrol.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 순찰 담당 클래스
public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // 순찰 지점 목록
    public float waitTimeAtPatrol = 1f; // 순찰 지점 대기 시간
    public float patrolPriorityRange = 10f; // 우선 순찰 거리

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool isWaiting = false;
    private Transform player; // 플레이어 참조 (Transform player의 경우 Unity내에서 참조?)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.enabled = true; // NavMeshAgent 활성화
    }

    // 순찰 시작 (초기화)
    public void StartPatrol()
    {
        MoveToNextPatrolPoint();
    }

    // 순찰 동작
    public void Patrol()
    {
        if (agent.remainingDistance < 0.5f && !isWaiting)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    // 순찰 지점 대기
    public IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPatrol);
        isWaiting = false;
        MoveToNextPatrolPoint();
    }

    // 다음 순찰 지점 이동
    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("순찰 지점 미설정: patrolPoints Array is empty.");
            return;
        }

        // 플레이어가 존재하는지 확인, 없을 시 기본 순찰 진행
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다: Player not found.");
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            return;
        }

        // 플레이어 존재 시 일정 반경 내 우선 순찰지점을 찾음
        // Transform Type(게임 오브젝트 위치, 회전, 크기 등 포함) 동적 배열
        List<Transform> priorityPoints = new List<Transform>();
        foreach (var point in patrolPoints)
        {
            float distanceToPlayer = Vector3.Distance(point.position, player.position);
            if (distanceToPlayer <= patrolPriorityRange)
            {
                priorityPoints.Add(point);
            }
        }

        // 플레이어 주변 순찰지점이 있으면 우선 선택, 없으면 기존 방식대로 선택
        if (priorityPoints.Count > 0)
        {
            currentPatrolIndex = Random.Range(0, priorityPoints.Count);
            agent.SetDestination(priorityPoints[currentPatrolIndex].position);
        }
        else
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
}
