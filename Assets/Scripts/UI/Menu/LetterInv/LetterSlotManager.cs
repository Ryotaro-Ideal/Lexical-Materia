using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class LetterSlotManager : MonoBehaviour
{
    public int slotIndex = -1;

    [SerializeField] private int count = 0;

    public LetterData letterData;

    [Header("UI References")]
    public TMP_Text countText;


    [Header("Tooltip")]
    public TooltipUI toolTipUI;
    public Vector2 toolTipOffset = new Vector2(12f, -18f);

    private InventorySlotManager inv => InventorySlotManager.Instance;




    public Sprite UIMaskSprite;

    void Awake()
    {
        countText.enabled = true;
        SetItem(0);
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnSlotClicked);
        //slot番号は自分が今いる場所から取得
        slotIndex = transform.GetSiblingIndex();

    }

    // ---------------- UI ----------------

    public void SetItem(int c)
    {
        count += c;
        countText.enabled = true;
        countText.text = count > 0 ? count.ToString() : "0";
        countText.raycastTarget = false;
    }

    // ---------------- Click ----------------

    private void OnSlotClicked()
    {

    }


}
