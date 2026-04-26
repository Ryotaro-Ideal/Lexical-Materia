using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class ItemManager : MonoBehaviour, IInteractable
{
    public ItemData itemData;
    public Vector3 uiOffset = Vector3.up * 0.6f; // UI表示位置のオフセット

    public event Action<ItemManager> OnPicked;
    private InventorySlotManager invSlotManager;
    private string itemName = "未設定";
    public string ItemName { get { return itemName; } }
    private void Awake()
    {
        invSlotManager = FindFirstObjectByType<InventorySlotManager>();
        if (itemData != null) itemName = itemData.itemName;
    }
    public virtual void Interact()
    {
        if (invSlotManager != null)
        {
            invSlotManager.AutoPlaceItem(itemData, 1);
        }
        OnPicked?.Invoke(this);
        Destroy(gameObject);
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public string GetName()
    {
        return itemName;
    }

}