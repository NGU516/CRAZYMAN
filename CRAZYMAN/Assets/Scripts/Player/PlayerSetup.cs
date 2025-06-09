using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    [Tooltip("플레이어에게 자식으로 붙을 UI 프리팹")]
    [SerializeField]
    private GameObject playerUiPrefab;

    void Start()
    {
        // photonView.IsMine은 이 캐릭터의 주인이 나 자신인지를 확인하는 가장 중요한 속성입니다.
        // 즉, 수많은 플레이어 캐릭터 중 내가 직접 조종할 캐릭터인지를 판별합니다.
        if (photonView.IsMine)
        {
            // 내 캐릭터가 맞으므로, UI를 생성합니다.
            if (playerUiPrefab != null)
            {
                // Instantiate를 사용하여 "로컬"로 UI를 생성합니다.
                // UI는 다른 사람에게 보여줄 필요가 없기 때문입니다.
                // transform은 이 스크립트가 붙어있는 플레이어 캐릭터의 Transform을 의미합니다.
                Instantiate(playerUiPrefab, transform);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }
    }
}
