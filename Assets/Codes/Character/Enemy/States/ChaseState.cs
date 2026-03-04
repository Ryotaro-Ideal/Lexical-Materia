using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{
    EnemyBase enemy;
    Transform target;

    public ChaseState(EnemyBase enemy, Transform target)
    {
        this.enemy = enemy;
        this.target = target;
    }

    public void Enter()
    {


    }
    public void Update()
    {
        float speed = enemy.chaseSpeed;
        enemy.gameObject.transform.position = Vector3.MoveTowards(enemy.gameObject.transform.position, target.position, speed * Time.deltaTime);


    }
    public void Exit()
    {

    }
}