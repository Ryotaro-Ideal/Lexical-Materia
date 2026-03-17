using UnityEngine;
using UnityEngine.UI;

public class CraftSlotManager : SlotBase
{

    CraftInfo CraftInfo => CraftInfo.Instance;

    public CraftData craftData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);
        ClearSlot();
    }

    public void SetCraftData(CraftData data)
    {
        craftData = data;
        if (craftData != null && craftData.itemData != null)
        {
            icon.sprite = craftData.itemData.icon;
            icon.enabled = true;
            icon.color = Color.white;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        craftData = null;
        icon.sprite = UIMaskSprite;
        icon.enabled = false;
        icon.color = slotColor;
    }

    protected override string GetDisplayName()
    {
        return craftData?.itemData?.itemName;
    }

    protected override Sprite GetIcon()
    {
        return craftData?.itemData?.icon;
    }

    // ---------------- SlotBase 抽象実装 ----------------

    protected override bool HasItem()
    {
        return craftData != null;
    }



    protected override void OnDropSlot(SlotBase other)
    {
        // 今回は LetterSlot 同士の処理は未実装
        // 必要になったらここに書く
    }

    private void OnSlotClicked()
    {
        CraftInfo.SetInfo(this);
    }


}
