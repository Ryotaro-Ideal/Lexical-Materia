using UnityEngine;
using System;

/// <summary>
/// ホットバーの表示管理。インベントリのデータソースを参照して表示を更新する。
/// - ホットバーは表示オンリー（ドラッグで移動は不可）
/// - クリックで EquipController に装備要求を送る
/// </summary>
public class HotbarManager : MonoBehaviour
{
    [Header("参照")]
    public InventorySlotManager inventoryManager; // シーンの InventorySlotManager
    public SlotManager[] hotbarSlots;             // ホットバー上の SlotManager UI (表示用)
    public HotbarSlotProxy[] hotbarProxies;       // 各ホットスロットが参照する inventory index（同順）

    public EquipController equipController;       // 装備を担当するコンポーネント（プレイヤー）

    private void Awake()
    {
        if (inventoryManager == null) inventoryManager = InventorySlotManager.Instance;
        if (equipController == null) equipController = FindFirstObjectByType<EquipController>();

        // ホットバースロットとプロキシの数合わせチェック
        if (hotbarSlots.Length != hotbarProxies.Length)
        {
            Debug.LogWarning("HotbarManager: hotbarSlots と hotbarProxies の要素数が一致していません。");
        }

        // UIがクリックされたときのコールバックを登録
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            int idx = i;
            var slot = hotbarSlots[idx];
            // Button コンポーネント経由でクリックイベントを登録してあるならここは不要だが、
            // シンプルに SlotManager の OnSlotClicked() を拡張するかボタンを参照しても良い。
            // ここでは SlotManager の OnSlotClicked がログ出すだけなので、代替として Button を使う場合は変更してください.
            var btn = slot.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnHotbarClicked(idx));
            }
        }
    }

    private void OnEnable()
    {
        // Inventory 変更を購読（InventorySlotManager にイベントがある前提）
        inventoryManager.OnInventoryChanged += RefreshAll;
        RefreshAll();
    }

    private void OnDisable()
    {
        if (inventoryManager != null) inventoryManager.OnInventoryChanged -= RefreshAll;
    }

    // 全体更新
    public void RefreshAll()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            UpdateSlot(i);
        }
    }

    // 個別スロット更新（hotbarSlots[i] が hotbarProxies[i] の参照を表示）
    public void UpdateSlot(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSlots.Length) return;
        var proxy = hotbarProxies.Length > hotbarIndex ? hotbarProxies[hotbarIndex] : null;
        var uiSlot = hotbarSlots[hotbarIndex];
        if (proxy == null || uiSlot == null)
        {
            // 空にする
            uiSlot.ClearSlot();
            return;
        }

        var entry = inventoryManager.GetEntry(uiSlot);
        if (entry == null || entry.IsEmpty())
        {
            uiSlot.ClearSlot();
        }
        else
        {
            uiSlot.SetItem(entry.item, entry.count);
            // ホットバー側はドラッグ禁止にしておく（SlotManager 内のドラッグ処理を無効化するか、Button.interactable = trueでOK）
            var btn = uiSlot.GetComponent<UnityEngine.UI.Button>();
            if (btn != null) btn.interactable = true; // クリックは有効、ドラッグは UI 側で操作しない
        }
    }

    // ホットバーの UI がクリックされた
    private void OnHotbarClicked(int hotbarIndex)
    {
        Debug.Log($"Hotbar slot {hotbarIndex} clicked.");
        var proxy = hotbarProxies.Length > hotbarIndex ? hotbarProxies[hotbarIndex] : null;
        if (proxy == null) return;

        SlotManager s = hotbarSlots[hotbarIndex];
        var entry = inventoryManager.GetEntry(s);
        if (entry == null || entry.IsEmpty()) return;

        // 装備要求（EquipController が対応）
        equipController?.EquipFromInventorySlot(s);
    }

    // 外部から特定 Inventory スロットが変化したときに呼ぶ補助
    public void NotifyInventorySlotChanged(int slotIndex)
    {
        // hotbarProxies を走査して該当する hotbar インデックスを更新
        for (int i = 0; i < hotbarProxies.Length; i++)
        {
            if (hotbarProxies[i] != null && hotbarProxies[i].GetInventoryIndex() == slotIndex)
            {
                UpdateSlot(i);
            }
        }
    }
}
