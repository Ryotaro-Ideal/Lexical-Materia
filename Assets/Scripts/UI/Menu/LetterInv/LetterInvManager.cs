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
        //
        letterSlots = FindObjectsByType<LetterSlotManager>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetLetters(List<SlotEntry> slots)
    {
        foreach (var slot in slots)
        {
            int count = slot.count;
            ItemData item = slot.item;
            List<LetterData> letters = item.destroyMaterials;
            foreach (var s in letters)
            {
                foreach (var l in letterSlots)
                {
                    if (l.letterData == null) continue;
                    if (l.letterData == s)
                    {
                        l.SetItem(count);
                    }
                }
            }

        }
    }
}
