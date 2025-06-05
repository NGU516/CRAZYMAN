using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]

public class NetworkItem : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView photonView;

    public Item item;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        item = GetComponent<Item>();

        if (photonView == null)
        {
            Debug.LogError("NetworkItem: PhotonView ������Ʈ�� �����ϴ�! ��Ʈ��ũ ����ȭ �Ұ�!");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        if (item == null)
        {
            Debug.LogError("NetworkItem: Item ������Ʈ�� �����ϴ�! ������ ���� ���� �Ұ�!");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        Collider itemCollider = GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� �÷��̾� (������ ����)
            Debug.Log($"OnPhotonSerializeView (����): ���� �÷��̾� ({PhotonNetwork.LocalPlayer.NickName})�� ������ ����!");
            stream.SendNext(transform.position); // position ����ȭ
        }

        else
        {
            // ����Ʈ �÷��̾� (������ ����)
            Debug.Log($"OnPhotonSerializeView (�б�): ����Ʈ �÷��̾� ({PhotonNetwork.LocalPlayer.NickName})�� ������ ����!");
            transform.position = (Vector3)stream.ReceiveNext(); // position ����ȭ
        }
    }

    [PunRPC]
    void RemoveItemRPC(PhotonMessageInfo info)
    {
        Debug.Log($"NetworkItem: RemoveItemRPC ȣ��� on {gameObject.name} by Player {info.Sender.ActorNumber}!");
        Destroy(gameObject);
    }
}
