using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Button_BreakItem : MonoBehaviour
{
    public Button button;
    private BreakItemSlotManager breakItemSlotManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        breakItemSlotManager = BreakItemSlotManager.Instance;
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        breakItemSlotManager.ConvertItems();
    }

}
