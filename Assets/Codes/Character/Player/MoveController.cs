using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float dashSpeed = 10f;

    [SerializeField] private float gravity = -15f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.5f;
    private CharacterController cc;
    private InputHandler input;

    private AnimationController animController;
    private PlayerHealth playerHealth;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isKnockingBack;
    private float knockbackTimer = 0f;
    private Transform cam;

    private void Awake()
    {

        cc = GetComponent<CharacterController>();
        input = GetComponent<InputHandler>();
        animController = GetComponent<AnimationController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerHealth.OnDeath += StopMove;
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (!cc.enabled) return;
        if (isKnockingBack)
        {
            knockbackTimer += Time.deltaTime;

            // ノックバック方向に移動しながら徐々に減速
            cc.Move(velocity * Time.deltaTime);
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime / knockbackDuration);

            if (knockbackTimer >= knockbackDuration)
            {
                isKnockingBack = false;
                knockbackTimer = 0f;
                velocity = Vector3.zero; // 通常移動に影響しないようリセット
            }
            return;
        }
        // メニューが開いている場合は移動処理をしない（UI操作中はプレイヤーを停止させる）
        if (input != null && (input.IsMenuOpened || input.IsGimmickUIOpened))
        {
            // ジャンプや移動入力は無視し、速度を滑らかにゼロにする（急停止を避ける場合はさらに工夫）
            velocity.x = 0f;
            velocity.z = 0f;
            return;
        }

        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 forward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
        Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
        Vector3 moveDir = forward * input.MoveInput.y + right * input.MoveInput.x;

        float speed = input.DashHeld ? dashSpeed : moveSpeed;

        // Move
        cc.Move(moveDir * speed * Time.deltaTime);
        animController.SetMovement(moveDir.magnitude, input.DashHeld, isGrounded);

        // Look（カメラの水平方向に体を向ける）
        Vector3 camForwardFlat = new Vector3(cam.forward.x, 0f, cam.forward.z);
        if (camForwardFlat.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForwardFlat);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (input.JumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            animController.TriggerJump();
        }
        velocity.y += -gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
    public void Knockback(Vector3 dir)
    {
        isKnockingBack = true;
        knockbackTimer = 0f;
        velocity = dir * knockbackForce;
    }
    public void StopMove()
    {
        isKnockingBack = false;
        knockbackTimer = 0f;
        velocity = Vector3.zero;
        cc.enabled = false;
    }
}
