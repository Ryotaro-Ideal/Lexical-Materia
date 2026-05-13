using UnityEngine;

public class WaitState : IEnemyState
{
    private EnemyBase enemy;
    private float waitTime;
    private float timer;
    private Transform target;

    public WaitState(EnemyBase enemy, float waitTime, Transform target)
    {
        this.enemy = enemy;
        this.waitTime = waitTime;
        this.target = target;
    }

    public void Enter()
    {
        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            if (target == null || enemy.HasLostTarget()) enemy.StateMachine.ChangeState(enemy.CreateMoveState());
            else enemy.StateMachine.ChangeState(enemy.CreateChaseState(target));
        }
    }

    public void Exit()
    {
    }
}
