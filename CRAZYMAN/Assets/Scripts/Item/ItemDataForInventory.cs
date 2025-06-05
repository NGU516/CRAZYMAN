using UnityEngine;
using System;

public class ItemDataForInventory
{
    public ItemType itemType; // 아이템 종류
    public string itemName; // 아이템 이름
    public Sprite inventoryIcon; // 인벤토리 UI 아이콘

    public ItemDataForInventory(Item item)
    {
        if (item == null)
        {
            Debug.LogError("ItemDataForInventory 생성 오류: 전달된 Item 컴포넌트가 null입니다!");
            return; // null이면 초기화 실패
        }

        this.itemType = item.itemType;
        this.itemName = item.itemName;
        this.inventoryIcon = item.inventoryIcon;
    }
}