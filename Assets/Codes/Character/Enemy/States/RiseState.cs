using UnityEngine;

public class RiseState : IEnemyState
{
    private EnemyBase enemy;
    private float riseSpeed;
    private float originalHeight;
    private float waitTime;
    private Transform target;

    public RiseState(EnemyBase enemy, float riseSpeed, float originalHeight, float waitTime, Transform target)
    {
        this.enemy = enemy;
        this.riseSpeed = riseSpeed;
        this.originalHeight = originalHeight;
        this.target = target;
        this.waitTime = waitTime;
    }

    public void Enter()
    {
        // 必要であれば、真上を向かせる
        enemy.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }

    public void Update()
    {
        // Y軸のみを目標の高さまで移動
        Vector3 pos = enemy.gameObject.transform.position;
        pos.y = Mathf.MoveTowards(pos.y, originalHeight, riseSpeed * Time.deltaTime);
        enemy.gameObject.transform.position = pos;

        // 元の高さに到達したらWaitStateへ移行
        if (Mathf.Abs(pos.y - originalHeight) < 0.01f)
        {
            enemy.StateMachine.ChangeState(new WaitState(enemy, waitTime, target));
        }
    }

    public void Exit()
    {
    }
}
