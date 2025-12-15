using System;
using UnityEngine;


public class InventoryAllManager : MonoBehaviour
{
    // 外部からアクセスするための静的インスタンス（シングルトン的な運用）
    public static InventoryAllManager Instance { get; private set; }

    // 内部の各インベントリへの参照
    private InventorySlotManager inv => InventorySlotManager.Instance;

    private void Awake()
    {
        // シングルトンとしてのセットアップ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    public void AddItemToInventory(ItemData itemData, int count = 1)
    {
        inv.AutoPlaceItem(itemData, count);
    }


    void Update()
    {

    }

    public static implicit operator InventoryAllManager(InventorySlotManager v)
    {
        throw new NotImplementedException();
    }
}
