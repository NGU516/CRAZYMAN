using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Network_ExitDoor : MonoBehaviourPun
{
    public Transform KeySpawner; // 인스펙터에서 KeySpawner 할당
    public Transform doorTransform; // Door 오브젝트 직접 할당 (또는 자동 할당)
    public AudioSource audioSource; // 문 오디오 소스
    public AudioClip openSound; // 문 열리는 소리
    public float openAngle = -90f; // 열릴 각도
    private bool isOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        // Door 자식 자동 할당 (Door_Exit 하위의 Door)
        if (doorTransform == null)
            doorTransform = transform.Find("Door");
    }

    // Update is called once per frame
    void Update()
    {
        // 아직 KeySpawner가 할당되지 않았다면 계속 찾기
        if (KeySpawner == null)
        {
            var obj = GameObject.Find("KeySpawner(Clone)");
            if (obj != null)
            {
                KeySpawner = obj.transform;
                Debug.Log("KeySpawner 할당됨!");
            }
            return; // 아직 할당 안 됐으면 아래 코드 실행하지 않음
        }

        // Debug.Log("키 개수: " + KeySpawner.childCount);
        if (!isOpened && AllKeysCollected())
        {
            Debug.Log("문 열기 조건 충족");
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("OpenDoorRPC", RpcTarget.All);
            }
        }
    }

    private bool AllKeysCollected()
    {
        // 자식이 하나도 없으면 true
        if (KeySpawner.childCount == 3)
            return true;

        // 자식이 있지만 모두 비활성화면 true
        foreach (Transform child in KeySpawner)
        {
            if (child.gameObject.activeSelf)
                return false;
        }
        return true;
    }

    [PunRPC]
    void OpenDoorRPC()
    {
        if (isOpened) return;
        isOpened = true;

        // 문 열기: Y축으로 openAngle만큼 회전
        if (doorTransform != null)
        {
            Vector3 baseEuler = doorTransform.localRotation.eulerAngles;
            Vector3 targetEuler = new Vector3(baseEuler.x, baseEuler.y + openAngle, baseEuler.z);
            doorTransform.localRotation = Quaternion.Euler(targetEuler);
        }

        // 사운드
        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound);

        Debug.Log("문이 열렸습니다! (모든 플레이어에게 동기화, 자동 열림)");
    }
}
