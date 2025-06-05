using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject[] itemPrefabs;
    public int numberOfItemsToSpawn = 10; // ������ � ��������

    [Header("Spawn Spots")]
    private List<Transform> spawnSpots = new List<Transform>();

    void Start()
    {
        spawnSpots.Clear();
        foreach (Transform child in transform)
        {
            spawnSpots.Add(child);
            Debug.Log($"ItemSpawner : �ڽ� {child.name} (��ġ: {child.position}) �߰�");
        }
        Debug.Log($"ItemSpawner : �� {spawnSpots.Count}���� ���� ��ġ ã��");

        // ���� �ڽ��� ������ spawnSpots.Count�� 0�� ��
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ItemSpawner : ������ Ŭ���̾�Ʈ ������ ���� ���� (Start �Լ�����)");
            SpawnItems(); // �ڽ��� ������ �� �Լ� �ȿ��� spawnSpots.Count == 0 üũ�� �ɸ�
        }
        else
        {
            Debug.Log("ItemSpawner : ������ Ŭ���̾�Ʈ�� �ƴ� (Start �Լ�����)");
        }
    }

    public void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("ItemSpawner : ������ ������ �������� �����ϴ�!");
            return;
        }

        if (spawnSpots.Count == 0)
        {
            Debug.LogWarning("ItemSpawner : ���� ��ġ�� �����ϴ�!");
            return;
        }

        // ������ ������ ������ ���� ��ġ �������� Ŭ �� ����
        int itemsToSpawn = Mathf.Min(numberOfItemsToSpawn, spawnSpots.Count);
        Debug.Log($"ItemSpawner : �� {itemsToSpawn}���� ������ ���� �õ�");

        // ��ġ ���纻 ���� (�ߺ� ���� ����)
        List<Transform> availableSpots = new List<Transform>(spawnSpots);

        for (int i = 0; i < itemsToSpawn; i++)
        {
            Debug.Log($"ItemSpawner : {i + 1}��° ������ ���� �õ�");

            // ���� ������ ����
            int prefabIndex = Random.Range(0, itemPrefabs.Length);
            GameObject itemToSpawn = itemPrefabs[prefabIndex];

            // ���� ��ġ ����
            int spotIndex = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[spotIndex];

            Debug.Log($"ItemSpawner : {itemToSpawn.name} �������� {spawnPoint.name} ��ġ�� ����");

            // Photon���� ������ ���� (Resources ���� �� ������ �̸��� ��ġ�ؾ� ��)
            GameObject spawnedItem = PhotonNetwork.Instantiate(
                "Prefabs/Items/" + itemToSpawn.name,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // ������ �������� ItemSpawner�� �ڽ����� ����
            spawnedItem.transform.SetParent(transform);

            // �ش� ��ġ�� ��������� ����
            availableSpots.RemoveAt(spotIndex);
        }
    }

}
