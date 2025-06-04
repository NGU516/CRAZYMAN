using UnityEngine;
using Photon.Pun;

public class ItemGet : MonoBehaviourPunCallbacks
{
    public Item item; // 획득할 아이템
    private PhotonView itemPhotonView;
    private Inventory nearbyPlayerInventory = null;
    private bool isPlayerNearby = false; // 플레이어가 아이템 근처에 있는지 확인하는 변수

    private void Awake()
    {
        itemPhotonView = GetComponent<PhotonView>();
        if (itemPhotonView == null)
        {
            Debug.LogError("ItemGet: 아이템 오브젝트에 PhotonView 컴포넌트가 없습니다! 네트워크 제거 불가!");
            enabled = false; // PhotonView 없으면 스크립트 중단
            return;
        }
        // ItemGet 스크립트가 붙어있는 게임 오브젝트에서 Item 컴포넌트 가져오기
        item = GetComponent<Item>();
        if (item == null)
        {
            Debug.LogError("ItemGet: Item 컴포넌트가 없습니다! 획득 불가!");
            enabled = false; // Item 컴포넌트 없으면 스크립트 중단
            return;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌한 오브젝트에서 PhotonView 컴포넌트를 가져옴
            PhotonView playerPhotonView = collision.GetComponent<PhotonView>();

            if (playerPhotonView.IsMine)
            {
                Debug.Log($"ItemGet: 로컬 플레이어({playerPhotonView.ViewID})가 아이템에 접근! ({gameObject.name}). F 키 입력 대기.");

                nearbyPlayerInventory = collision.GetComponent<Inventory>(); // 충돌한 플레이어에서 Inventory 가져오기
                
                // 플레이어가 아이템 근처에 있다는 상태 변수 설정
                isPlayerNearby = true;
                
                if (nearbyPlayerInventory == null)
                {
                    Debug.LogError("ItemGet: 로컬 플레이어에게 Inventory 컴포넌트가 없습니다! 획득 불가!");
                }

            }
            else
            {
                Debug.Log($"ItemGet: 다른 플레이어({playerPhotonView.ViewID})가 아이템에 접근.");
            }
        }

        /*if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = true;

            playerInventory = collision.GetComponent<Inventory>();

            if (playerInventory == null)
            {
                Debug.LogWarning("플레이어에 Invetory 스크립트가 없음");
            }
            Debug.Log("아이템 습득 가능 F 키 눌러서 습득");
        }*/
    }

    [PunRPC]
    void RemoveItemRPC(PhotonMessageInfo info)
    {
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("멀어짐");
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F 키 입력!");

            ItemDataForInventory itemData = new ItemDataForInventory(item);

            if (nearbyPlayerInventory.AddItem(itemData)) // 아이템 추가 성공 시
            {
                Debug.Log("인벤토리에 아이템 추가 성공!");

                itemPhotonView.RPC("RemoveItemRPC", RpcTarget.All); // 모든 클라이언트에서 아이템 제거
            }
            else
            {
                Debug.LogWarning("인벤토리에 아이템 추가 실패!"); // 인벤토리 가득 찼을 때 등
            }

            isPlayerNearby = false; // 습득 후 초기화
            nearbyPlayerInventory = null;
        }
    }
}