using UnityEngine;

public class EnemyStateMachine
{
    private EnemyBase enemy;
    private IEnemyState currentState;


    public EnemyStateMachine(EnemyBase enemy)
    {
        this.enemy = enemy;
    }
    private void Awake()
    {

    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }

    }

    public EnemyBase Enemy => enemy;
    public IEnemyState CurrentState => currentState;
}
public interface IEnemyState
{
    void Enter();   // 状態に入った瞬間
    void Update();  // 毎フレーム
    void Exit();    // 状態を抜ける瞬間
}
