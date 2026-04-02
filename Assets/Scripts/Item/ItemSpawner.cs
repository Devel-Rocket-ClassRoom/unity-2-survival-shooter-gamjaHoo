using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public ItemData itemData;
        public GameObject itemPrefab;
        [Range(0f, 1f)]
        public float spawnWeight = 1f;
    }

    [Header("스폰할 아이템 목록")]
    public SpawnEntry[] spawnEntries;

    [Header("스폰 범위 — 맵 중심 기준")]
    public Vector3 spawnCenter = Vector3.zero;  // 맵 중심 좌표
    public float spawnRadius = 20f;            // 스폰 반경

    [Header("타이밍")]
    public float minInterval = 8f;   // 최소 스폰 간격
    public float maxInterval = 20f;  // 최대 스폰 간격

    [Header("수명")]
    public float minLifetime = 8f;   // 아이템 최소 유지 시간
    public float maxLifetime = 15f;  // 아이템 최대 유지 시간

    [Header("제한")]
    public int maxItemsOnMap = 5;

    private List<GameObject> activeItems = new();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // 랜덤 타이밍 대기
            float wait = UnityEngine.Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            TrySpawn();
        }
    }

    void TrySpawn()
    {
        activeItems.RemoveAll(item => item == null);
        if (activeItems.Count >= maxItemsOnMap) return;

        SpawnEntry entry = PickRandomEntry();
        if (entry == null) return;

        // NavMesh 위 랜덤 위치 탐색
        Vector3 spawnPos;
        if (!TryGetRandomNavMeshPos(out spawnPos)) return;

        GameObject obj = Instantiate(entry.itemPrefab, spawnPos, Quaternion.identity);

        Item item = obj.GetComponent<Item>();
        if (item != null) item.data = entry.itemData;

        activeItems.Add(obj);

        // 랜덤 수명
        float lifetime = UnityEngine.Random.Range(minLifetime, maxLifetime);
        StartCoroutine(DespawnAfter(obj, lifetime));
    }

    bool TryGetRandomNavMeshPos(out Vector3 result)
    {
        // 최대 10번 시도
        for (int i = 0; i < 10; i++)
        {
            // 반경 안 랜덤 방향 + 거리
            Vector2 rand2D = UnityEngine.Random.insideUnitCircle * spawnRadius;
            Vector3 randPos = spawnCenter + new Vector3(rand2D.x, 0f, rand2D.y);

            // NavMesh 위 가장 가까운 점 탐색
            if (NavMesh.SamplePosition(randPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        Debug.LogWarning("ItemSpawner: NavMesh 위 스폰 위치를 찾지 못했어요.");
        return false;
    }

    IEnumerator DespawnAfter(GameObject obj, float lifetime)
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            if (obj == null) yield break;

            elapsed += Time.deltaTime;

            // 삭제 3초 전부터 깜빡임
            if (lifetime - elapsed <= 3f)
            {
                Renderer rend = obj.GetComponentInChildren<Renderer>();
                if (rend != null)
                    rend.enabled = Mathf.FloorToInt(elapsed * 6f) % 2 == 0;
            }

            yield return null;
        }

        if (obj != null)
        {
            activeItems.Remove(obj);
            Destroy(obj);
        }
    }

    SpawnEntry PickRandomEntry()
    {
        if (spawnEntries == null || spawnEntries.Length == 0) return null;

        float total = 0f;
        foreach (var e in spawnEntries) total += e.spawnWeight;

        float rand = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in spawnEntries)
        {
            cumulative += e.spawnWeight;
            if (rand <= cumulative) return e;
        }

        return spawnEntries[spawnEntries.Length - 1];
    }

    // 씬 뷰에서 스폰 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.2f);
        Gizmos.DrawSphere(spawnCenter, spawnRadius);
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireSphere(spawnCenter, spawnRadius);
    }
}