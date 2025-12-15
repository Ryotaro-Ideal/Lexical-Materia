using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// 入力ハンドラ：Player / UI のアクションマップ切替を担当します。
/// - PlayerInput があればそれを使ってマップを切り替え（推奨）
/// - なければ internal の PlayerControls の ActionMap を Enable/Disable で切り替え
/// - QuickMove（UI側の E）は OnQuickMove イベントで公開（SlotInputRouter などが購読）
/// - Interact（ゲームプレイ側の拾う等）は ConsumeInteract() で別扱い（UI と競合しない）
/// </summary>
public class InputHandler : MonoBehaviour
{
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

    // ゲームプレイのインタラクト（拾う等）用フラグ
    private bool interactTriggered = false;

    // 内部 PlayerControls（InputActionAsset から自動生成したクラス）
    private PlayerControls controls;


    // ActionMap 名（必要なら Inspector やコードから変更可能にしておく）
    private const string PLAYER_MAP = "Player";
    private const string UI_MAP = "UI";

    private void Awake()
    {



        // PlayerControls を生成して入力をバインド
        controls = new PlayerControls();

        // ---------------- Player (Gameplay) マップのバインド ----------------
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => JumpPressed = true;
        controls.Player.Jump.canceled += ctx => JumpPressed = false;

        controls.Player.Dash.performed += ctx => DashHeld = true;
        controls.Player.Dash.canceled += ctx => DashHeld = false;

        controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        controls.Player.Point.performed += ctx => PointerPosition = ctx.ReadValue<Vector2>();

        // ゲームプレイのインタラクト（例：拾う）— UI と分けるため別のフラグ
        controls.Player.Interact.performed += ctx => interactTriggered = true;

        // Player マップ側に Menu Open ボタンがある場合（押下でトグル）
        controls.Player.OpenMenu.performed += ctx =>
        {
            IsMenuOpened = !IsMenuOpened;
        };

        // ---------------- UI (Menu) マップのバインド ----------------
        // UI 側に QuickMove が定義されていれば購読（UI マップでのみ発火する想定）
        controls.UI.QuickMove.performed += ctx =>
        {
            OnQuickMove?.Invoke();
        };

        // 最初の有効化は OnEnable で行う
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


    public bool ConsumeInteract()
    {
        if (!interactTriggered) return false;
        interactTriggered = false;
        return true;
    }
}
