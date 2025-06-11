using UnityEngine;
using System;

public class ItemDataForInventory
{
    public ItemType itemType; // ������ ����
    public string itemName; // ������ �̸�
    public Sprite inventoryIcon; // �κ��丮 UI ������

    public float RecoveryStamina; // ���׹̳� ȸ����
    public float RecoveryMental; // ���ŷ� ȸ����
    public float RecoveryBattery; // ���͸� ȸ����

    public ItemDataForInventory(Item item)
    {
        if (item == null)
        {
            Debug.LogError("ItemDataForInventory ���� ����: ���޵� Item ������Ʈ�� null�Դϴ�!");
            this.itemType = ItemType.pills; // �Ǵ� ItemType.Default �� �⺻��
            this.itemName = "Null Item";
            this.inventoryIcon = null;
            this.RecoveryStamina = 0f;
            this.RecoveryMental = 0f;
            this.RecoveryBattery = 0f;
            return; // null�̸� �ʱ�ȭ ����
        }

        this.itemType = item.itemType;
        this.itemName = item.itemName;
        this.inventoryIcon = item.inventoryIcon;

        this.RecoveryStamina = item.staminaRecoveryAmount;
        this.RecoveryMental = item.mentalRecoveryAmount;
        this.RecoveryBattery = item.batteryRecoveryAmount;
    }
}