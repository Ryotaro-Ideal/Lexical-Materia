using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TooltipUI : MonoBehaviour
{

    [SerializeField] private Canvas rootCanvas; // Screen Space - Overlay 推奨
    [SerializeField] private RectTransform panel; // 表示用パネル（Text を子に置く）
    [SerializeField] private TMP_Text label; // "名前" と "Press LMB to pick up" を表示

    void Awake()
    {

        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        Hide();
    }

    // text は多段表示可能（例： "リンゴ\n[左クリックで拾う]"）
    public void Show(string text, Vector3 screenPosition)
    {
        if (panel == null || label == null) return;
        panel.gameObject.SetActive(true);
        label.text = text;

        // パネルを画面上の座標に移動（スクリーン座標 → Canvas内部の座標）
        Vector2 anchored;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rootCanvas.transform, screenPosition, rootCanvas.worldCamera, out anchored);
        panel.anchoredPosition = anchored;
    }

    public void Hide()
    {
        if (panel != null) panel.gameObject.SetActive(false);
    }
}
