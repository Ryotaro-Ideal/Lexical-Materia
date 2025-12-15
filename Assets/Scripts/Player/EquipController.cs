using UnityEngine;

/// <summary>
/// 装備（手に持つ）を管理するシンプルなコントローラ。
/// - EquipFromInventorySlot(int slotIndex) を呼ばれると、その slot のアイテムを参照して装備する。
/// - 実モデルの表示は prefab を用意して Instantiate / Destroy で切替える（ここではアイコン表示や簡易モデル表示を想定）。
/// </summary>
public class EquipController : MonoBehaviour
{
    public InventorySlotManager inventoryManager;
    public Transform handTransform; // 手の位置に装備モデルを出す場合
    public GameObject modelRoot;    // 装備モデルのルート（生成物はここに置く）
    private GameObject currentModelInstance;
    private ItemData equippedItem;
    private int equippedCount;

    private void Awake()
    {
        if (inventoryManager == null) inventoryManager = InventorySlotManager.Instance;
    }

    // インベントリスロットから装備（表示だけ。必要ならインベントリを減らす）
    public void EquipFromInventorySlot(SlotManager s)
    {
        var entry = inventoryManager.GetEntry(s);
        if (entry == null || entry.IsEmpty()) return;



        // 装備
        equippedItem = entry.item;
        equippedCount = entry.count;

        // 見た目を切替（モデルがあるなら Instantiate）
        UpdateEquippedModel();
        Debug.Log($"Equipped: {equippedItem.itemName}");
    }

    // 装備解除
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
        // 既存オブジェクトを削除
        if (currentModelInstance != null) Destroy(currentModelInstance);

        if (equippedItem == null) return;

        // ItemData に prefab の参照があれば Instantiate（想定: itemPrefab）
        if (equippedItem.visualPrefab != null)
        {
            currentModelInstance = Instantiate(equippedItem.visualPrefab, modelRoot != null ? modelRoot.transform : handTransform);
            // ローカル位置調整（アイテムの見た目に合わせて調整）
            currentModelInstance.transform.localPosition = Vector3.zero;
            currentModelInstance.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // なければアイコンで代替（または何もしない）
        }
    }
}
