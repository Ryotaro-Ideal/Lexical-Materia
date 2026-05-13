using UnityEngine;

public class Enemy04 : EnemyBase
{
    [Header("Aerial Settings")]
    public float riseSpeed = 5f;
    public float diveRadius = 1.0f;
    public float waitTime = 2.0f;
    private float firstHeight;

    protected override void Awake()
    {
        base.Awake();
        firstHeight = transform.position.y;
        stateMachine.ChangeState(new MoveState(this));
    }

    protected override void Update()
    {
        base.Update();
    }

    public override IEnemyState CreateChaseState(Transform target)
    {
        return new DiveChaseState(this, target, target.position, chaseSpeed, diveRadius, riseSpeed, firstHeight, waitTime);
    }

    protected override void Die()
    {
        base.Die();
    }
}