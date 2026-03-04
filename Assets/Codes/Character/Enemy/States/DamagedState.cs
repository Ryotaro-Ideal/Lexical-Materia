using UnityEngine;
using System.Collections;

public class DamagedState : IEnemyState
{
    private EnemyBase enemy;
    private float timer;
    private float damagedDuration = 1.0f;

    public DamagedState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        if (enemy.InvincibleController != null)
        {
            enemy.InvincibleController.StartInvincibility();
        }

        if (enemy.animator != null)
        {
            enemy.animator.SetTrigger("OnDamaged");
        }

        timer = 0f;
    }

    public void Update()
    {

        timer += Time.deltaTime;
        if (timer >= damagedDuration)
        {
            enemy.StateMachine.ChangeState(new MoveState(enemy));
        }
    }

    public void Exit()
    {
    }
}