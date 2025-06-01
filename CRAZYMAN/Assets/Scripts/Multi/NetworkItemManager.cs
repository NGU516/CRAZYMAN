using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class NetworkItemManager : MonoBehaviourPunCallbacks
{
    public static NetworkItemManager Instance { get; private set; }
    
    [Header("Item Settings")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Transform itemSpawnerParent; // ItemSpawner 오브젝트만 할당

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 하위 오브젝트들을 자동으로 스폰 포인트로 등록
        if (itemSpawnerParent != null)
        {
            foreach (Transform child in itemSpawnerParent)
            {
                spawnPoints.Add(child);
            }
            Debug.Log($"[PHOTON] NetworkItemManager: Found {spawnPoints.Count} item spawn points under {itemSpawnerParent.name}");
        }
        else
        {
            Debug.LogWarning("[PHOTON] NetworkItemManager: itemSpawnerParent not set!");
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PHOTON] NetworkItemManager: MasterClient, spawning items");
            SpawnInitialItems();
        }
        else
        {
            Debug.Log("[PHOTON] NetworkItemManager: Not MasterClient, will not spawn items");
        }
    }

    private void SpawnInitialItems()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (itemPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, itemPrefabs.Length);
                GameObject item = PhotonNetwork.Instantiate(
                    itemPrefabs[randomIndex].name,
                    spawnPoint.position,
                    Quaternion.identity
                );
                spawnedItems.Add(item);
                Debug.Log($"[PHOTON] Item spawned: {item.name} at {spawnPoint.position}");
            }
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnedItems.Remove(item);
            PhotonNetwork.Destroy(item);
            Debug.Log($"[PHOTON] Item removed: {item.name}");
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject item in spawnedItems)
            {
                if (item != null)
                {
                    PhotonView itemView = item.GetComponent<PhotonView>();
                    if (itemView != null)
                    {
                        itemView.RPC("SyncItemState", newPlayer);
                        Debug.Log($"[PHOTON] Syncing item state for {item.name} to new player {newPlayer.NickName}");
                    }
                }
            }
        }
    }
}
