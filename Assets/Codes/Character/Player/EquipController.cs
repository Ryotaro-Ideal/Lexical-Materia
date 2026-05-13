using UnityEngine;

public class EquipController : MonoBehaviour
{
    public static EquipController Instance { get; private set; }
    private InventorySlotManager inventorySlotManager;
    public Transform handTransform;
    public GameObject modelRoot;
    private GameObject currentModelInstance;
    private ItemData equippedItem;
    private int equippedCount;

    public ItemData GetEquippedItem() => equippedItem;
    public int GetEquippedCount() => equippedCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        inventorySlotManager = InventorySlotManager.Instance;
        if (inventorySlotManager == null) inventorySlotManager = InventorySlotManager.Instance;
    }

    public void EquipFromInventorySlot(SlotManager s, int idx)
    {
        var entry = inventorySlotManager.GetEntry(s, idx);
        if (entry == null || entry.IsEmpty()) return;
        EquipItem(entry.item, entry.count);
    }

    public void EquipItem(ItemData item, int count)
    {
        if (item == null) return;
        equippedItem = item;
        equippedCount = count;
        UpdateEquippedModel();
        Debug.Log($"Equipped: {equippedItem.itemName} (x{equippedCount})");
    }

    public void Unequip()
    {
        equippedItem = null;
        equippedCount = 0;
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }
    }

    private void UpdateEquippedModel()
    {
        if (currentModelInstance != null) Destroy(currentModelInstance);
        if (equippedItem == null) return;

        if (equippedItem.visualPrefab != null)
        {
            currentModelInstance = Instantiate(equippedItem.visualPrefab, modelRoot != null ? modelRoot.transform : handTransform);
            currentModelInstance.transform.localPosition = Vector3.zero;
            currentModelInstance.transform.localRotation = Quaternion.identity;
        }
    }
}
