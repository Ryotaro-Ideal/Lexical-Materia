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
    [SerializeField] private string BASE_AREA_SCENE_NAME = "BaseArea";
    public UnityEvent OnVictory;
    public UnityEvent OnGameOver;



    private void Awake()
    {
        Time.timeScale = 1f;
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
    }

    public void OnPlayerDeath()
    {
        if (CurrentState == GameState.Playing)
        {
            LoseGame();
        }
    }

    public void WinGame()
    {

        CurrentState = GameState.Victory;
        Debug.Log("Game Clear!");
        OnVictory?.Invoke();
    }

    private void LoseGame()
    {
        CurrentState = GameState.GameOver;
        FindFirstObjectByType<ResultUI>()?.ShowResult();
        OnGameOver?.Invoke();

    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 死亡演出で止めたtimeScaleをリセット
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(BASE_AREA_SCENE_NAME);
    }
}
