using System;
using System.Collections.Generic;


[Serializable]
public class SaveData
{
    public List<LetterSaveEntry> letters = new List<LetterSaveEntry>();

    public List<ItemSaveEntry> toolSlots = new List<ItemSaveEntry>();
    public List<ItemSaveEntry> consumableSlots = new List<ItemSaveEntry>();
    public List<ItemSaveEntry> freeSlots = new List<ItemSaveEntry>();
    public List<ItemSaveEntry> inventorySlots = new List<ItemSaveEntry>();
}


[Serializable]
public class LetterSaveEntry
{
    public string letterName;
    public int count;

    public LetterSaveEntry() { }
    public LetterSaveEntry(string letterName, int count)
    {
        this.letterName = letterName;
        this.count = count;
    }
}


[Serializable]
public class ItemSaveEntry
{
    public string itemId;
    public int count;

    public ItemSaveEntry() { }
    public ItemSaveEntry(string itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }

    public bool IsEmpty() => string.IsNullOrEmpty(itemId) || count <= 0;
}
