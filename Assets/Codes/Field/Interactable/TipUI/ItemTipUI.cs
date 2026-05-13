using UnityEngine;

public class ItemTipUI : ToolTipUI
{
    public void Show(string name, Vector3 screenPosition)
    {
        ShowPanel($"{name} : E", screenPosition);
    }
}