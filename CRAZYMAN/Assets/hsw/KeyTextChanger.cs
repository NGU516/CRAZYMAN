using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyTextChanger : MonoBehaviour
{
    public Text[] txt;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

    void Start()
    {
        KeyTextChange();
    }

    void Update()
    {
        //KeyTextChange();
    }

    public void KeyTextChange()
    {
        for (int i = 0; i < txt.Length; i++)
            txt[i].text = GetDisplayKeyName(KeySetting.keys[(KeyInput)i]); // 사용자 친화적 이름 표시
    }

    public void UpdateKeyText(KeyInput key, KeyCode keyCode)
    {
        txt[(int)key].text = GetDisplayKeyName(keyCode);
    }
    public void StartBlinking(KeyInput key)
    {
        if (isBlinking)
            StopCoroutine(blinkCoroutine); // 기존 깜빡임 중단

        blinkCoroutine = StartCoroutine(BlinkText(key)); // 새로운 깜빡임 시작
    }

    public void StopBlinking(KeyInput key, KeyCode newKey)
    {
        if (isBlinking)
            StopCoroutine(blinkCoroutine); // 깜빡임 중단

        isBlinking = false;
        UpdateKeyText(key, newKey); // 새 키코드로 텍스트 표시
    }

    private IEnumerator BlinkText(KeyInput key)
    {
        isBlinking = true;
        Text target = txt[(int)key];

        while (true)
        {
            target.text = "_"; // 깜빡일 때 빈 텍스트와 '_' 반복
            yield return new WaitForSeconds(0.5f);
            target.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    private string GetDisplayKeyName(KeyCode keyCode)
    {
        // 유저 친화적으로 표시하기 위한 Alpha 키 처리
        switch (keyCode)
        {
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";
            case KeyCode.Alpha0: return "0";
            default: return keyCode.ToString(); // 그 외 키는 그대로 표시
        }
    }
}