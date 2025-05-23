using UnityEngine;

public class MainSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("메인 씬 초기화");

        Time.timeScale = 0;

        if (Managers.UI != null)
        {
            Debug.Log("UIManager 찾음! UIMain 팝업 띄우기 시도!");
            Managers.UI.ShowPopupUI<UIMain>("UIMain"); // <UIMain>은 스크립트 타입 이름, "UIMain"은 프리팹 이름!
            Debug.Log("UIMain 팝업 띄우기 호출");
        }
        else
        {
            Debug.LogError("UIManager를 찾을 수 없음. Managers 클래스가 초기화되었는지 확인");
        }

        Debug.Log("메인 씬 초기화 완료");
    }
}
