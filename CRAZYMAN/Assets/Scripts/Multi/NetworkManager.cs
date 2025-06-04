using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private string gameVersion = "1.0";

    [Header("Room Settings")]
    [SerializeField] private string roomName = "MyCustomRoom";
    [SerializeField] private int maxPlayers = 4;

    [Header("Item Settings")]
    [SerializeField] private GameObject itemSpawnerPrefab;
    [SerializeField] private GameObject keySpawnerPrefab;

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
        Debug.Log("[PHOTON] 포톤 네트워크 초기화");
    }

    private void Start()
    {
        Debug.Log($"[PHOTON] 포톤 네트워크 시작");
        Debug.Log($"[PHOTON] 마스터 클라이언트: {PhotonNetwork.IsMasterClient}");
        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("[PHOTON] 포톤 연결 시도 중...");
            // Photon 서버 연결
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("[PHOTON] 이미 포톤 연결됨");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PHOTON] 포톤 마스터 서버 연결 완료");
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"[PHOTON] 방 참여 실패: {message} (code: {returnCode})");
        CreateRoom();
    }

    // 방 생성
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };

        Debug.Log($"[PHOTON] 새로운 방 생성: {roomName}");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PHOTON] 방 참여: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"[PHOTON] 방 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"[PHOTON] 마스터 클라이언트: {PhotonNetwork.IsMasterClient}");
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnEnemies();
            SpawnItems();
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

    void SpawnItems()
    {
        // ItemSpawner 프리팹이 할당되었는지 확인
        if (itemSpawnerPrefab != null)
        {
            // ItemSpawner 생성 (위치는 (0, 0, 0)으로 설정!)
            GameObject itemSpawner = PhotonNetwork.Instantiate(
                "Prefabs/Items/"+ itemSpawnerPrefab.name,
                Vector3.zero,  // 위치는 (0, 0, 0)으로 설정!
                Quaternion.identity
            );

            // ItemSpawner 스크립트 가져오기
            ItemSpawner spawner = itemSpawner.GetComponent<ItemSpawner>();
            if (spawner != null)
            {
                // ItemSpawner에게 아이템 생성하도록 시킴
                spawner.SpawnItems();
            }
            else
            {
                Debug.LogError("ItemSpawner 프리팹에 ItemSpawner 스크립트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("ItemSpawner 프리팹이 할당되지 않았습니다!");
        }

        // KeySpawner 프리팹이 할당되었는지 확인
        if (keySpawnerPrefab != null)
        {
            // ItemSpawner 생성 (위치는 (0, 0, 0)으로 설정!)
            GameObject itemSpawner = PhotonNetwork.Instantiate(
                "Prefabs/Items/" + keySpawnerPrefab.name,
                Vector3.zero,  // 위치는 (0, 0, 0)으로 설정!
                Quaternion.identity
            );

            // ItemSpawner 스크립트 가져오기
            ItemSpawner spawner = itemSpawner.GetComponent<ItemSpawner>();
            if (spawner != null)
            {
                // ItemSpawner에게 아이템 생성하도록 시킴
                spawner.SpawnItems();
            }
            else
            {
                Debug.LogError("ItemSpawner 프리팹에 ItemSpawner 스크립트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("ItemSpawner 프리팹이 할당되지 않았습니다!");
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

}
