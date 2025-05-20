using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MentalGauge mentalGauge;
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if(mentalGauge == null)
        {
            Debug.LogError("MetalGauge is not assigned in GameManager! Please assign it in the Inspector");
        }
        else
        {
            mentalGauge.OnDeathRequest += HandleDeath;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    private void HandleDeath(string cause)
    {
        if (isGameOver)
            return;

        isGameOver = true;
        Debug.Log($"player Died! Cause: {cause}");
// ���� ���� UI ǥ�� (���߿� �߰�)
// ��: gameOverUI.SetActive(true);
    }

    private void RestartGame()
    {
        isGameOver = false;
        mentalGauge.ResetMentalGauge();
        Debug.Log("Game Restarted!");
    }

    public void RequestDeath(string cause)
    {
        HandleDeath(cause);
    }
}
