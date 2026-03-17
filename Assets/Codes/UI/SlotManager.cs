using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SlotManager : SlotBase, IPointerEnterHandler, IPointerExitHandler
{
    public enum SlotType
    {
        Tool,
        Consumable,
        Inventory,
        BreakItem
    }

    public SlotType slotType;

    InventorySlotManager inv => InventorySlotManager.Instance;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetItem(ItemData item, int count)
    {
        if (item == null || count <= 0) { ClearSlot(); return; }
        icon.enabled = true;
        countText.enabled = true;
        icon.sprite = item.icon;
        countText.text = count > 1 ? count.ToString() : "";
        icon.raycastTarget = false;
        countText.raycastTarget = false;
    }
    public void ClearSlot()
    {
        icon.enabled = false;
        countText.enabled = false;
        icon.sprite = UIMaskSprite;
        countText.text = "";
    }
    protected override bool HasItem()
    {
        return slotType == SlotType.BreakItem
            ? !BreakItemSlotManager.Instance.GetEntry(slotIndex).IsEmpty()
            : !inv.GetEntry(this, slotIndex).IsEmpty();
    }

    protected override string GetDisplayName()
    {
        return slotType == SlotType.BreakItem
            ? BreakItemSlotManager.Instance.GetEntry(slotIndex).item.itemName
            : inv.GetEntry(this, slotIndex).item.itemName;
    }

    protected override Sprite GetIcon()
    {
        return slotType == SlotType.BreakItem
            ? BreakItemSlotManager.Instance.GetEntry(slotIndex).item.icon
            : inv.GetEntry(this, slotIndex).item.icon;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotEntry entry = null;

        if (slotType == SlotType.BreakItem)
            entry = BreakItemSlotManager.Instance.GetEntry(slotIndex);
        else
            entry = inv.GetEntry(this, slotIndex);
        if (entry != null && !entry.IsEmpty())
        {
            toolTipUI?.Show(entry.item.itemName, eventData.position + toolTipOffset);
            SlotHoverState.HoveredSlotIndex = slotIndex;
            SlotHoverState.slotManager = this;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipUI?.Hide();
        SlotHoverState.HoveredSlotIndex = -1;
        SlotHoverState.slotManager = null;
    }

    protected override void OnDropSlot(SlotBase other)
    {
        if (other is not SlotManager o) return;
        if (slotType == SlotType.BreakItem || o.slotType == SlotType.BreakItem) return;

        inv.SwapSlots(this, o, slotIndex, o.slotIndex);
    }
}
