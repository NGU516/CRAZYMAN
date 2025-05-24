// EnemyPatrol.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 순찰 담당 클래스
public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // 순찰 지점 목록
    public Transform[] LightOffPoints; // 전등 끄기 지점 목록
    public float waitTimeAtPatrol = 1f; // 순찰 지점 대기 시간
    public float patrolPriorityRange = 10f; // 우선 순찰 거리
    public float lightOffTime = 180f; // 전등 끄기 대기 시간
    public float normalSpeed = 3.5f; // 순찰 속도

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool isWaiting = false;
    private float lightOffTimer = 0f; // 전등 끄기 타이머
    private Transform player; // 플레이어 참조 (Transform player의 경우 Unity내에서 참조?)
    private float stuckTimer = 0f;
    private Vector3 lastPosition;
    // 압박/휴식 시스템
    private float pressureTimer = 0f;
    public float pressureDuration = 15f; // 플레이어 압박 시간
    public float relaxDuration = 10f;    // 휴식 시간
    private bool isPressuringPlayer = true;
    // 최근 방문 순찰지점 메모리
    private Queue<int> recentPatrolIndices = new Queue<int>();
    public int recentPatrolMemory = 3;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.enabled = true; // NavMeshAgent 활성화
    }

    void Update()
    {
        // 압박/휴식 타이머 갱신
        pressureTimer += Time.deltaTime;
        if (isPressuringPlayer && pressureTimer > pressureDuration)
        {
            isPressuringPlayer = false;
            pressureTimer = 0f;
        }
        else if (!isPressuringPlayer && pressureTimer > relaxDuration)
        {
            isPressuringPlayer = true;
            pressureTimer = 0f;
        }

        // 타이머 갱신
        lightOffTimer += Time.deltaTime;

        // 특정시간 경과 시 언제든지 특수 지점으로 강제 이동
        if (lightOffTimer >= lightOffTime && !isWaiting && !agent.pathPending)
        {
            MoveToLightPatrolPoint();
            lightOffTimer = 0f;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();
            if (door != null && !door.isOpen && !door.isLocked)
            {
                if (door.IsPlayerInRange)
                    door.ToggleDoor();
                else
                    Patrol(); // 다른 곳 순찰
            }
        }

        if (agent.hasPath && agent.remainingDistance > 0.5f)
        {
            // 실제로 거의 멈췄는지 velocity(속도)로 체크
            if (agent.velocity.magnitude < 0.05f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > 3f && agent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    MoveToNextPatrolPoint();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
            lastPosition = transform.position;
        }
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            stuckTimer = 0f; // 도착 시 무조건 리셋
        }
    }

    // 순찰 동작
    public void Patrol()
    {
        if (agent.speed != normalSpeed)
        {
            agent.speed = normalSpeed;
        }
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("StartPatrol 실패: patrolPoints가 비어 있음");
            return;
        }

        // 도착 전에는 아무것도 하지 않음
        if (agent.pathPending || isWaiting) return;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
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

    private bool IsReachable(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(target, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }
        return false;
    }

    // 다음 순찰 지점 이동
    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("순찰 지점 미설정: patrolPoints Array is empty.");
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        int nextIndex = -1;
        int attempts = 0;
        int maxAttempts = patrolPoints.Length * 2;
        while (attempts < maxAttempts)
        {
            // 기존 압박/휴식/랜덤 로직
            if (isPressuringPlayer && player != null)
            {
                List<int> priorityIndices = new List<int>();
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    float dist = Vector3.Distance(patrolPoints[i].position, player.position);
                    if (dist <= patrolPriorityRange && !recentPatrolIndices.Contains(i))
                    {
                        priorityIndices.Add(i);
                    }
                }
                if (priorityIndices.Count > 0)
                {
                    nextIndex = priorityIndices[Random.Range(0, priorityIndices.Count)];
                }
            }
            if (nextIndex == -1)
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (!recentPatrolIndices.Contains(i)) candidates.Add(i);
                }
                if (candidates.Count > 0)
                    nextIndex = candidates[Random.Range(0, candidates.Count)];
                else
                    nextIndex = Random.Range(0, patrolPoints.Length); // fallback
            }
            // 경로 유효성 체크
            if (IsReachable(patrolPoints[nextIndex].position))
            {
                break;
            }
            else
            {
                nextIndex = -1; // 다시 선택
            }
            attempts++;
        }
        // 최근 방문 기록
        recentPatrolIndices.Enqueue(nextIndex);
        if (recentPatrolIndices.Count > recentPatrolMemory)
            recentPatrolIndices.Dequeue();
        currentPatrolIndex = nextIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void MoveToLightPatrolPoint()
    {
        if (LightOffPoints == null || LightOffPoints.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, LightOffPoints.Length);
        agent.SetDestination(LightOffPoints[index].position);
        Debug.Log("전등 끄기 특수 지점으로 이동: " + LightOffPoints[index].name);
    }
}
