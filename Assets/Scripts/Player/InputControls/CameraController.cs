using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;

    private Vector2 moveInput;
    private Transform cameraTR;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTR = Camera.main.transform;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        // カメラ基準の方向を計算
        Vector3 forward = cameraTR.forward;
        Vector3 right = cameraTR.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;

        if (direction.magnitude >= 0.1f)
        {
            // プレイヤーの向きを移動方向に合わせる（Y軸回転）
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // 移動
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }
    }
}
