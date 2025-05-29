using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 2; // 인벤토리 크기
    public List<Item> items = new List<Item>(); // 인벤토리 아이템 리스트
    public GameObject[] itemSlots; // 인벤토리 슬롯 (UI)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItem(0);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItem(1);
        }
    }

    void UseItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            Item itemToUse = items[slotIndex];

            bool itemConsumed = itemToUse.Use(); // Use() 메소드가 아이템 제거 여부 반환

            if (itemConsumed)
            {
                Debug.Log($"{itemToUse.itemName} 아이템 사용");

                items.RemoveAt(slotIndex );

                UpdateUI();
            }
        }
    }

    // 아이템 획득
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            Debug.Log("아이템 획득");
            items.Add(item);
            UpdateUI();
            Destroy(item.gameObject); // 획득한 아이템은 씬에서 제거
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
        }
    }

    public void SetItemSlots(GameObject[] slots)
    {
        if (slots != null && slots.Length > 0)
        {
            itemSlots = slots;
            Debug.Log($"Inventory: Received {itemSlots.Length} item slots!");

            UpdateUI();
        }
        else
        {
            Debug.LogError("Inventory: Attempted to set null or empty item slots!");
        }
    }

    // UI 업데이트
    void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Image slotImage = itemSlots[i].GetComponent<Image>();

            slotImage.preserveAspect = true;

            if (i < items.Count)
            {
                slotImage.sprite = items[i].inventoryIcon;
                slotImage.enabled = true;
            }
            else
            {
                slotImage.sprite = null;
                slotImage.enabled = false;
            }
        }
    }
}
