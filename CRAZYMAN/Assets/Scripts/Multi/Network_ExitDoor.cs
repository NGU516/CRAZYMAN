using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Network_ExitDoor : MonoBehaviourPun
{
    public Transform keySpawner; // 인스펙터에서 KeySpawner 할당
    public AudioSource audioSource; // 문 오디오 소스
    public AudioClip openSound; // 문 열리는 소리
    public Animator doorAnimator; // 문 애니메이터 (옵션)
    private bool isOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpened && keySpawner.childCount == 0)
        {
            // 마스터 클라이언트만 문 열기 RPC 호출
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("OpenDoorRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void OpenDoorRPC()
    {
        if (isOpened) return;
        isOpened = true;
        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound);
        if (doorAnimator)
            doorAnimator.SetTrigger("Open"); // 애니메이터에 Open 트리거가 있다면
        Debug.Log("문이 열렸습니다! (모든 플레이어에게 동기화)");
    }
}
