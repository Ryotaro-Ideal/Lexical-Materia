using UnityEngine;
using UnityEngine.InputSystem;
using System;


public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    // 公開プロパティ（外部参照用）
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashHeld { get; private set; }
    public bool IsMenuOpened { get; private set; }
    public Vector2 LookInput { get; private set; }
    public Vector2 PointerPosition { get; private set; }

    // QuickMove (UI側の E 等) をイベントで公開（UIマップの QuickMove が発火したときに呼ばれる）
    public event Action OnQuickMove;

    // メニュー開閉イベント（外部で UI 表示等を連携したいときに使う）
    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    // ギミックUI用の状態と、閉じるリクエストのイベント
    public bool IsGimmickUIOpened { get; set; }
    public event Action OnGimmickUICloseRequested;

    // ゲームプレイのインタラクト（拾う等）用フラグ
    private bool interactTriggered = false;

    // 内部 PlayerControls（InputActionAsset から自動生成したクラス）
    private PlayerControls controls;

    public AttackController attackController;

    // ActionMap 名（必要なら Inspector やコードから変更可能にしておく）
    private const string PLAYER_MAP = "Player";
    private const string UI_MAP = "UI";

    private void Awake()
    {
        Instance = this;
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => JumpPressed = true;
        controls.Player.Jump.canceled += ctx => JumpPressed = false;

        controls.Player.Dash.performed += ctx => DashHeld = true;
        controls.Player.Dash.canceled += ctx => DashHeld = false;

        controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        controls.Player.Point.performed += ctx => PointerPosition = ctx.ReadValue<Vector2>();

        controls.Player.Interact.performed += ctx => OnInteractPressed();
        controls.Player.Attack.performed += OnAttack;
        controls.Player.Attack.canceled += ctx => { };


        // Player マップ側に Menu Open ボタンがある場合（押下でトグル）
        controls.Player.OpenMenu.performed += ctx =>
        {
            // 文字操作UIなどのギミックが開いている場合は、メニューは開かずにギミックを閉じる
            if (IsGimmickUIOpened)
            {
                OnGimmickUICloseRequested?.Invoke();
                return;
            }
            IsMenuOpened = !IsMenuOpened;
        };

        controls.UI.QuickMove.performed += ctx =>
        {
            OnQuickMove?.Invoke();
        };

        // 最初の有効化は OnEnable で行う
    }
    public void OnAttack(InputAction.CallbackContext context)
    {

        if (attackController != null)
        {
            attackController.Attack();
        }
        else
        {
            Debug.Log("AttackController is not found on this GameObject.");
        }

    }

    private void OnEnable()
    {
        controls.Enable();
        // 初期状態は Gameplay を有効、UI を無効にしておく（安全側）
        controls.Player.Enable();
        controls.UI.Disable();
    }

    private void OnDisable()
    {
        // 解除
        controls.Disable();
    }


    // インタラクトボタン（既存のアイテム取得キー）が押されたら
    public void OnInteractPressed()
    {
        PickupItemManager pickupItemManager = FindFirstObjectByType<PickupItemManager>();
        if (pickupItemManager == null) return;

        IInteractable target = pickupItemManager.GetTargetItem();

        if (target != null)
        {
            target.Interact();
            pickupItemManager.ClearHover();
        }
    }


}
