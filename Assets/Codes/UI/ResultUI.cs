using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    private void Start()
    {
        // 初期状態は非表示
        resultPanel.SetActive(false);

        // ボタンのイベント登録
        retryButton.onClick.AddListener(() => SoundManager.Instance?.PlaySE(SoundName.Click));
        retryButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        titleButton.onClick.AddListener(() => SoundManager.Instance?.PlaySE(SoundName.Click));
        titleButton.onClick.AddListener(() => GameManager.Instance.BackToTitle());
    }

    public void ShowResult()
    {
        resultPanel.SetActive(true);
        // カーソルを表示して操作可能にする
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
