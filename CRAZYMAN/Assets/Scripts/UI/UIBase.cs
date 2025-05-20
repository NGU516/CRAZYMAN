using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    protected bool _init = false;

    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    public virtual void CloseUI()
    {
        Debug.Log($"{gameObject.name} UI Close ȣ��� (UIBase)");
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i]  = Utils.FindChild(gameObject, names[i], true);

            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]}) in {gameObject.name}");
            else
                Debug.Log($"Successfully bound({names[i]}) in {gameObject.name}");
        }
    }

    public static void BindEvent(GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
    {
        switch (type)
        {
            case Define.UIEvent.Click:
                
                // GameObject���� Button ������Ʈ�� ã�Ƽ� Ŭ�� �̺�Ʈ�� Action ����
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => { action.Invoke(); }); // ���ٽ����� ���μ� Action ȣ��
                    Debug.Log($"Button click event bound for {go.name}");
                }
                else
                {
                    Debug.LogError($"GameObject {go.name} does not have a Button component for Click event binding!");
                }
                break;

            default:
                Debug.LogWarning($"Unsupported UIEvent type: {type} for GameObject {go.name}");
                break;
        }
    }

    protected void BindObject(Type type) { Bind<GameObject>(type); }
    protected void BindImage(Type type) { Bind<Image>(type); }
    protected void BindText(Type type) { Bind<TextMeshProUGUI>(type); }
    protected void BindButton(Type type) { Bind<Button>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        if (idx < 0 || idx >= objects.Length)
        {
            Debug.Log($"Invalid index {idx} for type {typeof(T).Name} in {gameObject.name}");
            return null;
        }
        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
}
