using UnityEngine;

public class SlotTipUI : ToolTipUI
{
    public void Show(string name, Vector2 screenPosition)
    {
        ShowPanel(name, screenPosition);
    }
}