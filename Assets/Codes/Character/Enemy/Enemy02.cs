using UnityEngine;

public class Enemy02 : EnemyBase
{
    [SerializeField] private float fastChaseTime = 3f;  // 速いフェーズの秒数
    [SerializeField] private float coolTime = 2f;        // 遅いフェーズの秒数
    [SerializeField] private float minChaseSpeed = 1f;  // 遅いフェーズの速度

    private float chaseTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        stateMachine.ChangeState(new MoveState(this));
    }

    protected override void Update()
    {
        // ChaseState中だけタイマーを進め、それ以外はリセット
        if (StateMachine.CurrentState is ChaseState)
            chaseTimer += Time.deltaTime;
        else
            chaseTimer = 0f;

        base.Update();
    }

    public override float GetCurrentChaseSpeed()
    {
        float cycleTime = fastChaseTime + coolTime;
        float timeInCycle = chaseTimer % cycleTime; // サイクル内の経過時間（自動でループする）

        if (timeInCycle < fastChaseTime)
            return chaseSpeed;   // 速いフェーズ
        else
            return minChaseSpeed; // 遅いフェーズ
    }

    protected override void Die()
    {
        base.Die();
    }
}