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
    public NavMeshObstacle doorObstacle;

    [Header("Interaction Settings")]
    public float interactionRadius = 2f;    // 상호작용 가능 거리
    public string playerTag = "Player";     // 플레이어 태그
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public string lockedMessage = "문이 잠겨있습니다."; // 잠긴 문 메시지
    public string openMessage = "F키를 눌러 문을 열기"; // 열기 메시지
    public string closeMessage = "F키를 눌러 문을 닫기"; // 닫기 메시지

    private bool isPlayerInRange = false;   // 플레이어가 상호작용 범위 내에 있는지
    private Quaternion[] initialRotations;  // 각 문의 초기 회전값
    private Quaternion[] targetRotations;   // 각 문의 목표 회전값
    private AudioEventRX audioEventRX;      // 오디오 이벤트 컴포넌트

    void Start()
    {
        // doorRoot가 비어 있으면 자기 자신으로 자동 할당
        if (doorRoot == null)
        {
            doorRoot = this.gameObject;
        }

        // doorObjects가 비어 있으면 자동으로 자식에서 이름이 'Door'인 오브젝트만 찾아서 할당
        if (doorObjects == null || doorObjects.Length == 0)
        {
            var allChildren = doorRoot.GetComponentsInChildren<Transform>(true);
            doorObjects = System.Array.FindAll(allChildren, t => t.name == "Door" && t != doorRoot.transform);
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
        // Collider가 없으면 자동으로 추가
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(interactionRadius * 2, 2f, interactionRadius * 2);
            collider.center = new Vector3(0, 1f, 0);
        }
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
                ToggleDoor();
            }
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        // 모든 문의 목표 회전값 업데이트
        for (int i = 0; i < doorObjects.Length; i++)
        {
            if (doorObjects[i] != null)
            {
                Vector3 baseEuler = initialRotations[i].eulerAngles;
                Vector3 targetEuler = isOpen
                    ? new Vector3(baseEuler.x, baseEuler.y + openAngle, baseEuler.z)
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
            doorObstacle.enabled = isOpen;
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

    // 디버깅용 Gizmo
    void OnDrawGizmosSelected()
    {
        // 상호작용 범위 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
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