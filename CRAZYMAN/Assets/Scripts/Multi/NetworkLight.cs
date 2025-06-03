using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class NetworkLight : MonoBehaviourPunCallbacks
{
    private new PhotonView photonView;
    private LightOff lightOff;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        lightOff = GetComponent<LightOff>();
    }

    [PunRPC]
    private void SyncLightState(bool isOn)
    {
        if (lightOff != null)
        {
            if (isOn)
            {
                Debug.Log("[NetworkLight] 라이트 켜짐");
                lightOff.TurnOnLight();
            }
            else
            {
                Debug.Log("[NetworkLight] 라이트 꺼짐");
                lightOff.TurnOffLight();
            }
        }
    }

    // 외부에서 호출할 메서드들
    public void RequestToggleLight()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SyncLightState", RpcTarget.All, !lightOff.isLightOn);
        }
    }

    public void RequestTurnOffLight()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncLightState", RpcTarget.All, false);
        }
    }
} 