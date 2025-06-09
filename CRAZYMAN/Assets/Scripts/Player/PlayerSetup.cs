using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    [Tooltip("�÷��̾�� �ڽ����� ���� UI ������")]
    [SerializeField]
    private GameObject playerUiPrefab;

    void Start()
    {
        // photonView.IsMine�� �� ĳ������ ������ �� �ڽ������� Ȯ���ϴ� ���� �߿��� �Ӽ��Դϴ�.
        // ��, ������ �÷��̾� ĳ���� �� ���� ���� ������ ĳ���������� �Ǻ��մϴ�.
        if (photonView.IsMine)
        {
            // �� ĳ���Ͱ� �����Ƿ�, UI�� �����մϴ�.
            if (playerUiPrefab != null)
            {
                // Instantiate�� ����Ͽ� "����"�� UI�� �����մϴ�.
                // UI�� �ٸ� ������� ������ �ʿ䰡 ���� �����Դϴ�.
                // transform�� �� ��ũ��Ʈ�� �پ��ִ� �÷��̾� ĳ������ Transform�� �ǹ��մϴ�.
                Instantiate(playerUiPrefab, transform);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }
    }
}
