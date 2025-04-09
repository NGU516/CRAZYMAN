using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyInput
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    INTERACT,
    /*
    Item1,
    Item2,
    Item3,
    */
    KEYCOUNT
}

public static class KeySetting
{
    public static Dictionary<KeyInput, KeyCode> keys = new Dictionary<KeyInput, KeyCode>();
}

public class KeyManager : MonoBehaviour
{
    KeyCode[] defaultKeys = new KeyCode[]//키 초기설정
    {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,//이동
        KeyCode.F//상호작용
    };
    void awake()
    {
        KeySetting.keys.Clear();//키설정 초기화
        for(int i = 0; i < (int)KeyInput.KEYCOUNT; i++)
            KeySetting.keys.Add((KeyInput)i, defaultKeys[i]);
    }
    // Update is called once per frame
    void Update()
    {
        TestInput();
    }
    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            KeySetting.keys[(KeyInput)key] = keyEvent.keyCode;
            key = -1;
        }
    }
    int key = -1;
    public void ChangeKey(int num)//키 바꾸는 함수
    {
        key = num;
    }

    // Start is called before the first frame update
    public void TestInput()
    {
        if (Input.GetKey(KeySetting.keys[KeyInput.UP]))//앞
        {
            Debug.Log("UP");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.DOWN]))//뒤
        {
            Debug.Log("DOWN");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))//좌
        {
            Debug.Log("LEFT");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT]))//우
        {
            Debug.Log("RIGHT");
        }
        /*
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item1]))//아이템1
        {
            ItemInventory.instance.selectedItemIndex = 0;//첫번째 슬롯 선택
            ItemInventory.instance.UseSelectedItem();//아이템 사용
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//아이템2
        {
            ItemInventory.instance.selectedItemIndex = 1;//두번째 슬롯 선택
            ItemInventory.instance.UseSelectedItem();//아이템 사용
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//아이템3
        {
            ItemInventory.instance.selectedItemIndex = 2;//세번째 슬롯 선택
            ItemInventory.instance.UseSelectedItem();//아이템 사용
        }
        */
        else if (Input.GetKey(KeySetting.keys[KeyInput.INTERACT]))//상호작용
        {
            //Debug.Log("INTERACT");
        }
    }
}
