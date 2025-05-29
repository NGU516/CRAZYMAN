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

        // 인벤토리 UI 슬롯을 가져와서 Inventory에 설정
        GameObject[] itemImageSlots = new GameObject[2]; // 슬롯 개수에 맞춰 배열 생성
        itemImageSlots[0] = GetObject((int)GameObjects.Slot1Image);
        itemImageSlots[1] = GetObject((int)GameObjects.Slot2Image);

        // Inventory에 슬롯 배열 설정
        inventory.SetItemSlots(itemImageSlots);

        return true;
    }
}
