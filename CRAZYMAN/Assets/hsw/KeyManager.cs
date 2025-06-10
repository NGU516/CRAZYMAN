using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum KeyInput
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    INTERACT,
    RUN,
    SIT,
    Item1,
    Item2,//9
    KEYCOUNT
}

public static class KeySetting
{
    public static Dictionary<KeyInput, KeyCode> keys = new Dictionary<KeyInput, KeyCode>();
}

public class KeyManager : MonoBehaviour
{
    public KeyTextChanger keyTextChanger;
    KeyCode[] defaultKeys = new KeyCode[]//키 초기설정
    {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,//이동
        KeyCode.F, KeyCode.LeftShift, KeyCode.LeftControl,//상호작용, 뛰기, 앉기
        KeyCode.Alpha1, KeyCode.Alpha2,//아이템 1,2
    };
    private KeyInput? keyToRebind = null;

    void Awake()
    {
        KeySetting.keys.Clear();//키설정 초기화
        for (int i = 0; i < (int)KeyInput.KEYCOUNT; i++)
            KeySetting.keys.Add((KeyInput)i, defaultKeys[i]);
    }
    // Update is called once per frame
    void Update()
    {
        TestInput();
    }

    private void OnGUI()
    {
        if (keyToRebind.HasValue)
        {
            Event keyEvent = Event.current;
            if (keyEvent.isKey)
            {
                KeyInput key = keyToRebind.Value;
                KeyCode newKey = keyEvent.keyCode;

                if (newKey == KeyCode.Escape)
                {
                    keyTextChanger.StopBlinking(key, KeySetting.keys[key]); // 원래 키 유지
                    keyToRebind = null;
                    return;
                }

                bool isDuplicate = false;
                foreach (var kvp in KeySetting.keys)
                {
                    if (kvp.Key != key && kvp.Value == newKey)
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                if (isDuplicate)
                {
                    // 중복되면 기본값으로 설정
                    KeySetting.keys[key] = defaultKeys[(int)key];
                    keyTextChanger.StopBlinking(key, defaultKeys[(int)key]); // 깜빡임 멈추고 기본값 표시
                }
                else
                {
                    KeySetting.keys[key] = newKey;
                    keyTextChanger.StopBlinking(key, newKey); // 깜빡임 멈추고 새 키 표시
                }

                keyToRebind = null; // 변경 종료
            }
        }
    }
    public void ChangeKey(int num)//키 바꾸는 함수
    {
        keyToRebind = (KeyInput)num;
        keyTextChanger.StartBlinking((KeyInput)num); // 해당 텍스트 깜빡임 시작
    }

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
        else if (Input.GetKey(KeySetting.keys[KeyInput.INTERACT]))//상호작용
        {
            Debug.Log("INTERACT");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.RUN]))//뛰기
        {
            Debug.Log("RUN");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.SIT]))//앉기
        {
            Debug.Log("SIT");
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item1]))//아이템1
        {
            Debug.Log("ITEM1");
            //ItemInventory.instance.selectedItemIndex = 0;//첫번째 슬롯 선택
            //ItemInventory.instance.UseSelectedItem();//아이템 사용
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//아이템2
        {
            Debug.Log("ITEM2");
            //ItemInventory.instance.selectedItemIndex = 1;//두번째 슬롯 선택
            //ItemInventory.instance.UseSelectedItem();//아이템 사용
        }
    }
}
