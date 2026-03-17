using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class SlideButton : MonoBehaviour
{
    private int pageCount;
    private int currentPage = 0;
    [SerializeField] private float slideTime = 0.35f;
    [Header("Navigation Buttons")]
    [SerializeField] private Button btnPrev;   // ← ボタン
    [SerializeField] private Button btnNext;   // → ボタン
    private RectTransform rt;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        pageCount = transform.childCount; // 子に Page だけ数える（ボタンは除外）
        // 子オブジェクトが Page だけになるように配置しておく
        rt.anchoredPosition = Vector2.zero;
        UpdateNavButtons();                     // 初期状態のボタン有効/無効を設定
    }
    // ★ 追加 ★ 右ボタンから呼び出す
    public void MoveNext()
    {
        if (currentPage < pageCount - 1) SlideTo(currentPage + 1);
    }
    // ★ 追加 ★ 左ボタンから呼び出す
    public void MovePrev()
    {
        if (currentPage > 0) SlideTo(currentPage - 1);
    }
    // 既存の外部呼び出し用（必要なら残す）
    public void SlideTo(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageCount) return;
        if (pageIndex == currentPage) return;
        float targetX = -pageIndex * rt.rect.width;
        StopAllCoroutines();
        StartCoroutine(SlideCoroutine(targetX));
        currentPage = pageIndex;
        UpdateNavButtons();                     // ページが変わったらボタン状態更新
    }
    private IEnumerator SlideCoroutine(float targetX)
    {
        float startX = rt.anchoredPosition.x;
        float elapsed = 0f;
        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideTime);
            float newX = Mathf.Lerp(startX, targetX, t);
            rt.anchoredPosition = new Vector2(newX, rt.anchoredPosition.y);
            yield return null;
        }
        rt.anchoredPosition = new Vector2(targetX, rt.anchoredPosition.y);
    }
    // ★ 追加 ★ ボタンの有効/無効を切り替えるだけのメソッド
    private void UpdateNavButtons()
    {
        if (btnPrev != null) btnPrev.interactable = currentPage > 0;
        if (btnNext != null) btnNext.interactable = currentPage < pageCount - 1;
    }
}
