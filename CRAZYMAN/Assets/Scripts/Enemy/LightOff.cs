using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOff : MonoBehaviour
{
    // 제어할 Light 오브젝트
    public Light allLight;

    // 괴물 감지 범위
    public float detectionRadius = 5f;

    // 괴물 태그
    public string monster = "Monster";

    // 현재 라이트 상태
    private bool isLightOn = true;

    void Start()
    {
        if (allLight == null)
        {
            Debug.LogError("라이트 없음");

            // 스크립트 비활성화
            enabled = false;
        }
    }

    void Update()
    {
        // 주변에 괴물이 있는지 확인
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        bool monsterDetected = false;
        foreach(var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(monster))
            {
                monsterDetected = true;
                break;
            }
        }

        // 괴물 감지 여부에 따라 라이트 상태 변경
        if (monsterDetected && isLightOn)
        {
            TurnOffLight();
        }

        void TurnOffLight()
        {
            allLight.enabled = false;
            isLightOn = false;
            Debug.Log("소등~!");
        }

        // 감지 범위를 시각적으로 확인 (Gizmos)
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
