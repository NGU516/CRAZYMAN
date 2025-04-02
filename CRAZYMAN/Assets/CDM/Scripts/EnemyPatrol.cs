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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false; // NavMeshAgent 활성화
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
    void MoveToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("🚨 Patrol points list is empty! Assign patrol points in the Inspector.");
            return;
        }

        List<Transform> validPoints = new List<Transform>();

        foreach (var point in patrolPoints)
        {
            if (point != patrolPoints[currentPatrolIndex]) // 이전 순찰 지점 제외
            {
                validPoints.Add(point);
            }
        }

        if (validPoints.Count > 0)
        {
            currentPatrolIndex = Random.Range(0, validPoints.Count);
            Transform targetPoint = validPoints[currentPatrolIndex];

            Debug.Log($"➡ Moving to: {targetPoint.name} at {targetPoint.position}");
            agent.SetDestination(targetPoint.position);
        }
        else
        {
            Debug.LogWarning("⚠ No valid patrol points available.");
        }
    }


}
