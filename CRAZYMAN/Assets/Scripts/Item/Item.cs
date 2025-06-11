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

    public string itemName = "새 아이템";

    [Header("Inventory UI")]
    public Sprite inventoryIcon;

    [Header("Effect Data - Pills")] // 약 아이템 전용 효과 데이터
    [SerializeField] public float staminaRecoveryAmount = 0f; // 스테미너 회복량 (Inspector에서 설정)
    [SerializeField] public float mentalRecoveryAmount = 0f; // 정신력 회복량 (Inspector에서 설정)

    [SerializeField] public float batteryRecoveryAmount = 0f; // 정신력 회복량 (Inspector에서 설정)

    public virtual bool Use()
    {
        Debug.Log($"{itemName} 사용됨");

        switch(itemName)
        {
            case "카메라":
                // 적 스턴
                Debug.Log("적 스턴");
                break;
            case "붕대":
                // 체력 회복
                Debug.Log("체력 회복");
                break;
            case "큰배터리":
                // 손전등 회복
                Debug.Log("손전등 회복");
                break;
            case "작은배터리":
                // 손전등 회복
                Debug.Log("손전등 소폭 회복");
                break;
            case "약1":
                // 스테미너 회복
                Debug.Log("스테미너 회복");
                break;
            case "약2":
                // 스테미너 회복
                Debug.Log("스테미너 소폭 회복");
                break;
            case "키1":
                Debug.Log("1번 키 사용");
                break;
            case "키2":
                Debug.Log("2번 키 사용");
                break;
            case "키3":
                Debug.Log("3번 키 사용");
                break;
        }

        return true;
    }
}
