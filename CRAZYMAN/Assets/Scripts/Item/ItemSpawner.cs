using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject[] itemPrefabs;
    public int numberOfItemsToSpawn = 10; // 아이템 몇개 스폰할지

    [Header("Spawn Spots")]
    public Transform[] spawnSpots;

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("아이템 비어잇음");
            return;
        }

        if (spawnSpots == null || spawnSpots.Length == 0)
        {
            Debug.LogWarning("스폰지점이 비어있음");
            return;
        }

        List<Transform> availableSpawnSpots = new List<Transform>(spawnSpots);
        int itemsToActuallySpawn = Mathf.Min(numberOfItemsToSpawn, availableSpawnSpots.Count);

        Debug.Log($"spawn {numberOfItemsToSpawn} items.");

        for (int i = 0; i < itemsToActuallySpawn; i++)
        {
            // 스폰할 아이템 랜덤하게 고름
            int randomPrefabIndex = Random.Range(0, itemPrefabs.Length);

            GameObject itemToSpawn = itemPrefabs[randomPrefabIndex];

            if (itemToSpawn == null)
            {
                Debug.LogWarning($"Item prefab at index {randomPrefabIndex} is null");
                i--;
                continue;
            }

            int randomSpotListIndex = Random.Range(0, availableSpawnSpots.Count);

            Transform spawnSpot = availableSpawnSpots[randomSpotListIndex];

            if (spawnSpot == null)
            {
                Debug.LogWarning($"Spawn spot at available list index {randomSpotListIndex} is null in available list! 이 지점이 비어있음. 스폰 스킵.");
                availableSpawnSpots.RemoveAt(randomSpotListIndex); // 사용 불가능한 지점 제거
                i--;
                continue;
            }

            GameObject spawnedItem = Instantiate(itemToSpawn, spawnSpot.position, spawnSpot.rotation);
            spawnedItem.transform.SetParent(transform);

            availableSpawnSpots.RemoveAt(randomSpotListIndex);
        }
    }
}
