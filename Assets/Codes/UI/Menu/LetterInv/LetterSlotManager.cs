using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LetterSlotManager : SlotBase
{

    private int count = 0;
    public LetterData letterData;
    public TMP_Text iconText;
    public Color activeColor;
    public bool isGimmickMode = false;

    public event System.Action<LetterSlotManager> OnGimmickClicked;

    public int Count { get { return count; } }

    protected override void Awake()
    {
        base.Awake();
        if (letterData != null) iconText.text = letterData.letterName;
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);

    }



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
    public void SetTextColor(int currentCount, int requiredCount)
    {
        countText.color = currentCount >= requiredCount ? Color.black : Color.red;
    }
    public void ClearLetter()
    {
        letterData = null;
        iconText.text = "";
        count = 0;
        icon.color = slotColor;
        countText.enabled = false;
    }


    public void ResetCount()
    {
        count = 0;
        countText.enabled = true;
        countText.text = "0";
    }

    protected override string GetDisplayName()
    {
        return letterData != null ? letterData.letterName : "";
    }

    protected override Sprite GetIcon()
    {
        return icon != null ? icon.sprite : null;
    }



    protected override bool HasItem()
    {
        return letterData != null && count > 0;
    }

    protected override void OnDropSlot(SlotBase other)
    {
        if (isGimmickMode) return;
    }

    private void OnSlotClicked()
    {
        if (!HasItem()) return;

        if (isGimmickMode)
        {
            OnGimmickClicked?.Invoke(this);
            return;
        }

        Debug.Log($"LetterSlot {slotIndex}: {letterData.letterName} x{count}");
    }
}
