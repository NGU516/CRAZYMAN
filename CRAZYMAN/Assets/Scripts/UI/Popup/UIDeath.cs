using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDeath : UIPopup
{
    public GameObject ghostCamPrefab;
    private GameObject myGhostCam;

    enum Buttons
    {
        GhostButton, // 유령 모드
        HomeButton   // 홈으로 돌아가기
    }

    public override bool Init()
    {
        Debug.Log("UIDeath Init 진입");
        if (base.Init() == false)
        {
            Debug.LogError("UIDeath Init 실패");
            return false;
        }

        Debug.Log("UIDeath 호출");
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.GhostButton).gameObject.BindEvent(OnClickGhost);
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(OnClickHome);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // 커서 보이기

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            FirstPerson fp = player.GetComponentInChildren<FirstPerson>();
            if (fp != null)
                fp.enabled = false; // 플레이어 컨트롤 비활성화
        }
        return true;
    }

    public void OnClickGhost()
    {
        Debug.Log("UIDeath OnClickGhost 호출");

        Vector3 deathPosition = new Vector3(
            PlayerPrefs.GetFloat("DeathX", 0f),
            PlayerPrefs.GetFloat("DeathY", 0f),
            PlayerPrefs.GetFloat("DeathZ", 0f)
        );
        deathPosition.y += 1.5f;

        // 유령 카메라 활성화
        // GameObject ghostCam = GameObject.Find("GhostCamera");
        if (ghostCamPrefab != null)
        {
            ghostCamPrefab = Resources.Load<GameObject>("Prefabs/GhostCam");
            Debug.Log("GhostCam Prefab 로드 성공");
        }

        myGhostCam = Instantiate(ghostCamPrefab, deathPosition, Quaternion.identity);
        Debug.Log("GhostCam 인스턴스화 성공");
        myGhostCam.name = $"GhostCam_{PhotonNetwork.LocalPlayer.ActorNumber}";

        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
        Cursor.visible = false; // 커서 숨기기

        // UIDeath UI 닫기
        Destroy(gameObject);
    }

    public void OnClickHome()
    {
        Debug.Log("UIDeath OnClickHome 호출");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.SetActive(false); // 플레이어 오브젝트 비활성화

        GameObject ghostCam = GameObject.Find("GhostCam");
        if (ghostCam != null)
            ghostCam.SetActive(false); // 유령 카메라 비활성화

        Destroy(gameObject); // UIDeath 오브젝트 제거

        Managers.UI.ShowPopupUI<UIMain>("UIMain");
    }
}
