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
    private float stuckThreshold = 0.1f; // 위치 변화 허용 오차
    private float stuckTimeLimit = 5f;   // 5초
    // 압박/휴식 시스템
    private float pressureTimer = 0f;
    public float pressureDuration = 15f; // 플레이어 압박 시간
    public float relaxDuration = 10f;    // 휴식 시간
    private bool isPressuringPlayer = true;
    // 최근 방문 순찰지점 메모리
    private Queue<int> recentPatrolIndices = new Queue<int>();
    public int recentPatrolMemory = 3;

    public float doorInteractionRange = 5f; // 문 상호작용 가능 거리
    public float playerDetectionRange = 10f; // 플레이어 감지 거리
    private float doorStuckTimer = 0f; // 문 앞에서 멈춰있는 시간
    private Vector3 lastDoorPosition; // 마지막으로 시도한 문의 위치
    private float doorRetryTime = 5f; // 문 재시도 대기 시간

    public float pressureRange = 15f;    // 압박 모드에서의 플레이어 감지 거리
    public float relaxRange = 5f;        // 휴식 모드에서의 플레이어 감지 거리

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.enabled = true; // NavMeshAgent 활성화
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // 압박/휴식 타이머 갱신
        pressureTimer += Time.deltaTime;
        if (isPressuringPlayer && pressureTimer > pressureDuration)
        {
            isPressuringPlayer = false;
            pressureTimer = 0f;
            Debug.Log("휴식 모드로 전환");
        }
        else if (!isPressuringPlayer && pressureTimer > relaxDuration)
        {
            isPressuringPlayer = true;
            pressureTimer = 0f;
            Debug.Log("압박 모드로 전환");
        }

        // 타이머 갱신
        lightOffTimer += Time.deltaTime;

        // 특정시간 경과 시 언제든지 특수 지점으로 강제 이동
        if (lightOffTimer >= lightOffTime && !isWaiting && !agent.pathPending)
        {
            MoveToLightPatrolPoint();
            lightOffTimer = 0f;
        }

        // 위치 변화량 체크 (stuckTimer)
        if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckTimeLimit)
            {
                Debug.Log($"[EnemyPatrol] stuck! {gameObject.name}가 {stuckTimeLimit}초 동안 위치 {transform.position}에서 멈춤. 다른 순찰지점으로 이동");
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

    // 플레이어가 특정 범위 내에 있는지 확인
    private bool IsPlayerInRange(float range)
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) return false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= range;
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

        int nextIndex = -1;
        int attempts = 0;
        int maxAttempts = patrolPoints.Length * 2;
        List<int> triedIndices = new List<int>();
        bool foundReachable = false;

        while (attempts < maxAttempts)
        {
            // 압박 모드일 때는 플레이어 근처의 순찰 지점을 우선적으로 선택
            if (isPressuringPlayer && IsPlayerInRange(pressureRange))
            {
                List<int> priorityIndices = new List<int>();
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (triedIndices.Contains(i)) continue;
                    float dist = Vector3.Distance(patrolPoints[i].position, player.position);
                    if (dist <= pressureRange && !recentPatrolIndices.Contains(i))
                    {
                        priorityIndices.Add(i);
                    }
                }
                if (priorityIndices.Count > 0)
                {
                    nextIndex = priorityIndices[Random.Range(0, priorityIndices.Count)];
                }
            }

            // 일반적인 순찰 지점 선택
            if (nextIndex == -1)
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (!triedIndices.Contains(i) && !recentPatrolIndices.Contains(i))
                        candidates.Add(i);
                }
                if (candidates.Count > 0)
                    nextIndex = candidates[Random.Range(0, candidates.Count)];
                else
                {
                    // 모든 지점을 시도했다면 최근 방문 기록을 무시하고 랜덤 선택
                    nextIndex = Random.Range(0, patrolPoints.Length);
                }
            }

            // 이미 시도한 지점은 제외
            if (triedIndices.Contains(nextIndex))
            {
                nextIndex = -1;
                attempts++;
                continue;
            }

            // 경로 유효성 체크
            if (IsReachable(patrolPoints[nextIndex].position))
            {
                foundReachable = true;
                break;
            }
            else
            {
                triedIndices.Add(nextIndex);
                nextIndex = -1;
            }
            attempts++;
        }

        if (foundReachable && nextIndex != -1)
        {
            recentPatrolIndices.Enqueue(nextIndex);
            if (recentPatrolIndices.Count > recentPatrolMemory)
                recentPatrolIndices.Dequeue();
            currentPatrolIndex = nextIndex;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            Debug.Log($"[EnemyPatrol] 이동 시도: {patrolPoints[currentPatrolIndex].position}, pathStatus: {agent.pathStatus}, hasPath: {agent.hasPath}, remainingDistance: {agent.remainingDistance}, velocity: {agent.velocity}");
        }
        else
        {
            // 모든 경로가 실패한 경우, agent를 멈추고 로그 출력
            agent.isStopped = true;
            Debug.LogWarning($"[EnemyPatrol] 모든 순찰지점이 막혀있음! {gameObject.name}가 이동 불가 상태. 위치: {transform.position}");
        }
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

    private void OnTriggerEnter(Collider other)
    {
        DoorController door = other.GetComponent<DoorController>();
        if (door != null)
        {
            var obstacle = door.doorObstacle;
            float playerDist = player ? Vector3.Distance(transform.position, player.position) : float.MaxValue;
            if (obstacle != null && obstacle.enabled)
            {
                if (playerDist <= 30f)
                {
                    // 플레이어가 근처에 있으면 문을 연다
                    door.ToggleDoor();
                    Debug.Log($"[EnemyPatrol] OnTrigger 문 열기 시도: {door.gameObject.name}, Obstacle.enabled: {obstacle.enabled}");
                }
                else
                {
                    // 플레이어가 멀리 있으면 다른 순찰지점으로 이동
                    Debug.Log($"[EnemyPatrol] OnTrigger 문 닫힘 & 플레이어 멀리 있음: {door.gameObject.name}, 다른 순찰지점 이동");
                    MoveToNextPatrolPoint();
                }
            }
        }
    }
}
