using UnityEngine;
using Photon.Pun;
using System.Collections;

public class MainSceneInitializer : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("���� �� �ʱ�ȭ");

        // Photon ���� �Ϸ���� ���
        float timeout = 10f;
        float elapsed = 0f;
        while (!NetworkManager.Instance.IsPhotonReady && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (!NetworkManager.Instance.IsPhotonReady)
        {
            Debug.LogError("Photon ���� ���� ���� (Ÿ�Ӿƿ�)");
            yield break;
        }

        Time.timeScale = 0;

        if (Managers.UI != null)
        {
            Debug.Log("UIManager ã��! UIMain �˾� ���� �õ�!");
            Managers.UI.ShowPopupUI<UIMain>("UIMain"); // <UIMain>�� ��ũ��Ʈ Ÿ�� �̸�, "UIMain"�� ������ �̸�!
            Debug.Log("UIMain �˾� ���� ȣ��");
        }
        else
        {
            Debug.LogError("UIManager�� ã�� �� ����. Managers Ŭ������ �ʱ�ȭ�Ǿ����� Ȯ��");
        }

        Debug.Log("���� �� �ʱ�ȭ �Ϸ�");
    }
}
