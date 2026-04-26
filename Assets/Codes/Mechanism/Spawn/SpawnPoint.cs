using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnEntry
{
    public GameObject prefab;

    [Tooltip("出現の重み。値が大きいほど選ばれやすい")]
    [Min(0f)]
    public float weight = 1f;
}

public class SpawnPoint : MonoBehaviour
{
    [Header("スポーン確率")]
    [Range(0f, 1f)]
    [Tooltip("このポイントが何かをスポーンする確率（0=必ずスポーンしない, 1=必ずスポーンする）")]
    [SerializeField] private float spawnChance = 1f;

    [Header("スポーンテーブル")]
    [Tooltip("スポーン候補のリスト。weightの比率に応じてランダムで1つが選ばれる")]
    [SerializeField] private List<SpawnEntry> spawnTable = new List<SpawnEntry>();

    private void Start()
    {
        TrySpawn();
    }

    private void TrySpawn()
    {
        if (spawnTable == null || spawnTable.Count == 0) return;

        if (Random.value > spawnChance) return;

        float totalWeight = 0f;
        foreach (var entry in spawnTable)
        {
            if (entry.prefab != null)
                totalWeight += entry.weight;
        }

        if (totalWeight <= 0f) return;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in spawnTable)
        {
            if (entry.prefab == null) continue;

            cumulative += entry.weight;
            if (roll <= cumulative)
            {
                Instantiate(entry.prefab, transform.position, transform.rotation);
                return;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
        Gizmos.DrawSphere(transform.position, 1.0f);
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
    }
#endif
}
