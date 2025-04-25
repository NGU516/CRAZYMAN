using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 2; // �κ��丮 ũ��
    public List<Item> items = new List<Item>(); // �κ��丮 ������ ����Ʈ
    public GameObject[] itemSlots; // �κ��丮 ���� (UI)

    void Start()
    {
        UpdateUI(); // �ʱ� UI ������Ʈ
    }

    // ������ ȹ��
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            UpdateUI();
            Destroy(item.gameObject); // ȹ���� �������� ������ ����
        }
        else
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�!");
        }
    }

    // UI ������Ʈ
    void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Image slotImage = itemSlots[i].GetComponent<Image>();
            if (i < items.Count)
            {
                slotImage.sprite = items[i].cameraIcon;
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
