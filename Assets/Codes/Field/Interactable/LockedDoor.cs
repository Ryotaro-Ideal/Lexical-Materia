using UnityEngine;
using UnityEngine.Events;


public class LockedDoor : Door
{
    [Header("鍵設定")]
    [SerializeField] private ItemData requiredKey;
    private string lockedDisplayName = "";

    [Header("失敗時")]
    public UnityEvent OnFail;

    public override void Awake()
    {
        base.Awake();
        lockedDisplayName = requiredKey != null ? requiredKey.itemName + "が必要" : "";
    }

    public override string GetName()
    {
        if (isUnlocked || requiredKey == null) return base.GetName();

        bool hasKey = InventorySlotManager.Instance != null && HasKeyInInventory();
        return hasKey ? base.GetName() : lockedDisplayName;
    }

    public override void Interact()
    {
        if (isUnlocked || requiredKey == null)
        {
            base.Interact();
            return;
        }

        var inv = InventorySlotManager.Instance;
        if (inv == null) return;

        if (inv.RemoveItem(requiredKey, 1))
        {
            Debug.Log($"[LockedDoor] '{requiredKey.itemName}' を消費して解錠しました。");
            isUnlocked = true;
            base.Interact();
        }
        else
        {
            Debug.Log($"[LockedDoor] '{requiredKey.itemName}' が必要です。");
            OnFail?.Invoke();
        }
    }

    // インベントリ全体（Tool / Consumable / Inventory）に鍵が存在するか確認
    private bool HasKeyInInventory()
    {
        var inv = InventorySlotManager.Instance;

        if (ContainsKey(inv.toolEntries)) return true;
        if (ContainsKey(inv.consumableEntries)) return true;
        if (ContainsKey(inv.inventoryEntries)) return true;

        return false;
    }

    private bool ContainsKey(SlotEntry[] entries)
    {
        if (entries == null) return false;
        foreach (var e in entries)
        {
            if (!e.IsEmpty() && e.item == requiredKey) return true;
        }
        return false;
    }
}
