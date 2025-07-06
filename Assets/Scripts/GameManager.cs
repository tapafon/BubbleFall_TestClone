using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool gamePaused = false;
    public ulong nextID = 0;
    public ulong score = 0;
    public float movementSpeed = 0.1f;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private GameObject spawnSupport;
    [SerializeField] private GameObject regularSupport;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private GameObject gameOverScreen;
    private float _gameSpeed = 1f;
    private GameSpeed _currentGameSpeed = GameSpeed.Full;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        Time.timeScale = _gameSpeed;
    }

    void Start()
    {
        StartCoroutine(StartNewGame());
    }

    void Update()
    {
        var fps = (int)(1f / (Time.smoothDeltaTime / _gameSpeed));
        scoreText.text = $"Score: {score}\n{fps} FPS";
        if (Input.GetKey(KeyCode.Escape)) OnBack();
    }

    public void TogglePause()
    {
        var state = !gamePaused;
        pauseMenu.SetActive(state);
        Time.timeScale = state ? 0 : _gameSpeed;
        StartCoroutine(DelayedTogglePause(state));
    }

    IEnumerator DelayedTogglePause(bool state)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        gamePaused = state;
    }

    public void LevelRestart()
    {
        Time.timeScale = _gameSpeed;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeSpeed()
    {
        _currentGameSpeed++;
        if ((int)_currentGameSpeed > 4) _currentGameSpeed = 0;
        switch (_currentGameSpeed)
        {
            case GameSpeed.Full:
                _gameSpeed = 1f;
                break;
            case GameSpeed.Half:
                _gameSpeed = 0.5f;
                break;
            case GameSpeed.Quarter:
                _gameSpeed = 0.25f;
                break;
            case GameSpeed.Turtle:
                _gameSpeed = 0.1f;
                break;
            case GameSpeed.Double:
                _gameSpeed = 2f;
                break;
        }
        speedText.text = $"{_currentGameSpeed} speed";
    }

    public void OnBack()
    {
        if (!gameOver && !gamePaused) TogglePause();
        else QuitGame();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator StartNewGame()
    {
        ballSpawner.InitialSpawn();
        yield return new WaitForSeconds(5f); //while balls are falling and fixing themselves, just like in lottery
        ballSpawner.enableRegularSpawn = true;
        spawnSupport.SetActive(false);
        regularSupport.SetActive(true);
        gameStarted = true;
    }

    public void EndGame()
    {
        gameOver = true;
        Time.timeScale = 1f;
        _gameSpeed = 1f;
        gameOverScreen.SetActive(true);
        ballSpawner.enableRegularSpawn = false;
        ballSpawner.ReleaseBalls();
        ballLauncher.GetComponentInChildren<BallController>().Release(true);
    }

    enum GameSpeed
    {
        Turtle,
        Quarter,
        Half,
        Full,
        Double
    }
}
