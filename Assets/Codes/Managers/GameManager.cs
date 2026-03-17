using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public enum GameState { Playing, Victory, GameOver }
    public GameState CurrentState { get; private set; }
    public UnityEvent<string> OnVictory;
    public UnityEvent OnGameOver;
    [SerializeField] private string nextSceneName = "";

    [SerializeField] private List<EnemyBase> enemies = new List<EnemyBase>();
    private int enemyCount;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        CurrentState = GameState.Playing;
        RefreshEnemyCount();
    }

    public void RefreshEnemyCount()
    {
        enemies.Clear();
        enemies.AddRange(FindObjectsByType<EnemyBase>(FindObjectsSortMode.None));
        enemyCount = enemies.Count;
        Debug.Log($"Total Enemies: {enemyCount}");
    }

    public void OnEnemyKilled()
    {
        enemyCount--;
        Debug.Log($"Enemy Killed. Remaining: {enemyCount}");

        if (enemyCount <= 0 && CurrentState == GameState.Playing)
        {
            WinGame();
        }
    }

    public void OnPlayerDeath()
    {
        if (CurrentState == GameState.Playing)
        {
            LoseGame();
        }
    }

    private void WinGame()
    {

        CurrentState = GameState.Victory;
        Debug.Log("Game Clear!");
        OnVictory?.Invoke(nextSceneName);
        FindFirstObjectByType<ResultUI>()?.ShowResult(true);

    }

    private void LoseGame()
    {
        CurrentState = GameState.GameOver;
        Debug.Log("Game Over...");
        OnGameOver?.Invoke();
        FindFirstObjectByType<ResultUI>()?.ShowResult(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToTitle()
    {
        // TODO: タイトルシーンができたらその名前を指定
        // SceneManager.LoadScene("Title");
    }
}
