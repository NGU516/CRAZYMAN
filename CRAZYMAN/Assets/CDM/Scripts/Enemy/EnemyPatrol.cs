// EnemyPatrol.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// 순찰 담당 클래스
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float waitTimeAtPatrol = 1f; // 순찰 지점 대기 시간
    public float patrolPriorityRange = 10f; // 우선 순찰 거리
    public float lightOffTime = 10f; // 전등 끄기 대기 시간
    public float normalSpeed = 3.5f; // 순찰 속도
    public float pressureDuration = 15f; // 플레이어 압박 시간
    public float relaxDuration = 10f;    // 휴식 시간
    public float pressureRange = 15f;    // 압박 모드에서의 플레이어 감지 거리
    public float relaxRange = 5f;        // 휴식 모드에서의 플레이어 감지 거리
    public float doorInteractionRange = 5f; // 문 상호작용 가능 거리
    public float playerDetectionRange = 10f; // 플레이어 감지 거리
    public int recentPatrolMemory = 3;
    public LightOff specialLightTarget; // ElectoPanel의 LightOff를 자동 할당

    private Transform[] patrolPoints; // 순찰 지점 목록
    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool isWaiting = false;
    private float lightOffTimer = 0f; // 전등 끄기 타이머
    private Transform player; // 플레이어 참조
    private float stuckTimer = 0f;
    private Vector3 lastPosition;
    private float stuckThreshold = 0.1f; // 위치 변화 허용 오차
    private float stuckTimeLimit = 5f;   // 5초
    private float pressureTimer = 0f;
    private bool isPressuringPlayer = true;
    private Queue<int> recentPatrolIndices = new Queue<int>();  // 최근 방문 기록

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.enabled = true; // NavMeshAgent 활성화
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 자동으로 특수 순찰 목표 LightOff 할당
        specialLightTarget = FindObjectOfType<LightOff>();
        if (specialLightTarget != null)
            Debug.Log($"[EnemyPatrol] 자동 할당된 특수 순찰 목표: {specialLightTarget.name}");
        else
            Debug.LogWarning("[EnemyPatrol] LightOff 오브젝트를 찾지 못했습니다!");

        // 자동으로 PatrolPoints 찾기
        GameObject patrolPointsParent = GameObject.Find("PatrolPoints");
        if (patrolPointsParent != null)
        {
            int childCount = patrolPointsParent.transform.childCount;
            if (childCount > 0)
            {
                patrolPoints = new Transform[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    patrolPoints[i] = patrolPointsParent.transform.GetChild(i);
                }
            }
            else
            {
                patrolPoints = new Transform[0];
            }
        }
        else
        {
            patrolPoints = new Transform[0];
        }

        // 초기 순찰 시작
        if (patrolPoints.Length > 0)
        {
            MoveToNextPatrolPoint();
        }

        lightOffTimer = 0f; // Start에서 명시적으로 0으로 초기화
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

        // 10초마다 특수 순찰 목표로 이동
        lightOffTimer += Time.deltaTime;
        if (lightOffTimer >= lightOffTime && specialLightTarget == null)
        {
            specialLightTarget = FindObjectOfType<LightOff>();
            if (specialLightTarget != null)
            {
                Debug.Log($"[EnemyPatrol] lightOfftime 경과, ElectoPanel 특수 순찰 시작: {specialLightTarget.name}");
                agent.SetDestination(specialLightTarget.transform.position);
            }
            else
            {
                Debug.LogWarning("[EnemyPatrol] 특수 순찰 목표(LightOff) 없음");
            }
        }

        // 특수 순찰 목표가 있을 때 계속 이동만 시도 (도달 체크 X)
        if (specialLightTarget != null)
        {
            if (!agent.pathPending && (agent.pathStatus != NavMeshPathStatus.PathComplete || agent.remainingDistance > 1.0f))
            {
                agent.SetDestination(specialLightTarget.transform.position);
            }
        }
        else
        {
            Patrol(); // 일반 순찰
        }

        // 위치 변화량 체크 (stuckTimer)
        if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckTimeLimit)
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
            return;
        }

        int nextIndex = -1;
        List<int> candidates = new List<int>();

        // 압박 모드일 때는 플레이어 근처의 순찰 지점을 우선적으로 선택
        if (isPressuringPlayer && IsPlayerInRange(pressureRange))
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                float dist = Vector3.Distance(patrolPoints[i].position, player.position);
                if (dist <= pressureRange && !recentPatrolIndices.Contains(i))
                {
                    candidates.Add(i);
                }
            }
        }

        // 일반적인 순찰 지점 선택
        if (candidates.Count == 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (!recentPatrolIndices.Contains(i))
                    candidates.Add(i);
            }
        }

        // 후보가 없으면 최근 방문 기록을 무시하고 모든 지점을 후보로 추가
        if (candidates.Count == 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                candidates.Add(i);
            }
        }

        // 후보들 중에서 도달 가능한 첫 번째 지점을 선택
        foreach (int index in candidates)
        {
            if (IsReachable(patrolPoints[index].position))
            {
                nextIndex = index;
                break;
            }
        }

        // 도달 가능한 지점을 찾았다면 이동
        if (nextIndex != -1)
        {
            recentPatrolIndices.Enqueue(nextIndex);
            if (recentPatrolIndices.Count > recentPatrolMemory)
                recentPatrolIndices.Dequeue();
            currentPatrolIndex = nextIndex;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            // 도달 가능한 지점이 없는 경우, agent를 멈춤
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ElectoPanel(특수 순찰 목표)와 충돌 시에만 상태 리셋
        if (specialLightTarget != null && other.gameObject == specialLightTarget.gameObject)
        {
            NetworkLight networkLight = specialLightTarget.GetComponent<NetworkLight>();
            if (networkLight != null && Photon.Pun.PhotonNetwork.IsMasterClient)
            {
                networkLight.RequestTurnOffLight();
            }
            else if (specialLightTarget != null)
            {
                specialLightTarget.TurnOffLight();
            }
            specialLightTarget = null;
            lightOffTimer = 0f;
            MoveToNextPatrolPoint();
            return;
        }
        // 기존 문 충돌 등은 그대로 유지
        DoorController door = other.GetComponent<DoorController>();
        if (door != null)
        {
            var obstacle = door.doorObstacle;
            float playerDist = player ? Vector3.Distance(transform.position, player.position) : float.MaxValue;
            if (obstacle != null && obstacle.enabled)
            {
                NetworkDoor networkDoor = door.GetComponent<NetworkDoor>();
                if (networkDoor != null)
                {
                    networkDoor.RequestToggleDoor(Vector3.zero);
                }
                else
                {
                    door.ToggleDoor(); // fallback (싱글플레이 등)
                }
            }
            else
            {
                MoveToNextPatrolPoint();
            }
        }
    }
}
