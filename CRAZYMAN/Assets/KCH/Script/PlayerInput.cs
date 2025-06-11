using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    public float interactDistance = 5f; // 상호작용 거리
    public LayerMask doorLayerMask;

    private Camera playerCamera;

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false; // 내 카메라가 아니면 스크립트 꺼버림
            return;
        }

        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInteractWithDoor();
        }
    }

    void TryInteractWithDoor()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, doorLayerMask))
        {
            NetworkDoor networkDoor = hit.collider.GetComponent<NetworkDoor>();
            if (networkDoor != null)
            {
                networkDoor.RequestToggleDoor(transform.position);
            }
        }
    }
}