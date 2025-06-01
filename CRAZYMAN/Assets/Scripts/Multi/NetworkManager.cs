using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    //[SerializeField] private GameObject playerPrefab;
    //[SerializeField] private Transform[] spawnPoints;
    //[SerializeField] private string gameVersion = "1.0";

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private string gameVersion = "1.0";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = gameVersion;
        Debug.Log("[PHOTON] Awake: NetworkManager initialized");
    }

    private void Start()
    {
        Debug.Log($"[PHOTON] Start: Attempting to connect to Photon");
        Debug.Log($"[PHOTON] IsMasterClient: {PhotonNetwork.IsMasterClient}");
        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("[PHOTON] Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("[PHOTON] Already connected to Photon");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PHOTON] Connected to Photon Master Server");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"[PHOTON] Failed to join random room: {message} (code: {returnCode})");
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };

        Debug.Log("[PHOTON] Creating new room...");
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PHOTON] Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"[PHOTON] Room player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab != null && enemySpawnPoints.Length > 0)
        {
            foreach (Transform spawnPoint in enemySpawnPoints)
            {
                Debug.Log($"[PHOTON] Spawning enemy at {spawnPoint.position}");
                PhotonNetwork.Instantiate(
                    "Prefabs/" + enemyPrefab.name,
                    spawnPoint.position,
                    spawnPoint.rotation
                );
            }
        }
        else
        {
            Debug.LogError("[PHOTON] Enemy prefab or spawn points not set!");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PHOTON] Player {newPlayer.NickName} joined the room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PHOTON] Player {otherPlayer.NickName} left the room");
    }

    // 플레이어 재스폰 요청
    //public void RequestRespawn()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        SpawnPlayer();
    //    }
    //}
}
