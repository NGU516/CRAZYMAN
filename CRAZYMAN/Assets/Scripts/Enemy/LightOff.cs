using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOff : MonoBehaviour
{
    public GameObject lampRoot;              // Lamp GameObject를 넣는 슬롯
    // 제어할 Light 컴포넌트
    public Light[] allLight;

    // 몬스터 감지 범위
    public float detectionRadius = 5f;

    // 감지 대상 태그 (기본값: "Monster")
    public string monster = "Monster";

    // 전등 상태 추적
    public bool isLightOn = true;

    public float interactionRadius = 2f;     // 플레이어 상호작용 범위
    public string playerTag = "Player";      // 플레이어 태그
    public KeyCode interactionKey = KeyCode.E; // 상호작용 키

    private bool isPlayerInRange = false;    // 플레이어가 상호작용 범위 내에 있는지

    void Start()
    {
        // allLight가 비어 있으면 자동으로 자식에서 Light 컴포넌트 찾아서 할당
        if ((allLight == null || allLight.Length == 0) && lampRoot != null)
        {
            allLight = lampRoot.GetComponentsInChildren<Light>();
            Debug.Log($"{name}에서 자동으로 {allLight.Length}개의 Light를 할당했습니다.");
        }
    }

    void Update()
    {
        Debug.Log($"[LightOff] Update 실행 중, isPlayerInRange={isPlayerInRange}, isLightOn={isLightOn}");
        // 전등이 꺼져있고 플레이어가 상호작용 범위 내에 있을 때
        if (!isLightOn && isPlayerInRange)
        {
            Debug.Log($"[LightOff] E키 입력 대기 중 (isPlayerInRange={isPlayerInRange}, isLightOn={isLightOn})");
            if (Input.GetKeyDown(interactionKey))
            {
                Debug.Log($"[LightOff] E키 입력 감지: 불 켜기 시도 (isPlayerInRange={isPlayerInRange}, isLightOn={isLightOn})");
                TurnOnLight();
            }
        }

        // 전등이 켜져있을 때만 몬스터 감지
        if (!isLightOn) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(monster))
            {
                Debug.Log("몬스터 감지됨, 전등 OFF");
                TurnOffLight();
                break;
            }
        }
    }

    void TurnOffLight()
    {
        foreach (var light in allLight)
        {
            if (light != null) light.enabled = false;
        }
        isLightOn = false;
        Debug.Log("[LightOff] 전체 전등 OFF! (TurnOffLight 호출됨)");
    }

    void TurnOnLight()
    {
        Debug.Log("[LightOff] TurnOnLight() 호출됨");
        foreach (var light in allLight)
        {
            if (light != null) light.enabled = true;
        }
        isLightOn = true;
        Debug.Log("[LightOff] 전체 전등 ON! (TurnOnLight 호출됨)");
    }

    // 플레이어가 상호작용 범위에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            Debug.Log($"[LightOff] OnTriggerEnter: 플레이어가 상호작용 범위에 들어옴 (isPlayerInRange={isPlayerInRange})");
            if (!isLightOn)
            {
                Debug.Log("[LightOff] 전등을 켤 수 있습니다. E키를 눌러주세요.");
            }
        }
    }

    // 플레이어가 상호작용 범위를 벗어났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            Debug.Log($"[LightOff] OnTriggerExit: 플레이어가 상호작용 범위를 벗어남 (isPlayerInRange={isPlayerInRange})");
        }
    }

    // 디버깅용 Gizmo
    void OnDrawGizmosSelected()
    {
        // 몬스터 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 플레이어 상호작용 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if ((allLight == null || allLight.Length == 0) && lampRoot != null)
        {
            allLight = lampRoot.GetComponentsInChildren<Light>();
            Debug.Log($"{name} OnValidate: 자동으로 Light {allLight.Length}개 할당");
        }
    }
#endif
}
