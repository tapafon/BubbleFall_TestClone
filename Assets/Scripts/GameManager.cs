using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameStarted = false;
    public bool gameOver = false;
    public ulong nextID = 0;
    public ulong score = 0;
    public float movementSpeed = 0.1f;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private GameObject spawnSupport;
    [SerializeField] private GameObject regularSupport;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        StartCoroutine(StartNewGame());
    }

    void Update()
    {
        scoreText.text = $"Score: {score}";
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
        ballSpawner.enableRegularSpawn = false;
        ballSpawner.ReleaseBalls();
        ballLauncher.GetComponentInChildren<BallController>().Release(true);
    }
}
