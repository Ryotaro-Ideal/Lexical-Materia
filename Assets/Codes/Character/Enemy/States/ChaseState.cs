using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{
    EnemyBase enemy;
    Transform target;
    float speed;

    public ChaseState(EnemyBase enemy, Transform target, float speed)
    {
        this.enemy = enemy;
        this.target = target;
        this.speed = speed;
    }

    public void Enter()
    {


    }
    public void Update()
    {
        if (enemy.HasLostTarget())
        {
            enemy.StateMachine.ChangeState(new MoveState(enemy));
            return;
        }
        Vector3 dir = (target.position - enemy.gameObject.transform.position).normalized;
        enemy.gameObject.transform.rotation = Quaternion.LookRotation(dir);
        enemy.gameObject.transform.position = Vector3.MoveTowards(enemy.gameObject.transform.position, target.position, speed * Time.deltaTime);
    }
    public void Exit()
    {

    }
}