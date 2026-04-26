using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyKillDitector : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> targets = new List<EnemyBase>();
    public UnityEvent OnAllKilled;

    private void OnEnable()
    {
        EnemyBase.OnAnyEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        EnemyBase.OnAnyEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyBase enemy)
    {
        if (!targets.Remove(enemy)) return;

        if (targets.Count <= 0)
            OnAllKilled?.Invoke();
    }
}
