using UnityEngine;
using Photon.Pun;

public class ItemCamera : MonoBehaviourPun
{
    public float stunRadius = 10f;             // 스턴 범위
    public LayerMask enemyLayer;               // 괴인 레이어
    public AudioClip cameraSound;              // 셔터 사운드
    public GameObject cameraFlashEffect;       // 이펙트 프리팹
    public Transform effectSpawnPoint;         // 이펙트 위치

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Use()
    {
        // 모든 클라이언트에서 이펙트 & 사운드 실행 (위치 전달)
        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
        photonView.RPC("PlayFlashEffectAndSound", RpcTarget.All, spawnPos);

        // 괴인 스턴 요청 (마스터만 처리)
        if (PhotonNetwork.IsMasterClient)
        {
            StunNearbyEnemies();
        }
        else
        {
            photonView.RPC("RequestStunEnemies", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void PlayFlashEffectAndSound(Vector3 position)
    {
        if (cameraSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cameraSound);
        }

        if (cameraFlashEffect != null)
        {
            Instantiate(cameraFlashEffect, position, Quaternion.identity);
        }
    }

    [PunRPC]
    void RequestStunEnemies()
    {
        StunNearbyEnemies();
    }

    void StunNearbyEnemies()
    {
        Debug.Log("[ItemCamera] StunNearbyEnemies");
        Collider[] hits = Physics.OverlapSphere(transform.position, stunRadius, enemyLayer);
        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.SetBlind(true); // 스턴 적용
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}
