using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using Photon.Pun;

public class Inventory : MonoBehaviourPun
{
    public int inventorySize = 2;
    public List<ItemDataForInventory> items = new List<ItemDataForInventory>();
    public GameObject[] itemSlots;
    //private PhotonView playerPhotonView;

    private void Start()
    {
        /*playerPhotonView = GetComponent<PhotonView>();
        UpdateUI();*/
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        if (slotIndex < 0 || slotIndex >= items.Count || items[slotIndex] == null) return;

        ItemDataForInventory itemDataToUse = items[slotIndex];
        //ApplyLocalItemEffect(itemDataToUse);
        photonView.RPC("UseItemRPC", RpcTarget.All,
                         (int)itemDataToUse.itemType, // ������ ���� (int�� ��ȯ)
                         photonView.ViewID, // �������� ����� �÷��̾��� ViewID
                         itemDataToUse.RecoveryStamina, // ItemDataForInventory�� ���׹̳� ȸ����
                         itemDataToUse.RecoveryMental, // ItemDataForInventory�� ���ŷ� ȸ����
                         itemDataToUse.RecoveryBattery); // ItemDataForInventory�� ���͸� ȸ����
        items.RemoveAt(slotIndex);
        UpdateUI();
    }

    /*void ApplyLocalItemEffect(ItemDataForInventory itemData)
    {
        PhotonControl playerControl = GetComponent<PhotonControl>();
        // ������ ��ũ��Ʈ�� null���� Ȯ�� (�÷��̾� ������Ʈ�� �ش� ��ũ��Ʈ�� �پ��ִ��� Ȯ��!)
        if (playerControl == null) // �Ǵ� playerStats == null
        {
            Debug.LogError("Inventory.ApplyLocalItemEffect: �÷��̾�Լ� ����/ȸ�� ��ũ��Ʈ(PhotonControl �Ǵ� PlayerStats)�� ã�� �� �����ϴ�! ���� ȸ�� �Ұ�.");
            return; // ��ũ��Ʈ ������ ȸ�� �Ұ�
        }

        // ItemType enum�� ����Ͽ� switch ������ ȿ�� �б�
        switch (itemData.itemType)
        {
            case ItemType.pills:
                Debug.Log($"Inventory.ApplyLocalItemEffect: Player used Pills. Calling Player's Recover functions. Stamina: {itemData.RecoveryStamina}, Sanity: {itemData.RecoveryMental}");
                
                break;

            case ItemType.bandage: // �ش�
                                   // TODO: ���� �÷��̾� ü�� ȸ�� ���� ���� (itemData.healthRecoveryAmount ���)
                Debug.Log("Inventory.ApplyLocalItemEffect: Player used Bandage. Healing player health locally.");
                // ��: playerControl.RecoverHealth(itemData.healthRecoveryAmount); // PhotonControl�� ü�� ȸ�� �Լ� �߰� �ʿ�
                break;

            case ItemType.Battery: // ���͸�
                                   // TODO: ���� �÷��̾� ������ ȸ�� ���� ���� (itemData.batteryChargeAmount ���)
                Debug.Log("Inventory.ApplyLocalItemEffect: Player used Battery. Restoring flashlight charge locally.");
                // ��: playerControl.RestoreFlashlightCharge(itemData.batteryChargeAmount); // PhotonControl �Ǵ� �ٸ� ��ũ��Ʈ�� �Լ� �߰� �ʿ�
                break;

            // �ٸ� �������� ���� ȿ���� �ִٸ� ���⿡ �߰�
            default:
                Debug.Log($"Inventory.ApplyLocalItemEffect: No specific local effect defined for Item Type: {itemData.itemType}.");
                break;
        }
    }*/

    [PunRPC]
    void UseItemRPC(int itemTpyeInt, int senderViewID, float staminaRecover, float mentalRecover, float batteryRecover, PhotonMessageInfo info)
    {
        ItemType usedItemType = (ItemType)itemTpyeInt;
        PhotonView senderPV = PhotonView.Find(senderViewID);

        GameObject senderObject = senderPV?.gameObject;

        if (senderPV == null) return;

        switch (usedItemType)
        {            
            case ItemType.pills:
                MentalGauge mentalGauge = senderObject.GetComponent<MentalGauge>();
                if (mentalGauge != null)
                {
                    mentalGauge.RecoveryMental(mentalRecover); // MentalGauge�� RecoveryMental �Լ� ȣ��
                    Debug.Log($"UseItemRPC: Player {senderViewID} ���ŷ� {mentalRecover} ȸ�� �Ϸ�.");
                }
                else
                {
                    Debug.LogError($"UseItemRPC: MentalGauge ������Ʈ�� Player {senderViewID}���� ã�� �� �����ϴ�! ���ŷ� ȸ�� �Ұ�.");
                }
                
            break;

            case ItemType.bandage:
                StaminaSystem staminaSystemBandage = senderObject.GetComponent<StaminaSystem>();
                if (staminaSystemBandage != null)
                {
                    staminaSystemBandage.RecoverStamina(staminaRecover); // StaminaSystem�� RecoverStamina �Լ� ȣ��
                    Debug.Log($"UseItemRPC: Player {senderViewID} ���׹̳� {staminaRecover} ȸ�� �Ϸ� (Bandage).");
                }
                else
                {
                    Debug.LogError($"UseItemRPC: StaminaSystem ������Ʈ�� Player {senderViewID}���� ã�� �� �����ϴ�! ���׹̳� ȸ�� �Ұ�.");
                }
                // �ش� �������� ���ŷµ� ȸ����Ų�ٸ� ���� (ItemDataForInventory�� ������ �� ���)
                if (mentalRecover > 0)
                {
                    MentalGauge mentalGaugeBandage = senderObject.GetComponent<MentalGauge>(); // �ش� �÷��̾� ������Ʈ���� MentalGauge ������Ʈ ã��
                    if (mentalGaugeBandage != null)
                    {
                        mentalGaugeBandage.RecoveryMental(mentalRecover); // MentalGauge�� RecoveryMental �Լ� ȣ��
                        Debug.Log($"UseItemRPC: Player {senderViewID} ���ŷ� {mentalRecover} ȸ�� �Ϸ� (Bandage).");
                    }
                    else
                    {
                        Debug.LogError($"UseItemRPC: MentalGauge ������Ʈ�� Player {senderViewID}���� ã�� �� �����ϴ�! ���ŷ� ȸ�� �Ұ�.");
                    }
                }
                break;
            case ItemType.BatteryBig: // ���͸� ������
                Debug.Log($"UseItemRPC: Player {senderViewID} used Battery. Battery Charge: {staminaRecover}"); // ���͸� �������� ItemDataForInventory�� RecoveryStamina �ʵ带 ������� ���� �ֽ��ϴ�.
                // TODO: ���͸� ������ ȿ�� ���� (������ ���� ��)
                // senderObject���� ������ ��ũ��Ʈ ã�Ƽ� �Լ� ȣ�� (��: FlashlightSystem flashlight = senderObject.GetComponent<FlashlightSystem>(); flashlight?.ChargeBattery(staminaRecover);)
                ElectricTorchOnOff electricTorch = senderObject.GetComponentInChildren<ElectricTorchOnOff>();

                if (electricTorch != null)
                {
                    electricTorch.AddBattery(batteryRecover);
                    Debug.Log($"UseItemRPC: Player {senderViewID} ������ ���͸� {batteryRecover} ���� �Ϸ� (ū ���͸�).");
                }
                else
                {
                    Debug.LogError($"UseItemRPC: ElectricTorchOnOff ������Ʈ�� Player {senderViewID}���� ã�� �� �����ϴ�! ������ ���͸� ���� �Ұ�.");
                }
                break;
            
            case ItemType.BatterySmall: // ���͸� ������
                Debug.Log($"UseItemRPC: Player {senderViewID} used Battery. Battery Charge: {batteryRecover}"); // ���͸� �������� ItemDataForInventory�� RecoveryStamina �ʵ带 ������� ���� �ֽ��ϴ�.
                                                                                                            // TODO: ���͸� ������ ȿ�� ���� (������ ���� ��)
                electricTorch = senderObject.GetComponentInChildren<ElectricTorchOnOff>();

                if (electricTorch != null)
                {
                    electricTorch.AddBattery(batteryRecover);
                    Debug.Log($"UseItemRPC: Player {senderViewID} ������ ���͸� {batteryRecover} ���� �Ϸ� (ū ���͸�).");
                }
                else
                {
                    Debug.LogError($"UseItemRPC: ElectricTorchOnOff ������Ʈ�� Player {senderViewID}���� ã�� �� �����ϴ�! ������ ���͸� ���� �Ұ�.");
                }
                break;

            case ItemType.Camera: // ī�޶� ������
                Debug.Log($"UseItemRPC: Player {senderViewID} used Camera.");
                // TODO: ī�޶� ������ ȿ�� ���� (�� ���� ��)
                // senderObject ��ó�� ���� ã�Ƽ� ���� ����
                break;
        }
    }

    // ������ ȹ��
    public bool AddItem(ItemDataForInventory itemData)
    {
        if (itemData == null) return false;

        if (items.Count < inventorySize)
        {
            Debug.Log("������ ȹ��");
            items.Add(itemData);
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�!");
            return false;
        }
    }

    public void SetItemSlots(GameObject[] slots)
    {
        Debug.Log("Inventory.SetItemSlots() ȣ���"); // SetItemSlots ȣ�� �α�
        if (slots != null && slots.Length > 0)
        {
            itemSlots = slots;
            Debug.Log($"Inventory: ������ ���� ���� �Ϸ�! ���� ����: {itemSlots.Length}");
            UpdateUI(); // ���� ���� �� UI ������Ʈ
        }
        else
        {
            Debug.LogError("Inventory: ��ȿ���� ���� ������ ���޵�!");
        }
    }

    // UI ������Ʈ (UI ���Կ� ������ ǥ��)
    public void UpdateUI()
    {
        Debug.Log("Inventory.UpdateUI() ȣ���"); // UpdateUI ȣ�� �α�

        if (itemSlots == null)
        {
            Debug.LogError("ItemSlots �迭�� null�Դϴ�!");
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
                    Debug.LogError($"itemSlots[{i}]�� Image ������Ʈ�� �����ϴ�.");
                }
            }
        }
    }
}
