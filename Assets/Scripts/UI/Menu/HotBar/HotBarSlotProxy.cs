using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ホットバー上のスロットがどのインベントリスロットを参照するかを保持するプロキシ。
/// SlotManager（表示用）とは別にこのコンポーネントで紐づけを行うと運用が楽。
/// </summary>
public class HotbarSlotProxy : MonoBehaviour
{
    [Tooltip("参照する InventorySlotManager のスロットインデックス（0 始まり）")]
    public int inventorySlotIndex = -1;

    // 表示は SlotManager に任せるが、簡易アクセスのため getter を用意
    public int GetInventoryIndex() => inventorySlotIndex;
}
