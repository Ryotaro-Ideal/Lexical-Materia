using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 10;
    private int currentHP;

    private InvincibleController invincibleController;

    public event Action<int, int> OnHPChanged;
    public event Action<int, int> OnMaxHPChanged;

    void Awake()
    {
        currentHP = maxHP;
        invincibleController = GetComponent<InvincibleController>();
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0 || invincibleController.IsInvincible) return;
        currentHP -= damage;
        invincibleController.StartInvincibility();
        OnHPChanged?.Invoke(currentHP, maxHP);
        if (currentHP <= 0)
        {
            Die();
        }

    }
    void Die()
    {
        Debug.Log("PlayerHealth: Die");
        GameManager.Instance?.OnPlayerDeath();
    }


}