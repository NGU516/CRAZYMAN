using UnityEngine;

public class ItemGet : MonoBehaviour
{
    public Item item; // ȹ���� ������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("������ ȹ��");
            Inventory inventory = collision.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(item);
            }
        }
    }
}
