// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;
// using System.Collections.Generic;

// public class TempNetworkManager : MonoBehaviourPunCallbacks
// {
//     public static TempNetworkManager Instance { get; private set; }

//     [Header("Enemy Settings")]
//     [SerializeField] private GameObject enemyPrefab;
//     [SerializeField] private Transform[] enemySpawnPoints;
//     [SerializeField] private GameObject playerPrefab;
//     [SerializeField] private Transform[] playerSpawnPoints;
//     [SerializeField] private string gameVersion = "1.0";

//     [Header("Room Settings")]
//     [SerializeField] private string roomName = "MyCustomRoom";

//     [Header("Player Settings")]
//     private Dictionary<int, int> playerNumbers = new Dictionary<int, int>(); // ActorNumber -> PlayerNumber
//     private int nextPlayerNumber = 1;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         } else
//         {
//             Destroy(gameObject);
//         }

//         PhotonNetwork.AutomaticallySyncScene = false;
//         PhotonNetwork.GameVersion = gameVersion;
//         Debug.Log("[PHOTON] ���� ��Ʈ��ũ �ʱ�ȭ");
//     }

//     private void Start()
//     {
//         Debug.Log($"[PHOTON] ���� ��Ʈ��ũ ����");
//         Debug.Log($"[PHOTON] ������ Ŭ���̾�Ʈ: {PhotonNetwork.IsMasterClient}");
//         ConnectToPhoton();
//     }

//     private void ConnectToPhoton()
//     {
//         if (!PhotonNetwork.IsConnected)
//         {
//             Debug.Log("[PHOTON] ���� ���� �õ� ��...");
//             // Photon ���� ����
//             PhotonNetwork.ConnectUsingSettings();
//         } else
//         {
//             Debug.Log("[PHOTON] �̹� ���� �����");
//         }
//     }

//     public override void OnConnectedToMaster()
//     {
//         Debug.Log("[PHOTON] ���� ������ ���� ���� �Ϸ�");
//         PhotonNetwork.JoinRoom(roomName);
//     }

//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         Debug.LogWarning($"[PHOTON] �� ���� ����: {message} (code: {returnCode})");
//         CreateRoom();
//     }

//     // �� ����
//     private void CreateRoom()
//     {
//         RoomOptions roomOptions = new RoomOptions
//         {
//             MaxPlayers = 4,
//             IsVisible = true,
//             IsOpen = true
//         };

//         Debug.Log($"[PHOTON] ���ο� �� ����: {roomName}");
//         PhotonNetwork.CreateRoom(roomName, roomOptions);
//     }

//     public override void OnJoinedRoom()
//     {
//         Debug.Log($"[PHOTON] �� ����: {PhotonNetwork.CurrentRoom.Name}");
//         Debug.Log($"[PHOTON] �� �ο�: {PhotonNetwork.CurrentRoom.PlayerCount}");
//         Debug.Log($"[PHOTON] ������ Ŭ���̾�Ʈ: {PhotonNetwork.IsMasterClient}");
//         SpawnPlayer();

//         if (PhotonNetwork.IsMasterClient)
//         {
//             SpawnEnemies();
//             AssignPlayerNumbers();
//         }
//     }

//     private void AssignPlayerNumbers()
//     {
//         // ��� �÷��̾�� ������� ��ȣ �Ҵ�
//         foreach (Player player in PhotonNetwork.PlayerList)
//         {
//             if (!playerNumbers.ContainsKey(player.ActorNumber))
//             {
//                 playerNumbers[player.ActorNumber] = nextPlayerNumber++;
//                 Debug.Log($"[PHOTON] Player {player.NickName} (ID: {player.ActorNumber}) assigned number: {playerNumbers[player.ActorNumber]}");
//             }
//         }
//     }

//     private void SpawnEnemies()
//     {
//         if (enemyPrefab != null && enemySpawnPoints.Length > 0)
//         {
//             foreach (Transform spawnPoint in enemySpawnPoints)
//             {
//                 Debug.Log($"[PHOTON] Spawning enemy at {spawnPoint.position}");
//                 PhotonNetwork.Instantiate(
//                     "Prefabs/" + enemyPrefab.name,
//                     spawnPoint.position,
//                     spawnPoint.rotation
//                 );
//             }
//         } else
//         {
//             Debug.LogError("[PHOTON] Enemy prefab or spawn points not set!");
//         }
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         Debug.Log($"[PHOTON] Player {newPlayer.NickName} joined the room");

//         // ���� ���� �÷��̾�� ��ȣ �Ҵ�
//         if (PhotonNetwork.IsMasterClient)
//         {
//             playerNumbers[newPlayer.ActorNumber] = nextPlayerNumber++;
//             Debug.Log($"[PHOTON] New Player {newPlayer.NickName} assigned number: {playerNumbers[newPlayer.ActorNumber]}");

//         }
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         Debug.Log($"[PHOTON] Player {otherPlayer.NickName} left the room");

//         // ���� �÷��̾��� ��ȣ ����
//         if (playerNumbers.ContainsKey(otherPlayer.ActorNumber))
//         {
//             Debug.Log($"[PHOTON] Removed player number {playerNumbers[otherPlayer.ActorNumber]} for {otherPlayer.NickName}");
//             playerNumbers.Remove(otherPlayer.ActorNumber);
//         }
//     }

//     // �÷��̾� ��ȣ ��������
//     public int GetPlayerNumber(int actorNumber)
//     {
//         if (playerNumbers.ContainsKey(actorNumber))
//         {
//             return playerNumbers[actorNumber];
//         }
//         return -1; // ��ȣ�� �Ҵ���� ���� ���
//     }

//     private void SpawnPlayer()
//     {
//         if (playerPrefab != null && playerSpawnPoints.Length > 0)
//         {
//             int spawnIndex = Random.Range(0, playerSpawnPoints.Length);
//             Transform spawnPoint = playerSpawnPoints[spawnIndex];

//             PhotonNetwork.Instantiate("Prefabs/Player_Object", spawnPoint.position, spawnPoint.rotation);
//         }
//     }
// }
