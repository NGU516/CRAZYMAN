using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class NetworkDoor : MonoBehaviourPunCallbacks
{
    private new PhotonView photonView;
    private DoorController doorController;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        doorController = GetComponent<DoorController>();
    }

    [PunRPC]
    private void SyncDoorState(bool isOpen, Vector3 playerPosition)
    {
        if (doorController != null)
        {
            doorController.ToggleDoor(playerPosition != Vector3.zero ? 
                GameObject.FindGameObjectWithTag("Player")?.transform : null);
            Debug.Log($"[NetworkDoor] 문 상태 변경: {(doorController.isOpen ? "열림" : "닫힘")} (요청자: {PhotonNetwork.LocalPlayer.NickName})");
        }
    }

    [PunRPC]
    private void SyncLockState(bool isLocked)
    {
        if (doorController != null)
        {
            doorController.SetLocked(isLocked);
        }
    }

    // 외부에서 호출할 메서드들
    public void RequestToggleDoor(Vector3 playerPosition)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SyncDoorState", RpcTarget.All, !doorController.isOpen, playerPosition);
        }
    }

    public void RequestSetLocked(bool locked)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SyncLockState", RpcTarget.All, locked);
        }
    }
} 