using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;


public class UIInGame : UIPopup
{
    enum Texts
    {

    }


    enum GameObjects
    {
        Sliders,
        Slot1,
        Slot2,
        Slot1Image,
        Slot2Image
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));

        Inventory inventory = FindObjectOfType<Inventory>();

        // �κ��丮 UI ������ �����ͼ� Inventory�� ����
        GameObject[] itemImageSlots = new GameObject[2]; // ���� ������ ���� �迭 ����
        itemImageSlots[0] = GetObject((int)GameObjects.Slot1Image);
        itemImageSlots[1] = GetObject((int)GameObjects.Slot2Image);

        // Inventory�� ���� �迭 ����
        inventory.SetItemSlots(itemImageSlots);

        return true;
    }
}
