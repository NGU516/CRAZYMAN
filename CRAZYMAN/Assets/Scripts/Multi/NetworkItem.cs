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
            Debug.LogError("NetworkItem: PhotonView 컴포넌트가 없습니다! 네트워크 동기화 불가!");
            enabled = false; // 스크립트 비활성화
            return;
        }

        if (item == null)
        {
            Debug.LogError("NetworkItem: Item 컴포넌트가 없습니다! 아이템 정보 접근 불가!");
            enabled = false; // 스크립트 비활성화
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
            // 로컬 플레이어 (데이터 전송)
            Debug.Log($"OnPhotonSerializeView (쓰기): 로컬 플레이어 ({PhotonNetwork.LocalPlayer.NickName})가 데이터 전송!");
            stream.SendNext(transform.position); // position 동기화
        }

        else
        {
            // 리모트 플레이어 (데이터 수신)
            Debug.Log($"OnPhotonSerializeView (읽기): 리모트 플레이어 ({PhotonNetwork.LocalPlayer.NickName})가 데이터 수신!");
            transform.position = (Vector3)stream.ReceiveNext(); // position 동기화
        }
    }

    [PunRPC]
    void RemoveItemRPC(PhotonMessageInfo info)
    {
        Debug.Log($"NetworkItem: RemoveItemRPC 호출됨 on {gameObject.name} by Player {info.Sender.ActorNumber}!");
        Destroy(gameObject);
    }
}
