using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyTextManager : MonoBehaviour
{
    public Text[] txt;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < txt.Length; i++)
            txt[i].text = KeySetting.keys[(KeyInput)i].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        KeyTextChange();
    }

    public void KeyTextChange()
    {
        for (int i = 0; i < txt.Length; i++)
            txt[i].text = KeySetting.keys[(KeyInput)i].ToString();//입력한 키로 바뀜
    }
}
