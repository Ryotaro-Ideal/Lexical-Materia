using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CraftInfo : MonoBehaviour
{
    public static CraftInfo Instance { get; private set; }
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public LetterSlotManager[] requiredLetterSlots;

    void Awake()
    {
        Instance = this;
    }
    public void SetInfo(CraftSlotManager slot)
    {
        CraftData craftData = slot.craftData;
        if (craftData == null) { Debug.LogError("CraftInfo:CraftData is null"); return; }
        itemName.text = craftData.itemData.itemName;
        itemDescription.text = craftData.itemData.description;
        List<RequiredLetter> requiredLetters = craftData.requiredLetters;
        for (int i = 0; i < requiredLetterSlots.Length; i++)
        {
            if (i < requiredLetters.Count)
            {
                requiredLetterSlots[i].SetLetter(requiredLetters[i].letterData, requiredLetters[i].count);
            }
            else
            {
                requiredLetterSlots[i].ClearLetter();
            }
        }
    }


}