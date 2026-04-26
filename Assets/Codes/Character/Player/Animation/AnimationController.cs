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
    private static readonly int DamagedHash = Animator.StringToHash("Damaged");
    private static readonly int DeathHash = Animator.StringToHash("Death");

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

    public void TriggerDamaged()
    {
        animator.SetTrigger(DamagedHash);
    }

    public void TriggerDeath()
    {
        animator.SetTrigger(DeathHash);
    }

    /// <summary>
    /// Time.timeScale=0中もアニメを再生し続けるか切り替える。
    /// 死亡演出でtimeScaleを止めながらアニメだけ動かしたい時に使う。
    /// </summary>
    public void SetUnscaledTime(bool useUnscaled)
    {
        animator.updateMode = useUnscaled
            ? AnimatorUpdateMode.UnscaledTime
            : AnimatorUpdateMode.Normal;
    }
}
