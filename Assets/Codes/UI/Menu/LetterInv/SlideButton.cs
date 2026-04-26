using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlideButton : MonoBehaviour
{
    private int pageCount;
    private int currentPage = 0;
    [SerializeField] private float slideTime = 0.35f;
    [Header("Navigation Buttons")]
    [SerializeField] private Button btnPrev;
    [SerializeField] private Button btnNext;
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        pageCount = transform.childCount;
        rt.anchoredPosition = Vector2.zero;

        if (btnPrev != null) btnPrev.onClick.AddListener(MovePrev);
        if (btnNext != null) btnNext.onClick.AddListener(MoveNext);

        UpdateNavButtons();
    }

    public void MoveNext()
    {
        if (currentPage < pageCount - 1)
        {
            SoundManager.Instance?.PlaySE(SoundName.Click);
            SlideTo(currentPage + 1);
        }
    }

    public void MovePrev()
    {
        if (currentPage > 0)
        {
            SoundManager.Instance?.PlaySE(SoundName.Click);
            SlideTo(currentPage - 1);
        }
    }

    public void SlideTo(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageCount) return;
        if (pageIndex == currentPage) return;
        float targetX = -pageIndex * rt.rect.width;
        StopAllCoroutines();
        StartCoroutine(SlideCoroutine(targetX));
        currentPage = pageIndex;
        UpdateNavButtons();
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

    private void UpdateNavButtons()
    {
        if (btnPrev != null) btnPrev.interactable = currentPage > 0;
        if (btnNext != null) btnNext.interactable = currentPage < pageCount - 1;
    }
}
