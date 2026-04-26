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
        // ターゲットを見失ったらMoveStateに戻る
        if (enemy.HasLostTarget())
        {
            enemy.StateMachine.ChangeState(new MoveState(enemy));
            return;
        }

        float speed = enemy.GetCurrentChaseSpeed();
        Vector3 dir = (target.position - enemy.gameObject.transform.position).normalized;
        enemy.gameObject.transform.rotation = Quaternion.LookRotation(dir);
        enemy.gameObject.transform.position = Vector3.MoveTowards(enemy.gameObject.transform.position, target.position, speed * Time.deltaTime);
    }
    public void Exit()
    {

    }
}