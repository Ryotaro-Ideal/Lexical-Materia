using UnityEngine;
using System;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Range(0, 20)]
    public int maxHp = 5;
    protected int currentHp;
    [Range(0, 10)]
    public int collideDamage = 1;
    [Range(0, 10)]
    public float moveSpeed;
    [Range(0, 10)]
    public float chaseSpeed;


    // 任意のKillTriggerが購読できる静的イベント
    public static event Action<EnemyBase> OnAnyEnemyDied;

    public GameObject dropItem;
    private float dropUpForce = 6f; // ドロップ時の上方向への力
    protected Rigidbody rb;
    protected EnemyStateMachine stateMachine; // 継承先でも使うのでprotectedのまま

    public EnemyStateMachine StateMachine => stateMachine; // Stateからアクセスするためにプロパティ化

    public InvincibleController InvincibleController { get; private set; }

    public Animator animator;
    private ISenser[] sensers;
    private ILoseSightSenser[] loseSensers;
    public GameObject CurrentTarget { get; private set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHp = maxHp;
        InvincibleController = GetComponent<InvincibleController>(); // 取得漏れを防ぐ
        stateMachine = new EnemyStateMachine(this);
        sensers = GetComponents<ISenser>();
        loseSensers = GetComponents<ILoseSightSenser>();

        // 初期ステート
        stateMachine.ChangeState(new MoveState(this));
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }

    public virtual IEnemyState CreateChaseState(Transform target)
    {
        return new ChaseState(this, target, chaseSpeed);
    }

    public virtual IEnemyState CreateMoveState()
    {
        return new MoveState(this);
    }

    public virtual float GetCurrentChaseSpeed() => chaseSpeed;

    public bool HasLostTarget()
    {
        if (CurrentTarget == null) return true;
        if (loseSensers == null || loseSensers.Length == 0) return false;
        foreach (var senser in loseSensers)
            if (senser.HasLostSight(CurrentTarget)) return true;
        return false;
    }

    public GameObject ScanTarget()
    {
        if (sensers == null) return null;

        foreach (var senser in sensers)
        {
            if (senser.TryDetect(out GameObject target))
            {
                CurrentTarget = target;
                return target;
            }
        }
        return null;
    }

    public void Move(Vector3 direction)
    {
        Vector3 dir = direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + dir);
    }

    public virtual void TakeDamage(int damage, Transform attacker)
    {
        if (currentHp <= 0) return;
        currentHp -= damage;

        // ダメージステートへの遷移（ここで強制的に上書き）
        stateMachine.ChangeState(new DamagedState(this, attacker));

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void OnDamaged()
    {

    }
    public void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (player != null && damageable != null)
        {
            damageable.TakeDamage(collideDamage, transform);
        }
    }
    protected virtual void Die()
    {
        OnAnyEnemyDied?.Invoke(this);
        if (dropItem != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
            GameObject dropped = Instantiate(dropItem, spawnPos, Quaternion.identity);

            if (dropped.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(Vector3.up * dropUpForce, ForceMode.Impulse);
            }
        }
        Destroy(gameObject);
    }
}