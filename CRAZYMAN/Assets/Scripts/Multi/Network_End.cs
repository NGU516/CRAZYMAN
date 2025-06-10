using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class Network_End : MonoBehaviourPun
{
    private HashSet<int> playersInZone = new HashSet<int>();

    void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag("Player"))
        {
            playersInZone.Add(pv.OwnerActorNr);
            CheckAllPlayersInZone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag("Player"))
        {
            playersInZone.Remove(pv.OwnerActorNr);
        }
    }

    void CheckAllPlayersInZone()
    {
        // 현재 방에 남아있는 모든 플레이어가 다 들어왔는지 체크
        bool allIn = true;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!playersInZone.Contains(p.ActorNumber))
            {
                allIn = false;
                break;
            }
        }
        if (allIn)
        {
            photonView.RPC("ShowEndingUI", RpcTarget.All);
        }
    }

    [PunRPC]
    void ShowEndingUI()
    {
        Managers.UI.ShowPopupUI<UIEnding>("UIEnding");
    }
}
