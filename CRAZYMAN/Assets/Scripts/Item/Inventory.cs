using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using Photon.Pun;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 2;
    public List<ItemDataForInventory> items = new List<ItemDataForInventory>();
    public GameObject[] itemSlots;
    private PhotonView playerPhotonView;

    private void Start()
    {
        playerPhotonView = GetComponent<PhotonView>();
        UpdateUI();
    }

    private void Update()
    {
        if (playerPhotonView == null || !playerPhotonView.IsMine) return;

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
        if (playerPhotonView == null || !playerPhotonView.IsMine) return;

        if (slotIndex < 0 || slotIndex >= items.Count || items[slotIndex] == null) return;

        ItemDataForInventory itemDataToUse = items[slotIndex];
        ApplyLocalItemEffect(itemDataToUse);
        playerPhotonView.RPC("UseItemRPC", RpcTarget.All, (int)itemDataToUse.itemType, playerPhotonView.ViewID);

        items.RemoveAt(slotIndex);
        UpdateUI();
    }

    void ApplyLocalItemEffect(ItemDataForInventory itemData)
    {
        PhotonControl playerControl = GetComponent<PhotonControl>();
        // 가져온 스크립트가 null인지 확인 (플레이어 오브젝트에 해당 스크립트가 붙어있는지 확인!)
        if (playerControl == null) // 또는 playerStats == null
        {
            Debug.LogError("Inventory.ApplyLocalItemEffect: 플레이어에게서 스탯/회복 스크립트(PhotonControl 또는 PlayerStats)를 찾을 수 없습니다! 스탯 회복 불가.");
            return; // 스크립트 없으면 회복 불가
        }

        // ItemType enum을 사용하여 switch 문으로 효과 분기
        switch (itemData.itemType)
        {
            case ItemType.pills:
                Debug.Log($"Inventory.ApplyLocalItemEffect: Player used Pills. Calling Player's Recover functions. Stamina: {itemData.RecoveryStamina}, Sanity: {itemData.RecoveryMental}");
                
                playerControl.RecoverStamina(itemData.RecoveryStamina);
                playerControl.RecoverMental(itemData.RecoveryMental);
                break;

            case ItemType.bandage: // 붕대
                                   // TODO: 로컬 플레이어 체력 회복 로직 구현 (itemData.healthRecoveryAmount 사용)
                Debug.Log("Inventory.ApplyLocalItemEffect: Player used Bandage. Healing player health locally.");
                // 예: playerControl.RecoverHealth(itemData.healthRecoveryAmount); // PhotonControl에 체력 회복 함수 추가 필요
                break;

            case ItemType.Battery: // 배터리
                                   // TODO: 로컬 플레이어 손전등 회복 로직 구현 (itemData.batteryChargeAmount 사용)
                Debug.Log("Inventory.ApplyLocalItemEffect: Player used Battery. Restoring flashlight charge locally.");
                // 예: playerControl.RestoreFlashlightCharge(itemData.batteryChargeAmount); // PhotonControl 또는 다른 스크립트에 함수 추가 필요
                break;

            // 다른 아이템의 로컬 효과가 있다면 여기에 추가
            default:
                Debug.Log($"Inventory.ApplyLocalItemEffect: No specific local effect defined for Item Type: {itemData.itemType}.");
                break;
        }
    }

    [PunRPC]
    void UseItemRPC(int itemTpyeInt, int senderViewID, PhotonMessageInfo info)
    {
        ItemType usedItemType = (ItemType)itemTpyeInt;
        PhotonView senderPV = PhotonView.Find(senderViewID);

        GameObject senderObject = senderPV?.gameObject;

        if (senderPV == null) return;

        switch (usedItemType)
        {
            case ItemType.Camera:
                break;

            case ItemType.pills:
                break;
        }
    }

    // 아이템 획득
    public bool AddItem(ItemDataForInventory itemData)
    {
        if (itemData == null) return false;

        if (items.Count < inventorySize)
        {
            Debug.Log("아이템 획득");
            items.Add(itemData);
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            return false;
        }
    }

    public void SetItemSlots(GameObject[] slots)
    {
        Debug.Log("Inventory.SetItemSlots() 호출됨"); // SetItemSlots 호출 로그
        if (slots != null && slots.Length > 0)
        {
            itemSlots = slots;
            Debug.Log($"Inventory: 아이템 슬롯 설정 완료! 슬롯 개수: {itemSlots.Length}");
            UpdateUI(); // 슬롯 설정 후 UI 업데이트
        }
        else
        {
            Debug.LogError("Inventory: 유효하지 않은 슬롯이 전달됨!");
        }
    }

    // UI 업데이트 (UI 슬롯에 아이콘 표시)
    public void UpdateUI()
    {
        Debug.Log("Inventory.UpdateUI() 호출됨"); // UpdateUI 호출 로그

        if (itemSlots == null)
        {
            Debug.LogError("ItemSlots 배열이 null입니다!");
            return;
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] != null)
            {
                Image slotImage = itemSlots[i].GetComponent<Image>();
                if (slotImage != null)
                {
                    if (i < items.Count && items[i] != null)
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
                else
                {
                    Debug.LogError($"itemSlots[{i}]에 Image 컴포넌트가 없습니다.");
                }
            }
        }
    }
}
