using UnityEngine;
using Photon.Pun;

public class NetworkItem : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private bool isPickedUp = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isPickedUp);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            isPickedUp = (bool)stream.ReceiveNext();
        }
    }

    // RPC란 네트워크를 통해 메서드를 호출하는 것
    // 아이템 획득 시, 아이템 비활성화
    [PunRPC]
    public void PickupItem()
    {
        if (!isPickedUp)
        {
            isPickedUp = true;
            gameObject.SetActive(false);
        }
    }

    // 아이템 놓기 시, 아이템 활성화
    [PunRPC]
    public void DropItem(Vector3 position)
    {
        if (isPickedUp)
        {
            isPickedUp = false;
            transform.position = position;
            gameObject.SetActive(true);
        }
    }
}
