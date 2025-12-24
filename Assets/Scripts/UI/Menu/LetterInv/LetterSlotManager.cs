using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LetterSlotManager : SlotBase
{
    public int slotIndex = -1;

    [SerializeField] private int count = 0;
    public LetterData letterData;
    public TMP_Text iconText;
    public Color activeColor;

    protected override void Awake()
    {
        base.Awake();

        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);

        slotIndex = transform.GetSiblingIndex();
    }

    // ---------------- データ操作 ----------------

    public void AddCount(int c)
    {
        Debug.Log("LetterSlotManager: AddCount発火");
        count += c;
        countText.enabled = true;
        countText.text = count > 0 ? count.ToString() : "0";
        countText.raycastTarget = false;
    }
    public void SetLetter(LetterData data, int c)
    {
        letterData = data;
        iconText.text = data.letterName;
        count = c;
        countText.enabled = true;
        countText.text = count > 0 ? count.ToString() : "0";
        countText.raycastTarget = false;
        icon.color = activeColor;
    }
    public void ClearLetter()
    {
        letterData = null;
        iconText.text = "";
        count = 0;
        icon.color = slotColor;
    }

    protected override string GetDisplayName()
    {
        return letterData != null ? letterData.letterName : "";
    }

    protected override Sprite GetIcon()
    {
        return icon != null ? icon.sprite : null;
    }

    // ---------------- SlotBase 抽象実装 ----------------

    protected override bool HasItem()
    {
        return letterData != null && count > 0;
    }



    protected override void OnDropSlot(SlotBase other)
    {
        // 今回は LetterSlot 同士の処理は未実装
        // 必要になったらここに書く
    }

    // ---------------- Click ----------------

    private void OnSlotClicked()
    {
        if (!HasItem())
            return;

        Debug.Log($"LetterSlot {slotIndex}: {letterData.letterName} x{count}");
    }
}
