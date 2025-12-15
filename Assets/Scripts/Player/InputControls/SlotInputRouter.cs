using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerInput 経由で UI マップ内の QuickMove アクションを購読し、
/// ホバー中のスロットをホットバー⇄インベントリへクイック移動する。
/// ・PlayerInput が切り替えたマップに応じて QuickMove の performed が発火する想定。
/// ・ホバー管理は SlotHoverState.HoveredSlotIndex を利用（既存実装と連携）。
/// </summary>
public class SlotInputRouter : MonoBehaviour
{
    [Header("PlayerInput (Inspector に割当)")]
    public PlayerInput playerInput;

    [Header("QuickMove アクション名（UI マップ内のアクション名）")]
    public string quickMoveActionName = "QuickMove"; // UIマップに QuickMove がある想定

    // デバウンス（短時間の多重トリガ防止）
    [Header("デバウンス(秒)")]
    [Range(0f, 0.5f)]
    public float debounceSeconds = 0.05f;
    private double lastTriggerTime = -1.0;

    private InputAction quickMoveAction;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogWarning("SlotInputRouter_PlayerInput: PlayerInput が見つかりません。Inspectorで割り当ててください。");
                return;
            }
        }

        // PlayerInput の ActionAsset からアクションを探す（UI マップが切替で有効なときに fired される）
        if (playerInput.actions != null)
        {
            // true を渡すことで全マップから検索（衝突するアクション名がなければ安全）
            quickMoveAction = playerInput.actions.FindAction(quickMoveActionName, true);
            if (quickMoveAction == null)
            {
                Debug.LogWarning($"SlotInputRouter_PlayerInput: アクション '{quickMoveActionName}' が見つかりません。InputActionAsset を確認してください。");
            }
        }
    }

    private void OnEnable()
    {
        if (quickMoveAction != null)
        {
            quickMoveAction.performed += OnQuickMovePerformed;
            // quickMoveAction は PlayerInput が管理するため Enable/Disable は任せる（マップ切替で自動的に発火制御される）
        }
    }

    private void OnDisable()
    {
        if (quickMoveAction != null)
        {
            quickMoveAction.performed -= OnQuickMovePerformed;
        }
    }

    private void OnQuickMovePerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("SlotInputRouter_PlayerInput: QuickMove アクションが発火しました。");
        // デバウンス
        if (Time.realtimeSinceStartup - lastTriggerTime < debounceSeconds) return;

        // ホバー中でなければ無視
        SlotManager hovered = SlotHoverState.slotManager;

        if (hovered == null)
        {
            Debug.Log("SlotInputRouter_PlayerInput: ホバー中のスロットがありません。");
            return;

        }

        lastTriggerTime = Time.realtimeSinceStartup;
        HandleQuickMove(hovered);
    }

    // 実際の移動処理（InventorySlotManager の API を利用）
    private void HandleQuickMove(SlotManager hovered)
    {


        if (MenuButtonManager.isBreakItemMenuOpen && hovered.slotType != SlotManager.SlotType.BreakItem) // 分解メニューが開いていて, インベントリ領域にいるなら
        {
            var breakItemManager = BreakItemSlotManager.Instance;

            bool ok = breakItemManager.MoveToBreakItemSlotFromInventory(hovered);
            if (!ok) Debug.Log("分解スロットへの移動に失敗しました（空きがない等）。");
            return;
        }
        else if (MenuButtonManager.isBreakItemMenuOpen && hovered.slotType == SlotManager.SlotType.BreakItem)
        {//分解メニューを開いていて、分解スロットにいるなら
            var breakItemManager = BreakItemSlotManager.Instance;
            Debug.Log("分解メニューが開いているので通常インベントリへ戻します");
            bool ok = breakItemManager.MoveToInventoryFromBreakItemSlot(hovered.slotIndex);
            if (!ok) Debug.Log("インベントリへの移動に失敗しました（空きがない等）。");
            return;
        }

        // ホットバー領域にいるなら通常インベントリへ戻す
        if (hovered.slotType == SlotManager.SlotType.Tool || hovered.slotType == SlotManager.SlotType.Consumable)
        {
            var invManager = InventorySlotManager.Instance;
            bool ok = invManager.MoveToInventoryFromSlot(hovered);
            if (!ok) Debug.Log("インベントリへの移動に失敗しました（空きがない等）。");
            return;
        }
        else
        {
            // 通常インベントリにいるならホットバーへ移す
            var invManager = InventorySlotManager.Instance;
            bool moved = invManager.MoveToFirstHotbarSlot(hovered);
            if (!moved) Debug.Log("ホットバーへの移動に失敗しました（空きがない、区分不一致等）。");
        }


    }
}
