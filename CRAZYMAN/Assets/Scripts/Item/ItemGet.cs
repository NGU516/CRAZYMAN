using UnityEngine;

public class ItemGet : MonoBehaviour
{
    public Item item; // »πµÊ«“ æ∆¿Ã≈€

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("æ∆¿Ã≈€ »πµÊ");
            Inventory inventory = collision.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(item);
            }
        }
    }
}
