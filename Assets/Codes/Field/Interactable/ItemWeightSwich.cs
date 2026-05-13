using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ItemWeightSwitch : MonoBehaviour, IInteractable
{
    public int targetWeight = 10;
    public UnityEvent OnSucceed;
    public UnityEvent OnFail;

    private int currentWeight = 0;
    private EquipController equipController;
    private string currentMessage;
    private bool isReverting = false;
    private bool isSucceeded = false;

    private void Start()
    {
        equipController = FindFirstObjectByType<EquipController>();
        ResetMessage();
    }

    public void Interact()
    {
        if (isSucceeded) return;
        ItemData currentEquipped = equipController.GetEquippedItem();

        if (currentEquipped == null)
        {
            ShowTemporaryMessage("アイテムを装備していません");
            return;
        }

        if (currentEquipped.weight >= targetWeight)
        {
            Debug.Log($"成功: {currentEquipped.itemName} (Weight:{currentEquipped.weight}) を使用しました。");
            ConsumeItem(currentEquipped);
            isSucceeded = true;
            currentMessage = "スイッチが作動した";
            OnSucceed?.Invoke();
        }
        else
        {
            Debug.Log($"失敗: 重さが足りません (必要:{targetWeight}, 現在:{currentEquipped.weight})");
            ShowTemporaryMessage("軽すぎます");
            OnFail?.Invoke();
        }
    }

    private void ConsumeItem(ItemData item)
    {
        InventorySlotManager.Instance.RemoveItem(item, 1);
        equipController.Unequip();
    }

    private void ShowTemporaryMessage(string msg)
    {
        if (isReverting) StopAllCoroutines();
        StartCoroutine(TemporaryMessageRoutine(msg));
    }

    private IEnumerator TemporaryMessageRoutine(string msg)
    {
        isReverting = true;
        currentMessage = msg;
        yield return new WaitForSeconds(1.5f);
        ResetMessage();
        isReverting = false;
    }

    private void ResetMessage()
    {
        currentMessage = $"{targetWeight}より重いものを乗せてください";
    }

    public string GetName()
    {
        return currentMessage;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
}