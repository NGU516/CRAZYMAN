using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public MentalGauge mentalGauge;
    private bool isGameOver = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (mentalGauge == null)
        {
            mentalGauge = FindObjectOfType<MentalGauge>();
            Debug.LogError("MetalGauge is not assigned in GameManager! Please assign it in the Inspector");
            yield return null;
        }
        mentalGauge.OnDeathRequest += HandleDeath;
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
        
        // 네트워크 상의 모든 클라이언트에게 죽음 상태를 동기화
        photonView.RPC("SyncDeathState", RpcTarget.All, cause);
    }

    [PunRPC]
    private void SyncDeathState(string cause)
    {
        isGameOver = true;
        Debug.Log($"Player died on all clients! Cause: {cause}");
        
        // 게임 오버 UI 표시 (나중에 추가)
        // 예: gameOverUI.SetActive(true);
    }

    private void RestartGame()
    {
        photonView.RPC("SyncRestartGame", RpcTarget.All);
    }

    [PunRPC]
    private void SyncRestartGame()
    {
        isGameOver = false;
        if (mentalGauge != null)
        {
            mentalGauge.ResetMentalGauge();
        }
        Debug.Log("Game Restarted on all clients!");
    }

    public void RequestDeath(string cause)
    {
        HandleDeath(cause);
    }
}
