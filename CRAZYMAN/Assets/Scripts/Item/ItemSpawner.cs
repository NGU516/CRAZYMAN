using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject[] itemPrefabs;
    public int numberOfItemsToSpawn = 10; // 아이템 몇개 스폰할지

    [Header("Spawn Spots")]
    private List<Transform> spawnSpots = new List<Transform>();

    void Start()
    {
        spawnSpots.Clear();
        foreach (Transform child in transform)
        {
            spawnSpots.Add(child);
            Debug.Log($"ItemSpawner : 자식 {child.name} (위치: {child.position}) 추가");
        }
        Debug.Log($"ItemSpawner : 총 {spawnSpots.Count}개의 스폰 위치 찾음");

        // 만약 자식이 없으면 spawnSpots.Count는 0이 됨
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ItemSpawner : 마스터 클라이언트 아이템 스폰 시작 (Start 함수에서)");
            SpawnItems(); // 자식이 없으면 이 함수 안에서 spawnSpots.Count == 0 체크에 걸림
        }
        else
        {
            Debug.Log("ItemSpawner : 마스터 클라이언트가 아님 (Start 함수에서)");
        }
    }

    public void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("ItemSpawner : 스폰할 아이템 프리팹이 없습니다!");
            return;
        }

        if (spawnSpots.Count == 0)
        {
            Debug.LogWarning("ItemSpawner : 스폰 위치가 없습니다!");
            return;
        }

        // 스폰할 아이템 개수는 스폰 위치 개수보다 클 수 없음
        int itemsToSpawn = Mathf.Min(numberOfItemsToSpawn, spawnSpots.Count);
        Debug.Log($"ItemSpawner : 총 {itemsToSpawn}개의 아이템 스폰 시도");

        // 위치 복사본 생성 (중복 스폰 방지)
        List<Transform> availableSpots = new List<Transform>(spawnSpots);

        for (int i = 0; i < itemsToSpawn; i++)
        {
            Debug.Log($"ItemSpawner : {i + 1}번째 아이템 스폰 시도");

            // 랜덤 아이템 선택
            int prefabIndex = Random.Range(0, itemPrefabs.Length);
            GameObject itemToSpawn = itemPrefabs[prefabIndex];

            // 랜덤 위치 선택
            int spotIndex = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[spotIndex];

            Debug.Log($"ItemSpawner : {itemToSpawn.name} 아이템을 {spawnPoint.name} 위치에 생성");

            // Photon으로 아이템 생성 (Resources 폴더 내 프리팹 이름과 일치해야 함)
            GameObject spawnedItem = PhotonNetwork.Instantiate(
                "Prefabs/Items/" + itemToSpawn.name,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // 생성된 아이템을 ItemSpawner의 자식으로 설정
            spawnedItem.transform.SetParent(transform);

            // 해당 위치는 사용했으니 제거
            availableSpots.RemoveAt(spotIndex);
        }
    }

}
