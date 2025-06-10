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
        Debug.Log("게임 끝내기: " + other.name);
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
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.GetComponent<UIEnding>() != null) continue;
            if (obj.name == "SoundManager") continue;
            if (obj.name == "@SoundRoot") continue;
            obj.SetActive(false);
        }

        // 2. 엔딩 팝업 띄우기
        Managers.UI.ShowPopupUI<UIEnding>("UIEnding");
        // Managers.SoundManager.Play(Define.Sound.StartBgm);
    }
}
