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
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키

    private bool isPlayerInRange = false;    // 플레이어가 상호작용 범위 내에 있는지

    void Start()
    {
        // allLight가 비어 있으면 자동으로 자식에서 Light 컴포넌트 찾아서 할당
        if ((allLight == null || allLight.Length == 0) && lampRoot != null)
        {
            allLight = lampRoot.GetComponentsInChildren<Light>();
        }
    }

    void Update()
    {
        // 전등이 꺼져있고 플레이어가 상호작용 범위 내에 있을 때
        if (!isLightOn && isPlayerInRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
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
    }

    void TurnOnLight()
    {
        foreach (var light in allLight)
        {
            if (light != null) light.enabled = true;
        }
        isLightOn = true;
    }

    // 플레이어가 상호작용 범위에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
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
