using UnityEngine;

public class MainSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("���� �� �ʱ�ȭ");

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
