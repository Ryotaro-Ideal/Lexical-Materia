using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // シーンロード完了時のフェードイン登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 暗転してからシーンを読み込む。TransitionTriggerやResultUIから呼ぶ。
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // フェードアウト（透明 → 黒）
        yield return StartCoroutine(Fade(0f, 1f));

        Time.timeScale = 1f; // 死亡演出などで止まっていた場合のリセット
        SceneManager.LoadScene(sceneName);
        // OnSceneLoadedでフェードインが走る
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // timeScale=0でも動く
            fadePanel.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = to;

        // フェードイン完了後は非表示にする（完全に透明になったら不要）
        if (to == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}