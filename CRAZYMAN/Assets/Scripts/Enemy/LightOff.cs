using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOff : MonoBehaviour
{
    // ������ Light ������Ʈ
    public Light allLight;

    // ���� ���� ����
    public float detectionRadius = 5f;

    // ���� �±�
    public string monster = "Monster";

    // ���� ����Ʈ ����
    private bool isLightOn = true;

    void Start()
    {
        if (allLight == null)
        {
            Debug.LogError("����Ʈ ����");

            // ��ũ��Ʈ ��Ȱ��ȭ
            enabled = false;
        }
    }

    void Update()
    {
        // �ֺ��� ������ �ִ��� Ȯ��
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

        // ���� ���� ���ο� ���� ����Ʈ ���� ����
        if (monsterDetected && isLightOn)
        {
            TurnOffLight();
        }

        void TurnOffLight()
        {
            allLight.enabled = false;
            isLightOn = false;
            Debug.Log("�ҵ�~!");
        }

        // ���� ������ �ð������� Ȯ�� (Gizmos)
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
