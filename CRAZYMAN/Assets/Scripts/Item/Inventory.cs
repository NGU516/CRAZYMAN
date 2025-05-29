using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 2; // �κ��丮 ũ��
    public List<Item> items = new List<Item>(); // �κ��丮 ������ ����Ʈ
    public GameObject[] itemSlots; // �κ��丮 ���� (UI)

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

            bool itemConsumed = itemToUse.Use(); // Use() �޼ҵ尡 ������ ���� ���� ��ȯ

            if (itemConsumed)
            {
                Debug.Log($"{itemToUse.itemName} ������ ���");

                items.RemoveAt(slotIndex );

                UpdateUI();
            }
        }
    }

    // ������ ȹ��
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            Debug.Log("������ ȹ��");
            items.Add(item);
            UpdateUI();
            Destroy(item.gameObject); // ȹ���� �������� ������ ����
        }
        else
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�!");
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

    // UI ������Ʈ
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
