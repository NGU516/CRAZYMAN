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
    KeyCode[] defaultKeys = new KeyCode[]//Ű �ʱ⼳��
    {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,//�̵�
        KeyCode.F//��ȣ�ۿ�
    };
    void awake()
    {
        KeySetting.keys.Clear();//Ű���� �ʱ�ȭ
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
    public void ChangeKey(int num)//Ű �ٲٴ� �Լ�
    {
        key = num;
    }

    // Start is called before the first frame update
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
        /*
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item1]))//������1
        {
            ItemInventory.instance.selectedItemIndex = 0;//ù��° ���� ����
            ItemInventory.instance.UseSelectedItem();//������ ���
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//������2
        {
            ItemInventory.instance.selectedItemIndex = 1;//�ι�° ���� ����
            ItemInventory.instance.UseSelectedItem();//������ ���
        }
        else if (Input.GetKeyDown(KeySetting.keys[KeyInput.Item2]))//������3
        {
            ItemInventory.instance.selectedItemIndex = 2;//����° ���� ����
            ItemInventory.instance.UseSelectedItem();//������ ���
        }
        */
        else if (Input.GetKey(KeySetting.keys[KeyInput.INTERACT]))//��ȣ�ۿ�
        {
            //Debug.Log("INTERACT");
        }
    }
}
