using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 10;
    private int currentHP;

    private InvincibleController invincibleController;
    private MoveController moveController;
    public event Action OnDamaged;
    public event Action OnDeath;
    public event Action<int, int> OnHPChanged;
    public event Action<int, int> OnMaxHPChanged;
    private AnimationController animationController;
    private InputHandler inputHandler;
    GameManager gameManager => GameManager.Instance;

    [SerializeField] private float deathFreezeTime = 1.5f; // 時間停止の演出秒数

    void Awake()
    {
        currentHP = maxHP;
        invincibleController = GetComponent<InvincibleController>();
        animationController = GetComponent<AnimationController>();
        moveController = GetComponent<MoveController>();
        inputHandler = GetComponent<InputHandler>();
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    public void TakeDamage(int damage, Transform attacker)
    {
        if (currentHP <= 0 || invincibleController.IsInvincible) return;
        currentHP -= damage;
        invincibleController.StartInvincibility();
        Vector3 directionToAttacker = (transform.position - attacker.position).normalized;
        moveController.Knockback(directionToAttacker);
        animationController.TriggerDamaged();
        OnDamaged?.Invoke();
        OnHPChanged?.Invoke(currentHP, maxHP);
        if (currentHP <= 0)
        {
            Die();
        }

    }
    void Die()
    {
        inputHandler.enabled = false;
        moveController.StopMove();
        SaveManager.Instance.ClearItemSaveAndKeepLetters();
        OnDeath?.Invoke();
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        // プレイヤーのAnimatorだけUnscaledTimeで動かす（止まった世界でも再生される）
        animationController.SetUnscaledTime(true);
        animationController.TriggerDeath();

        // 世界を止める（以降ずっと止まったまま）
        Time.timeScale = 0f;

        // 死亡アニメが見える実時間だけ待つ
        yield return new WaitForSecondsRealtime(deathFreezeTime);

        // timeScaleは戻さない。RestartGame()側でリセットする
        GameManager.Instance?.OnPlayerDeath();
    }


}