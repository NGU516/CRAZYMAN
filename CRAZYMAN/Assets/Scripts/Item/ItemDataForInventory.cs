using UnityEngine;
using System;

public class ItemDataForInventory
{
    public ItemType itemType; // 아이템 종류
    public string itemName; // 아이템 이름
    public Sprite inventoryIcon; // 인벤토리 UI 아이콘

    public float RecoveryStamina; // 스테미너 회복량
    public float RecoveryMental; // 정신력 회복량
    public float RecoveryBattery; // 배터리 회복량

    public ItemDataForInventory(Item item)
    {
        if (item == null)
        {
            Debug.LogError("ItemDataForInventory 생성 오류: 전달된 Item 컴포넌트가 null입니다!");
            this.itemType = ItemType.pills; // 또는 ItemType.Default 등 기본값
            this.itemName = "Null Item";
            this.inventoryIcon = null;
            this.RecoveryStamina = 0f;
            this.RecoveryMental = 0f;
            this.RecoveryBattery = 0f;
            return; // null이면 초기화 실패
        }

        this.itemType = item.itemType;
        this.itemName = item.itemName;
        this.inventoryIcon = item.inventoryIcon;

        this.RecoveryStamina = item.staminaRecoveryAmount;
        this.RecoveryMental = item.mentalRecoveryAmount;
        this.RecoveryBattery = item.batteryRecoveryAmount;
    }
}