using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 特定のアイテムを装備している時のみインタラクト可能なオブジェクト。
/// 例：斧を装備している時だけ壊せる木、鍵を装備している時だけ開く扉など。
/// </summary>
public class RequirementInteractable : MonoBehaviour, IInteractable
{
    [Header("設定")]
    [SerializeField] private ItemData requiredItem; // 必要なアイテム
    [SerializeField] private string interactionName = "調べる"; // UIに表示する名前
    [SerializeField] private string failMessage = "アイテムが足りません"; // 条件を満たさない時のメッセージ（拡張用）
    [SerializeField] private bool isOneShot = true; // 一度だけ実行するか

    [Header("イベント")]
    public UnityEvent OnSucceed; // 条件を満たしてインタラクトした時の処理
    public UnityEvent OnFail;    // 条件を満たさずにインタラクトした時の処理

    private EquipController equipController;
    private bool isExecuted = false; // すでに成功したか

    private void Awake()
    {
        equipController = FindFirstObjectByType<EquipController>();
    }

    public void Interact()
    {
        if (isExecuted && isOneShot) return;
        if (equipController == null) return;

        // 現在装備しているアイテムを取得
        ItemData currentEquipped = equipController.GetEquippedItem();

        // 判定
        if (currentEquipped != null && currentEquipped == requiredItem)
        {
            Debug.Log($"条件達成: {requiredItem.itemName} を使用しました。");
            if (isOneShot) isExecuted = true;
            OnSucceed?.Invoke();
        }
        else
        {
            Debug.Log($"条件未達成: {requiredItem.itemName} が必要です。");
            OnFail?.Invoke();
        }
    }

    public string GetName()
    {
        return interactionName;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
}
