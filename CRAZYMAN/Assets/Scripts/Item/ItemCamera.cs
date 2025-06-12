using UnityEngine;
using Photon.Pun;

public class ItemCamera : MonoBehaviourPun
{
    public float stunRadius = 10f;             // ���� ����
    public LayerMask enemyLayer;               // ���� ���̾�
    public AudioClip cameraSound;              // ���� ����
    public GameObject cameraFlashEffect;       // ����Ʈ ������
    public Transform effectSpawnPoint;         // ����Ʈ ��ġ

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Use()
    {
        // ��� Ŭ���̾�Ʈ���� ����Ʈ & ���� ���� (��ġ ����)
        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
        photonView.RPC("PlayFlashEffectAndSound", RpcTarget.All, spawnPos);

        // ���� ���� ��û (�����͸� ó��)
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
                enemy.SetBlind(true); // ���� ����
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}
