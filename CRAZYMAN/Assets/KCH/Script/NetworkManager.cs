using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    //[SerializeField] private GameObject playerPrefab;
    //[SerializeField] private Transform[] spawnPoints;
    //[SerializeField] private string gameVersion = "1.0";

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playerSpawnPoints;
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

        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = gameVersion;

        Debug.Log("[PHOTON] Awake: NetworkManager initialized");
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
        Debug.Log("[PHOTON] State : " + PhotonNetwork.NetworkClientState);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("[PHOTON DEBUG] 연결됨!");
        }
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("[PHOTON] Connecting to Photon...");
            // PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
            // PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "7ebf3330-4f81-45b7-9d6b-49d630f2d9ec";
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("[PHOTON DEBUG] App ID : " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);

            if (PhotonNetwork.IsConnected)
                Debug.Log("[PHOTON] ✅ 연결 성공");
            else
                Debug.LogError("[PHOTON] ❌ 연결 실패");
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

        if (playerPrefab != null && playerSpawnPoints.Length > 0)
        {
            int spawnIndex = Random.Range(0, playerSpawnPoints.Length);
            Transform spawnPoint = playerSpawnPoints[spawnIndex];

            Debug.Log($"[PHOTON] Instantiating player prefab at {spawnPoint.position}");
            GameObject player = PhotonNetwork.Instantiate("Prefabs/Player_Object", spawnPoint.position, spawnPoint.rotation);
            if (player == null)
            {
                Debug.LogError("[PHOTON] Failed to instantiate player prefab!");
            }
        }
        else
        {
            Debug.LogError("[PHOTON] PlayerPrefab or spawnPoints not set");
        }

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     SpawnPlayers();
        // }
    }

    private void SpawnPlayers()
    {
        if (playerPrefab != null && playerSpawnPoints.Length > 0)
        {
            foreach (Transform spawnPoint in playerSpawnPoints)
            {
                Debug.Log($"[PHOTON] Spawning player at {spawnPoint.position}");
                PhotonNetwork.Instantiate(
                    "Prefabs/" + playerPrefab.name,
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("[PHOTON] 연결 끊김 원인: " + cause);
    }
}
