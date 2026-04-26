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
        base.Die(); // EnemyBase.Die() でドロップ生成 → Destroy まで行う
    }
}