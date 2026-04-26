using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreasureReward
{
    public ItemData item;

    [Min(1)]
    public int count = 1;

    [Range(0f, 1f)]
    [Tooltip("出現確率。1 = 100%確定、0.5 = 50%の確率で出る")]
    public float dropChance = 1f;
}


public class TreasureChest : MonoBehaviour
{
    [SerializeField] private List<TreasureReward> rewards = new List<TreasureReward>();

    [Tooltip("一度開けたら再度開けられないようにする")]
    [SerializeField] private bool isOneShot = true;

    private bool hasOpened = false;


    public void OpenChest()
    {
        if (isOneShot && hasOpened) return;

        var inv = InventorySlotManager.Instance;
        if (inv == null)
        {
            Debug.LogWarning("TreasureChest: InventorySlotManager が見つかりません");
            return;
        }

        foreach (var reward in rewards)
        {
            if (reward.item == null) continue;

            if (Random.value <= reward.dropChance)
            {
                inv.AutoPlaceItem(reward.item, reward.count);
                Debug.Log($"[TreasureChest] {reward.item.itemName} × {reward.count} を入手");
            }
        }

        if (isOneShot) hasOpened = true;
    }
}
