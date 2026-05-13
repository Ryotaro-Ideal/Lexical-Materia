using UnityEngine;
using System.Collections;

public class DamagedState : IEnemyState
{
    private EnemyBase enemy;
    private float timer;
    private float damagedDuration = 1.0f;
    private Transform attacker;

    public DamagedState(EnemyBase enemy, Transform attacker)
    {
        this.enemy = enemy;
        this.attacker = attacker;
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
            enemy.StateMachine.ChangeState(enemy.CreateChaseState(attacker));
        }
    }

    public void Exit()
    {
    }
}