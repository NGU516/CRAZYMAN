using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorRoot;              // Door GameObject들을 넣는 슬롯
    public Transform[] doorObjects;          // 제어할 문 오브젝트들
    public float openAngle = 90f;           // 문이 열리는 각도
    public float closeAngle = 0f;           // 문이 닫히는 각도
    public float smoothSpeed = 2f;          // 문이 열리고 닫히는 속도
    public bool isOpen = false;             // 문의 현재 상태
    public bool isLocked = false;           // 문이 잠겨있는지 여부
    public NavMeshObstacle doorObstacle;    // 문 장애물 처리 컴포넌트

    [Header("Interaction Settings")]
    public string playerTag = "Player";     // 플레이어 태그
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public string lockedMessage = "문이 잠겨있습니다."; // 잠긴 문 메시지
    public string openMessage = "F키를 눌러 문을 열기"; // 열기 메시지
    public string closeMessage = "F키를 눌러 문을 닫기"; // 닫기 메시지

    private bool isPlayerInRange = false;   // 플레이어가 상호작용 범위 내에 있는지
    private Quaternion[] initialRotations;  // 각 문의 초기 회전값
    private Quaternion[] targetRotations;   // 각 문의 목표 회전값
    private AudioEventRX audioEventRX;      // 오디오 이벤트 컴포넌트

    public bool IsPlayerInRange => isPlayerInRange; // 외부에서 읽기 전용 접근자

    void Start()
    {
        // doorRoot가 비어 있으면 자기 자신으로 자동 할당
        if (doorRoot == null)
        {
            doorRoot = this.gameObject;
        }

        // doorObjects가 비어 있거나 None(Transform)이 들어가 있으면 자동 할당
        bool needsAutoAssign = (doorObjects == null || doorObjects.Length == 0 || (doorObjects.Length == 1 && doorObjects[0] == null));
        if (needsAutoAssign)
        {
            var allChildren = doorRoot.GetComponentsInChildren<Transform>(true);
            var found = System.Array.FindAll(allChildren, t => t.name == "Door" && t != doorRoot.transform);
            if (found.Length == 0) {
                // 자식이 없으면 자기 자신을 DoorObjects로 할당
                doorObjects = new Transform[] { doorRoot.transform };
            } else {
                doorObjects = found;
            }
        }

        // 각 문의 초기 회전값 및 목표 회전값 설정
        initialRotations = new Quaternion[doorObjects.Length];
        targetRotations = new Quaternion[doorObjects.Length];
        for (int i = 0; i < doorObjects.Length; i++)
        {
            if (doorObjects[i] != null)
            {
                initialRotations[i] = doorObjects[i].rotation;
                targetRotations[i] = doorObjects[i].rotation;
            }
        }
        // 오디오 이벤트 컴포넌트 가져오기
        audioEventRX = GetComponent<AudioEventRX>();
        if (audioEventRX == null)
        {
            audioEventRX = gameObject.AddComponent<AudioEventRX>();
        }
        // 항상 BoxCollider를 붙인다 (이미 있으면 중복 추가 안 됨)
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            // 원하는 크기와 위치로 설정 (예시)
            collider.size = new Vector3(0.1f, 2f, 1f);   // x, y, z 크기
            collider.center = new Vector3(0f, 1f, 0f);   // x, y, z 중심 위치
        }
        // collider.size, collider.center는 Inspector에서 직접 조절도 가능
    }

    void Update()
    {
        // 각 문의 회전 처리
        for (int i = 0; i < doorObjects.Length; i++)
        {
            if (doorObjects[i] != null)
            {
                doorObjects[i].rotation = Quaternion.Slerp(
                    doorObjects[i].rotation, 
                    targetRotations[i], 
                    Time.deltaTime * smoothSpeed
                );
            }
        }
        // 플레이어가 상호작용 범위 내에 있을 때
        if (isPlayerInRange)
        {
            // 상호작용 키를 누르면 문 상태 토글
            if (Input.GetKeyDown(interactionKey))
            {
                if (isLocked)
                {
                    return;
                }
                // NetworkDoor를 통해 문 상태 변경 요청
                NetworkDoor networkDoor = GetComponent<NetworkDoor>();
                if (networkDoor != null)
                {
                    networkDoor.RequestToggleDoor(transform.position);
                }
                else
                {
                    // NetworkDoor가 없는 경우 (싱글플레이 등) 기존 방식으로 처리
                    Transform playerTr = GameObject.FindGameObjectWithTag(playerTag)?.transform;
                    ToggleDoor(playerTr);
                }
            }
        }
    }

    public void ToggleDoor(Transform player = null)
    {
        isOpen = !isOpen;
        for (int i = 0; i < doorObjects.Length; i++)
        {
            if (doorObjects[i] != null)
            {
                Vector3 baseEuler = initialRotations[i].eulerAngles;
                float angle = openAngle;

                // 플레이어가 있으면 방향에 따라 열리는 각도 결정
                if (player != null)
                {
                    Vector3 doorForward = doorObjects[i].forward;
                    Vector3 toPlayer = (player.position - doorObjects[i].position).normalized;
                    float dot = Vector3.Dot(doorForward, toPlayer);

                    // dot > 0: 문 앞, dot < 0: 문 뒤
                    angle = (dot > 0) ? openAngle : -openAngle;
                }

                Vector3 targetEuler = isOpen
                    ? new Vector3(baseEuler.x, baseEuler.y + angle, baseEuler.z)
                    : baseEuler;
                targetRotations[i] = Quaternion.Euler(targetEuler);
            }
        }
        // 문 소리 재생
        if (audioEventRX != null)
        {
            audioEventRX.PlayDoorinteractSound();
        }
        if (doorObstacle != null)
            doorObstacle.enabled = !isOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
        }
    }

    // 문 잠금 상태 변경
    public void SetLocked(bool locked)
    {
        isLocked = locked;
        if (locked && isOpen)
        {
            // 잠금 상태로 변경 시 문이 열려있으면 닫기
            isOpen = false;
            for (int i = 0; i < doorObjects.Length; i++)
            {
                if (doorObjects[i] != null)
                {
                    Vector3 baseEuler = initialRotations[i].eulerAngles;
                    targetRotations[i] = Quaternion.Euler(baseEuler);
                }
            }
        }
        if (doorObstacle != null)
            doorObstacle.enabled = !isOpen;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // doorRoot가 비어 있으면 자기 자신으로 자동 할당
        if (doorRoot == null)
        {
            doorRoot = this.gameObject;
        }
        if ((doorObjects == null || doorObjects.Length == 0) && doorRoot != null)
        {
            var allChildren = doorRoot.GetComponentsInChildren<Transform>(true);
            doorObjects = System.Array.FindAll(allChildren, t => t.name == "Door" && t != doorRoot.transform);
        }
    }
#endif
} 