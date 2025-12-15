using System;
using UnityEngine;

public abstract class SlotCollectionBase : MonoBehaviour
{
    [Header("UI Slots")]
    [SerializeField] protected SlotManager[] slotUIs;

    protected SlotEntry[] entries;

    public event Action OnChanged;

    protected virtual void Awake()
    {
        Initialize(slotUIs);
    }

    protected void Initialize(SlotManager[] slots)
    {
        if (slots == null) return;

        entries = new SlotEntry[slots.Length];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = new SlotEntry();
    }

    public SlotEntry GetEntry(int index)
    {
        if (!IsValidIndex(index)) return null;
        return entries[index];
    }

    protected bool IsValidIndex(int index)
        => entries != null && index >= 0 && index < entries.Length;

    public bool IsEmpty(int index)
        => !IsValidIndex(index) || entries[index].IsEmpty();
    protected bool IsAllEmpty()
    {
        foreach (var e in entries)
            if (!e.IsEmpty()) return false;
        return true;
    }
    protected void RefreshUI()
    {
        if (slotUIs == null) return;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (!IsValidIndex(i)) continue;
            slotUIs[i].SetItem(entries[i].item, entries[i].count);
        }

        OnChanged?.Invoke();
    }

    protected bool MoveAll(SlotEntry from, SlotEntry to)
    {
        if (from == null || to == null) return false;
        if (!to.IsEmpty()) return false;

        to.item = from.item;
        to.count = from.count;

        from.item = null;
        from.count = 0;

        return true;
    }
    protected SlotEntry FindFirstEmptyEntry()
    {
        foreach (var e in entries)
            if (e.IsEmpty()) return e;
        return null;
    }
}