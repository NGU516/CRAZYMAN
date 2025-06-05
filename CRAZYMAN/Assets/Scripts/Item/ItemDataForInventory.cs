using UnityEngine;
using System;

public class ItemDataForInventory
{
    public ItemType itemType; // ������ ����
    public string itemName; // ������ �̸�
    public Sprite inventoryIcon; // �κ��丮 UI ������

    public ItemDataForInventory(Item item)
    {
        if (item == null)
        {
            Debug.LogError("ItemDataForInventory ���� ����: ���޵� Item ������Ʈ�� null�Դϴ�!");
            return; // null�̸� �ʱ�ȭ ����
        }

        this.itemType = item.itemType;
        this.itemName = item.itemName;
        this.inventoryIcon = item.inventoryIcon;
    }
}