using UnityEngine;
public class Enemy01 : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        stateMachine.ChangeState(new MoveState(this));
    }


    protected override void Die()
    {
        Destroy(gameObject);
    }
}