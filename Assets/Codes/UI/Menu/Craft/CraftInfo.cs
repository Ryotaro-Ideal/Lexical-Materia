using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CraftInfo : MonoBehaviour
{
    public static CraftInfo Instance { get; private set; }
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public LetterSlotManager[] currentLetterSlots;

    private LetterInvManager letterInvManager => LetterInvManager.Instance;

    private CraftData currentCraftData;

    void Awake()
    {
        Instance = this;
        currentLetterSlots = GetComponentsInChildren<LetterSlotManager>();
    }
    public void SetInfo(CraftSlotManager craftSlot)
    {
        currentCraftData = craftSlot.craftData;
        if (currentCraftData == null) { Debug.LogError("CraftInfo:CraftData is null"); return; }
        itemName.text = currentCraftData.itemData.itemName;
        itemDescription.text = currentCraftData.itemData.description;
        List<DestroyMaterial> requiredLetters = currentCraftData.requiredLetters;
        for (int i = 0; i < currentLetterSlots.Length; i++)
        {
            if (i < requiredLetters.Count)
            {
                currentLetterSlots[i].SetLetter(requiredLetters[i].letterData, requiredLetters[i].count);
                currentLetterSlots[i].SetTextColor(letterInvManager.GetCount(requiredLetters[i]), requiredLetters[i].count);
            }
            else
            {
                currentLetterSlots[i].ClearLetter();
            }
        }
    }
    public void ExecuteCraft()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        if (currentCraftData == null) { Debug.LogError("CraftInfo:CraftData is null"); return; }
        if (letterInvManager == null) { Debug.LogError("CraftInfo:LetterInvManager is null"); return; }

        if (letterInvManager.CanCraft(currentCraftData))
        {
            letterInvManager.ConsumeLetters(currentCraftData);
            InventorySlotManager.Instance.AutoPlaceItem(currentCraftData.itemData, 1);
            Debug.Log("CraftInfo: クラフト成功、アイテムを追加");
        }
        else
        {
            Debug.Log("CraftInfo:必要な文字が足りない");
        }

    }


}