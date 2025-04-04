using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private static DataManager dataManager = new();
    private static UIManager uiManager = new();
    private static ResourceManager resourceManager = new();
    private static SoundManager soundManager = new();
    private static GameManager gameManager = new();
    private static ItemManager itemManager = new();

    public static DataManager Data { get { Init(); return dataManager; } }
    public static UIManager UI { get { Init(); return uiManager; } }
    public static ResourceManager Resource { get { Init(); return resourceManager; } }
    public static GameManager GameManager { get { Init(); return gameManager; } }
    public static SoundManager SoundManager { get { Init(); return soundManager; } }
    public static ItemManager ItemManager { get { Init(); return itemManager; } }

    void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            Instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);

            dataManager.Init();
            soundManager.Init();
            //itemManager.Init();

            Application.targetFrameRate = 60;
        }
    }
}
