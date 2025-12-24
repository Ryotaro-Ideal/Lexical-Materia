using UnityEngine;
using UnityEngine.UI;

public class CraftSlotManager : SlotBase
{

    CraftInfo CraftInfo => CraftInfo.Instance;
    public int slotIndex = -1;
    public CraftData craftData;

    protected override void Awake()
    {
        base.Awake();
        slotIndex = transform.GetSiblingIndex();
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);
    }
    protected override string GetDisplayName()
    {
        return null;
    }

    protected override Sprite GetIcon()
    {
        return null;
    }

    // ---------------- SlotBase 抽象実装 ----------------

    protected override bool HasItem()
    {
        return false;
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
