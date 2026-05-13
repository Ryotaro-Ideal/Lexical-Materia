using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    [Header("参照")]
    public InventorySlotManager inventoryManager;

    [Header("外部ホットバー表示スロット（メニュー外のUI）")]
    public SlotManager[] toolHotbarSlots;
    public SlotManager[] consumableHotbarSlots;
    public SlotManager[] freeHotbarSlots;

    private void Awake()
    {
        if (inventoryManager == null) inventoryManager = InventorySlotManager.Instance;
        
        // スロット側の設定を強制上書き（index ずれ防止）
        SetupHotbarSlots(toolHotbarSlots, SlotManager.SlotType.Tool);
        SetupHotbarSlots(consumableHotbarSlots, SlotManager.SlotType.Consumable);
        SetupHotbarSlots(freeHotbarSlots, SlotManager.SlotType.Free);

        RegisterClickEvents(toolHotbarSlots, inventoryManager.toolEntries);
        RegisterClickEvents(consumableHotbarSlots, inventoryManager.consumableEntries);
        RegisterClickEvents(freeHotbarSlots, inventoryManager.freeEntries);
    }

    private void OnEnable()
    {
        if (inventoryManager == null) inventoryManager = InventorySlotManager.Instance;
        if (inventoryManager != null) inventoryManager.OnInventoryChanged += RefreshAll;
        RefreshAll();
    }

    private void OnDisable()
    {
        if (inventoryManager != null) inventoryManager.OnInventoryChanged -= RefreshAll;
    }

    private void SetupHotbarSlots(SlotManager[] slots, SlotManager.SlotType type)
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].slotType = type;
                slots[i].SetSlotIndex(i);
            }
        }
    }

    private void RegisterClickEvents(SlotManager[] slots, SlotEntry[] entries)
    {
        if (slots == null || entries == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            var btn = slots[i].GetComponent<UnityEngine.UI.Button>();
            if (btn == null) continue;
            
            int capturedIndex = i;
            SlotEntry[] capturedEntries = entries;
            btn.onClick.AddListener(() => OnHotbarSlotClicked(capturedEntries, capturedIndex));
        }
    }

    public void RefreshAll()
    {
        if (inventoryManager == null) return;
        RefreshSlots(toolHotbarSlots, inventoryManager.toolEntries);
        RefreshSlots(consumableHotbarSlots, inventoryManager.consumableEntries);
        RefreshSlots(freeHotbarSlots, inventoryManager.freeEntries);
    }

    private void RefreshSlots(SlotManager[] slots, SlotEntry[] entries)
    {
        if (slots == null || entries == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (i < entries.Length && !entries[i].IsEmpty())
                slots[i].SetItem(entries[i].item, entries[i].count);
            else
                slots[i].ClearSlot();
        }
    }

    private void OnHotbarSlotClicked(SlotEntry[] entries, int index)
    {
        if (entries == null || index < 0 || index >= entries.Length) return;
        var entry = entries[index];
        if (entry == null || entry.IsEmpty()) return;
        EquipController.Instance?.EquipItem(entry.item, entry.count);
    }
}
