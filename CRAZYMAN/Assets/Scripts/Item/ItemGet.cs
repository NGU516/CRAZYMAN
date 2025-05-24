using UnityEngine;

public class ItemGet : MonoBehaviour
{
    private bool isPlayerNearby = false; // �÷��̾ ������ ��ó�� �ִ��� Ȯ���ϴ� ����
    private Inventory playerInventory = null;

    public Item item; // ȹ���� ������

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
                Debug.LogWarning("�÷��̾ Invetory ��ũ��Ʈ�� ����");
            }
            Debug.Log("������ ���� ���� F Ű ������ ����");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerInventory = null;
            Debug.Log("�־���");
        }
    }

    private void Update()
    {
        if(isPlayerNearby && playerInventory != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F Ű �Է�");

            playerInventory.AddItem(item);
        }
    }
}