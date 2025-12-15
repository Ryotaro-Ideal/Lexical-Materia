using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InventorySlotManager : MonoBehaviour
{
    public static InventorySlotManager Instance { get; private set; }

    [Header("Inspector に Scene 上の SlotManager を割当てる")]
    public SlotManager[] toolSlots;
    public SlotManager[] consumableSlots;
    public SlotManager[] inventorySlots;

    // 内部データ
    public SlotEntry[] toolEntries;
    public SlotEntry[] consumableEntries;
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
        inventoryEntries = new SlotEntry[inventorySlots != null ? inventorySlots.Length : 0];

        InitializeEntries(toolEntries);
        InitializeEntries(consumableEntries);
        InitializeEntries(inventoryEntries);

        RefreshUI();
    }

    private void InitializeEntries(SlotEntry[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = new SlotEntry();
    }

    // ------------------ Entry 取得 ------------------

    public SlotEntry GetEntry(SlotManager s)
    {
        if (s == null) return null;

        switch (s.slotType)
        {
            case SlotManager.SlotType.Tool:
                return IsValidIndex(s.slotIndex, toolEntries) ? toolEntries[s.slotIndex] : null;

            case SlotManager.SlotType.Consumable:
                return IsValidIndex(s.slotIndex, consumableEntries) ? consumableEntries[s.slotIndex] : null;

            case SlotManager.SlotType.Inventory:
                return IsValidIndex(s.slotIndex, inventoryEntries) ? inventoryEntries[s.slotIndex] : null;

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

            case SlotManager.SlotType.Inventory:
                return true;

            default:
                return false;
        }
    }

    // ------------------ スワップ ------------------

    public bool SwapSlots(SlotManager a, SlotManager b)
    {
        if (a == null || b == null) return false;

        var ea = GetEntry(a);
        var eb = GetEntry(b);
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

        TryPlace(inventoryEntries);

        RefreshUI();
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

        TryTake(toolEntries);
        TryTake(consumableEntries);
        TryTake(inventoryEntries);

        if (need > 0) return false;

        RefreshUI();
        return true;
    }
    public bool MoveToInventoryFromSlot(SlotManager from)
    {
        if (from == null) return false;

        var entry = GetEntry(from);
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

        entry.Clear();
        RefreshUI();
        return true;
    }

    /// <summary>
    /// 指定スロットの中身を最初に空いているホットバー（Tool）へ移動
    /// </summary>
    public bool MoveToFirstHotbarSlot(SlotManager from)
    {
        if (from == null) return false;

        var entry = GetEntry(from);
        if (entry == null || entry.IsEmpty()) return false;

        if (toolEntries == null) return false;

        foreach (var target in toolEntries)
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


    // ------------------ UI ------------------

    public void RefreshUI()
    {
        UpdateSlots(toolSlots, toolEntries);
        UpdateSlots(consumableSlots, consumableEntries);
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
        (inventorySlots?.Length ?? 0);
}
