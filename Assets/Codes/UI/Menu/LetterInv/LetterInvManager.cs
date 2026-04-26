using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterInvManager : MonoBehaviour
{
    public static LetterInvManager Instance { get; private set; }
    [SerializeField] LetterSlotManager[] letterSlots;

    public event Action OnLetterChanged;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        letterSlots = gameObject.GetComponentsInChildren<LetterSlotManager>(true);
    }


    void Update()
    {

    }
    public void SetLetters(List<SlotEntry> slots)
    {
        Debug.Log("LetterInvManager: SetLetters発火");
        foreach (var slot in slots)
        {
            if (slot.item == null) continue;

            ItemData item = slot.item;
            List<DestroyMaterial> letters = item.destroyMaterials;
            foreach (var s in letters)
            {
                foreach (var l in letterSlots)
                {
                    if (l.letterData == null) continue;
                    if (l.letterData == s.letterData)
                    {
                        // アイテムの個数(slot.count)を掛ける
                        int totalAddCount = s.count * slot.count;
                        l.AddCount(totalAddCount);
                    }
                }
            }
        }
        OnLetterChanged?.Invoke();
    }
    public int GetCount(DestroyMaterial requiredLetter)
    {
        int total = 0;
        LetterData reqLetterData = requiredLetter.letterData;
        foreach (var slot in letterSlots)
        {
            if (slot.letterData == reqLetterData)
            {
                total += slot.Count;
            }
        }
        return total;
    }
    public bool CanCraft(CraftData craftData)
    {
        foreach (DestroyMaterial reqLetter in craftData.requiredLetters)
        {
            int count = GetCount(reqLetter);
            if (count < reqLetter.count)
            {
                return false;
            }

        }
        return true;
    }
    public void ConsumeLetters(CraftData craftData)
    {
        foreach (DestroyMaterial reqLetter in craftData.requiredLetters)
        {
            int remaining = reqLetter.count;
            foreach (var slot in letterSlots)
            {
                if (slot.letterData == reqLetter.letterData)
                {
                    int slotCount = slot.Count;
                    if (slotCount >= remaining)
                    {
                        slot.AddCount(-remaining);
                        break;
                    }
                    else
                    {
                        slot.AddCount(-slotCount);
                        remaining -= slotCount;
                    }
                    if (slotCount <= 0)
                    {
                        slot.ClearLetter();
                    }

                }

            }
        }
        OnLetterChanged?.Invoke();
    }

    // ===== セーブ・ロード =====

    /// <summary>文字インベントリの現在状態をSaveDataに書き込む</summary>
    public void WriteSaveData(SaveData data)
    {
        data.letters.Clear();
        foreach (var slot in letterSlots)
        {
            if (slot.letterData == null || slot.Count <= 0) continue;
            data.letters.Add(new LetterSaveEntry(slot.letterData.letterName, slot.Count));
        }
    }

    /// <summary>SaveDataから文字インベントリを復元する</summary>
    public void ReadSaveData(SaveData data)
    {
        if (data == null || data.letters == null) return;

        // カウントのみリセット（letterDataは保持してletterNameによる検索を維持する）
        foreach (var slot in letterSlots)
            slot.ResetCount();

        // letterNameで対応スロットを検索してカウントを復元
        foreach (var entry in data.letters)
        {
            foreach (var slot in letterSlots)
            {
                if (slot.letterData != null && slot.letterData.letterName == entry.letterName)
                {
                    slot.AddCount(entry.count);
                    break;
                }
            }
        }
    }
}

