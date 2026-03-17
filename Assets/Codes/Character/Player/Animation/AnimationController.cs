using UnityEngine;

/// <summary>
/// Animatorへのアクセスをここに集約する。
/// 各コントローラーはこのクラスを通じてアニメーションを指示する。
/// </summary>
public class AnimationController : MonoBehaviour
{
    private Animator animator;

    // Animatorパラメータ名を定数化（タイポ防止）
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsDashHash = Animator.StringToHash("IsDash");
    private static readonly int IsGroundHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int PickupHash = Animator.StringToHash("Pickup");
    private static readonly int AttackHash = Animator.StringToHash("Attack_Punch");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("AnimationController: Animator が見つかりません。FBXオブジェクトにAnimatorがついているか確認してください。");
    }

    // --- MoveControllerから毎フレーム呼ぶ ---
    public void SetMovement(float speed, bool isDash, bool isGrounded)
    {
        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(IsDashHash, isDash);
        animator.SetBool(IsGroundHash, isGrounded);
    }

    // --- ジャンプ（トリガー：瞬間的なイベント）---
    public void TriggerJump()
    {
        animator.SetTrigger(JumpHash);
    }

    // --- 攻撃（上半身レイヤー用）---
    public void TriggerAttack()
    {
        animator.SetTrigger(AttackHash);
    }

    // --- アイテムを拾う ---
    public void TriggerPickup()
    {
        animator.SetTrigger(PickupHash);
    }
}
