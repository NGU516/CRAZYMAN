using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playerSpawnPoints;
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

        // 마스터 클라이언트가 플레이어 번호 할당
        if (PhotonNetwork.IsMasterClient)
        {
            AssignPlayerNumbers();
            SpawnEnemies();
            SpawnItems();
            SpawnPlayer();
        }
    }

    private void AssignPlayerNumbers()
    {
        // 모든 플레이어에게 순서대로 번호 할당
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerNumbers.ContainsKey(player.ActorNumber))
            {
                playerNumbers[player.ActorNumber] = nextPlayerNumber++;
                Debug.Log($"[PHOTON] Player {player.NickName} (ID: {player.ActorNumber}) assigned number: {playerNumbers[player.ActorNumber]}");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PHOTON] Player {newPlayer.NickName} (ID: {newPlayer.ActorNumber}) joined the room");

        // 새로 들어온 플레이어에게 번호 할당
        if (PhotonNetwork.IsMasterClient)
        {
            playerNumbers[newPlayer.ActorNumber] = nextPlayerNumber++;
            Debug.Log($"[PHOTON] New player {newPlayer.NickName} assigned number: {playerNumbers[newPlayer.ActorNumber]}");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PHOTON] Player {otherPlayer.NickName} (ID: {otherPlayer.ActorNumber}) left the room");

        // 나간 플레이어의 번호 제거
        if (playerNumbers.ContainsKey(otherPlayer.ActorNumber))
        {
            Debug.Log($"[PHOTON] Removed player number {playerNumbers[otherPlayer.ActorNumber]} for {otherPlayer.NickName}");
            playerNumbers.Remove(otherPlayer.ActorNumber);
        }
    }

    // 플레이어 번호 가져오기
    public int GetPlayerNumber(int actorNumber)
    {
        if (playerNumbers.ContainsKey(actorNumber))
        {
            return playerNumbers[actorNumber];
        }
        return -1; // 번호가 할당되지 않은 경우
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
        if (playerPrefab != null && playerSpawnPoints.Length > 0)
        {
            int spawnIndex = Random.Range(0, playerSpawnPoints.Length);
            Transform spawnPoint = playerSpawnPoints[spawnIndex];

            PhotonNetwork.Instantiate("Prefabs/Player_Object", spawnPoint.position, spawnPoint.rotation);
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
    
    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoints.Length > 0)
        {
            int spawnIndex = Random.Range(0, playerSpawnPoints.Length);
            Transform spawnPoint = playerSpawnPoints[spawnIndex];

            PhotonNetwork.Instantiate("Prefabs/Player_Object", spawnPoint.position, spawnPoint.rotation);
        }
    }
}
