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
    KeyCode[] defaultKeys = new KeyCode[]//Ű �ʱ⼳��
    {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,//�̵�
        KeyCode.F, KeyCode.LeftShift, KeyCode.LeftControl,//��ȣ�ۿ�, �ٱ�, �ɱ�
        KeyCode.Alpha1, KeyCode.Alpha2,//������ 1,2
    };
    private KeyInput? keyToRebind = null;

    void Awake()
    {
        KeySetting.keys.Clear();//Ű���� �ʱ�ȭ
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
                    keyTextChanger.StopBlinking(key, KeySetting.keys[key]); // ���� Ű ����
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
                    // �ߺ��Ǹ� �⺻������ ����
                    KeySetting.keys[key] = defaultKeys[(int)key];
                    keyTextChanger.StopBlinking(key, defaultKeys[(int)key]); // ������ ���߰� �⺻�� ǥ��
                }
                else
                {
                    KeySetting.keys[key] = newKey;
                    keyTextChanger.StopBlinking(key, newKey); // ������ ���߰� �� Ű ǥ��
                }

                keyToRebind = null; // ���� ����
            }
        }
    }
    public void ChangeKey(int num)//Ű �ٲٴ� �Լ�
    {
        keyToRebind = (KeyInput)num;
        keyTextChanger.StartBlinking((KeyInput)num); // �ش� �ؽ�Ʈ ������ ����
    }

    public void TestInput()
    {
        if (Input.GetKey(KeySetting.keys[KeyInput.UP]))//��
        {
            Debug.Log("UP");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.DOWN]))//��
        {
            Debug.Log("DOWN");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))//��
        {
            Debug.Log("LEFT");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT]))//��
        {
            Debug.Log("RIGHT");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.INTERACT]))//��ȣ�ۿ�
        {
            Debug.Log("INTERACT");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.RUN]))//�ٱ�
        {
            Debug.Log("RUN");
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.SIT]))//�ɱ�
        {
            Debug.Log("SIT");
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item1]))//������1
        {
            Debug.Log("ITEM1");
            //ItemInventory.instance.selectedItemIndex = 0;//ù��° ���� ����
            //ItemInventory.instance.UseSelectedItem();//������ ���
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//������2
        {
            Debug.Log("ITEM2");
            //ItemInventory.instance.selectedItemIndex = 1;//�ι�° ���� ����
            //ItemInventory.instance.UseSelectedItem();//������ ���
        }
    }
}
