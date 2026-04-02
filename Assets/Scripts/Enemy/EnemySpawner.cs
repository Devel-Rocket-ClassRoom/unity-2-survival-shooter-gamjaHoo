using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject enemyPrefab;
        public EnemyData enemyData;
        [Range(0f, 1f)]
        public float spawnWeight = 1f;
    }

    [Header("적 목록")]
    public EnemyEntry[] enemyEntries;

    [Header("스폰 포인트")]
    public Transform[] spawnPoints;

    [Header("스폰 설정")]
    public float minInterval = 2f;   // 최소 스폰 간격
    public float maxInterval = 5f;   // 최대 스폰 간격
    public int maxEnemiesOnMap = 20;   // 동시 최대 적 수

    private List<GameObject> activeEnemies = new();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            TrySpawn();
        }
    }

    void TrySpawn()
    {
        activeEnemies.RemoveAll(e => e == null);
        if (activeEnemies.Count >= maxEnemiesOnMap) return;

        EnemyEntry entry = PickRandom();
        if (entry == null) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject obj = Instantiate(
            entry.enemyPrefab,
            point.position,
            point.rotation
        );

        obj.GetComponent<EnemyHealth>()?.Setup(entry.enemyData);
        obj.GetComponent<EnemyMovement>()?.Setup(entry.enemyData);
        obj.GetComponent<EnemyAttack>()?.Setup(entry.enemyData);

        // 죽으면 리스트에서 제거
        obj.GetComponent<LivingEntity>().OnDead += () => activeEnemies.Remove(obj);

        activeEnemies.Add(obj);
    }

    EnemyEntry PickRandom()
    {
        if (enemyEntries == null || enemyEntries.Length == 0) return null;

        float total = 0f;
        foreach (var e in enemyEntries) total += e.spawnWeight;

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in enemyEntries)
        {
            cumulative += e.spawnWeight;
            if (rand <= cumulative) return e;
        }

        return enemyEntries[enemyEntries.Length - 1];
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.red;
        foreach (var p in spawnPoints)
            if (p != null) Gizmos.DrawWireSphere(p.position, 0.5f);
    }
}