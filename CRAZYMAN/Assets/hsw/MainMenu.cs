using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //���� �����ڵ�
    public void OnClickStart()
    {
        Debug.Log("���� ����");
    }
    public void OnCLickSettings()
    {
        Debug.Log("ȯ�漳��");
    }
    public void OnClickQuit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
