using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    private void Start()
    {
        // 初期状態は非表示
        resultPanel.SetActive(false);

        // ボタンのイベント登録
        retryButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        titleButton.onClick.AddListener(() => GameManager.Instance.BackToTitle());
    }

    public void ShowResult(bool isVictory)
    {
        resultPanel.SetActive(true);
        resultText.text = isVictory ? "VICTORY" : "GAME OVER";
        resultText.color = isVictory ? Color.yellow : Color.red;

        // カーソルを表示して操作可能にする
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
