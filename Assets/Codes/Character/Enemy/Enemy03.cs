using UnityEngine;

public class Enemy03 : EnemyBase
{
    [Header("Aerial Settings")]
    public float riseSpeed = 5f;
    public float diveRadius = 1.0f;
    public float waitTime = 2.0f;
    public float rotateSpeed = 10f;
    private float firstHeight;
    private bool isChasing = false;


    protected override void Awake()
    {
        base.Awake();
        firstHeight = transform.position.y + 2f;
        stateMachine.ChangeState(new MoveState(this));
    }

    protected override void Update()
    {
        base.Update();
        if (isChasing) transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime);
    }

    public override IEnemyState CreateChaseState(Transform target)
    {
        if (!isChasing)
        {
            isChasing = true;
            return new RiseState(this, riseSpeed, firstHeight, waitTime, target);
        }
        return new DiveChaseState(this, target, target.position, chaseSpeed, diveRadius, riseSpeed, firstHeight, waitTime);

    }

    public override IEnemyState CreateMoveState()
    {
        isChasing = false;
        return new MoveState(this);
    }

    protected override void Die()
    {
        base.Die();
    }
}