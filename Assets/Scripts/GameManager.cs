using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameStarted = false;
    public bool gameOver = false;
    public ulong nextID = 0;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private GameObject spawnSupport;
    [SerializeField] private GameObject regularSupport;
    void Start()
    {
        StartCoroutine(StartNewGame());
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
    }
}
