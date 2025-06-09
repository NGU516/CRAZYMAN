using UnityEngine;
using Photon.Pun;
using System.Collections;

public class MainSceneInitializer : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("메인 씬 초기화");

        // Photon 연결 완료까지 대기
        float timeout = 10f;
        float elapsed = 0f;
        while (!NetworkManager.Instance.IsPhotonReady && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (!NetworkManager.Instance.IsPhotonReady)
        {
            Debug.LogError("Photon 서버 연결 실패 (타임아웃)");
            yield break;
        }

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
