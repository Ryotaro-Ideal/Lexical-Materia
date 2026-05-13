using UnityEngine;

public class MoveState : IEnemyState
{
    private EnemyBase enemy;

    public MoveState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        var target = enemy.ScanTarget();
        if (target != null)
        {
            enemy.StateMachine.ChangeState(enemy.CreateChaseState(target.transform));
            return;
        }

        enemy.Move(enemy.transform.forward);
    }

    public void Exit()
    {
    }
}
