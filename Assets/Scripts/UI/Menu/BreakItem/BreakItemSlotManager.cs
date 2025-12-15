using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakItemSlotManager : SlotCollectionBase
{
    public static BreakItemSlotManager Instance { get; private set; }
    List<SlotEntry> tempEntries = new List<SlotEntry>();


    protected override void Awake()
    {

        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        base.Awake(); // SlotCollectionBase 側の初期化

    }

    /// <summary>
    /// Inventory から分解スロットへ移動
    /// </summary>
    public bool MoveToBreakItemSlotFromInventory(SlotManager inventorySlot)
    {
        var inv = InventorySlotManager.Instance;
        if (inv == null || inventorySlot == null) { Debug.Log("InventorySlotManagerNotFind"); return false; }

        var src = inv.GetEntry(inventorySlot);
        if (src == null || src.IsEmpty()) { Debug.Log("SlotEntryNotFind"); return false; }

        var dst = FindFirstEmptyEntry();
        if (dst == null) { Debug.Log("FirstEmptyEntryNotFind"); return false; }

        dst.MoveFrom(src);

        RefreshUI();
        inv.RefreshUI();
        return true;
    }

    public bool MoveToInventoryFromBreakItemSlot(int breakSlotIndex)
    {
        var inv = InventorySlotManager.Instance;
        if (inv == null) return false;

        var src = GetEntry(breakSlotIndex);
        if (src == null || src.IsEmpty()) return false;

        if (!inv.AutoPlaceItem(src.item, src.count))
            return false;

        src.Clear();
        RefreshUI();
        inv.RefreshUI();
        return true;
    }

    public bool ConvertItems()
    {
        LetterInvManager letterInvManager = LetterInvManager.Instance;
        if (letterInvManager == null) { Debug.Log("letterInvManager Not Found"); return false; }
        if (!ClearAll()) return false;
        letterInvManager.SetLetters(tempEntries);
        tempEntries.Clear();
        return true;

    }
    public bool ClearAll()
    {
        if (IsAllEmpty()) return false;

        foreach (var e in entries)
            if (!e.IsEmpty())
            {

                tempEntries.Add(new SlotEntry());
                tempEntries[tempEntries.Count - 1].MoveFrom(e);
                e.Clear();
            }


        RefreshUI();
        return true;
    }

}
