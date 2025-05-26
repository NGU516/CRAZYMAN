using UnityEngine;

public class ItemGet : MonoBehaviour
{
    private bool isPlayerNearby = false; // 플레이어가 아이템 근처에 있는지 확인하는 변수
    private Inventory playerInventory = null;

    public Item item; // 획득할 아이템

    private void Awake()
    {
        item = GetComponent<Item>();
        if (item == null )
        {
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = true;

            playerInventory = collision.GetComponent<Inventory>();

            if (playerInventory == null)
            {
                Debug.LogWarning("플레이어에 Invetory 스크립트가 없음");
            }
            Debug.Log("아이템 습득 가능 F 키 눌러서 습득");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerInventory = null;
            Debug.Log("멀어짐");
        }
    }

    private void Update()
    {
        if(isPlayerNearby && playerInventory != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F 키 입력");

            playerInventory.AddItem(item);
        }
    }
}