using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class ToolTipUI : MonoBehaviour
{
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] protected RectTransform panel;
    [SerializeField] protected TMP_Text label;

    protected virtual void Awake()
    {
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        Hide();
    }

    protected void ShowPanel(string text, Vector2 screenPosition)
    {
        if (panel == null || label == null) return;
        panel.gameObject.SetActive(true);
        label.text = text;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rootCanvas.transform, screenPosition,
            rootCanvas.worldCamera, out var anchored);
        panel.anchoredPosition = anchored;
    }

    public void Hide()
    {
        if (panel != null) panel.gameObject.SetActive(false);
    }
}
