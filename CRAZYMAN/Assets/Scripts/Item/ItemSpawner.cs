using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject[] itemPrefabs;
    public int numberOfItemsToSpawn = 10; // ������ � ��������

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
            Debug.LogWarning("������ �������");
            return;
        }

        if (spawnSpots == null || spawnSpots.Length == 0)
        {
            Debug.LogWarning("���������� �������");
            return;
        }

        List<Transform> availableSpawnSpots = new List<Transform>(spawnSpots);
        int itemsToActuallySpawn = Mathf.Min(numberOfItemsToSpawn, availableSpawnSpots.Count);

        Debug.Log($"spawn {numberOfItemsToSpawn} items.");

        for (int i = 0; i < itemsToActuallySpawn; i++)
        {
            // ������ ������ �����ϰ� ��
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
                Debug.LogWarning($"Spawn spot at available list index {randomSpotListIndex} is null in available list! �� ������ �������. ���� ��ŵ.");
                availableSpawnSpots.RemoveAt(randomSpotListIndex); // ��� �Ұ����� ���� ����
                i--;
                continue;
            }

            GameObject spawnedItem = Instantiate(itemToSpawn, spawnSpot.position, spawnSpot.rotation);
            spawnedItem.transform.SetParent(transform);

            availableSpawnSpots.RemoveAt(randomSpotListIndex);
        }
    }
}
