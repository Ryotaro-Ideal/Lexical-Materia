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
        // アニメーションなどでMove状態にするならここ
    }

    public void Update()
    {
        // 索敵を行う
        var target = enemy.ScanTarget();
        if (target != null)
        {
            // 見つけたら ChaseState に遷移
            enemy.StateMachine.ChangeState(new ChaseState(enemy, target.transform));
            return;
        }

        // 見つからなければ通常の移動（本来はWanderなど詳細な動きが入る）
        enemy.Move(enemy.transform.forward);
    }

    public void Exit()
    {
    }
}
