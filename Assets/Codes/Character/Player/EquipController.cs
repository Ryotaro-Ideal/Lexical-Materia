using UnityEngine;

public class EquipController : MonoBehaviour
{
    public static EquipController Instance { get; private set; }
    private InventorySlotManager inventorySlotManager = InventorySlotManager.Instance; // インベントリスロットの管理クラス（シングルトン想定）
    public Transform handTransform; // 手の位置に装備モデルを出す場合
    public GameObject modelRoot;    // 装備モデルのルート（生成物はここに置く）
    private GameObject currentModelInstance;
    private ItemData equippedItem;
    private int equippedCount;


    // 現在装備しているアイテムを返す
    public ItemData GetEquippedItem() => equippedItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (inventorySlotManager == null) inventorySlotManager = InventorySlotManager.Instance;
    }

    // インベントリスロットから装備（表示だけ。必要ならインベントリを減らす）
    public void EquipFromInventorySlot(SlotManager s, int idx)
    {
        var entry = inventorySlotManager.GetEntry(s, idx);
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
