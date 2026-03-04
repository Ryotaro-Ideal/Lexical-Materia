using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public int maxHp = 5;
    protected int currentHp;
    public int collideDamage = 1;

    public float moveSpeed;
    public float chaseSpeed;


    public GameObject dropItem;
    protected Rigidbody rb;
    protected EnemyStateMachine stateMachine; // 継承先でも使うのでprotectedのまま

    public EnemyStateMachine StateMachine => stateMachine; // Stateからアクセスするためにプロパティ化

    public InvincibleController InvincibleController { get; private set; }

    public Animator animator;
    private ISenser[] sensers;
    public GameObject CurrentTarget { get; private set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHp = maxHp;
        InvincibleController = GetComponent<InvincibleController>(); // 取得漏れを防ぐ
        stateMachine = new EnemyStateMachine(this);
        sensers = GetComponents<ISenser>();

        // 初期ステート
        stateMachine.ChangeState(new MoveState(this));
    }

    protected virtual void Update()
    {
        stateMachine.Update();
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

    public virtual void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;
        currentHp -= damage;

        // ダメージステートへの遷移（ここで強制的に上書き）
        stateMachine.ChangeState(new DamagedState(this));

        if (currentHp <= 0)
        {
            GameManager.Instance?.OnEnemyKilled();
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
            damageable.TakeDamage(collideDamage);
        }
    }
    protected abstract void Die();
}