using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class SlotBase : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI")]
    public Image icon;
    public TMP_Text countText;
    public Color slotColor;
    public Sprite UIMaskSprite;

    [Header("Tooltip")]
    public TooltipUI toolTipUI;
    public Vector2 toolTipOffset = new Vector2(12f, -18f);

    protected Canvas rootCanvas;
    protected RectTransform dragIconRT;
    protected Image dragIconImage;
    protected Image slotImage;

    protected virtual void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        slotImage = GetComponent<Image>();
        slotImage.color = slotColor;
        CreateDragIcon();
        if (icon.sprite != UIMaskSprite)
        {
            icon.enabled = true;
            countText.enabled = true;

        }
    }

    // ---------- 抽象：派生で実装 ----------

    protected abstract bool HasItem();
    protected abstract string GetDisplayName();
    protected abstract Sprite GetIcon();
    protected abstract void OnDropSlot(SlotBase other);

    // ---------- UI ----------

    protected void ClearSlotUI()
    {
        icon.enabled = false;
        countText.enabled = false;
        icon.sprite = UIMaskSprite;
        countText.text = "";
    }

    // private void OnSlotClicked()
    // {
    //     if (slotType == SlotType.BreakItem)
    //     {
    //         var entry = BreakItemSlotManager.Instance.GetEntry(slotIndex);
    //         if (entry == null || entry.IsEmpty())
    //         {
    //             Debug.Log("Break Item slot is empty");
    //             return;
    //         }

    //         Debug.Log($"Break Item slot {slotIndex}: {entry.item.itemName} x{entry.count}");
    //         return;
    //     }

    //     var e = inv.GetEntry(this);
    //     if (e == null || e.IsEmpty())
    //     {
    //         Debug.Log("Empty slot");
    //         return;
    //     }

    //     Debug.Log($"slot {slotIndex}: {e.item.itemName} x{e.count}");
    // }

    // ---------- Tooltip ----------



    // ---------- Drag ----------

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasItem()) return;

        dragIconImage.sprite = GetIcon();
        dragIconImage.enabled = true;
        dragIconRT.gameObject.SetActive(true);
        UpdateDragIconPosition(eventData);
        icon.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRT.gameObject.activeSelf)
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
        var other = eventData.pointerDrag?.GetComponent<SlotBase>();
        if (other == null) return;
        OnDropSlot(other);
    }

    // ---------- DragIcon ----------

    void UpdateDragIconPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out var pos);

        dragIconRT.anchoredPosition = pos;
    }

    void CreateDragIcon()
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
