using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 2; // 인벤토리 크기
    public List<Item> items = new List<Item>(); // 인벤토리 아이템 리스트
    public GameObject[] itemSlots; // 인벤토리 슬롯 (UI)

    void Start()
    {
        UpdateUI(); // 초기 UI 업데이트
    }

    // 아이템 획득
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            UpdateUI();
            Destroy(item.gameObject); // 획득한 아이템은 씬에서 제거
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
        }
    }

    // UI 업데이트
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
