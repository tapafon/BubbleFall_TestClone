using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public BallSpawner ballSpawner;
    void Start()
    {
        StartCoroutine(StartNewGame());
    }

    IEnumerator StartNewGame()
    {
        ballSpawner.InitialSpawn();
        yield return new WaitForSeconds(5f);
        ballSpawner.enableRegularSpawn = true;
    }
}
