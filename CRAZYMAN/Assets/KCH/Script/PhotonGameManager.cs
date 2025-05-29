using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class PhotonGameManager : MonoBehaviour
{
    public MentalGauge mentalGauge;
    private bool isGameOver = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate("prefabs/Player", new Vector3(0, 1, 0), Quaternion.identity);
        }

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
