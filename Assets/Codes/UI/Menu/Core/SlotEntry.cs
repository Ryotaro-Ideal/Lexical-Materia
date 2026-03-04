using System;
[Serializable]
public class SlotEntry
{
    public ItemData item;
    public int count;

    public bool IsEmpty() => item == null || count <= 0;

    public void Clear()
    {
        item = null;
        count = 0;
    }
    public void MoveFrom(SlotEntry src)
    {
        item = src.item;
        count = src.count;
        src.Clear();
    }

}
