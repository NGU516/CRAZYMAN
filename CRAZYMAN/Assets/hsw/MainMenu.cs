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
    //이하 제작코드
    public void OnClickStart()
    {
        Debug.Log("게임 시작");
    }
    public void OnCLickSettings()
    {
        Debug.Log("환경설정");
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
