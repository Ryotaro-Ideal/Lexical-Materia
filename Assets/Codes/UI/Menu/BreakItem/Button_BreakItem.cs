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
    }

    // Update is called once per frame
    void Update()
    {
        button.onClick.AddListener(OnButtonClicked);
    }
    private void OnButtonClicked()
    {
        //ボタンを押したら、breakItemSlotManagerでアイテム削除と文字取得を行う
        breakItemSlotManager.ConvertItems();
    }

}
