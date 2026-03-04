using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterInvManager : MonoBehaviour
{
    public static LetterInvManager Instance { get; private set; }
    //スロットの種類(ひらがなやカタカナなど)とスロットマネージャーの情報が入った二次元リストを作成
    [SerializeField] LetterSlotManager[] letterSlots;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        letterSlots = gameObject.GetComponentsInChildren<LetterSlotManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetLetters(List<SlotEntry> slots)
    {
        Debug.Log("LetterInvManager: SetLetters発火");
        foreach (var slot in slots)
        {
            ItemData item = slot.item;
            List<DestroyMaterial> letters = item.destroyMaterials;
            foreach (var s in letters)
            {
                foreach (var l in letterSlots)
                {
                    int count = s.count;
                    if (l.letterData == null) continue;
                    if (l.letterData == s.letterData)
                    {
                        l.AddCount(count);
                    }
                }
            }

        }
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

    }
}
