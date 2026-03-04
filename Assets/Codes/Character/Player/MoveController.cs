using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float dashSpeed = 10f;

    private float gravity = -15f;
    private float rotationSpeed = 20f;

    private CharacterController cc;
    private InputHandler input;

    private Vector3 velocity;
    private bool isGrounded;

    private Transform cam;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<InputHandler>();
        cam = Camera.main.transform;
    }

    private void Update()
    {
        // メニューが開いている場合は移動処理をしない（UI操作中はプレイヤーを停止させる）
        if (input != null && input.IsMenuOpened)
        {
            // ジャンプや移動入力は無視し、速度を滑らかにゼロにする（急停止を避ける場合はさらに工夫）
            velocity.x = 0f;
            velocity.z = 0f;
            return;
        }

        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.MoveInput.y + right * input.MoveInput.x;

        float speed = input.DashHeld ? dashSpeed : moveSpeed;

        // Move
        cc.Move(moveDir * speed * Time.deltaTime);

        // Look (移動方向に向ける)
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ジャンプ
        if (input.JumpPressed && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 重力
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
