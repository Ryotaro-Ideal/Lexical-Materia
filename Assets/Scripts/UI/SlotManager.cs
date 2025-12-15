using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SlotManager : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public enum SlotType
    {
        Tool,
        Consumable,
        Inventory,
        BreakItem
    }

    public SlotType slotType;
    public int slotIndex = -1;

    [Header("UI References")]
    public Image icon;
    public TMP_Text countText;
    public Color slotColor;

    [Header("Tooltip")]
    public TooltipUI toolTipUI;
    public Vector2 toolTipOffset = new Vector2(12f, -18f);

    private InventorySlotManager inv => InventorySlotManager.Instance;

    private Canvas rootCanvas;
    private RectTransform dragIconRT;
    private Image dragIconImage;
    private Image slotImage;

    public Sprite UIMaskSprite;

    void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);

        rootCanvas = GetComponentInParent<Canvas>();
        slotImage = GetComponent<Image>();
        slotImage.color = slotColor;

        CreateDragIcon();
    }

    // ---------------- UI ----------------

    public void SetItem(ItemData item, int count)
    {
        if (item == null || count <= 0)
        {
            ClearSlot();
            return;
        }

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

    // ---------------- Click ----------------

    private void OnSlotClicked()
    {
        if (slotType == SlotType.BreakItem)
        {
            var entry = BreakItemSlotManager.Instance.GetEntry(slotIndex);
            if (entry == null || entry.IsEmpty())
            {
                Debug.Log("Break Item slot is empty");
                return;
            }

            Debug.Log($"Break Item slot {slotIndex}: {entry.item.itemName} x{entry.count}");
            return;
        }

        var e = inv.GetEntry(this);
        if (e == null || e.IsEmpty())
        {
            Debug.Log("Empty slot");
            return;
        }

        Debug.Log($"slot {slotIndex}: {e.item.itemName} x{e.count}");
    }

    // ---------------- Tooltip ----------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotEntry entry = null;

        if (slotType == SlotType.BreakItem)
            entry = BreakItemSlotManager.Instance.GetEntry(slotIndex);
        else
            entry = inv.GetEntry(this);

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

    // ---------------- Drag & Drop ----------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // BreakItem スロットは今の設計ではドラッグ不可
        if (slotType == SlotType.BreakItem)
            return;

        var e = inv.GetEntry(this);
        if (e == null || e.IsEmpty())
            return;

        dragIconImage.sprite = e.item.icon;
        dragIconImage.enabled = true;
        dragIconRT.gameObject.SetActive(true);
        UpdateDragIconPosition(eventData);
        icon.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRT != null && dragIconRT.gameObject.activeSelf)
            UpdateDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragIconRT.gameObject.SetActive(false);
        dragIconImage.enabled = false;
        icon.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var otherSlot = dragged.GetComponent<SlotManager>();
        if (otherSlot == null) return;

        // Inventory 同士の Swap のみ許可（最小修正）
        if (slotType == SlotType.BreakItem || otherSlot.slotType == SlotType.BreakItem)
            return;

        bool ok = inv.SwapSlots(this, otherSlot);
        if (!ok)
            Debug.Log("スワップが拒否されました");
    }

    // ---------------- Drag Icon ----------------

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out var pos);

        dragIconRT.anchoredPosition = pos;
    }

    private void CreateDragIcon()
    {
        var go = new GameObject("DragIcon");
        go.transform.SetParent(rootCanvas.transform, false);

        dragIconRT = go.AddComponent<RectTransform>();
        dragIconRT.sizeDelta = new Vector2(90, 90);

        dragIconImage = go.AddComponent<Image>();
        dragIconImage.raycastTarget = false;
        dragIconImage.enabled = false;

        go.SetActive(false);
    }
}
