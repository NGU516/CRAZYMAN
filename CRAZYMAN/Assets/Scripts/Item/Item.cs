using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Camera,
    Key,
    BatteryBig,
    BatterySmall,
    bandage,
    pills
}

public class Item : MonoBehaviour
{
    [Header("Item Data")]

    public ItemType itemType;

    public string itemName = "�� ������";

    [Header("Inventory UI")]
    public Sprite inventoryIcon;

    [Header("Effect Data - Pills")] // �� ������ ���� ȿ�� ������
    [SerializeField] public float staminaRecoveryAmount = 0f; // ���׹̳� ȸ���� (Inspector���� ����)
    [SerializeField] public float mentalRecoveryAmount = 0f; // ���ŷ� ȸ���� (Inspector���� ����)

    [SerializeField] public float batteryRecoveryAmount = 0f; // ���ŷ� ȸ���� (Inspector���� ����)

    public virtual bool Use()
    {
        Debug.Log($"{itemName} ����");

        switch(itemName)
        {
            case "ī�޶�":
                // �� ����
                Debug.Log("�� ����");
                break;
            case "�ش�":
                // ü�� ȸ��
                Debug.Log("ü�� ȸ��");
                break;
            case "ū���͸�":
                // ������ ȸ��
                Debug.Log("������ ȸ��");
                break;
            case "�������͸�":
                // ������ ȸ��
                Debug.Log("������ ���� ȸ��");
                break;
            case "��1":
                // ���׹̳� ȸ��
                Debug.Log("���׹̳� ȸ��");
                break;
            case "��2":
                // ���׹̳� ȸ��
                Debug.Log("���׹̳� ���� ȸ��");
                break;
            case "Ű1":
                Debug.Log("1�� Ű ���");
                break;
            case "Ű2":
                Debug.Log("2�� Ű ���");
                break;
            case "Ű3":
                Debug.Log("3�� Ű ���");
                break;
        }

        return true;
    }
}
