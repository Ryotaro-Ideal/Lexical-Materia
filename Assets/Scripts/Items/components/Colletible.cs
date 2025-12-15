using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
  public ItemData itemData;
  public Vector3 uiOffset = Vector3.up * 0.6f; // UI表示位置のオフセット

  public event Action<Collectible> OnPicked;
  private InventoryAllManager invAllManager;
  private string itemName = "未設定";
  public string ItemName { get { return itemName; } }
  private void Awake()
  {
    invAllManager = FindFirstObjectByType<InventoryAllManager>();
    if (itemData != null) itemName = itemData.itemName;
  }
  public virtual void Pickup()
  {
    if (invAllManager != null)
    {
      invAllManager.AddItemToInventory(itemData);
    }
    OnPicked?.Invoke(this);
    Destroy(gameObject);
  }

}