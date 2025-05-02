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
        Debug.Log($"{name} Update: 전등 상태: {(isLightOn ? "켜짐" : "꺼짐")}");
        if (!isLightOn) return; // 전등이 꺼져 있으면 감지하지 않음

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
        Debug.Log("전체 전등 OFF!");
    }

    // 씬에서 감지 반경 시각화용 Gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
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
