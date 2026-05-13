using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InventorySlotManager : MonoBehaviour
{
    public static InventorySlotManager Instance { get; private set; }

    [Header("Inspector に Scene 上の SlotManager を割当てる")]
    public SlotManager[] toolSlots;
    public SlotManager[] consumableSlots;
    public SlotManager[] freeSlots;
    public SlotManager[] inventorySlots;

    [Header("セーブ復元用：ItemDatabaseアセットを登録する")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("開始時に所持させるアイテム")]
    public ItemData[] startItems;

    public SlotEntry[] toolEntries;
    public SlotEntry[] consumableEntries;
    public SlotEntry[] freeEntries;
    public SlotEntry[] inventoryEntries;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        toolEntries = new SlotEntry[toolSlots != null ? toolSlots.Length : 0];
        consumableEntries = new SlotEntry[consumableSlots != null ? consumableSlots.Length : 0];
        freeEntries = new SlotEntry[freeSlots != null ? freeSlots.Length : 0];
        inventoryEntries = new SlotEntry[inventorySlots != null ? inventorySlots.Length : 0];

        InitializeEntries(toolEntries);
        InitializeEntries(consumableEntries);
        InitializeEntries(freeEntries);
        InitializeEntries(inventoryEntries);

        // スロットUI側の設定を強制的に上書き
        SetupSlots(toolSlots, SlotManager.SlotType.Tool);
        SetupSlots(consumableSlots, SlotManager.SlotType.Consumable);
        SetupSlots(freeSlots, SlotManager.SlotType.Free);
        SetupSlots(inventorySlots, SlotManager.SlotType.Inventory);

        // 開始アイテムの配布
        if (startItems != null)
        {
            foreach (var item in startItems)
            {
                if (item != null) AutoPlaceItem(item, 1);
            }
        }

        RefreshUI();
    }

    private void SetupSlots(SlotManager[] slots, SlotManager.SlotType type)
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            slots[i].slotType = type;
            slots[i].SetSlotIndex(i);
        }
    }

    private void InitializeEntries(SlotEntry[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = new SlotEntry();
    }

    // ------------------ Entry 取得 ------------------

    public SlotEntry GetEntry(SlotManager s, int idx)
    {
        if (s == null) return null;

        switch (s.slotType)
        {
            case SlotManager.SlotType.Tool:
                return IsValidIndex(idx, toolEntries) ? toolEntries[idx] : null;

            case SlotManager.SlotType.Consumable:
                return IsValidIndex(idx, consumableEntries) ? consumableEntries[idx] : null;

            case SlotManager.SlotType.Free:
                return IsValidIndex(idx, freeEntries) ? freeEntries[idx] : null;

            case SlotManager.SlotType.Inventory:
                return IsValidIndex(idx, inventoryEntries) ? inventoryEntries[idx] : null;

            default:
                return null;
        }
    }

    private bool IsValidIndex(int idx, SlotEntry[] arr)
        => arr != null && idx >= 0 && idx < arr.Length;

    // ------------------ 受け入れ判定 ------------------

    public bool CanAcceptItemAtSlot(SlotManager s, ItemData item)
    {
        if (s == null) return false;
        if (item == null) return true;

        switch (s.slotType)
        {
            case SlotManager.SlotType.Tool:
                return item.itemType == ItemType.Tool;

            case SlotManager.SlotType.Consumable:
                return item.itemType == ItemType.Consumable;

            case SlotManager.SlotType.Free:
                return true;

            case SlotManager.SlotType.Inventory:
                return true;

            default:
                return false;
        }
    }

    // ------------------ スワップ ------------------

    public bool SwapSlots(SlotManager a, SlotManager b, int idxA, int idxB)
    {
        if (a == null || b == null) return false;

        var ea = GetEntry(a, idxA);
        var eb = GetEntry(b, idxB);
        if (ea == null || eb == null) return false;

        if (!CanAcceptItemAtSlot(a, eb.item) || !CanAcceptItemAtSlot(b, ea.item))
            return false;

        SwapEntries(ea, eb);
        RefreshUI();
        return true;
    }

    private void SwapEntries(SlotEntry a, SlotEntry b)
    {
        (a.item, b.item) = (b.item, a.item);
        (a.count, b.count) = (b.count, a.count);
    }

    // ------------------ 自動配置 ------------------

    public bool AutoPlaceItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return false;


        int remaining = count;
        int maxStack = Mathf.Max(1, item.maxStack);

        void TryMerge(SlotEntry[] arr)
        {
            foreach (var s in arr)
            {
                if (remaining <= 0) break;
                if (s.item == item && s.count < maxStack)
                {
                    int add = Mathf.Min(maxStack - s.count, remaining);
                    s.count += add;
                    remaining -= add;
                }
            }
        }

        TryMerge(toolEntries);
        TryMerge(consumableEntries);
        TryMerge(freeEntries);
        TryMerge(inventoryEntries);

        void TryPlace(SlotEntry[] arr)
        {
            foreach (var s in arr)
            {
                if (remaining <= 0) break;
                if (s.IsEmpty())
                {
                    int put = Mathf.Min(maxStack, remaining);
                    s.item = item;
                    s.count = put;
                    remaining -= put;
                }
            }
        }

        if (item.itemType == ItemType.Tool) TryPlace(toolEntries);
        if (item.itemType == ItemType.Consumable) TryPlace(consumableEntries);

        TryPlace(freeEntries);
        TryPlace(inventoryEntries);
        RefreshUI();
        if (remaining <= 0)
        {
            SoundManager.Instance?.PlaySE(SoundName.ItemPickUp);
        }


        return remaining == 0;
    }

    // ------------------ 削除 ------------------

    public bool RemoveItem(ItemData item, int count)
    {
        if (item == null || count <= 0) return false;

        int need = count;

        void TryTake(SlotEntry[] arr)
        {
            foreach (var s in arr)
            {
                if (need <= 0) break;
                if (s.item == item)
                {
                    int take = Mathf.Min(s.count, need);
                    s.count -= take;
                    need -= take;

                    if (s.count <= 0)
                        s.Clear();
                }
            }
        }
        TryTake(freeEntries);
        TryTake(toolEntries);
        TryTake(consumableEntries);
        TryTake(inventoryEntries);

        if (need > 0) return false;

        RefreshUI();
        return true;
    }
    public bool MoveToInventoryFromSlot(SlotManager from, int idx)
    {
        if (from == null) return false;

        var entry = GetEntry(from, idx);
        if (entry == null || entry.IsEmpty()) return false;

        foreach (var target in inventoryEntries)
        {
            if (target.IsEmpty())
            {
                target.item = entry.item;
                target.count = entry.count;
                entry.Clear();
                RefreshUI();
                return true;
            }
        }

        return false;
    }

    public bool MoveToHotbar(SlotManager from, int idx)
    {
        if (from == null) return false;

        var entry = GetEntry(from, idx);
        if (entry == null || entry.IsEmpty()) return false;

        ItemType type = entry.item.itemType;

        if (type == ItemType.Tool && TryMoveEntryTo(entry, toolEntries)) return true;
        if (type == ItemType.Consumable && TryMoveEntryTo(entry, consumableEntries)) return true;

        return TryMoveEntryTo(entry, freeEntries);
    }

    private bool TryMoveEntryTo(SlotEntry entry, SlotEntry[] targets)
    {
        foreach (var t in targets)
        {
            if (!t.IsEmpty()) continue;
            t.item = entry.item;
            t.count = entry.count;
            entry.Clear();
            RefreshUI();
            return true;
        }
        return false;
    }


    // ------------------ UI ------------------

    public void RefreshUI()
    {
        UpdateSlots(toolSlots, toolEntries);
        UpdateSlots(consumableSlots, consumableEntries);
        UpdateSlots(freeSlots, freeEntries);
        UpdateSlots(inventorySlots, inventoryEntries);

        OnInventoryChanged?.Invoke();
    }

    private void UpdateSlots(SlotManager[] slots, SlotEntry[] entries)
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (IsValidIndex(i, entries))
                slots[i].SetItem(entries[i].item, entries[i].count);
            else
                slots[i].ClearSlot();
        }
    }

    public int InstanceSlotsCount =>
        (toolSlots?.Length ?? 0) +
        (consumableSlots?.Length ?? 0) +
        (freeSlots?.Length ?? 0) +
        (inventorySlots?.Length ?? 0);

    // ===== セーブ・ロード =====

    /// <summary>アイテムインベントリの現在状態をSaveDataに書き込む</summary>
    public void WriteSaveData(SaveData data)
    {
        data.toolSlots = EntriesToSave(toolEntries);
        data.consumableSlots = EntriesToSave(consumableEntries);
        data.freeSlots = EntriesToSave(freeEntries);
        data.inventorySlots = EntriesToSave(inventoryEntries);
    }

    private System.Collections.Generic.List<ItemSaveEntry> EntriesToSave(SlotEntry[] entries)
    {
        var list = new System.Collections.Generic.List<ItemSaveEntry>();
        foreach (var e in entries)
        {
            list.Add(e.IsEmpty()
                ? new ItemSaveEntry("", 0)
                : new ItemSaveEntry(e.item.ID, e.count));
        }
        return list;
    }

    /// <summary>SaveDataからアイテムインベントリを復元する</summary>
    public void ReadSaveData(SaveData data)
    {
        if (data == null) return;

        RestoreEntries(toolEntries, data.toolSlots);
        RestoreEntries(consumableEntries, data.consumableSlots);
        RestoreEntries(freeEntries, data.freeSlots);
        RestoreEntries(inventoryEntries, data.inventorySlots);

        RefreshUI();
    }

    private void RestoreEntries(SlotEntry[] entries,
        System.Collections.Generic.List<ItemSaveEntry> saved)
    {
        if (saved == null) return;
        for (int i = 0; i < entries.Length && i < saved.Count; i++)
        {
            var s = saved[i];
            if (s.IsEmpty()) { entries[i].Clear(); continue; }

            ItemData found = FindItemById(s.itemId);
            if (found != null)
            {
                entries[i].item = found;
                entries[i].count = s.count;
            }
            else
            {
                entries[i].Clear();
                Debug.LogWarning($"[InventorySlotManager] ID '{s.itemId}' のItemDataが見つかりません。");
            }
        }
    }

    private ItemData FindItemById(string id)
    {
        if (itemDatabase == null)
        {
            Debug.LogWarning("[InventorySlotManager] ItemDatabaseが未設定です。Inspectorで登録してください。");
            return null;
        }
        return itemDatabase.FindById(id);
    }

    public void ResetSaveData()
    {
        foreach (var e in toolEntries) e.Clear();
        foreach (var e in consumableEntries) e.Clear();
        foreach (var e in freeEntries) e.Clear();
        foreach (var e in inventoryEntries) e.Clear();
        RefreshUI();
    }
}
