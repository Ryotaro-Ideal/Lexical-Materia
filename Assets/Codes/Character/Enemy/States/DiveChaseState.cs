using UnityEngine;
using System.Collections;

public class DiveChaseState : IEnemyState
{
    EnemyBase enemy;
    Transform target;
    Vector3 targetPos; // Transformではなく、記憶した固定座標(Vector3)
    float speed;
    float diveRadius;
    float riseSpeed;
    float originalHeight;
    float waitTime;

    public DiveChaseState(EnemyBase enemy, Transform target, Vector3 targetPos, float speed, float diveRadius, float riseSpeed, float originalHeight, float waitTime)
    {
        this.enemy = enemy;
        this.target = target;
        this.targetPos = targetPos;
        this.speed = speed;
        this.diveRadius = diveRadius;
        this.riseSpeed = riseSpeed;
        this.originalHeight = originalHeight;
        this.waitTime = waitTime;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        // 記憶した座標へ向かって移動
        Vector3 dir = (targetPos - enemy.gameObject.transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            enemy.gameObject.transform.rotation = Quaternion.LookRotation(dir);
        }

        enemy.gameObject.transform.position = Vector3.MoveTowards(enemy.gameObject.transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(enemy.gameObject.transform.position, targetPos) <= diveRadius)
        {
            enemy.StateMachine.ChangeState(new RiseState(enemy, riseSpeed, originalHeight, waitTime, target));
        }
    }

    public void Exit()
    {
    }
}